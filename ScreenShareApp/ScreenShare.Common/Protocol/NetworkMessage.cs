using Newtonsoft.Json;
using ScreenShare.Common.Models;
using System;

namespace ScreenShare.Common.Protocol
{
    public class NetworkMessage
    {
        public MessageType Type { get; set; }
        public string SenderID { get; set; } = string.Empty;

        // Thay vì để null, khởi tạo sẵn mảng rỗng để tránh lỗi NullReferenceException
        public byte[] Payload { get; set; } = Array.Empty<byte>();

        public static NetworkMessage FromChatMessage(string senderId, ChatMessage chat)
        {
            if (chat == null) return CreateSimple(MessageType.ChatMessage, senderId);

            string json = JsonConvert.SerializeObject(chat);
            return new NetworkMessage
            {
                Type = MessageType.ChatMessage,
                SenderID = senderId,
                Payload = System.Text.Encoding.UTF8.GetBytes(json)
            };
        }

        // Chuyển đổi Payload từ mảng byte JSON sang object ChatMessage
        public ChatMessage ToChatMessage()
        {
            // Kiểm tra bảo vệ: Nếu payload rỗng hoặc null thì không parse tiếp
            if (Payload == null || Payload.Length == 0)
            {
                return null;
            }

            try
            {
                string json = System.Text.Encoding.UTF8.GetString(Payload);
                return JsonConvert.DeserializeObject<ChatMessage>(json);
            }
            catch (JsonException)
            {
                // Log lỗi hoặc xử lý khi JSON sai định dạng nếu cần
                return null;
            }
        }

        // Tạo NetworkMessage chứa dữ liệu ảnh màn hình
        public static NetworkMessage FromScreenFrame(string senderId, byte[] imageBytes)
        {
            return new NetworkMessage
            {
                Type = MessageType.ScreenFrame,
                SenderID = senderId,
                Payload = imageBytes ?? Array.Empty<byte>() // Tránh truyền null vào
            };
        }

        // Tạo NetworkMessage không kèm dữ liệu
        public static NetworkMessage CreateSimple(MessageType type, string senderId)
        {
            return new NetworkMessage
            {
                Type = type,
                SenderID = senderId,
                Payload = Array.Empty<byte>() // Dùng mảng rỗng an toàn hơn null
            };
        }
    }
}