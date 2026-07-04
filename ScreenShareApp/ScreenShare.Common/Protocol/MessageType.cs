// Enum liệt kê tất cả loại message có thể trao đổi qua mạng giữa Host và Client:
// ViewRequest, ViewAccept, ViewReject, ScreenFrame, ChatMessage, ClientDisconnect...
// Giúp bên nhận biết cần xử lý dữ liệu theo cách nào khi đọc được 1 gói tin.

namespace ScreenShare.Common.Protocol
{
    public enum  MessageType
    {
        ViewRequest,
        ViewAccept,
        ViewReject,
        ScreenFrame,
        ChatMessage,
        ClientDisconnect,
        ClientJoined,
        ClientLeft,
    }
}