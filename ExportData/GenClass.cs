using ExportData.DTOs;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExportData
{
    public partial class GenClass : Form
    {
        // Thêm một dictionary để cache code đã format
        private readonly Dictionary<TreeNode, string> _formattedCodeCache = new Dictionary<TreeNode, string>();

        public GenClass()
        {
            string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GenerateSuccess");

            InitializeComponent();
            LoadDirectory(treeView, basePath);
        }

        #region Private Methods

        private void LoadDirectory(TreeView treeView, string path)
        {
            treeView.Nodes.Clear();
            var rootDir = new DirectoryInfo(path);
            TreeNode rootNode = new TreeNode(rootDir.Name) { Tag = rootDir.FullName };
            treeView.Nodes.Add(rootNode);

            LoadSubDirectories(rootDir, rootNode);
        }
        private void LoadSubDirectories(DirectoryInfo dir, TreeNode node)
        {
            // Thêm thư mục con
            foreach (var subDir in dir.GetDirectories())
            {
                TreeNode subNode = new TreeNode(subDir.Name) { Tag = subDir.FullName };
                node.Nodes.Add(subNode);
                LoadSubDirectories(subDir, subNode);
            }

            // Thêm file .cs
            foreach (var file in dir.GetFiles("*.cs"))
            {
                TreeNode fileNode = new TreeNode(file.Name) { Tag = file.FullName };
                node.Nodes.Add(fileNode);
            }
        }

        #endregion

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string path = e.Node.Tag.ToString();
            if (File.Exists(path) && Path.GetExtension(path) == ".cs")
            {
                string codeText = File.ReadAllText(path);
                txtResponse.Text = codeText;
            }
        }

        private void btnHeaderSetting_Click(object sender, EventArgs e)
        {
            string text = txtResponse.Text;

            var dicPropNames = new Dictionary<string, string>();
            var matches = Regex.Matches(text, @"public\s+\w+\s+(P_\w+)\s*{");
            var propNames = matches.Cast<Match>().Select(m => m.Groups[1].Value).ToList();
            foreach (var item in propNames)
            {
                dicPropNames.Add(item, string.Empty);
            }

            if (propNames.Count == 0)
            {
                MessageBox.Show("Không tìm thấy property hợp lệ trong ViewModel");
                return;
            }

            var headerForm = new HeaderMappingForm(dicPropNames);
            if (headerForm.ShowDialog() == DialogResult.OK)
            {
                //var headers = headerForm.HeaderMappings;
                //var title = headerForm.Title;
                //var maBieuMau = headerForm.MaBieuMau;

                //ExportExcel(headers, title, maBieuMau);
            }
        }
    }
}
