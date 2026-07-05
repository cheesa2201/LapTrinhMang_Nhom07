// Cho phép Client lưu lại ảnh màn hình đang xem tại thời điểm hiện tại thành file
// (ví dụ .jpg/.png) vào máy local, khi người dùng bấm nút "Chụp snapshot".

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ScreenShare.Client.Snapshot
{
    public class SnapshotService
    {
        // Thư mục mặc định lưu snapshot: My Pictures/ScreenShareSnapshots
        private readonly string _defaultFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
            "ScreenShareSnapshots");

        public SnapshotService()
        {
            // Tạo sẵn thư mục nếu chưa tồn tại, tránh lỗi khi Save
            if (!Directory.Exists(_defaultFolder))
                Directory.CreateDirectory(_defaultFolder);
        }

        // Lưu snapshot từ Image đang hiển thị trên PictureBox.
        // Trả về đường dẫn file đã lưu để MainForm có thể hiển thị thông báo "Đã lưu tại: ..."
        public string SaveSnapshot(Image currentImage)
        {
            if (currentImage == null)
                throw new InvalidOperationException("Chưa có màn hình nào để chụp snapshot.");

            // Đặt tên file theo thời gian để không bị trùng/ghi đè giữa các lần chụp
            string fileName = $"Snapshot_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
            string fullPath = Path.Combine(_defaultFolder, fileName);

            // Clone lại ảnh trước khi save, tránh lỗi "GDI+ generic error" do đang giữ handle
            // của ảnh đang được PictureBox sử dụng đồng thời
            using (var clonedImage = new Bitmap(currentImage))
            {
                clonedImage.Save(fullPath, ImageFormat.Jpeg);
            }

            return fullPath;
        }

        // Cho phép người dùng chọn nơi lưu thay vì dùng mặc định (tuỳ chọn nâng cao)
        public string SaveSnapshotAs(Image currentImage, string filePath)
        {
            if (currentImage == null)
                throw new InvalidOperationException("Chưa có màn hình nào để chụp snapshot.");

            using (var clonedImage = new Bitmap(currentImage))
            {
                clonedImage.Save(filePath, ImageFormat.Jpeg);
            }

            return filePath;
        }
    }
}