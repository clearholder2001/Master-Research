using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
//using System.Data;
using System.Drawing;
using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

using Core;

namespace Core_WinForm_Solution
{
	public partial class Form1 : Form
	{
		/// <summary>
		/// global variable
		/// </summary>
		private Project project;
		private bool selectPointCloud;
		private bool allinOneClickSignal;
		private Stopwatch sw;

		public Form1()
		{
			InitializeComponent();
		}


		private void Form1_Load(object sender, EventArgs e)
		{
			project = new Project();

			for (int i = 0; i < Enum.GetValues(typeof(Project.parameterName)).Length - 1; i++)
			{
				comboBox2.Items.Add(Enum.GetName(typeof(Project.parameterName), i));
			}
			comboBox2.SelectedIndex = 0;
			textBox2.Text = project.parameter[0].ToString();


			for (int i = 0; i < Enum.GetValues(typeof(Emgu.CV.CvEnum.CV_ELEMENT_SHAPE)).Length; i++)
			{
				comboBox3.Items.Add(Enum.GetNames(typeof(Emgu.CV.CvEnum.CV_ELEMENT_SHAPE))[i]);
			}
			comboBox3.SelectedIndex = 0;

			for (int i = 0; i < coreClass.featureDimension; i++)
			{
				checkedListBox1.Items.Add(Enum.GetName(typeof(coreClass.feature), i));
				comboBox1.Items.Add(Enum.GetName(typeof(coreClass.feature), i));
			}
			checkedListBox1.SetItemChecked(0, true);
			checkedListBox1.SetItemChecked(1, true);
			checkedListBox1.SetItemChecked(3, true);
			comboBox1.SelectedIndex = 0;

			textBox1.Text = project.featureWeight[comboBox1.SelectedIndex].ToString();


			allinOneClickSignal = false;

			sw = new Stopwatch();
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			var result = MessageBox.Show("Do you really want to close the program？", "Exit", MessageBoxButtons.YesNo);
			if (result == DialogResult.Yes)
			{
				e.Cancel = false;
			}
			else
			{
				e.Cancel = true;
			}
		}

		private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Form1 newForm = new Form1();
			newForm.Show();
		}

		private void pointCloudQueryInputToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "xyz files (*.xyz)|*.xyz";
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				if (!project.Set_parameter())
				{
					MessageBox.Show("Invalid parameter!");
				}
				else
				{
					project.Load_pointCloud(ofd.FileName);
					pictureBox1.Image = project.Get_rangeImage();

					project.ifGenerateFeature = false;

					toolStripStatusLabel1.Text = "Load point cloud done.";
				}
			}
		}

		private void polygonalModelDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Multiselect = true;
			ofd.Filter = "obj files (*.obj)|*.obj";
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				if (!project.Set_parameter())
				{
					MessageBox.Show("Invalid parameter!");
				}
				else
				{
					foreach (string fileName in ofd.FileNames)
					{
						project.Add_polygonalModel_fileName(fileName);
						listBox1.Items.Add(Path.GetFileNameWithoutExtension(fileName));

						project.ifGenerateFeature = false;

						toolStripStatusLabel1.Text = "Load polygonal model to list done.";
					}
				}
			}
		}

		private void updateParametersToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!project.Set_parameter())
			{
				MessageBox.Show("Invalid parameter!");
			}
			else
			{
				project.Reload_pointCloud();
				Bitmap img = project.Get_rangeImage();
				if (img != null)
				{
					pictureBox1.Image = img;
				}

				project.ifGenerateFeature = false;
				listView2.Clear();

				toolStripStatusLabel1.Text = "Update parameters done.";
			}
		}

		private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
		{
			if (backgroundWorker_loadPolygonalModel.IsBusy || backgroundWorker_generateFeature.IsBusy)
				return;

			if (e.Button == MouseButtons.Right)
			{
				if (project.ifGenerateFeature)
				{
					selectPointCloud = true;
					contextMenuStrip_after.Show(Cursor.Position);
				}
				else
				{
					return;
				}
			}
		}

		private void pictureBox1_DoubleClick(object sender, EventArgs e)
		{
			SaveFileDialog sfd = new SaveFileDialog();
			//sfd.Filter = "bmp files (*.bmp)|*.bmp";
			sfd.Filter = "tif files (*.tif)|*.tif";
			if (sfd.ShowDialog() == DialogResult.OK)
			{
				pictureBox1.Image.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
			}
		}

		private void listView1_MouseClick(object sender, MouseEventArgs e)
		{
			if (backgroundWorker_loadPolygonalModel.IsBusy || backgroundWorker_generateFeature.IsBusy)
				return;

			if (e.Button == MouseButtons.Right)
			{
				if (listView1.FocusedItem.Bounds.Contains(e.Location) == true)
				{
					if (project.ifGenerateFeature)
					{
						selectPointCloud = false;
						contextMenuStrip_after.Show(Cursor.Position);
					}
					else
					{
						contextMenuStrip_before.Show(Cursor.Position);
					}
				}
			}
		}

		private void loadPolygonalModelsFromListToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (backgroundWorker_loadPolygonalModel.IsBusy)
				return;

			if (listBox1.Items.Count == 0 && project.Return_polygonalModelFileName().Count == 0)
			{
				MessageBox.Show("Please load file first!");
				return;
			}

			sw.Restart();

			updateParametersToolStripMenuItem.PerformClick();

			toolStripProgressBar1.Value = 0;
			toolStripProgressBar1.Maximum = project.Return_polygonalModelFileName().Count;

			toolStripStatusLabel1.Text = "Loading polygonal model......(0 / " + toolStripProgressBar1.Maximum.ToString() + ")";

			imageList1.Images.Clear();
			listView1.Items.Clear();
			listView2.Items.Clear();

			backgroundWorker_loadPolygonalModel.RunWorkerAsync();
		}

		private void button_removePolygonalModel_Click(object sender, EventArgs e)
		{
			if (listBox1.SelectedIndex != -1)
			{
				//記得button和listview要一起改
				int selectedIndex = listBox1.SelectedIndex;
				List<string> fileNameList = project.Return_polygonalModelFileName();
				string fileName = fileNameList[selectedIndex];
				if (!project.Delete_polygonalModel(fileName))
					MessageBox.Show("Remove failed!");
				listBox1.Items.RemoveAt(selectedIndex);
			}
		}

		private void button_removeAllPolygonalModel_Click(object sender, EventArgs e)
		{
			if (listBox1.Items.Count != 0)
			{
				//記得button和listview要一起改
				List<string> fileNameList = project.Return_polygonalModelFileName();

				foreach (int selectedIndex in Enumerable.Range(0, listBox1.Items.Count))
				{
					string fileName = fileNameList[selectedIndex];
					if (!project.Delete_polygonalModel(fileName))
						MessageBox.Show("Remove failed!");
				}

				listBox1.Items.Clear();
			}
		}

		private void removeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//記得button和listview要一起改
			int selectedIndex = listView1.FocusedItem.Index;
			List<string> fileNameList = project.Return_polygonalModelFileName();
			if (fileNameList.Count == 0)
			{
				MessageBox.Show("Remove failed!");
				return;
			}
			string fileName = fileNameList[selectedIndex];
			if (!project.Delete_polygonalModel(fileName))
			{
				MessageBox.Show("Remove failed!");
				return;
			}
			listBox1.Items.RemoveAt(selectedIndex);
			listView1.Items.RemoveAt(selectedIndex);
			imageList1.Images.RemoveByKey(Path.GetFileNameWithoutExtension(fileName));
		}

		private void generateAnnularFeatureToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (backgroundWorker_loadPolygonalModel.IsBusy)
				return;

			if (pictureBox1.Image == null || project.Return_polygonalModelFileName().Count == 0 || listView1.Items.Count == 0)
			{
				MessageBox.Show("Please load file first!");
				return;
			}

			sw.Restart();

			updateParametersToolStripMenuItem.PerformClick();

			toolStripProgressBar1.Value = 0;
			toolStripProgressBar1.Maximum = project.Return_polygonalModelFileName().Count + 1;

			toolStripStatusLabel1.Text = "Generate annular feature......(0 / " + toolStripProgressBar1.Maximum.ToString() + ")";

			backgroundWorker_generateFeature.RunWorkerAsync();
		}

		private void generateAnnularFeatureonlyForPointCloudToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (pictureBox1.Image == null)
			{
				MessageBox.Show("Please load file first!");
				return;
			}

			sw.Restart();

			updateParametersToolStripMenuItem.PerformClick();

			project.Generate_feature_onlyPointCloud();

			sw.Stop();

			project.ifGenerateFeature = true;

			toolStripStatusLabel1.Text = "Generate annular feature done. (" + String.Format("{0:0.000}", sw.Elapsed.TotalSeconds) + "s)";
		}

		private void detailToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Form2 DetailForm;

			if (selectPointCloud)
			{
				xyzClass pointCloud = project.Return_pointCloudData();
				DetailForm = new Form2(pointCloud);
			}
			else
			{
				objClass polygonalModel = project.Return_polygonalModelData(listView1.FocusedItem.Index);
				DetailForm = new Form2(polygonalModel);
			}

			DetailForm.Show();
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			try
			{
				project.featureWeight[comboBox1.SelectedIndex] = Convert.ToDouble(textBox1.Text);
			}
			catch (Exception)
			{
				MessageBox.Show("Invalid number!");
			}
		}

		private void textBox2_TextChanged(object sender, EventArgs e)
		{
			switch (comboBox2.SelectedIndex)
			{
				case 0:
					try
					{
						project.parameter[comboBox2.SelectedIndex] = Convert.ToDouble(textBox2.Text);
					}
					catch (Exception)
					{
						MessageBox.Show("Invalid number!");
					}
					break;
				default:
					try
					{
						project.parameter[comboBox2.SelectedIndex] = Convert.ToInt16(textBox2.Text);
					}
					catch (Exception)
					{
						MessageBox.Show("Invalid number!");
					}
					break;
			}
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			textBox1.Text = project.featureWeight[comboBox1.SelectedIndex].ToString();
		}

		private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
		{
			textBox2.Text = project.parameter[comboBox2.SelectedIndex].ToString();
		}

		private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch (comboBox3.SelectedIndex)
			{
				case 0:
					project.parameter[(int)Project.parameterName.Structure_Element_Shape] = 0;
					break;
				case 1:
					project.parameter[(int)Project.parameterName.Structure_Element_Shape] = 1;
					break;
				case 2:
					project.parameter[(int)Project.parameterName.Structure_Element_Shape] = 2;
					break;
				case 3:
					project.parameter[(int)Project.parameterName.Structure_Element_Shape] = 100;
					break;
				default:
					break;
			}
		}

		private void allInOneClickToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (pictureBox1.Image == null || listBox1.Items.Count == 0 || project.Return_polygonalModelFileName().Count == 0)
			{
				MessageBox.Show("Please load file first!");
				return;
			}

			allinOneClickSignal = true;

			loadPolygonalModelsFromListToolStripMenuItem.PerformClick();
		}

		private void retrieveAndRankToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (pictureBox1.Image == null || project.Return_polygonalModelFileName().Count == 0)
			{
				MessageBox.Show("Please load file first!");
				return;
			}
			else if (project.ifGenerateFeature == false)
			{
				MessageBox.Show("Please generate annular feature first!");
				return;
			}

			if (!allinOneClickSignal)
				sw.Restart();

			for (int i = 0; i < coreClass.featureDimension; i++)
			{
				project.usedFeature[i] = checkedListBox1.GetItemChecked(i);
			}

			project.sizeWeight = checkBox1.Checked;

			listView2.Clear();

			project.Retrieve_and_Rank();
			int[] rankingIndex = project.rankingIndex;
			double[] rankingDistance = project.rankingDistance;

			for (int i = 0; i < rankingIndex.Length; i++)
			{
				Bitmap img = utilClass.ResizeBitmap2Square(project.Get_rangeImage(rankingIndex[i]));
				listView2.Items.Add((i + 1) + ": " + Path.GetFileNameWithoutExtension(listBox1.Items[rankingIndex[i]].ToString()) + " (" + String.Format("{0:0.000}", rankingDistance[rankingIndex[i]]) + ")");
				listView2.Items[i].ImageIndex = rankingIndex[i];
			}

			if (allinOneClickSignal)
			{
				MessageBox.Show("Task Complete!");
				allinOneClickSignal = false;
				sw.Stop();
				toolStripStatusLabel1.Text = "All in one click done. (" + String.Format("{0:0.000}", sw.Elapsed.TotalSeconds) + "s)";
			}
			else
			{
				sw.Stop();
				toolStripStatusLabel1.Text = "Retrieve and rank done. (" + String.Format("{0:0.000}", sw.Elapsed.TotalSeconds) + "s)";
			}
		}

		private void cancelToolStripMenuItem_Click(object sender, EventArgs e)
		{
			allinOneClickSignal = false;
			backgroundWorker_loadPolygonalModel.CancelAsync();
			backgroundWorker_generateFeature.CancelAsync();
			sw.Stop();
		}

		//backgroundWorker_loadPolygonalModel
		private void backgroundWorker_loadPolygonalModel_DoWork(object sender, DoWorkEventArgs e)
		{
			project.Load_allPolygonalModel_withProgress(backgroundWorker_loadPolygonalModel, e);
		}

		private void backgroundWorker_loadPolygonalModel_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			//int i = toolStripProgressBar1.Value;
			//Bitmap img = utilClass.ResizeBitmap2Square(project.Get_rangeImage(i));
			//string key = Path.GetFileNameWithoutExtension(listBox1.Items[i].ToString());
			//imageList1.Images.Add(key, img);
			//listView1.Items.Add(key);
			//listView1.Items[i].ImageKey = key;

			toolStripProgressBar1.Value = e.ProgressPercentage;
			toolStripStatusLabel1.Text = "Loading polygonal model......(" + e.ProgressPercentage.ToString() + " / " + toolStripProgressBar1.Maximum.ToString() + ")";
		}

		private void backgroundWorker_loadPolygonalModel_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Cancelled == true)
			{
				toolStripStatusLabel1.Text = "Load polygonal model has been Canceled.";
				toolStripProgressBar1.Value = 0;
				return;
			}
			else
			{
				sw.Stop();
				toolStripStatusLabel1.Text = "Load polygonal model done. (" + String.Format("{0:0.000}", sw.Elapsed.TotalSeconds) + "s)";
			}

			for (int i = 0; i < listBox1.Items.Count; i++)
			{
				Bitmap img = utilClass.ResizeBitmap2Square(project.Get_rangeImage(i));
				string key = Path.GetFileNameWithoutExtension(listBox1.Items[i].ToString());
				imageList1.Images.Add(key, img);
				listView1.Items.Add(key);
				listView1.Items[i].ImageKey = key;
			}

			if (allinOneClickSignal)
			{
				generateAnnularFeatureToolStripMenuItem.PerformClick();
			}
		}


		//backgroundWorker_generateFeature
		private void backgroundWorker_generateFeature_DoWork(object sender, DoWorkEventArgs e)
		{
			project.Generate_feature_withProgress(backgroundWorker_generateFeature, e, checkBox_GPU_Acceleration.Checked);
		}

		private void backgroundWorker_generateFeature_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			toolStripProgressBar1.Value = e.ProgressPercentage;
			toolStripStatusLabel1.Text = "Generate annular feature......(" + e.ProgressPercentage.ToString() + " / " + toolStripProgressBar1.Maximum.ToString() + ")";
		}

		private void backgroundWorker_generateFeature_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Cancelled == true)
			{
				toolStripStatusLabel1.Text = "Load polygonal model has been Canceled.";
				toolStripProgressBar1.Value = 0;
				return;
			}
			else
			{
				sw.Stop();
				toolStripStatusLabel1.Text = "Generate annular feature done. (" + String.Format("{0:0.000}", sw.Elapsed.TotalSeconds) + "s)";
			}

			project.ifGenerateFeature = true;

			if (allinOneClickSignal)
			{
				retrieveAndRankToolStripMenuItem.PerformClick();
			}
		}

	}
}
