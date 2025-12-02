namespace ExportData
{
    partial class GenClass
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GenClass));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.treeView = new System.Windows.Forms.TreeView();
            this.btnHeaderSetting = new System.Windows.Forms.Button();
            this.txtResponse = new FastColoredTextBoxNS.FastColoredTextBox();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtResponse)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.tableLayoutPanel1.Controls.Add(this.treeView, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnHeaderSetting, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtResponse, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1184, 711);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // treeView
            // 
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.Location = new System.Drawing.Point(3, 3);
            this.treeView.Name = "treeView";
            this.tableLayoutPanel1.SetRowSpan(this.treeView, 2);
            this.treeView.Size = new System.Drawing.Size(290, 705);
            this.treeView.TabIndex = 0;
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            // 
            // btnHeaderSetting
            // 
            this.btnHeaderSetting.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnHeaderSetting.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHeaderSetting.Location = new System.Drawing.Point(299, 668);
            this.btnHeaderSetting.Name = "btnHeaderSetting";
            this.btnHeaderSetting.Size = new System.Drawing.Size(882, 40);
            this.btnHeaderSetting.TabIndex = 1;
            this.btnHeaderSetting.Text = "Thiết lập header";
            this.btnHeaderSetting.UseVisualStyleBackColor = true;
            this.btnHeaderSetting.Click += new System.EventHandler(this.btnHeaderSetting_Click);
            // 
            // txtResponse
            // 
            this.txtResponse.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            this.txtResponse.AutoIndentCharsPatterns = "\r\n^\\s*[\\w\\.]+(\\s\\w+)?\\s*(?<range>=)\\s*(?<range>[^;]+);\r\n^\\s*(case|default)\\s*[^:]" +
    "*(?<range>:)\\s*(?<range>[^;]+);\r\n";
            this.txtResponse.AutoScrollMinSize = new System.Drawing.Size(25, 15);
            this.txtResponse.BackBrush = null;
            this.txtResponse.BracketsHighlightStrategy = FastColoredTextBoxNS.BracketsHighlightStrategy.Strategy2;
            this.txtResponse.CharHeight = 15;
            this.txtResponse.CharWidth = 7;
            this.txtResponse.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtResponse.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.txtResponse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtResponse.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.txtResponse.IsReplaceMode = false;
            this.txtResponse.Language = FastColoredTextBoxNS.Language.CSharp;
            this.txtResponse.LeftBracket = '(';
            this.txtResponse.LeftBracket2 = '{';
            this.txtResponse.Location = new System.Drawing.Point(299, 3);
            this.txtResponse.Name = "txtResponse";
            this.txtResponse.Paddings = new System.Windows.Forms.Padding(0);
            this.txtResponse.RightBracket = ')';
            this.txtResponse.RightBracket2 = '}';
            this.txtResponse.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.txtResponse.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("txtResponse.ServiceColors")));
            this.txtResponse.Size = new System.Drawing.Size(882, 659);
            this.txtResponse.TabIndex = 2;
            this.txtResponse.Zoom = 100;
            // 
            // GenClass
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 711);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "GenClass";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "GenClass";
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtResponse)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.Button btnHeaderSetting;
        private FastColoredTextBoxNS.FastColoredTextBox txtResponse;
    }
}