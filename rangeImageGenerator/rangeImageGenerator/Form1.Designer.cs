namespace rangeImageGenerator
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
			this.loadobjToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.loadxyzToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.functionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.generateRangeImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.glControl1 = new OpenTK.GLControl();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
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
			this.menuStrip1.Size = new System.Drawing.Size(797, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadobjToolStripMenuItem,
            this.loadxyzToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(38, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// loadobjToolStripMenuItem
			// 
			this.loadobjToolStripMenuItem.Name = "loadobjToolStripMenuItem";
			this.loadobjToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
			this.loadobjToolStripMenuItem.Text = "Load .obj";
			this.loadobjToolStripMenuItem.Click += new System.EventHandler(this.loadobjToolStripMenuItem_Click);
			// 
			// loadxyzToolStripMenuItem
			// 
			this.loadxyzToolStripMenuItem.Name = "loadxyzToolStripMenuItem";
			this.loadxyzToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
			this.loadxyzToolStripMenuItem.Text = "Load .xyz";
			this.loadxyzToolStripMenuItem.Click += new System.EventHandler(this.loadxyzToolStripMenuItem_Click);
			// 
			// functionToolStripMenuItem
			// 
			this.functionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generateRangeImageToolStripMenuItem});
			this.functionToolStripMenuItem.Name = "functionToolStripMenuItem";
			this.functionToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
			this.functionToolStripMenuItem.Text = "Function";
			// 
			// generateRangeImageToolStripMenuItem
			// 
			this.generateRangeImageToolStripMenuItem.Name = "generateRangeImageToolStripMenuItem";
			this.generateRangeImageToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
			this.generateRangeImageToolStripMenuItem.Text = "Generate Range Image";
			this.generateRangeImageToolStripMenuItem.Click += new System.EventHandler(this.generateRangeImageToolStripMenuItem_Click);
			// 
			// glControl1
			// 
			this.glControl1.AutoScroll = true;
			this.glControl1.AutoSize = true;
			this.glControl1.BackColor = System.Drawing.Color.Transparent;
			this.glControl1.Location = new System.Drawing.Point(0, 333);
			this.glControl1.Name = "glControl1";
			this.glControl1.Size = new System.Drawing.Size(1082, 563);
			this.glControl1.TabIndex = 1;
			this.glControl1.Visible = false;
			this.glControl1.VSync = false;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Location = new System.Drawing.Point(0, 27);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(400, 300);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox1.TabIndex = 2;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
			// 
			// pictureBox2
			// 
			this.pictureBox2.Location = new System.Drawing.Point(397, 27);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(400, 300);
			this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox2.TabIndex = 3;
			this.pictureBox2.TabStop = false;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(797, 565);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.pictureBox2);
			this.Controls.Add(this.menuStrip1);
			this.Controls.Add(this.glControl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "Form1";
			this.Text = "Range Image Generator";
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
		private System.Windows.Forms.ToolStripMenuItem loadobjToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem loadxyzToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem functionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem generateRangeImageToolStripMenuItem;
		private OpenTK.GLControl glControl1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.PictureBox pictureBox2;
	}
}

