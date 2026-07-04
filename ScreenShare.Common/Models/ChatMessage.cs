// Đại diện cho 1 tin nhắn chat: người gửi, nội dung, thời gian gửi.
// Dùng chung cho cả chiều Host -> Client và Client -> Host, serialize thành JSON để gửi qua socket.

namespace ScreenShare.Common.Models
{
    public class ChatMessage
    {
        public string SenderId { get; set; } = string.Empty; // ID người gửi
        public string SenderName { get; set; } = string.Empty; // Tên người gửi
        public string Content { get; set; } = string.Empty; // Nội dung tin nhắn
        public DateTime SentAt { get; set; } // Thời gian gửi tin nhắn
        public ChatMessage()
        {
            SentAt = DateTime.Now; // Mặc định là thời gian hiện tại khi tạo tin nhắn
        }
    }
}