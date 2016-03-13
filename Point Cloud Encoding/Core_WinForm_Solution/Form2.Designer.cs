namespace Core_WinForm_Solution
{
	partial class Form2
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
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.panAndZoomPictureBox1 = new Emgu.CV.UI.PanAndZoomPictureBox();
			this.imageBox1 = new Emgu.CV.UI.ImageBox();
			this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
			this.richTextBox2 = new System.Windows.Forms.RichTextBox();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
			((System.ComponentModel.ISupportInitialize)(this.panAndZoomPictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.imageBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// richTextBox1
			// 
			this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.richTextBox1.Location = new System.Drawing.Point(12, 434);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.ReadOnly = true;
			this.richTextBox1.Size = new System.Drawing.Size(400, 191);
			this.richTextBox1.TabIndex = 0;
			this.richTextBox1.Text = "";
			// 
			// panAndZoomPictureBox1
			// 
			this.panAndZoomPictureBox1.Location = new System.Drawing.Point(181, 117);
			this.panAndZoomPictureBox1.Name = "panAndZoomPictureBox1";
			this.panAndZoomPictureBox1.Size = new System.Drawing.Size(8, 8);
			this.panAndZoomPictureBox1.TabIndex = 1;
			this.panAndZoomPictureBox1.TabStop = false;
			// 
			// imageBox1
			// 
			this.imageBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.imageBox1.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.RightClickMenu;
			this.imageBox1.Location = new System.Drawing.Point(12, 28);
			this.imageBox1.Name = "imageBox1";
			this.imageBox1.Size = new System.Drawing.Size(400, 400);
			this.imageBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.imageBox1.TabIndex = 2;
			this.imageBox1.TabStop = false;
			this.imageBox1.DoubleClick += new System.EventHandler(this.imageBox1_DoubleClick);
			// 
			// chart1
			// 
			this.chart1.Location = new System.Drawing.Point(418, 28);
			this.chart1.Name = "chart1";
			this.chart1.Size = new System.Drawing.Size(654, 312);
			this.chart1.TabIndex = 5;
			// 
			// richTextBox2
			// 
			this.richTextBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.richTextBox2.Location = new System.Drawing.Point(418, 346);
			this.richTextBox2.Name = "richTextBox2";
			this.richTextBox2.ReadOnly = true;
			this.richTextBox2.Size = new System.Drawing.Size(654, 279);
			this.richTextBox2.TabIndex = 6;
			this.richTextBox2.Text = "";
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripComboBox1});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(1084, 25);
			this.toolStrip1.TabIndex = 7;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// toolStripComboBox1
			// 
			this.toolStripComboBox1.Name = "toolStripComboBox1";
			this.toolStripComboBox1.Size = new System.Drawing.Size(121, 25);
			this.toolStripComboBox1.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox1_SelectedIndexChanged);
			// 
			// Form2
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1084, 637);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.richTextBox2);
			this.Controls.Add(this.chart1);
			this.Controls.Add(this.imageBox1);
			this.Controls.Add(this.panAndZoomPictureBox1);
			this.Controls.Add(this.richTextBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "Form2";
			this.Text = "Feature";
			this.Load += new System.EventHandler(this.Form2_Load);
			((System.ComponentModel.ISupportInitialize)(this.panAndZoomPictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.imageBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RichTextBox richTextBox1;
		private Emgu.CV.UI.PanAndZoomPictureBox panAndZoomPictureBox1;
		private Emgu.CV.UI.ImageBox imageBox1;
		private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
		private System.Windows.Forms.RichTextBox richTextBox2;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripComboBox toolStripComboBox1;
	}
}