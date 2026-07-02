// Đại diện cho 1 kết nối client cụ thể trên Host.
// Chạy vòng lặp đọc dữ liệu liên tục từ socket của client này trên 1 Task/Thread riêng,
// để nhiều client có thể được xử lý song song mà không chặn lẫn nhau.
// Lưu trạng thái: đã được Accept xem màn hình hay chưa, để quyết định có gửi ScreenFrame cho client này không.