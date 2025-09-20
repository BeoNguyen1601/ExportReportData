using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExportData
{
    public class VietnameseHumanizer
    {
        private static readonly Dictionary<string, string> _dict = new Dictionary<string, string>
        {
            {"MaSinhVien", "Mã sinh viên"},
            {"MaLopHocPhan", "Mã lớp học phần"},
            {"MaLopHoc", "Mã lớp học"},
            {"LopHoc", "Lớp học"},
            {"TenMonHoc", "TenMonHoc"},
            {"MaLopChu", "Mã lớp chữ"},
            {"HoDem", "Họ đệm"},
            {"Ten", "Tên"},
            {"HoVaTen", "Họ và tên"},
            {"NgaySinh", "Ngày sinh"},
            {"TenDot", "Tên đợt" },
            {"TenKhoaHoc", "Tên khóa học" },
            {"TenHeDaoTao", "Hệ đào tạo" },
            {"TenLoaiHinhDT", "Loại đào tạo" },
            {"TenLoaiDT", "Loại đào tạo" },
            {"NgayBatDau", "Ngày bắt đầu" },
            {"NgayKetThuc", "Ngày kết thúc" },
            {"NgayHetHanNopHP", "Ngày hết hạn nộp HP" },
            {"NgayHetHanNopHP2", "Ngày hết hạn nộp HP2" },
        };

        public static string Convert(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            // Nếu tồn tại key trong dictionary thì trả về luôn
            string vietnamese;
            if (_dict.TryGetValue(input, out vietnamese))
            {
                return vietnamese;
            }

            // Ngược lại: giữ nguyên
            return input;
        }
    }
}
