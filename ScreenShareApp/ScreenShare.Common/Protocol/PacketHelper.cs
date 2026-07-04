// Xử lý đóng gói/giải gói dữ liệu khi gửi qua TCP Socket.
// Vì TCP là stream liên tục không có ranh giới message rõ ràng, nên trước mỗi gói dữ liệu
// phải ghi thêm 4 byte thể hiện độ dài, để bên nhận đọc đúng số byte và tách đúng từng message,
// tránh bị dính 2 message làm 1 hoặc đọc thiếu dữ liệu (lỗi thường gặp nhất khi lập trình socket).

using System.Net.Sockets;
using Newtonsoft.Json;

namespace ScreenShare.Common.Protocol
{
    public static class PacketHelper
    {
        // Gửi 1 NetworkMessage qua NetworkStream
        public static void SendMessage(NetworkStream stream, NetworkMessage message)
        {
            // 1. Serialize message thành JSON, rồi chuyển thành mảng byte
            string json = JsonConvert.SerializeObject(message);
            byte[] bodyBytes = System.Text.Encoding.UTF8.GetBytes(json);

            // 2. Tạo 4 byte đầu chứa độ dài của bodyBytes
            byte[] lengthPrefix = BitConverter.GetBytes(bodyBytes.Length);

            // 3. Gửi lần lượt: 4 byte độ dài trước, rồi tới nội dung thực sự
            stream.Write(lengthPrefix, 0, lengthPrefix.Length);
            stream.Write(bodyBytes, 0, bodyBytes.Length);
            stream.Flush();
        }

        // Nhận 1 NetworkMessage từ NetworkStream (chặn/block cho tới khi đủ dữ liệu)
        public static NetworkMessage ReceiveMessage(NetworkStream stream)
        {
            // 1. Đọc đúng 4 byte đầu để biết độ dài nội dung
            byte[] lengthPrefix = ReadExact(stream, 4);
            if (lengthPrefix == null)
                return null; // Kết nối đã đóng

            int bodyLength = BitConverter.ToInt32(lengthPrefix, 0);

            // 2. Đọc đúng bodyLength byte tiếp theo
            byte[] bodyBytes = ReadExact(stream, bodyLength);
            if (bodyBytes == null)
                return null;

            // 3. Deserialize JSON trở lại thành NetworkMessage
            string json = System.Text.Encoding.UTF8.GetString(bodyBytes);
            return JsonConvert.DeserializeObject<NetworkMessage>(json);
        }

        // Đọc chính xác "count" byte từ stream, lặp lại tới khi đủ
        // (vì stream.Read() có thể trả về ít byte hơn yêu cầu trong 1 lần gọi)
        private static byte[] ReadExact(NetworkStream stream, int count)
        {
            byte[] buffer = new byte[count];
            int offset = 0;

            while (offset < count)
            {
                int bytesRead = stream.Read(buffer, offset, count - offset);
                if (bytesRead == 0)
                    return null; // Client đã ngắt kết nối giữa chừng

                offset += bytesRead;
            }

            return buffer;
        }
    }

}