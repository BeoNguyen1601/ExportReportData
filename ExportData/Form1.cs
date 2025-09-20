using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ExportData
{
    public partial class Form1 : Form
    {
        private const string GitHubOwner = "BeoNguyen1601"; // thay bằng của bạn
        private const string GitHubRepo = "ExportReportData";        // thay bằng repo của bạn
        private const string GitHubApiUrl = "https://api.github.com/repos/" + GitHubOwner + "/" + GitHubRepo + "/releases/latest";

        private async void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                await CheckForUpdate();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi trong Form_Load: " + ex.Message);
            }
        }

        private async Task CheckForUpdate()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "AppExportData"); // GitHub bắt buộc

                    var json = await client.GetStringAsync(GitHubApiUrl);
                    var release = JObject.Parse(json);

                    var latestVersion = release["tag_name"].ToString(); // ví dụ "v1.2.0"
                    var assets = release["assets"] as JArray;
                    var downloadUrl = assets != null && assets.Count > 0
                        ? assets[0]["browser_download_url"].ToString()
                        : null;

                    var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
                    var newVersion = new Version(latestVersion.TrimStart('v'));

                    if (newVersion > currentVersion)
                    {
                        var dr = MessageBox.Show(
                            string.Format("Có phiên bản mới {0} (hiện tại {1}).\nBạn có muốn tải về không?", newVersion, currentVersion),
                            "Cập nhật",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information);

                        if (dr == DialogResult.Yes && !string.IsNullOrEmpty(downloadUrl))
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = downloadUrl,
                                UseShellExecute = true
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi kiểm tra cập nhật: " + ex.Message);
            }
        }

        public Form1()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception)
            {
                MessageBox.Show("Không thể khởi tạo ứng dụng!");
            }

        }

        #region Private Methods

        private string PreFixBrokenProperties(string code)
        {
            // Thêm xuống dòng trước mỗi "public " nếu nó dính vào nhau
            return System.Text.RegularExpressions.Regex.Replace(
                code,
                @"(= string\.Empty\s*)(public\s+)",
                "$1;\n$2"
            );
        }
        private string FormatCSharp(string code)
        {
            try
            {
                // Parse code thành SyntaxTree
                var tree = CSharpSyntaxTree.ParseText(code);
                var root = tree.GetRoot();

                // NormalizeWhitespace() sẽ format lại
                var formatted = root.NormalizeWhitespace();

                return formatted.ToFullString();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        private void CreateFile(string basePath, string fileName, string classCode)
        {
            // Tạo file .cs
            string filePath = Path.Combine(basePath, fileName + ".cs");
            File.WriteAllText(filePath, classCode, Encoding.UTF8);
        }
        private string CreateFolder(string folderName)
        {
            // Tạo thư mục GenerateSuccess\Models
            string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GenerateSuccess", folderName);

            if (Directory.Exists(basePath))
            {
                Directory.Delete(basePath, true);
            }
            Directory.CreateDirectory(basePath);
            return basePath;
        }

        #endregion

        private void btnStart_Click(object sender, EventArgs e)
        {
            string input = txtRequest.Text;

            string className = string.Empty;
            var classMatch = Regex.Match(input, @"class\s+(\w+)");
            if (classMatch.Success)
            {
                className = classMatch.Groups[1].Value;
            }
            else
            {
                MessageBox.Show("Không tìm thấy tên class");
                return;
            }

            #region Class


            string[] lstPartClassName = className.Split('_');

            var exportClassName = "Export" + lstPartClassName.Last();

            // Tìm tất cả property
            int startIndex = input.IndexOf('{');
            int endIndex = input.LastIndexOf('}');
            if (startIndex < 0 || endIndex <= startIndex)
            {
                MessageBox.Show("Không xác định được class");
                return;
            }

            string classBody = input.Substring(startIndex + 1, endIndex - startIndex - 1);
            var lines = classBody.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            var sb = new StringBuilder();
            sb.AppendLine("public class " + exportClassName);
            sb.AppendLine("{");
            sb.AppendLine("public int P_STTExport { get; set; }");

            foreach (var line in lines)
            {
                var match = Regex.Match(line.Trim(), @"public\s+([\w\?\<\>]+)\s+(\w+)\s*\{");
                if (match.Success)
                {
                    string propType = match.Groups[1].Value;
                    string propName = match.Groups[2].Value;

                    if (propType == "bool" || propType == "bool?")
                        sb.AppendLine("[CustomBoolean(\"Có\", \"Không\")]");
                    sb.AppendLine($"public string P_{propName} {{ get; set; }}");
                }
            }

            sb.AppendLine("}");

            string codeText = PreFixBrokenProperties(sb.ToString());
            codeText = FormatCSharp(codeText);

            string basePath = CreateFolder("Class");
            CreateFile(basePath, exportClassName, codeText);


            #endregion

            #region Services

            string requestClass = Regex.Replace(className, "Result$", "Request", RegexOptions.IgnoreCase);

            string queries = Regex.Replace(className, "^SP_", "I");
            queries = Regex.Replace(className, "Result$", "Queries", RegexOptions.IgnoreCase);

            string[] queriesPart = queries.Split('_');
            string propNamePart = queriesPart.Last();
            string output = "_" + char.ToLower(propNamePart[0]) + propNamePart.Substring(1);

            string nameMethod = Regex.Replace(lstPartClassName.Last(), "Result$", "", RegexOptions.IgnoreCase);

            string nameGetData = Regex.Replace(className, "^SP_", "");
            nameGetData = Regex.Replace(nameGetData, "Result$", "", RegexOptions.IgnoreCase);

            var sb1 = new StringBuilder();
            sb1.AppendLine("namespace ASC.EDU.APPLICATION.Services");
            sb1.AppendLine("{");
            sb1.AppendLine("public interface IReportService");
            sb1.AppendLine("{");
            sb1.AppendLine($"Task<HT_BieuMauInfoViewModel> ReportExcel{nameMethod}Async({requestClass} request);");
            sb1.AppendLine("}");
            sb1.AppendLine("[TransientDependency(ServiceType = typeof(IReportService))]");
            sb1.AppendLine("public class ReportService : BaseService, IReportService");
            sb1.AppendLine("{");
            sb1.AppendLine($"private readonly {queries} {output}; \n");
            sb1.AppendLine($"public async Task<HT_BieuMauInfoViewModel> ReportExcel{nameMethod}Async({requestClass} param)");
            sb1.AppendLine("{");
            sb1.AppendLine("string maBieuMau = BieuMauConstants.BM_Excel;");
            sb1.AppendLine("Dictionary<string, string> dicReplace = new Dictionary<string, string>();");
            sb1.AppendLine($"var dataPrint = await {output}.{nameGetData}Async(param).ConfigureAwait(false);");
            sb1.AppendLine("if (!dataPrint.Result.Items.Any()) CommonBase.ShowErrorMessage(EDM_BieuMauErrorCode.DM_BIEUMAU_KHONGCODULIEU);");
            sb1.AppendLine($"var dataReport = dataPrint.Result.Items.ToList().MapToExportViewModel<{className}, {exportClassName}>();");
            sb1.AppendLine("var infoBieuMau = new HT_BieuMauInfoRequest()");
            sb1.AppendLine("{");
            sb1.AppendLine("DicReplace = dicReplace,");
            sb1.AppendLine("MaBieuMau = maBieuMau,");
            sb1.AppendLine("};");
            sb1.AppendLine("var result = await ReportExcelAsync(dataReport, infoBieuMau);");
            sb1.AppendLine("return result;");
            sb1.AppendLine("}");
            sb1.AppendLine("}");
            sb1.AppendLine("}");

            string codeText1 = PreFixBrokenProperties(sb1.ToString());
            codeText1 = FormatCSharp(codeText1);

            string basePath1 = CreateFolder("Services");
            CreateFile(basePath1, "ReportService", codeText1);

            #endregion

            #region Controller

            var sb2 = new StringBuilder();
            sb2.AppendLine("namespace ASC.EDU.API.Controllers.v1");
            sb2.AppendLine("{");
            sb2.AppendLine("\r\n");
            sb2.AppendLine("\r\n");
            sb2.AppendLine("[ApiController]");
            sb2.AppendLine("[Authorize]");
            sb2.AppendLine($"public class {lstPartClassName.Last()}Controller : APIControllerBase");
            sb2.AppendLine("{");
            sb2.AppendLine("private const string ExportDataList = nameof(ExportDataList);");
            sb2.AppendLine("private readonly IReportService _queriesBieuMau;");
            sb2.AppendLine(" ");
            sb2.AppendLine("[HttpPost]");
            sb2.AppendLine("[Route(ExportDataList)]");
            sb2.AppendLine("[ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.BadRequest)]");
            sb2.AppendLine($"public async Task<IActionResult> ExportReportExcel{nameMethod}Async({requestClass} request)");
            sb2.AppendLine("{");
            sb2.AppendLine($"var result = await _queriesBieuMau.ReportExcel{nameMethod}Async(request).ConfigureAwait(false);");
            sb2.AppendLine("return File(result.FileContent, result.ContentType, result.TenBieuMau);");
            sb2.AppendLine("}");
            sb2.AppendLine("}");
            sb2.AppendLine("}");

            string codeText2 = PreFixBrokenProperties(sb2.ToString());
            codeText2 = FormatCSharp(codeText2);

            string basePath2 = CreateFolder("Controller");
            CreateFile(basePath2, $"{lstPartClassName.Last()}Controller", codeText2);

            #endregion

            var genClass = new GenClass();
            if (genClass.ShowDialog() == DialogResult.OK)
            {

            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            var ggSheet = new GoogleSheet();
            if (ggSheet.ShowDialog() == DialogResult.OK)
            {
                var response = ggSheet.Response;
                var mapping = ggSheet.MappingData;

                txtRequest.Text = response;

                var headerForm = new HeaderMappingForm(mapping);
                if (headerForm.ShowDialog() == DialogResult.OK)
                {
                    var headers = headerForm.HeaderMappings;
                    var title = headerForm.Title;
                    var maBieuMau = headerForm.MaBieuMau;

                    ExportExcel(headers, title, maBieuMau);
                }
            }
        }
        private void ExportExcel(Dictionary<string, HeaderMappingInfo> headerMapping, string title, string key)
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel Workbook|*.xlsx";
                sfd.Title = "Chọn nơi lưu file Excel";
                sfd.FileName = $"{key}.xlsx";

                if (sfd.ShowDialog() != DialogResult.OK)
                    return;

                var fi = new FileInfo(sfd.FileName);

                // Cấu hình license cho EPPlus 8.x
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(fi))
                {
                    var ws = package.Workbook.Worksheets.Add("Sheet1");
                    ws.View.ShowGridLines = false;
                    ws.Cells.Style.Font.Name = "Times New Roman";
                    ws.Cells.Style.Font.Size = 12;
                    ws.View.PageBreakView = true;

                    ws.PrinterSettings.FitToPage = true;
                    ws.PrinterSettings.FitToWidth = 1;
                    ws.PrinterSettings.FitToHeight = 0; // Không giới hạn chiều cao

                    int totalCols = headerMapping.Count;

                    // === Floating TextBox ===
                    var shape = ws.Drawings.AddShape("txtShape1", eShapeStyle.Rect);
                    shape.SetPosition(0, 0, totalCols - 5, 0); // góc phải trên
                    shape.SetSize(250, 70);
                    shape.Fill.Style = eFillStyle.NoFill;
                    shape.Border.Fill.Style = eFillStyle.NoFill;
                    shape.Text = "CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM\nĐộc lập - Tự do - Hạnh phúc\n----------------------------";
                    shape.TextAnchoring = eTextAnchoringType.Top;

                    shape.Font.Size = 12;
                    shape.Font.Bold = true;
                    shape.Font.LatinFont = "Times New Roman";  // Font family
                    shape.Font.Fill.Color = System.Drawing.Color.Black; // Màu chữ

                    // TextAlign - căn giữa bằng cách dùng shape.TextAlignment
                    shape.TextAlignment = eTextAlignment.Center;

                    // === Row 4: Title merge toàn bộ
                    ws.Cells[4, 1, 4, totalCols].Merge = true;
                    ws.Cells[4, 1].Value = title;
                    ws.Cells[4, 1].Style.Font.Size = 16;
                    ws.Cells[4, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[4, 1].Style.Font.Bold = true;

                    // === Row 6: Header
                    int col = 1;
                    foreach (var kvp in headerMapping)
                    {
                        var cell = ws.Cells[6, col];
                        cell.Value = kvp.Value.Title;
                        cell.Style.Font.Bold = true;
                        cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(208, 206, 206));
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        col++;
                    }

                    // === Row 7: &=Key.Prop
                    col = 1;
                    foreach (var kvp in headerMapping)
                    {
                        var cell = ws.Cells[7, col];
                        cell.Value = $"&={key}.{kvp.Key}";
                        cell.Style.WrapText = true;
                        cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        cell.Style.Numberformat.Format = "@"; // Text format

                        // 🟨 Căn lề ngang
                        switch (kvp.Value.Align?.ToLowerInvariant())
                        {
                            case "center":
                                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                break;
                            case "right":
                                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                break;
                            default:
                                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                break;
                        }

                        // Set column width
                        if (kvp.Key.Equals("P_STTExport", StringComparison.OrdinalIgnoreCase))
                            ws.Column(col).Width = 10;
                        else
                            ws.Column(col).Width = 25;

                        col++;
                    }

                    // === Row 9-13: Signature
                    try
                    {
                        int mergeStartCol = totalCols - 2;
                        ws.Cells[9, mergeStartCol, 9, totalCols].Merge = true;
                        ws.Cells[9, mergeStartCol].Value = "..........., ngày %Ngay tháng %Thang năm %Nam";
                        ws.Cells[9, mergeStartCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        ws.Cells[10, mergeStartCol, 10, totalCols].Merge = true;
                        ws.Cells[10, mergeStartCol].Value = "Người lập biểu";
                        ws.Cells[10, mergeStartCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[10, mergeStartCol].Style.Font.Bold = true;


                        ws.Cells[13, mergeStartCol, 13, totalCols].Merge = true;
                        ws.Cells[13, mergeStartCol].Value = "%NguoiLapBieu";
                        ws.Cells[13, mergeStartCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[13, mergeStartCol].Style.Font.Bold = true;
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Số cột không đủ để merge!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    package.Save();
                    var result = MessageBox.Show("Xuất file thành công. Bạn có muốn mở file không?", "Mở file?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            System.Diagnostics.Process.Start(fi.FullName);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Không thể mở file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }


    }
}
