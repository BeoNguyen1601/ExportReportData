using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;

namespace ExportData
{
    public partial class GoogleSheet : Form
    {
        public string Response { get; private set; }
        public Dictionary<string, string> MappingData { get; private set; }

        private const string _idGoogleSheet = "10Bsl_mgRmkCUqWdV3C7nGCdIbx7CaTbjr3hiF5h9MuM";
        private const string _urlGoogleSheet = "https://docs.google.com/spreadsheets/d/";
        private const string _actionGoogleSheet = "/export?format=xlsx&id=";
        private string _sheetName = "";
        private ExcelPackage _currentExcelPackage;

        public GoogleSheet()
        {
            InitializeComponent();
            idGoogleSheet.Text = _idGoogleSheet;
        }

        private void btnGetData_Click(object sender, EventArgs e)
        {
            LoadSheetsFromGoogleSheet();
        }

        private void LoadSheetsFromGoogleSheet()
        {
            if (string.IsNullOrEmpty(idGoogleSheet.Text))
                MessageBox.Show("Không tìm thấy file!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            var fullUrl = $"{_urlGoogleSheet}{idGoogleSheet.Text}{_actionGoogleSheet}{idGoogleSheet.Text}";

            try
            {
                // Dùng WebClient để tải file đồng bộ
                using (WebClient client = new WebClient())
                {
                    byte[] data = client.DownloadData(fullUrl);
                    var stream = new MemoryStream(data);
                    // Không dùng using ở đây!
                    _currentExcelPackage = new ExcelPackage(stream);
                    GenerateSheetButtons(_currentExcelPackage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải hoặc đọc file Excel:\n" + ex.Message);
            }
        }

        private void GenerateSheetButtons(ExcelPackage package)
        {
            tlpTableGen.SuspendLayout();

            tlpTableGen.Controls.Clear();
            tlpTableGen.ColumnCount = 3;
            tlpTableGen.RowCount = 0;
            tlpTableGen.ColumnStyles.Clear();
            tlpTableGen.RowStyles.Clear();
            tlpTableGen.Padding = new Padding(0);
            tlpTableGen.Margin = new Padding(0);

            // Thiết lập 3 cột, mỗi cột chiếm 1/3
            for (int i = 0; i < 3; i++)
            {
                tlpTableGen.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            }

            int colCount = 3;
            int index = 0;

            foreach (var sheet in package.Workbook.Worksheets)
            {
                var btn = new Button();
                btn.Tag = sheet.Name;
                btn.Text = sheet.Name;
                btn.Height = 50;
                btn.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                btn.Margin = new Padding(5);

                // Gắn sheet name vào sự kiện click
                btn.Click += (sender, e) =>
                {
                    _sheetName = sheet.Name;
                    // Gọi hàm xử lý sheet theo tên
                    HandleSheetClick(sheet.Name);
                };

                int row = index / colCount;
                int col = index % colCount;

                // Nếu cần thêm dòng mới
                if (tlpTableGen.RowCount <= row)
                {
                    tlpTableGen.RowStyles.Add(new RowStyle(SizeType.Absolute, 60)); // Chiều cao dòng cố định
                    tlpTableGen.RowCount++;
                }

                tlpTableGen.Controls.Add(btn, col, row);

                index++;
            }

            tlpTableGen.ResumeLayout();
        }

        private void HandleSheetClick(string sheetName)
        {
            if (_currentExcelPackage == null)
            {
                MessageBox.Show("Excel chưa được load.");
                return;
            }

            var sheet = _currentExcelPackage.Workbook.Worksheets[sheetName];

            if (sheet == null)
            {
                MessageBox.Show($"Không tìm thấy sheet: {sheetName}");
                return;
            }

            var data = GetColumnNameTitleMap(sheet);
            if (data == null || !data.Any())
                MessageBox.Show($"{sheetName} không có dữ liệu");

            var dicTmp = new Dictionary<string, string>();

            dicTmp.Add("P_STTExport", "STT");

            foreach (var item in data)
            {
                dicTmp.Add($"P_{item.Key}", item.Value);
            }

            var headerForm = new HeaderMappingForm(dicTmp);
            if (headerForm.ShowDialog() == DialogResult.OK)
            {
                var headers = headerForm.HeaderMappings;
                var title = headerForm.Title;
                var maBieuMau = headerForm.MaBieuMau;

                ExportExcel(headers, title, maBieuMau);
            }
        }

        private Dictionary<string, string> GetColumnNameTitleMap(ExcelWorksheet sheet)
        {
            var dict = new Dictionary<string, string>();

            if (sheet == null || sheet.Dimension == null)
                return dict;

            int colCount = sheet.Dimension.End.Column;
            int rowCount = sheet.Dimension.End.Row;

            int colIndexColumnName = -1;
            int colIndexColumnTitle = -1;

            // Tìm vị trí cột có tiêu đề "ColumnName" và "ColumnTitle" ở dòng đầu tiên
            for (int col = 1; col <= colCount; col++)
            {
                string header = sheet.Cells[1, col].Text.Trim();

                if (header == "ColumnName")
                    colIndexColumnName = col;
                else if (header == "ColumnTitle")
                    colIndexColumnTitle = col;
            }

            // Nếu không tìm thấy một trong hai cột thì trả về rỗng
            if (colIndexColumnName == -1 || colIndexColumnTitle == -1)
                return dict;

            // Lấy dữ liệu từ các dòng còn lại
            for (int row = 2; row <= rowCount; row++)
            {
                string key = sheet.Cells[row, colIndexColumnName].Text.Trim();
                string value = sheet.Cells[row, colIndexColumnTitle].Text.Trim();

                if (!string.IsNullOrEmpty(key) && !dict.ContainsKey(key))
                {
                    dict.Add(key, value);
                }
            }

            var sb = new StringBuilder();
            sb.AppendLine("public class Export" + _sheetName);
            sb.AppendLine("{");
            sb.AppendLine("         public int P_STTExport { get; set; }");

            foreach (var item in dict)
            {
                if (item.Key.StartsWith("Is"))
                {
                    sb.AppendLine("         [CustomBoolean(\"Có\", \"Không\")]");
                }

                sb.AppendLine($"         public string P_{item.Key} {{ get; set; }}");
            }

            sb.AppendLine("}");

            Response = sb.ToString();
            return dict;
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
