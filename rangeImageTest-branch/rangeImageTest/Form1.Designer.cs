namespace rangeImageTest
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
		/// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
		/// 修改這個方法的內容。
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.button1 = new System.Windows.Forms.Button();
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.label1 = new System.Windows.Forms.Label();
			this.button2 = new System.Windows.Forms.Button();
			this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
			this.label2 = new System.Windows.Forms.Label();
			this.imageBox1 = new Emgu.CV.UI.ImageBox();
			((System.ComponentModel.ISupportInitialize)(this.imageBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(12, 443);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = "Load .xyz";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// richTextBox1
			// 
			this.richTextBox1.Location = new System.Drawing.Point(12, 12);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.ReadOnly = true;
			this.richTextBox1.Size = new System.Drawing.Size(228, 425);
			this.richTextBox1.TabIndex = 3;
			this.richTextBox1.Text = "";
			this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(110, 448);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(35, 12);
			this.label1.TabIndex = 4;
			this.label1.Text = "Total: ";
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(246, 443);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(141, 23);
			this.button2.TabIndex = 6;
			this.button2.Text = "Generate range image";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// hScrollBar1
			// 
			this.hScrollBar1.Location = new System.Drawing.Point(390, 443);
			this.hScrollBar1.Maximum = 109;
			this.hScrollBar1.Minimum = 1;
			this.hScrollBar1.Name = "hScrollBar1";
			this.hScrollBar1.Size = new System.Drawing.Size(328, 23);
			this.hScrollBar1.SmallChange = 5;
			this.hScrollBar1.TabIndex = 7;
			this.hScrollBar1.Value = 10;
			this.hScrollBar1.ValueChanged += new System.EventHandler(this.hScrollBar1_ValueChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(724, 448);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(54, 12);
			this.label2.TabIndex = 7;
			this.label2.Text = "Grid Size: ";
			// 
			// imageBox1
			// 
			this.imageBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.imageBox1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
			this.imageBox1.Location = new System.Drawing.Point(246, 12);
			this.imageBox1.Name = "imageBox1";
			this.imageBox1.Size = new System.Drawing.Size(558, 425);
			this.imageBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.imageBox1.TabIndex = 2;
			this.imageBox1.TabStop = false;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(817, 500);
			this.Controls.Add(this.imageBox1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.hScrollBar1);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.richTextBox1);
			this.Controls.Add(this.button1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "Form1";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			((System.ComponentModel.ISupportInitialize)(this.imageBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.RichTextBox richTextBox1;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.HScrollBar hScrollBar1;
		private System.Windows.Forms.Label label2;
		private Emgu.CV.UI.ImageBox imageBox1;
	}
}

