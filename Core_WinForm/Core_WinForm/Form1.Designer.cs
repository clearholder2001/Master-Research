namespace Core_WinForm
{
	partial class Form1
	{
		/// <summary>
		/// 設計工具所需的變數。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 清除任何使用中的資源。
		/// </summary>
		/// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form 設計工具產生的程式碼

		/// <summary>
		/// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
		/// 這個方法的內容。
		/// </summary>
		private void InitializeComponent()
		{
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.modelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pointCloudToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
			this.functionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.regenerateRangeImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.functionToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(829, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(38, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// loadToolStripMenuItem
			// 
			this.loadToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.modelToolStripMenuItem,
            this.pointCloudToolStripMenuItem});
			this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
			this.loadToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
			this.loadToolStripMenuItem.Text = "Load";
			// 
			// modelToolStripMenuItem
			// 
			this.modelToolStripMenuItem.Name = "modelToolStripMenuItem";
			this.modelToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
			this.modelToolStripMenuItem.Text = "Model";
			this.modelToolStripMenuItem.Click += new System.EventHandler(this.modelToolStripMenuItem_Click);
			// 
			// pointCloudToolStripMenuItem
			// 
			this.pointCloudToolStripMenuItem.Name = "pointCloudToolStripMenuItem";
			this.pointCloudToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
			this.pointCloudToolStripMenuItem.Text = "Point Cloud";
			this.pointCloudToolStripMenuItem.Click += new System.EventHandler(this.pointCloudToolStripMenuItem_Click);
			// 
			// pictureBox1
			// 
			this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBox1.Location = new System.Drawing.Point(12, 27);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(400, 400);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox1.TabIndex = 1;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.DoubleClick += new System.EventHandler(this.pictureBox1_DoubleClick);
			// 
			// pictureBox2
			// 
			this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBox2.Location = new System.Drawing.Point(418, 27);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(400, 400);
			this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox2.TabIndex = 2;
			this.pictureBox2.TabStop = false;
			this.pictureBox2.DoubleClick += new System.EventHandler(this.pictureBox2_DoubleClick);
			// 
			// richTextBox1
			// 
			this.richTextBox1.Location = new System.Drawing.Point(12, 433);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.ReadOnly = true;
			this.richTextBox1.Size = new System.Drawing.Size(806, 193);
			this.richTextBox1.TabIndex = 3;
			this.richTextBox1.Text = "";
			this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
			// 
			// hScrollBar1
			// 
			this.hScrollBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.hScrollBar1.Location = new System.Drawing.Point(0, 629);
			this.hScrollBar1.Minimum = 1;
			this.hScrollBar1.Name = "hScrollBar1";
			this.hScrollBar1.Size = new System.Drawing.Size(829, 17);
			this.hScrollBar1.TabIndex = 4;
			this.hScrollBar1.Value = 10;
			// 
			// functionToolStripMenuItem
			// 
			this.functionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.regenerateRangeImageToolStripMenuItem});
			this.functionToolStripMenuItem.Name = "functionToolStripMenuItem";
			this.functionToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
			this.functionToolStripMenuItem.Text = "Function";
			// 
			// regenerateRangeImageToolStripMenuItem
			// 
			this.regenerateRangeImageToolStripMenuItem.Name = "regenerateRangeImageToolStripMenuItem";
			this.regenerateRangeImageToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
			this.regenerateRangeImageToolStripMenuItem.Text = "Regenerate Range Image";
			this.regenerateRangeImageToolStripMenuItem.Click += new System.EventHandler(this.regenerateRangeImageToolStripMenuItem_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(829, 646);
			this.Controls.Add(this.hScrollBar1);
			this.Controls.Add(this.richTextBox1);
			this.Controls.Add(this.pictureBox2);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "Form1";
			this.Text = "Core WinForm";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem modelToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pointCloudToolStripMenuItem;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.PictureBox pictureBox2;
		private System.Windows.Forms.RichTextBox richTextBox1;
		private System.Windows.Forms.HScrollBar hScrollBar1;
		private System.Windows.Forms.ToolStripMenuItem functionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem regenerateRangeImageToolStripMenuItem;
	}
}

