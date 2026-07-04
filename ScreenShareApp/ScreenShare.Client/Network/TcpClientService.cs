// Xử lý kết nối mạng phía Client: mở TcpClient kết nối tới IP/Port của Host,
// gửi ViewRequest khi muốn xem màn hình, và chạy vòng lặp lắng nghe dữ liệu trả về từ Host
// (ScreenFrame để hiển thị, ChatMessage để đưa vào khung chat, ViewAccept/ViewReject để cập nhật trạng thái).