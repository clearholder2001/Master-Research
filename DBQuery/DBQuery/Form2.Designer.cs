namespace DBQuery
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
			this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
			this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.richTextBox2 = new System.Windows.Forms.RichTextBox();
			this.label2 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
			this.SuspendLayout();
			// 
			// chart1
			// 
			this.chart1.Location = new System.Drawing.Point(3, 27);
			this.chart1.Name = "chart1";
			this.chart1.Size = new System.Drawing.Size(740, 438);
			this.chart1.TabIndex = 1;
			this.chart1.Text = "chart1";
			// 
			// comboBox1
			// 
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Location = new System.Drawing.Point(3, 2);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(121, 20);
			this.comboBox1.TabIndex = 3;
			this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
			// 
			// textBox1
			// 
			this.textBox1.BackColor = System.Drawing.SystemColors.Control;
			this.textBox1.Location = new System.Drawing.Point(128, 1);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(615, 22);
			this.textBox1.TabIndex = 4;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(171, 468);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(30, 12);
			this.label1.TabIndex = 5;
			this.label1.Text = "Input";
			// 
			// richTextBox1
			// 
			this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.richTextBox1.Location = new System.Drawing.Point(3, 483);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.ReadOnly = true;
			this.richTextBox1.Size = new System.Drawing.Size(371, 175);
			this.richTextBox1.TabIndex = 6;
			this.richTextBox1.Text = "";
			this.richTextBox1.DoubleClick += new System.EventHandler(this.richTextBox1_DoubleClick);
			// 
			// richTextBox2
			// 
			this.richTextBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.richTextBox2.Location = new System.Drawing.Point(372, 483);
			this.richTextBox2.Name = "richTextBox2";
			this.richTextBox2.ReadOnly = true;
			this.richTextBox2.Size = new System.Drawing.Size(371, 175);
			this.richTextBox2.TabIndex = 7;
			this.richTextBox2.Text = "";
			this.richTextBox2.DoubleClick += new System.EventHandler(this.richTextBox2_DoubleClick);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(539, 468);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(34, 12);
			this.label2.TabIndex = 8;
			this.label2.Text = "Result";
			// 
			// Form2
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(743, 659);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.richTextBox2);
			this.Controls.Add(this.richTextBox1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.comboBox1);
			this.Controls.Add(this.chart1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "Form2";
			this.Text = "Feature Visualization";
			this.Load += new System.EventHandler(this.Form2_Load);
			((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
		private System.ComponentModel.BackgroundWorker backgroundWorker1;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RichTextBox richTextBox1;
		private System.Windows.Forms.RichTextBox richTextBox2;
		private System.Windows.Forms.Label label2;
	}
}