// Trung tâm xử lý mạng phía Host: mở TcpListener lắng nghe kết nối trên 1 Port,
// mỗi khi có client mới kết nối thì tạo 1 ClientSession riêng để quản lý,
// đồng thời giữ danh sách tất cả client đang kết nối (dùng để Broadcast màn hình/chat cho tất cả).