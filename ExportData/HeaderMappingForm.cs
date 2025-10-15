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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExportData
{
    public partial class HeaderMappingForm : Form
    {
        public Dictionary<string, HeaderMappingInfo> HeaderMappings = new Dictionary<string, HeaderMappingInfo>();
        public string Title { get; private set; }
        public string MaBieuMau { get; private set; }

        private readonly Dictionary<string, string> _propertyNames;
        private Panel _dragSourcePanel;

        public HeaderMappingForm(Dictionary<string, string> propertyNames)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Cascadia Code", 10F, FontStyle.Regular);
            this.Width = 660;
            _propertyNames = propertyNames;
            GenerateDynamicControls();
        }

        private void GenerateDynamicControls()
        {
            flpContainer.Controls.Clear();
            AddHeaderInputs();
            int index = 0;
            foreach (var prop in _propertyNames)
            {
                string[] propText = prop.Key.Split('_');

                var val = !string.IsNullOrEmpty(prop.Value) ? prop.Value : propText[1];

                var panel = new Panel();
                panel.Width = flpContainer.ClientSize.Width - 30;
                panel.Height = 34;
                panel.Margin = new Padding(3);
                panel.BackColor = Color.WhiteSmoke;
                panel.AllowDrop = true;
                panel.Tag = prop.Key;

                panel.DragEnter += ItemPanel_DragEnter;
                panel.DragDrop += ItemPanel_DragDrop;

                // Label
                var lbl = new Label();
                lbl.Text = prop.Key;
                lbl.AutoSize = false;
                lbl.Location = new Point(5, 5);
                lbl.Width = 195;
                lbl.AutoEllipsis = true; // thêm "..." nếu quá dài
                lbl.TabStop = false;

                // TextBox
                var txt = new TextBox();
                txt.Name = "txt_" + prop.Key;
                txt.Width = 240;
                txt.Location = new Point(200, 3);
                txt.Text = !string.IsNullOrEmpty(prop.Value) ? prop.Value : string.Empty;
                txt.TabIndex = index++;
                txt.Text = VietnameseHumanizer.Convert(val);

                var grpAlign = new GroupBox();
                grpAlign.Text = "";
                grpAlign.Width = 65;
                grpAlign.Height = 30;
                grpAlign.Location = new Point(panel.Width - 115, -3);
                grpAlign.TabStop = false;

                var rbLeft = new RadioButton();
                rbLeft.Width = 15;
                rbLeft.Height = 15;
                rbLeft.Location = new Point(5, 11);
                rbLeft.Tag = "Left";
                rbLeft.TabStop = false;

                var rbCenter = new RadioButton();
                rbCenter.Width = 15;
                rbCenter.Height = 15;
                rbCenter.Location = new Point(25, 11);
                rbCenter.Checked = true;
                rbCenter.Tag = "Center";
                rbCenter.TabStop = false;

                var rbRight = new RadioButton();
                rbRight.Width = 15;
                rbRight.Height = 15;
                rbRight.Location = new Point(45, 11);
                rbRight.Tag = "Right";
                rbRight.TabStop = false;

                grpAlign.Controls.Add(rbLeft);
                grpAlign.Controls.Add(rbCenter);
                grpAlign.Controls.Add(rbRight);

                // Button kéo ⇅
                var btnDrag = new Button();
                btnDrag.Text = "⇅";
                btnDrag.Width = 30;
                btnDrag.Height = 24;
                btnDrag.TextAlign = ContentAlignment.MiddleCenter;
                btnDrag.Location = new Point(panel.Width - 150, 3);
                btnDrag.Tag = panel;
                btnDrag.MouseDown += BtnDrag_MouseDown;
                btnDrag.TabStop = false;

                // Button xóa
                var btnDelete = new Button();
                btnDelete.Text = "✖";
                btnDelete.Width = 30;
                btnDelete.Height = 24;
                btnDelete.TextAlign = ContentAlignment.MiddleCenter;
                btnDelete.Location = new Point(panel.Width - 45, 3);
                btnDelete.Tag = prop.Key;
                btnDelete.Click += BtnDelete_Click;
                btnDelete.TabStop = false;

                // Add Controls
                panel.Controls.Add(lbl);
                panel.Controls.Add(txt);
                panel.Controls.Add(grpAlign);
                panel.Controls.Add(btnDrag);
                panel.Controls.Add(btnDelete);

                flpContainer.Controls.Add(panel);
            }

            var btnSave = new Button();
            btnSave.Text = "Lưu";
            btnSave.Width = 100;
            btnSave.Height = 30;
            btnSave.Dock = DockStyle.Fill;
            btnSave.Click += BtnSave_Click;
            flpContainer.Controls.Add(btnSave);
        }
        private void AddHeaderInputs()
        {
            var panel = new Panel
            {
                Width = flpContainer.ClientSize.Width - 35,
                Height = 120,
                Margin = new Padding(5),
                BackColor = Color.LightYellow
            };

            var lbl1 = new Label
            {
                Text = "Title Excel:",
                Font = new Font("Cascadia Code", 10F),
                AutoSize = false,
                Width = panel.Width - 20,
                Height = 20,
                Location = new Point(5, 5)
            };

            var txt1 = new TextBox
            {
                Name = "tbTitleExcel",
                Width = panel.Width - 20,
                Location = new Point(5, 28),
                Font = new Font("Cascadia Code", 10F)
            };

            var lbl2 = new Label
            {
                Text = "Mã biểu mẫu:",
                Font = new Font("Cascadia Code", 10F),
                AutoSize = false,
                Width = panel.Width - 20,
                Height = 20,
                Location = new Point(5, 58)
            };

            var txt2 = new TextBox
            {
                Name = "tbMaBieuMau",
                Width = panel.Width - 20,
                Location = new Point(5, 80),
                Font = new Font("Cascadia Code", 10F)
            };

            panel.Controls.Add(lbl1);
            panel.Controls.Add(txt1);
            panel.Controls.Add(lbl2);
            panel.Controls.Add(txt2);

            flpContainer.Controls.Add(panel);
        }
        private void BtnSave_Click(object sender, EventArgs e)
        {
            HeaderMappings.Clear();
            var titleExcel = flpContainer.Controls.Find("tbTitleExcel", true).FirstOrDefault() as TextBox;
            var maBM = flpContainer.Controls.Find("tbMaBieuMau", true).FirstOrDefault() as TextBox;

            Title = "title";
            MaBieuMau = "TKB";

            if (string.IsNullOrEmpty(titleExcel.Text))
            {
                MessageBox.Show("Vui lòng nhập Title Excel");
                return;
            }

            if (string.IsNullOrEmpty(maBM.Text))
            {
                MessageBox.Show("Vui lòng nhập mã biểu mẫu");
                return;
            }

            if (titleExcel != null && !string.IsNullOrEmpty(titleExcel.Text)) Title = titleExcel.Text.ToUpper();
            if (maBM != null && !string.IsNullOrEmpty(maBM.Text)) MaBieuMau = maBM.Text.ToUpper();

            foreach (Control ctrl in flpContainer.Controls)
            {
                var panel = ctrl as Panel;
                if (panel == null) continue;

                var prop = panel.Tag as string;
                var txtBox = panel.Controls.OfType<TextBox>().FirstOrDefault();

                string selectedAlign = "Left"; // Mặc định
                var grpAlign = panel.Controls.OfType<GroupBox>().FirstOrDefault();
                if (grpAlign != null)
                {
                    foreach (var rb in grpAlign.Controls.OfType<RadioButton>())
                    {
                        if (rb.Checked && rb.Tag != null)
                        {
                            selectedAlign = rb.Tag.ToString();
                            break;
                        }
                    }
                }

                if (prop != null && txtBox != null)
                {
                    var info = new HeaderMappingInfo();
                    info.Title = txtBox.Text;
                    info.Align = selectedAlign;

                    HeaderMappings[prop] = info;
                }
            }

            ExportExcel(HeaderMappings, Title, MaBieuMau);
        }
        private void BtnDrag_MouseDown(object sender, MouseEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            var panel = btn.Tag as Panel;
            if (panel == null) return;

            _dragSourcePanel = panel;
            DoDragDrop(panel, DragDropEffects.Move);
        }
        private void ItemPanel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Panel)))
            {
                e.Effect = DragDropEffects.Move;
            }
        }
        private void ItemPanel_DragDrop(object sender, DragEventArgs e)
        {
            var target = sender as Panel;
            var source = e.Data.GetData(typeof(Panel)) as Panel;

            if (target == null || source == null || target == source) return;

            int sourceIndex = flpContainer.Controls.IndexOf(source);
            int targetIndex = flpContainer.Controls.IndexOf(target);

            flpContainer.Controls.SetChildIndex(source, targetIndex);
            flpContainer.Invalidate();
        }
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;
            var propKey = btn.Tag as string;
            if (string.IsNullOrEmpty(propKey)) return;

            // Remove from _propertyNames
            if (_propertyNames.ContainsKey(propKey))
            {
                _propertyNames.Remove(propKey);
            }

            // Remove the panel from the UI
            var panel = btn.Parent as Panel;
            if (panel != null)
            {
                flpContainer.Controls.Remove(panel);
                panel.Dispose();
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
