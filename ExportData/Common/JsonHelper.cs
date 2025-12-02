using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExportData.Common
{
    public static class JsonHelper
    {
        public static void AddOrIgnore(string filePath, string key, string value)
        {
            JObject jsonData;

            // Nếu file chưa tồn tại → tạo file mới rỗng
            if (!File.Exists(filePath))
            {
                jsonData = new JObject();
            }
            else
            {
                string jsonText = File.ReadAllText(filePath);
                jsonData = JObject.Parse(jsonText);
            }

            key = key.Replace("P_", "");

            // Nếu đã tồn tại → bỏ qua
            if (jsonData.ContainsKey(key))
                return;

            // Chưa tồn tại → thêm vào
            jsonData[key] = value;

            // Ghi file lại (format đẹp)
            File.WriteAllText(filePath, jsonData.ToString(Formatting.Indented));
        }
    }
}
