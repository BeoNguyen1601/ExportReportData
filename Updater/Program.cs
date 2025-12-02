using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace Updater
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // args[0] = file ZIP tạm
            // args[1] = thư mục app
            // args[2] = file exe chính cần chạy lại

            if (args.Length < 3)
                return;

            string zipFile = args[0];
            string targetFolder = args[1];
            string mainExe = args[2];

            // Đợi app chính thoát (file không còn bị khóa)
            Thread.Sleep(2000);

            try
            {
                using (ZipArchive archive = ZipFile.OpenRead(zipFile))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        string fullPath = Path.Combine(targetFolder, entry.FullName);

                        // Là folder
                        if (entry.FullName.EndsWith("/"))
                        {
                            Directory.CreateDirectory(fullPath);
                            continue;
                        }

                        // Đảm bảo thư mục tồn tại
                        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                        // Ghi đè file
                        using (var entryStream = entry.Open())
                        using (var fileStream = File.Create(fullPath))
                        {
                            entryStream.CopyTo(fileStream);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Path.Combine(targetFolder, "update_error.txt"), ex.ToString());
            }

            // Chạy lại app
            try
            {
                Process.Start(mainExe);
            }
            catch { }

            // Thoát
        }
    }
}
