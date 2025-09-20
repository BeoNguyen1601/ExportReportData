namespace ExportData
{
    partial class HeaderMappingForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.FlowLayoutPanel flpContainer;
        private System.Windows.Forms.ImageList dragHandleImages;

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
            this.flpContainer = new System.Windows.Forms.FlowLayoutPanel();
            this.dragHandleImages = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // flpContainer
            // 
            this.flpContainer.AutoScroll = true;
            this.flpContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpContainer.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpContainer.Location = new System.Drawing.Point(8, 8);
            this.flpContainer.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.flpContainer.Name = "flpContainer";
            this.flpContainer.Padding = new System.Windows.Forms.Padding(8, 8, 8, 8);
            this.flpContainer.Size = new System.Drawing.Size(448, 634);
            this.flpContainer.TabIndex = 0;
            this.flpContainer.WrapContents = false;
            // 
            // dragHandleImages
            // 
            this.dragHandleImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.dragHandleImages.ImageSize = new System.Drawing.Size(16, 16);
            this.dragHandleImages.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // HeaderMappingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 650);
            this.Controls.Add(this.flpContainer);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "HeaderMappingForm";
            this.Padding = new System.Windows.Forms.Padding(8, 8, 8, 8);
            this.Text = "Thiết lập Header cho Excel";
            this.ResumeLayout(false);

        }

        #endregion
    }
}