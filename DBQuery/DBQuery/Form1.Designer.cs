namespace DBQuery
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
			this.components = new System.ComponentModel.Container();
			this.button1 = new System.Windows.Forms.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.button2 = new System.Windows.Forms.Button();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textBox3 = new System.Windows.Forms.TextBox();
			this.button3 = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
			this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
			this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
			this.textBox4 = new System.Windows.Forms.TextBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.button7 = new System.Windows.Forms.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.button4 = new System.Windows.Forms.Button();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.button5 = new System.Windows.Forms.Button();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.comboBox2 = new System.Windows.Forms.ComboBox();
			this.label8 = new System.Windows.Forms.Label();
			this.listView1 = new System.Windows.Forms.ListView();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.checkBox2 = new System.Windows.Forms.CheckBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.textBox6 = new System.Windows.Forms.TextBox();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
			this.textBox5 = new System.Windows.Forms.TextBox();
			this.button6 = new System.Windows.Forms.Button();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.featureVisualizationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openImageFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openSourcePathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openTargetPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openWebPageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.groupBox1.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.contextMenuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(6, 78);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(164, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = "Search File";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(6, 50);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(559, 22);
			this.textBox1.TabIndex = 1;
			this.textBox1.Text = "Y:\\";
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(6, 21);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(164, 23);
			this.button2.TabIndex = 2;
			this.button2.Text = "Select Folder";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// textBox2
			// 
			this.textBox2.Location = new System.Drawing.Point(235, 21);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(53, 22);
			this.textBox2.TabIndex = 3;
			this.textBox2.Text = "txt00";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(173, 26);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(57, 12);
			this.label1.TabIndex = 4;
			this.label1.Text = "Extension: ";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(305, 26);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(38, 12);
			this.label2.TabIndex = 6;
			this.label2.Text = "Regex:";
			// 
			// textBox3
			// 
			this.textBox3.Location = new System.Drawing.Point(349, 21);
			this.textBox3.Name = "textBox3";
			this.textBox3.Size = new System.Drawing.Size(115, 22);
			this.textBox3.TabIndex = 5;
			this.textBox3.Text = ".*\\\\[0-9a-z][0-9a-z]*$";
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(6, 21);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(276, 45);
			this.button3.TabIndex = 7;
			this.button3.Text = "Create Database File (.db)";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.textBox1);
			this.groupBox1.Controls.Add(this.button1);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.button2);
			this.groupBox1.Controls.Add(this.textBox3);
			this.groupBox1.Controls.Add(this.textBox2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(11, 7);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(573, 108);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Source";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(173, 83);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(35, 12);
			this.label3.TabIndex = 7;
			this.label3.Text = "Total: ";
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripProgressBar1});
			this.statusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
			this.statusStrip1.Location = new System.Drawing.Point(0, 617);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(596, 22);
			this.statusStrip1.SizingGrip = false;
			this.statusStrip1.TabIndex = 9;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// toolStripStatusLabel1
			// 
			this.toolStripStatusLabel1.AutoSize = false;
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			this.toolStripStatusLabel1.Size = new System.Drawing.Size(375, 17);
			this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// toolStripProgressBar1
			// 
			this.toolStripProgressBar1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.toolStripProgressBar1.Name = "toolStripProgressBar1";
			this.toolStripProgressBar1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.toolStripProgressBar1.Size = new System.Drawing.Size(200, 16);
			// 
			// backgroundWorker1
			// 
			this.backgroundWorker1.WorkerReportsProgress = true;
			this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
			this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
			this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
			// 
			// backgroundWorker2
			// 
			this.backgroundWorker2.WorkerReportsProgress = true;
			this.backgroundWorker2.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker2_DoWork);
			this.backgroundWorker2.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker2_ProgressChanged);
			this.backgroundWorker2.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker2_RunWorkerCompleted);
			// 
			// textBox4
			// 
			this.textBox4.Location = new System.Drawing.Point(6, 72);
			this.textBox4.Name = "textBox4";
			this.textBox4.ReadOnly = true;
			this.textBox4.Size = new System.Drawing.Size(559, 22);
			this.textBox4.TabIndex = 7;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.button7);
			this.groupBox2.Controls.Add(this.label7);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.button4);
			this.groupBox2.Controls.Add(this.checkBox1);
			this.groupBox2.Controls.Add(this.button3);
			this.groupBox2.Controls.Add(this.textBox4);
			this.groupBox2.Location = new System.Drawing.Point(11, 121);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(573, 131);
			this.groupBox2.TabIndex = 10;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Database";
			// 
			// button7
			// 
			this.button7.Location = new System.Drawing.Point(289, 21);
			this.button7.Name = "button7";
			this.button7.Size = new System.Drawing.Size(276, 45);
			this.button7.TabIndex = 10;
			this.button7.Text = "Create FileList File (.csv)";
			this.button7.UseVisualStyleBackColor = true;
			this.button7.Click += new System.EventHandler(this.button7_Click);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(459, 105);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(28, 12);
			this.label7.TabIndex = 9;
			this.label7.Text = "Fail: ";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(311, 105);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(74, 12);
			this.label4.TabIndex = 8;
			this.label4.Text = "Database Size: ";
			// 
			// button4
			// 
			this.button4.Location = new System.Drawing.Point(6, 100);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(164, 23);
			this.button4.TabIndex = 8;
			this.button4.Text = "Load Database";
			this.button4.UseVisualStyleBackColor = true;
			this.button4.Click += new System.EventHandler(this.button4_Click);
			// 
			// checkBox1
			// 
			this.checkBox1.AutoCheck = false;
			this.checkBox1.Location = new System.Drawing.Point(175, 97);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(102, 31);
			this.checkBox1.TabIndex = 0;
			this.checkBox1.Text = "Database Ready";
			this.checkBox1.UseVisualStyleBackColor = true;
			// 
			// button5
			// 
			this.button5.Location = new System.Drawing.Point(6, 21);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(164, 23);
			this.button5.TabIndex = 9;
			this.button5.Text = "Load Input";
			this.button5.UseVisualStyleBackColor = true;
			this.button5.Click += new System.EventHandler(this.button5_Click);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.comboBox2);
			this.groupBox3.Controls.Add(this.label8);
			this.groupBox3.Controls.Add(this.listView1);
			this.groupBox3.Controls.Add(this.groupBox4);
			this.groupBox3.Controls.Add(this.textBox5);
			this.groupBox3.Controls.Add(this.button6);
			this.groupBox3.Controls.Add(this.button5);
			this.groupBox3.Location = new System.Drawing.Point(11, 258);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(573, 355);
			this.groupBox3.TabIndex = 11;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Query and Rank";
			// 
			// comboBox2
			// 
			this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox2.FormattingEnabled = true;
			this.comboBox2.Location = new System.Drawing.Point(53, 79);
			this.comboBox2.Name = "comboBox2";
			this.comboBox2.Size = new System.Drawing.Size(117, 20);
			this.comboBox2.TabIndex = 12;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(16, 82);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(31, 12);
			this.label8.TabIndex = 10;
			this.label8.Text = "Table";
			// 
			// listView1
			// 
			this.listView1.FullRowSelect = true;
			this.listView1.GridLines = true;
			this.listView1.Location = new System.Drawing.Point(175, 50);
			this.listView1.MultiSelect = false;
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(389, 299);
			this.listView1.TabIndex = 10;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.Details;
			this.listView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseClick);
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.checkBox2);
			this.groupBox4.Controls.Add(this.label5);
			this.groupBox4.Controls.Add(this.label6);
			this.groupBox4.Controls.Add(this.textBox6);
			this.groupBox4.Controls.Add(this.comboBox1);
			this.groupBox4.Controls.Add(this.checkedListBox1);
			this.groupBox4.Location = new System.Drawing.Point(6, 107);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(164, 243);
			this.groupBox4.TabIndex = 11;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Option";
			// 
			// checkBox2
			// 
			this.checkBox2.AutoSize = true;
			this.checkBox2.Checked = true;
			this.checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBox2.Location = new System.Drawing.Point(12, 215);
			this.checkBox2.Name = "checkBox2";
			this.checkBox2.Size = new System.Drawing.Size(80, 16);
			this.checkBox2.TabIndex = 5;
			this.checkBox2.Text = "Size Weight";
			this.checkBox2.UseVisualStyleBackColor = true;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(10, 139);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(39, 12);
			this.label5.TabIndex = 4;
			this.label5.Text = "Weight";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(10, 22);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(39, 12);
			this.label6.TabIndex = 3;
			this.label6.Text = "Feature";
			// 
			// textBox6
			// 
			this.textBox6.Location = new System.Drawing.Point(12, 187);
			this.textBox6.Name = "textBox6";
			this.textBox6.Size = new System.Drawing.Size(140, 22);
			this.textBox6.TabIndex = 2;
			this.textBox6.TextChanged += new System.EventHandler(this.textBox6_TextChanged);
			// 
			// comboBox1
			// 
			this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Location = new System.Drawing.Point(12, 159);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(140, 20);
			this.comboBox1.TabIndex = 1;
			this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
			// 
			// checkedListBox1
			// 
			this.checkedListBox1.CheckOnClick = true;
			this.checkedListBox1.FormattingEnabled = true;
			this.checkedListBox1.Location = new System.Drawing.Point(12, 42);
			this.checkedListBox1.Name = "checkedListBox1";
			this.checkedListBox1.Size = new System.Drawing.Size(140, 89);
			this.checkedListBox1.TabIndex = 0;
			// 
			// textBox5
			// 
			this.textBox5.Location = new System.Drawing.Point(175, 21);
			this.textBox5.Name = "textBox5";
			this.textBox5.ReadOnly = true;
			this.textBox5.Size = new System.Drawing.Size(389, 22);
			this.textBox5.TabIndex = 9;
			// 
			// button6
			// 
			this.button6.Location = new System.Drawing.Point(6, 50);
			this.button6.Name = "button6";
			this.button6.Size = new System.Drawing.Size(164, 23);
			this.button6.TabIndex = 9;
			this.button6.Text = "Query";
			this.button6.UseVisualStyleBackColor = true;
			this.button6.Click += new System.EventHandler(this.button6_Click);
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.featureVisualizationToolStripMenuItem,
            this.openImageFileToolStripMenuItem,
            this.openSourcePathToolStripMenuItem,
            this.openTargetPathToolStripMenuItem,
            this.openWebPageToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(191, 114);
			// 
			// featureVisualizationToolStripMenuItem
			// 
			this.featureVisualizationToolStripMenuItem.Name = "featureVisualizationToolStripMenuItem";
			this.featureVisualizationToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
			this.featureVisualizationToolStripMenuItem.Text = "Feature Visualization";
			this.featureVisualizationToolStripMenuItem.Click += new System.EventHandler(this.featureVisualizationToolStripMenuItem_Click);
			// 
			// openImageFileToolStripMenuItem
			// 
			this.openImageFileToolStripMenuItem.Name = "openImageFileToolStripMenuItem";
			this.openImageFileToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
			this.openImageFileToolStripMenuItem.Text = "Open Image File";
			this.openImageFileToolStripMenuItem.Click += new System.EventHandler(this.openImageFileToolStripMenuItem_Click);
			// 
			// openSourcePathToolStripMenuItem
			// 
			this.openSourcePathToolStripMenuItem.Name = "openSourcePathToolStripMenuItem";
			this.openSourcePathToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
			this.openSourcePathToolStripMenuItem.Text = "Open Source Path";
			this.openSourcePathToolStripMenuItem.Click += new System.EventHandler(this.openSourcePathToolStripMenuItem_Click);
			// 
			// openTargetPathToolStripMenuItem
			// 
			this.openTargetPathToolStripMenuItem.Name = "openTargetPathToolStripMenuItem";
			this.openTargetPathToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
			this.openTargetPathToolStripMenuItem.Text = "Open Target Path";
			this.openTargetPathToolStripMenuItem.Click += new System.EventHandler(this.openTargetPathToolStripMenuItem_Click);
			// 
			// openWebPageToolStripMenuItem
			// 
			this.openWebPageToolStripMenuItem.Name = "openWebPageToolStripMenuItem";
			this.openWebPageToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
			this.openWebPageToolStripMenuItem.Text = "Open Web Page";
			this.openWebPageToolStripMenuItem.Click += new System.EventHandler(this.openWebPageToolStripMenuItem_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(596, 639);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "Form1";
			this.Text = "Database Query";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.contextMenuStrip1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBox3;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.ComponentModel.BackgroundWorker backgroundWorker1;
		private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.ComponentModel.BackgroundWorker backgroundWorker2;
		private System.Windows.Forms.TextBox textBox4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.Button button5;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Button button6;
		private System.Windows.Forms.TextBox textBox5;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem featureVisualizationToolStripMenuItem;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.CheckBox checkBox2;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textBox6;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.CheckedListBox checkedListBox1;
		private System.Windows.Forms.ToolStripMenuItem openImageFileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openSourcePathToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openTargetPathToolStripMenuItem;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.ToolStripMenuItem openWebPageToolStripMenuItem;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.ComboBox comboBox2;
		private System.Windows.Forms.Button button7;
	}
}

