namespace Core_WinForm_Solution
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
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pointCloudQueryInputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.polygonalModelDatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.updateParametersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.workflowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.loadPolygonalModelsFromListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.generateAnnularFeatureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.retrieveAndRankToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cancelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.scheduleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.allInOneClickToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.comboBox3 = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.comboBox2 = new System.Windows.Forms.ComboBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.listView1 = new System.Windows.Forms.ListView();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
			this.contextMenuStrip_before = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.button_removePolygonalModel = new System.Windows.Forms.Button();
			this.backgroundWorker_loadPolygonalModel = new System.ComponentModel.BackgroundWorker();
			this.backgroundWorker_generateFeature = new System.ComponentModel.BackgroundWorker();
			this.contextMenuStrip_after = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.detailToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.listView2 = new System.Windows.Forms.ListView();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
			this.button_removeAllPolygonalModel = new System.Windows.Forms.Button();
			this.menuStrip1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.statusStrip1.SuspendLayout();
			this.contextMenuStrip_before.SuspendLayout();
			this.contextMenuStrip_after.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.workflowToolStripMenuItem,
            this.scheduleToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(875, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProjectToolStripMenuItem,
            this.loadToolStripMenuItem,
            this.updateParametersToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(38, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// newProjectToolStripMenuItem
			// 
			this.newProjectToolStripMenuItem.Name = "newProjectToolStripMenuItem";
			this.newProjectToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
			this.newProjectToolStripMenuItem.Text = "New Project";
			this.newProjectToolStripMenuItem.Click += new System.EventHandler(this.newProjectToolStripMenuItem_Click);
			// 
			// loadToolStripMenuItem
			// 
			this.loadToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pointCloudQueryInputToolStripMenuItem,
            this.polygonalModelDatabaseToolStripMenuItem});
			this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
			this.loadToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
			this.loadToolStripMenuItem.Text = "Load";
			// 
			// pointCloudQueryInputToolStripMenuItem
			// 
			this.pointCloudQueryInputToolStripMenuItem.Name = "pointCloudQueryInputToolStripMenuItem";
			this.pointCloudQueryInputToolStripMenuItem.Size = new System.Drawing.Size(237, 22);
			this.pointCloudQueryInputToolStripMenuItem.Text = "Point Cloud (Query Input)";
			this.pointCloudQueryInputToolStripMenuItem.Click += new System.EventHandler(this.pointCloudQueryInputToolStripMenuItem_Click);
			// 
			// polygonalModelDatabaseToolStripMenuItem
			// 
			this.polygonalModelDatabaseToolStripMenuItem.Name = "polygonalModelDatabaseToolStripMenuItem";
			this.polygonalModelDatabaseToolStripMenuItem.Size = new System.Drawing.Size(237, 22);
			this.polygonalModelDatabaseToolStripMenuItem.Text = "Polygonal Model (Database)";
			this.polygonalModelDatabaseToolStripMenuItem.Click += new System.EventHandler(this.polygonalModelDatabaseToolStripMenuItem_Click);
			// 
			// updateParametersToolStripMenuItem
			// 
			this.updateParametersToolStripMenuItem.Name = "updateParametersToolStripMenuItem";
			this.updateParametersToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
			this.updateParametersToolStripMenuItem.Text = "Update Parameters";
			this.updateParametersToolStripMenuItem.Click += new System.EventHandler(this.updateParametersToolStripMenuItem_Click);
			// 
			// workflowToolStripMenuItem
			// 
			this.workflowToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadPolygonalModelsFromListToolStripMenuItem,
            this.generateAnnularFeatureToolStripMenuItem,
            this.retrieveAndRankToolStripMenuItem,
            this.cancelToolStripMenuItem});
			this.workflowToolStripMenuItem.Name = "workflowToolStripMenuItem";
			this.workflowToolStripMenuItem.Size = new System.Drawing.Size(73, 20);
			this.workflowToolStripMenuItem.Text = "Workflow";
			// 
			// loadPolygonalModelsFromListToolStripMenuItem
			// 
			this.loadPolygonalModelsFromListToolStripMenuItem.Name = "loadPolygonalModelsFromListToolStripMenuItem";
			this.loadPolygonalModelsFromListToolStripMenuItem.Size = new System.Drawing.Size(260, 22);
			this.loadPolygonalModelsFromListToolStripMenuItem.Text = "Load Polygonal Models from List";
			this.loadPolygonalModelsFromListToolStripMenuItem.Click += new System.EventHandler(this.loadPolygonalModelsFromListToolStripMenuItem_Click);
			// 
			// generateAnnularFeatureToolStripMenuItem
			// 
			this.generateAnnularFeatureToolStripMenuItem.Name = "generateAnnularFeatureToolStripMenuItem";
			this.generateAnnularFeatureToolStripMenuItem.Size = new System.Drawing.Size(260, 22);
			this.generateAnnularFeatureToolStripMenuItem.Text = "Generate Annular Feature";
			this.generateAnnularFeatureToolStripMenuItem.Click += new System.EventHandler(this.generateAnnularFeatureToolStripMenuItem_Click);
			// 
			// retrieveAndRankToolStripMenuItem
			// 
			this.retrieveAndRankToolStripMenuItem.Name = "retrieveAndRankToolStripMenuItem";
			this.retrieveAndRankToolStripMenuItem.Size = new System.Drawing.Size(260, 22);
			this.retrieveAndRankToolStripMenuItem.Text = "Retrieve and Rank";
			this.retrieveAndRankToolStripMenuItem.Click += new System.EventHandler(this.retrieveAndRankToolStripMenuItem_Click);
			// 
			// cancelToolStripMenuItem
			// 
			this.cancelToolStripMenuItem.Name = "cancelToolStripMenuItem";
			this.cancelToolStripMenuItem.Size = new System.Drawing.Size(260, 22);
			this.cancelToolStripMenuItem.Text = "Cancel";
			this.cancelToolStripMenuItem.Click += new System.EventHandler(this.cancelToolStripMenuItem_Click);
			// 
			// scheduleToolStripMenuItem
			// 
			this.scheduleToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allInOneClickToolStripMenuItem});
			this.scheduleToolStripMenuItem.Name = "scheduleToolStripMenuItem";
			this.scheduleToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
			this.scheduleToolStripMenuItem.Text = "Schedule";
			// 
			// allInOneClickToolStripMenuItem
			// 
			this.allInOneClickToolStripMenuItem.Name = "allInOneClickToolStripMenuItem";
			this.allInOneClickToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
			this.allInOneClickToolStripMenuItem.Text = "All in One Click";
			this.allInOneClickToolStripMenuItem.Click += new System.EventHandler(this.allInOneClickToolStripMenuItem_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.comboBox3);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.textBox2);
			this.groupBox1.Controls.Add(this.comboBox2);
			this.groupBox1.Location = new System.Drawing.Point(12, 27);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(200, 146);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Parameters";
			// 
			// comboBox3
			// 
			this.comboBox3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox3.FormattingEnabled = true;
			this.comboBox3.Location = new System.Drawing.Point(41, 109);
			this.comboBox3.Name = "comboBox3";
			this.comboBox3.Size = new System.Drawing.Size(120, 20);
			this.comboBox3.TabIndex = 7;
			this.comboBox3.SelectedIndexChanged += new System.EventHandler(this.comboBox3_SelectedIndexChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(39, 86);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(119, 12);
			this.label3.TabIndex = 6;
			this.label3.Text = "Structure Element Shape";
			// 
			// textBox2
			// 
			this.textBox2.Location = new System.Drawing.Point(41, 53);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(120, 22);
			this.textBox2.TabIndex = 5;
			this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
			// 
			// comboBox2
			// 
			this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox2.FormattingEnabled = true;
			this.comboBox2.Location = new System.Drawing.Point(41, 22);
			this.comboBox2.Name = "comboBox2";
			this.comboBox2.Size = new System.Drawing.Size(120, 20);
			this.comboBox2.TabIndex = 5;
			this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
			// 
			// pictureBox1
			// 
			this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBox1.Location = new System.Drawing.Point(12, 179);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(200, 200);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox1.TabIndex = 2;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.DoubleClick += new System.EventHandler(this.pictureBox1_DoubleClick);
			this.pictureBox1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseClick);
			// 
			// listView1
			// 
			this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.listView1.LargeImageList = this.imageList1;
			this.listView1.Location = new System.Drawing.Point(218, 27);
			this.listView1.MultiSelect = false;
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(460, 677);
			this.listView1.TabIndex = 3;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseClick);
			// 
			// imageList1
			// 
			this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imageList1.ImageSize = new System.Drawing.Size(100, 100);
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripProgressBar1});
			this.statusStrip1.Location = new System.Drawing.Point(0, 707);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
			this.statusStrip1.Size = new System.Drawing.Size(875, 22);
			this.statusStrip1.SizingGrip = false;
			this.statusStrip1.TabIndex = 4;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// toolStripStatusLabel1
			// 
			this.toolStripStatusLabel1.AutoSize = false;
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			this.toolStripStatusLabel1.Size = new System.Drawing.Size(300, 17);
			this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// toolStripProgressBar1
			// 
			this.toolStripProgressBar1.Name = "toolStripProgressBar1";
			this.toolStripProgressBar1.Size = new System.Drawing.Size(200, 16);
			// 
			// contextMenuStrip_before
			// 
			this.contextMenuStrip_before.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeToolStripMenuItem});
			this.contextMenuStrip_before.Name = "contextMenuStrip_before";
			this.contextMenuStrip_before.Size = new System.Drawing.Size(122, 26);
			// 
			// removeToolStripMenuItem
			// 
			this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
			this.removeToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
			this.removeToolStripMenuItem.Text = "Remove";
			this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
			// 
			// listBox1
			// 
			this.listBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.listBox1.FormattingEnabled = true;
			this.listBox1.ItemHeight = 12;
			this.listBox1.Location = new System.Drawing.Point(12, 385);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(200, 290);
			this.listBox1.TabIndex = 6;
			// 
			// button_removePolygonalModel
			// 
			this.button_removePolygonalModel.Location = new System.Drawing.Point(12, 681);
			this.button_removePolygonalModel.Name = "button_removePolygonalModel";
			this.button_removePolygonalModel.Size = new System.Drawing.Size(75, 23);
			this.button_removePolygonalModel.TabIndex = 8;
			this.button_removePolygonalModel.Text = "Remove";
			this.button_removePolygonalModel.UseVisualStyleBackColor = true;
			this.button_removePolygonalModel.Click += new System.EventHandler(this.button_removePolygonalModel_Click);
			// 
			// backgroundWorker_loadPolygonalModel
			// 
			this.backgroundWorker_loadPolygonalModel.WorkerReportsProgress = true;
			this.backgroundWorker_loadPolygonalModel.WorkerSupportsCancellation = true;
			this.backgroundWorker_loadPolygonalModel.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_loadPolygonalModel_DoWork);
			this.backgroundWorker_loadPolygonalModel.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_loadPolygonalModel_ProgressChanged);
			this.backgroundWorker_loadPolygonalModel.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_loadPolygonalModel_RunWorkerCompleted);
			// 
			// backgroundWorker_generateFeature
			// 
			this.backgroundWorker_generateFeature.WorkerReportsProgress = true;
			this.backgroundWorker_generateFeature.WorkerSupportsCancellation = true;
			this.backgroundWorker_generateFeature.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_generateFeature_DoWork);
			this.backgroundWorker_generateFeature.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_generateFeature_ProgressChanged);
			this.backgroundWorker_generateFeature.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_generateFeature_RunWorkerCompleted);
			// 
			// contextMenuStrip_after
			// 
			this.contextMenuStrip_after.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.detailToolStripMenuItem});
			this.contextMenuStrip_after.Name = "contextMenuStrip_before";
			this.contextMenuStrip_after.Size = new System.Drawing.Size(108, 26);
			// 
			// detailToolStripMenuItem
			// 
			this.detailToolStripMenuItem.Name = "detailToolStripMenuItem";
			this.detailToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			this.detailToolStripMenuItem.Text = "Detail";
			this.detailToolStripMenuItem.Click += new System.EventHandler(this.detailToolStripMenuItem_Click);
			// 
			// listView2
			// 
			this.listView2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.listView2.LargeImageList = this.imageList1;
			this.listView2.Location = new System.Drawing.Point(684, 253);
			this.listView2.MultiSelect = false;
			this.listView2.Name = "listView2";
			this.listView2.Size = new System.Drawing.Size(180, 451);
			this.listView2.TabIndex = 9;
			this.listView2.UseCompatibleStateImageBehavior = false;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.textBox1);
			this.groupBox2.Controls.Add(this.comboBox1);
			this.groupBox2.Controls.Add(this.checkedListBox1);
			this.groupBox2.Location = new System.Drawing.Point(684, 27);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(180, 220);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Ranking Options";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(27, 139);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(39, 12);
			this.label2.TabIndex = 4;
			this.label2.Text = "Weight";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(27, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(39, 12);
			this.label1.TabIndex = 3;
			this.label1.Text = "Feature";
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(29, 187);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(120, 22);
			this.textBox1.TabIndex = 2;
			this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
			// 
			// comboBox1
			// 
			this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Location = new System.Drawing.Point(29, 159);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(120, 20);
			this.comboBox1.TabIndex = 1;
			this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
			// 
			// checkedListBox1
			// 
			this.checkedListBox1.CheckOnClick = true;
			this.checkedListBox1.FormattingEnabled = true;
			this.checkedListBox1.Location = new System.Drawing.Point(29, 42);
			this.checkedListBox1.Name = "checkedListBox1";
			this.checkedListBox1.Size = new System.Drawing.Size(120, 89);
			this.checkedListBox1.TabIndex = 0;
			// 
			// button_removeAllPolygonalModel
			// 
			this.button_removeAllPolygonalModel.Location = new System.Drawing.Point(93, 681);
			this.button_removeAllPolygonalModel.Name = "button_removeAllPolygonalModel";
			this.button_removeAllPolygonalModel.Size = new System.Drawing.Size(119, 23);
			this.button_removeAllPolygonalModel.TabIndex = 10;
			this.button_removeAllPolygonalModel.Text = "Remove ALL";
			this.button_removeAllPolygonalModel.UseVisualStyleBackColor = true;
			this.button_removeAllPolygonalModel.Click += new System.EventHandler(this.button_removeAllPolygonalModel_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(875, 729);
			this.Controls.Add(this.button_removeAllPolygonalModel);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.listView2);
			this.Controls.Add(this.button_removePolygonalModel);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.listView1);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.menuStrip1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MainMenuStrip = this.menuStrip1;
			this.MaximizeBox = false;
			this.Name = "Form1";
			this.Text = " Image-based Airborne LiDAR Point Cloud Encoding for 3D Building Model Retrieval";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.Load += new System.EventHandler(this.Form1_Load);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.contextMenuStrip_before.ResumeLayout(false);
			this.contextMenuStrip_after.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newProjectToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem polygonalModelDatabaseToolStripMenuItem;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.ToolStripMenuItem pointCloudQueryInputToolStripMenuItem;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip_before;
		private System.Windows.Forms.ToolStripMenuItem workflowToolStripMenuItem;
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
		private System.Windows.Forms.Button button_removePolygonalModel;
		private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem generateAnnularFeatureToolStripMenuItem;
		private System.ComponentModel.BackgroundWorker backgroundWorker_loadPolygonalModel;
		private System.ComponentModel.BackgroundWorker backgroundWorker_generateFeature;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip_after;
		private System.Windows.Forms.ToolStripMenuItem detailToolStripMenuItem;
		private System.Windows.Forms.ListView listView2;
		private System.Windows.Forms.ToolStripMenuItem retrieveAndRankToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem loadPolygonalModelsFromListToolStripMenuItem;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.CheckedListBox checkedListBox1;
		private System.Windows.Forms.Button button_removeAllPolygonalModel;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.ToolStripMenuItem scheduleToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem allInOneClickToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem cancelToolStripMenuItem;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.ComboBox comboBox2;
		private System.Windows.Forms.ComboBox comboBox3;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ToolStripMenuItem updateParametersToolStripMenuItem;
	}
}

