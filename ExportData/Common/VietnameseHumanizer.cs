using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static ScintillaNET.Style;

namespace ExportData
{
    public class VietnameseHumanizer
    {
        private static Dictionary<string, string> _dict = new Dictionary<string, string>();

        public static string Convert(string input)
        {
            string appFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ExportDataResources");
            string filePath = Path.Combine(appFolder, "vi.json");

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                // Chuyển JSON object thành Dictionary<string, string>
                _dict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
            }

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
