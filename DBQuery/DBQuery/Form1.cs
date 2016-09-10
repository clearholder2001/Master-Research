using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;

using MySql.Data.MySqlClient;

namespace DBQuery
{
	public partial class Form1 : Form
	{
		private DBQuery dbQuery;
		private const int queryMaxNum = 20;

		//------Function-by-me-----------------------------------------------------------------------------------------------

		private bool display_ranking_result()
		{
			if (dbQuery.ConnectMariaDB())
			{
				int mod4;
				string mid;
				string path;

				for (int i = 0; i < queryMaxNum; i++)
				{
					MySqlDataReader dbReader = dbQuery.QueryDetail(dbQuery.rankingIndex[i] + 1);

					dbReader.Read();

					mod4 = (int)dbReader["mod4"];
					mid = dbReader["mid"].ToString();
					path = string.Format(@"{0}{1}\{2}\{3}\{4}", textBox1.Text, mod4, mid.Substring(0, 2), mid.Substring(2, 2), mid);

					dbReader.Close();

					ListViewItem item = new ListViewItem((i + 1).ToString());
					item.SubItems.Add((dbQuery.rankingIndex[i] + 1).ToString());
					item.SubItems.Add(string.Format("{0:0.0000}", dbQuery.rankingDistance[dbQuery.rankingIndex[i]]));
					item.SubItems.Add(mid);
					item.SubItems.Add(path);

					listView1.Items.Add(item);
				}

				listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
				listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

				dbQuery.DisconnectMariaDB();

				return true;
			}
			else
			{
				return false;
			}

		}

		//------BackgroundWorker---------------------------------------------------------------------------------------------

		private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
		{
			dbQuery.dbCreator.GetFileList(backgroundWorker1);
		}

		private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			toolStripProgressBar1.Value = e.ProgressPercentage;
		}

		private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			toolStripProgressBar1.Value = toolStripProgressBar1.Maximum;
			label3.Text = "Total: " + dbQuery.dbCreator.fileListArray.Length;
			toolStripStatusLabel1.Text = "Search File: done";
		}

		private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
		{
			dbQuery.dbCreator.CreateDatabaseFile(backgroundWorker2);
		}

		private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			toolStripProgressBar1.Value = e.ProgressPercentage;
			if (e.ProgressPercentage <= dbQuery.dbCreator.fileListArray.Length / 2)
				toolStripStatusLabel1.Text = "Processing: read file...(" + e.ProgressPercentage + "/" + dbQuery.dbCreator.fileListArray.Length + ")";
			else
				toolStripStatusLabel1.Text = "Processing: SQL and write file...(" + e.ProgressPercentage + "/" + dbQuery.dbCreator.fileListArray.Length + ")";

		}

		private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			toolStripProgressBar1.Value = toolStripProgressBar1.Maximum;

			if (dbQuery.LoadDatabase())
			{
				label4.Text = "Database Size: " + dbQuery.dbRecordArray.Length;
				label7.Text = "Fail: " + dbQuery.dbCreator.failCount;
				toolStripStatusLabel1.Text = "Done.";
				checkBox1.Checked = true;
			}
			else
			{
				label4.Text = "Database Size: 0";
				toolStripStatusLabel1.Text = "Failed.";
				checkBox1.Checked = false;
			}
		}

		//------Default-----------------------------------------------------------------------------------------------------

		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			dbQuery = new DBQuery();

			if (dbQuery.sqlTableNames.Length != 0)
			{
				foreach (string name in dbQuery.sqlTableNames)
				{
					comboBox2.Items.Add(name);
				}
			}
			else
			{
				MessageBox.Show("No SQL table found!");
			}

			comboBox2.SelectedItem = "models_979687_0515";


			for (int i = 0; i < DBRecord.featureDimension; i++)
			{
				checkedListBox1.Items.Add(Enum.GetName(typeof(DBRecord.feature), i));
				comboBox1.Items.Add(Enum.GetName(typeof(DBRecord.feature), i));
			}
			checkedListBox1.SetItemChecked(0, true);
			checkedListBox1.SetItemChecked(1, true);
			checkedListBox1.SetItemChecked(3, true);
			comboBox1.SelectedIndex = 0;

			textBox6.Text = dbQuery.featureWeight[comboBox1.SelectedIndex].ToString();

			listView1.Columns.Add("Rank");
			listView1.Columns.Add("Index");
			listView1.Columns.Add("Similarity");
			listView1.Columns.Add("Mid");
			listView1.Columns.Add("Path");
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (backgroundWorker2.IsBusy)
				return;

			if (textBox1.Text == string.Empty || textBox2.Text == string.Empty)
			{
				toolStripStatusLabel1.Text = "Please select folder and fill extension.";
				return;
			}

			dbQuery.dbCreator = new DBCreator(textBox1.Text, textBox2.Text, textBox3.Text);

			toolStripProgressBar1.Value = 0;
			toolStripProgressBar1.Maximum = 100;

			toolStripStatusLabel1.Text = "Processing: search file...";
			backgroundWorker1.RunWorkerAsync();

		}

		private void button2_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog fbd = new FolderBrowserDialog();
			if (fbd.ShowDialog() == DialogResult.OK)
			{
				textBox1.Text = fbd.SelectedPath;
				toolStripStatusLabel1.Text = "Folder has been selected.";
			}
		}

		private void button3_Click(object sender, EventArgs e)
		{
			if (backgroundWorker1.IsBusy)
				return;

			if (dbQuery.dbCreator == null)
			{
				toolStripStatusLabel1.Text = "Please search file first.";
				return;
			}

			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Filter = "DB File|*.db";
			sfd.Title = "Save Database File";
			if (sfd.ShowDialog() == DialogResult.OK)
			{
				textBox4.Text = sfd.FileName;
				dbQuery.dbCreator.dbFileName = sfd.FileName;


				toolStripProgressBar1.Value = 0;
				toolStripProgressBar1.Maximum = dbQuery.dbCreator.fileListArray.Length;


				toolStripStatusLabel1.Text = "Processing: read file...(0/" + dbQuery.dbCreator.fileListArray.Length + ")";
				backgroundWorker2.RunWorkerAsync();
			}
		}

		private void button4_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "DB File|*.db";
			ofd.Title = "Open Database File";
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				textBox4.Text = ofd.FileName;

				if (dbQuery.LoadDatabase(ofd.FileName))
				{
					label4.Text = "Database Size: " + dbQuery.dbRecordArray.Length;
					toolStripStatusLabel1.Text = "Load Database: done";
					checkBox1.Checked = true;
				}
				else
				{
					label4.Text = "Database Size: 0";
					toolStripStatusLabel1.Text = "Load Database: failed";
					checkBox1.Checked = false;
				}
			}
		}

		private void button5_Click(object sender, EventArgs e)
		{
			if (!checkBox1.Checked)
			{
				toolStripStatusLabel1.Text = "No database ready.";
				return;
			}

			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Title = "Load Input File";
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				textBox5.Text = ofd.FileName;

				if (dbQuery.LoadInput(ofd.FileName))
					toolStripStatusLabel1.Text = "Load Input File: done";
				else
					toolStripStatusLabel1.Text = "Load Input File: failed";
			}
		}

		private void button6_Click(object sender, EventArgs e)
		{
			if (!checkBox1.Checked)
			{
				toolStripStatusLabel1.Text = "No database ready.";
				return;
			}

			for (int i = 0; i < DBRecord.featureDimension; i++)
			{
				dbQuery.usedFeature[i] = checkedListBox1.GetItemChecked(i);
			}

			dbQuery.sizeWeight = checkBox2.Checked;

			listView1.Items.Clear();

			dbQuery.sqlTableName = comboBox2.SelectedItem.ToString();

			if (dbQuery.Qurey())
			{
				if (display_ranking_result())
					toolStripStatusLabel1.Text = "Query: done";
				else
					toolStripStatusLabel1.Text = "Query: failed";
			}
			else
			{
				toolStripStatusLabel1.Text = "Query: failed";
			}
		}

		private void button7_Click(object sender, EventArgs e)
		{
			//create filelist file

			if (backgroundWorker1.IsBusy)
				return;

			if (dbQuery.dbCreator == null)
			{
				toolStripStatusLabel1.Text = "Please search file first.";
				return;
			}

			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Filter = "CSV File|*.csv";
			sfd.Title = "Save FileList File";
			if (sfd.ShowDialog() == DialogResult.OK)
			{
				textBox4.Text = sfd.FileName;

				toolStripStatusLabel1.Text = "Processing: write file......";

				dbQuery.dbCreator.CreateFilelistFile(sfd.FileName);

				toolStripStatusLabel1.Text = "Done.";
			}
		}

		private void listView1_MouseClick(object sender, MouseEventArgs e)
		{
			if (listView1.Items.Count == 0)
				return;

			if (e.Button == MouseButtons.Right)
			{
				if (listView1.FocusedItem.Bounds.Contains(e.Location) == true)
				{
					contextMenuStrip1.Show(Cursor.Position);
				}
			}
		}

		private void featureVisualizationToolStripMenuItem_Click(object sender, EventArgs e)
		{
			int selectedIndex = listView1.FocusedItem.Index;

			Form2 featureForm = new Form2(dbQuery.dbInput, dbQuery.dbRecordArray[dbQuery.rankingIndex[selectedIndex]], listView1.Items[selectedIndex].SubItems[4].Text);
			featureForm.Show();

		}

		private void openImageFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			int selectedIndex = listView1.FocusedItem.Index;
			try
			{
				Process.Start(listView1.Items[selectedIndex].SubItems[4].Text + ".jpg");
			}
			catch (Exception)
			{
				MessageBox.Show("Failed!");
			}
		}

		private void openSourcePathToolStripMenuItem_Click(object sender, EventArgs e)
		{
			int selectedIndex = listView1.FocusedItem.Index;
			try
			{
				Process.Start(Path.GetDirectoryName(listView1.Items[selectedIndex].SubItems[4].Text));
			}
			catch (Exception)
			{
				MessageBox.Show("Failed!");
			}
		}

		private void openTargetPathToolStripMenuItem_Click(object sender, EventArgs e)
		{
			int selectedIndex = listView1.FocusedItem.Index;
			try
			{
				Process.Start(Path.GetDirectoryName(listView1.Items[selectedIndex].SubItems[4].Text.Replace(@"Y:\", @"Z:\")));
			}
			catch (Exception)
			{
				MessageBox.Show("Failed!");
			}
		}

		private void openWebPageToolStripMenuItem_Click(object sender, EventArgs e)
		{
			int selectedIndex = listView1.FocusedItem.Index;
			try
			{
				Process.Start("http://3dwarehouse.sketchup.com/model.html?id=" + listView1.Items[selectedIndex].SubItems[3].Text);
			}
			catch (Exception)
			{
				MessageBox.Show("Failed!");
			}
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			textBox6.Text = dbQuery.featureWeight[comboBox1.SelectedIndex].ToString();
		}

		private void textBox6_TextChanged(object sender, EventArgs e)
		{
			try
			{
				dbQuery.featureWeight[comboBox1.SelectedIndex] = Convert.ToDouble(textBox6.Text);
			}
			catch (Exception)
			{
				MessageBox.Show("Invalid number!");
			}
		}
	}
}
