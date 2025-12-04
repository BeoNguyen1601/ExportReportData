using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Windows.Forms;

namespace Updater
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
                return;

            string zipPath = args[0];
            string targetExe = args[1];
            string targetFolder = Path.GetDirectoryName(targetExe);

            // Đợi ứng dụng chính đóng hoàn toàn
            Thread.Sleep(1500);

            try
            {
                using (ZipArchive archive = ZipFile.OpenRead(zipPath))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        string destinationPath = Path.Combine(targetFolder, entry.FullName);

                        // Nếu là thư mục
                        if (string.IsNullOrEmpty(entry.Name))
                        {
                            Directory.CreateDirectory(destinationPath);
                            continue;
                        }

                        // Tạo thư mục cha nếu chưa có
                        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));

                        // Nếu file tồn tại -> xóa trước
                        if (File.Exists(destinationPath))
                            File.Delete(destinationPath);

                        // Ghi file
                        using (var entryStream = entry.Open())
                        using (var fileStream = File.Create(destinationPath))
                        {
                            entryStream.CopyTo(fileStream);
                        }
                    }
                }

                // Xóa file zip
                File.Delete(zipPath);

                // Khởi động lại ứng dụng
                Process.Start(targetExe);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật: " + ex.Message);
            }
        }
    }
}
