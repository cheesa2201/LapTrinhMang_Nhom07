

// Xử lý logic chat phía Client: gửi tin nhắn người dùng nhập lên Host,
// và nhận tin nhắn từ Host/các client khác để hiển thị lên khung chat.
// Class này KHÔNG tự mở kết nối riêng - nó dùng lại TcpClientService đã có sẵn kết nối.

using System;
using System.Collections.Generic;
using ScreenShare.Common.Models;
using ScreenShare.Client.Network;

namespace ScreenShare.Client.Chat
{
    public class ChatClientService
    {
        private readonly TcpClientService _tcpClientService;

        // Lưu lại lịch sử chat trong phiên hiện tại, để MainForm có thể hiển thị lại
        // (ví dụ khi cần refresh UI hoặc export log chat)
        public List<ChatMessage> ChatHistory { get; } = new List<ChatMessage>();

        // Event để MainForm đăng ký, bắn ra mỗi khi có tin nhắn mới (của mình hoặc người khác)
        public event Action<ChatMessage>? MessageReceived;

        public ChatClientService(TcpClientService tcpClientService)
        {
            _tcpClientService = tcpClientService;

            // Lắng nghe sẵn sự kiện ChatMessageReceived từ TcpClientService
            _tcpClientService.ChatMessageReceived += OnChatMessageReceived;
        }

        // Gọi hàm này khi người dùng bấm nút Send trên UI
        public void SendMessage(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return; // Không gửi tin nhắn rỗng

            _tcpClientService.SendChatMessage(content);

            // Tự thêm luôn tin nhắn của mình vào lịch sử + bắn event để UI hiển thị ngay,
            // không cần chờ Host gửi ngược lại (Host không echo lại cho chính người gửi)
            var ownMessage = new ChatMessage
            {
                SenderId = _tcpClientService.ClientId,
                SenderName = _tcpClientService.DisplayName,
                Content = content
            };
            ChatHistory.Add(ownMessage);
            MessageReceived?.Invoke(ownMessage);
        }

        // Xử lý khi nhận được tin nhắn từ Host/client khác
        private void OnChatMessageReceived(ChatMessage chat)
        {
            ChatHistory.Add(chat);
            MessageReceived?.Invoke(chat);
        }
    }
}