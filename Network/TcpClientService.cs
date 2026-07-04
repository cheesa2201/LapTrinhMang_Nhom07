// Xử lý kết nối mạng phía Client: mở TcpClient kết nối tới IP/Port của Host,
// gửi ViewRequest khi muốn xem màn hình, và chạy vòng lặp lắng nghe dữ liệu trả về từ Host
// (ScreenFrame để hiển thị, ChatMessage để đưa vào khung chat, ViewAccept/ViewReject để cập nhật trạng thái).

using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ScreenShare.Common.Models;
using ScreenShare.Common.Protocol;

namespace ScreenShare.Client.Network
{
    public class TcpClientService
    {
        private TcpClient? _tcpClient;
        private NetworkStream? _stream;
        private CancellationTokenSource? _cancellationTokenSource;
        private readonly object _sendLock = new object(); // Tránh 2 thread cùng ghi vào stream 1 lúc

        // Id tự sinh cho phiên làm việc này, gửi kèm mọi message để Host phân biệt được người gửi
        public string ClientId { get; } = Guid.NewGuid().ToString();
        public string DisplayName { get; set; } = Environment.MachineName;

        public bool IsConnected => _tcpClient?.Connected ?? false;

        // Các event để MainForm đăng ký lắng nghe và cập nhật UI tương ứng
        public event Action? Connected;                          // Kết nối TCP thành công tới Host
        public event Action<string>? ConnectionFailed;            // Kết nối thất bại, kèm lý do lỗi
        public event Action? ViewAccepted;                        // Host đồng ý cho xem màn hình
        public event Action? ViewRejected;                        // Host từ chối
        public event Action<byte[]>? ScreenFrameReceived;         // Nhận được 1 khung hình màn hình mới
        public event Action<ChatMessage>? ChatMessageReceived;    // Nhận được tin nhắn chat mới
        public event Action? Disconnected;                        // Mất kết nối / Host đóng kết nối

        // Kết nối tới Host theo IP và Port, chạy nền để không chặn UI
        public async Task ConnectAsync(string ipAddress, int port)
        {
            try
            {
                _tcpClient = new TcpClient();
                await _tcpClient.ConnectAsync(ipAddress, port);
                _stream = _tcpClient.GetStream();
                _cancellationTokenSource = new CancellationTokenSource();

                Connected?.Invoke();

                // Bắt đầu vòng lặp lắng nghe message từ Host, chạy song song không chặn UI thread
                _ = Task.Run(() => ReceiveLoopAsync(_cancellationTokenSource.Token));
            }
            catch (Exception ex)
            {
                ConnectionFailed?.Invoke(ex.Message);
            }
        }

        // Gửi yêu cầu xin xem màn hình tới Host, gọi ngay sau khi Connected thành công
        public void SendViewRequest()
        {
            var message = NetworkMessage.CreateSimple(MessageType.ViewRequest, ClientId);
            SendMessage(message);
        }

        // Gửi 1 tin nhắn chat lên Host
        public void SendChatMessage(string content)
        {
            var chat = new ChatMessage
            {
                SenderId = ClientId,
                SenderName = DisplayName,
                Content = content
            };
            var message = NetworkMessage.FromChatMessage(ClientId, chat);
            SendMessage(message);
        }

        // Ngắt kết nối chủ động (khi người dùng đóng app hoặc bấm Disconnect)
        public void Disconnect()
        {
            try
            {
                if (IsConnected)
                {
                    // Báo cho Host biết mình chủ động rời đi, trước khi đóng socket
                    var message = NetworkMessage.CreateSimple(MessageType.ClientDisconnect, ClientId);
                    SendMessage(message);
                }
            }
            catch (Exception)
            {
                // Bỏ qua lỗi nếu kết nối đã hỏng sẵn, không cần báo được cho Host nữa
            }
            finally
            {
                _cancellationTokenSource?.Cancel();
                _stream?.Close();
                _tcpClient?.Close();
            }
        }

        // Vòng lặp chạy nền: liên tục đọc message từ Host cho tới khi mất kết nối
        private async Task ReceiveLoopAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested && IsConnected)
                {
                    NetworkMessage? message = await Task.Run(
                        () => PacketHelper.ReceiveMessage(_stream!), cancellationToken);

                    if (message == null)
                        break; // Host đã đóng kết nối

                    HandleMessage(message);
                }
            }
            catch (Exception)
            {
                // Lỗi đọc stream (rớt mạng đột ngột...) -> coi như mất kết nối, xử lý ở finally
            }
            finally
            {
                Disconnected?.Invoke();
            }
        }

        // Phân loại message nhận được từ Host và bắn event tương ứng
        private void HandleMessage(NetworkMessage message)
        {
            switch (message.Type)
            {
                case MessageType.ViewAccept:
                    ViewAccepted?.Invoke();
                    break;

                case MessageType.ViewReject:
                    ViewRejected?.Invoke();
                    break;

                case MessageType.ScreenFrame:
                    ScreenFrameReceived?.Invoke(message.Payload);
                    break;

                case MessageType.ChatMessage:
                    var chat = message.ToChatMessage();
                    ChatMessageReceived?.Invoke(chat);
                    break;

                default:
                    // Các loại khác Host không nên gửi xuống Client, bỏ qua
                    break;
            }
        }

        // Gửi message thô tới Host, dùng lock để tránh 2 thread cùng ghi vào stream 1 lúc
        // (ví dụ: gửi chat và gửi snapshot request xảy ra gần như đồng thời)
        private void SendMessage(NetworkMessage message)
        {
            if (!IsConnected) return;

            lock (_sendLock)
            {
                try
                {
                    PacketHelper.SendMessage(_stream!, message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending message: {ex.Message}");
                }
            }
        }
    }
    }