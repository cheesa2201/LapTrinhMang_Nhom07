// Lưu thông tin của 1 client đang kết nối tới Host: 
// Id (định danh duy nhất), Tên máy/người dùng, trạng thái (đang chờ duyệt / đã được Accept / bị Reject),
// và địa chỉ IP để Host hiển thị danh sách client trong UI.

using System.Net.Sockets;

namespace ScreenShare.Common.Models
{
    public class ClientInfo
    {
        public string Id { get; set; } = string.Empty; // Định danh duy nhất của client
        public string DisplayName { get; set; } = string.Empty; // Tên máy/người dùng
        public string IpAddress { get; set; } = string.Empty; // Địa chỉ IP của client
        public string IsAccepted { get; set; } = string.Empty; // Trạng thái chấp nhận của client
    

    public ClientInfo()
        {
            Id = Guid.NewGuid().ToString(); // Tạo Id duy nhất khi khởi tạo
            IsAccepted = "false"; // Mặc định chưa được chấp nhận
        }
    }
}