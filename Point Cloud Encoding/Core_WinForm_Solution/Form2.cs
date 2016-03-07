using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;


using Emgu.CV;
using Emgu.CV.Structure;

using Core;

namespace Core_WinForm_Solution
{
	public partial class Form2 : Form
	{
		/// <summary>
		/// global variable
		/// </summary>
		private coreClass obj;

		public Form2()
		{
			InitializeComponent();
		}

		public void PassObject(coreClass obj)
		{
			this.obj = obj;
		}

		private void Form2_Load(object sender, EventArgs e)
		{
			for (int i = 0; i < coreClass.featureDimension; i++)
			{
				toolStripComboBox1.Items.Add(Enum.GetName(typeof(coreClass.feature), i));
			}

			toolStripComboBox1.SelectedIndex = 0;

			richTextBox1.Text = obj.OutputSeries();
			richTextBox2.Text = obj.OutputConsole();
		}

		private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			Bitmap img = utilClass.Data2Bitmap(obj.data2D, toolStripComboBox1.SelectedIndex);
			imageBox1.Image = new Image<Gray, byte>(img);

			chart1.Series.Clear();
			chart1.Series.Add("Series1");
			chart1.Series["Series1"].ChartType = SeriesChartType.Column;

			chart1.ChartAreas.Clear();
			chart1.ChartAreas.Add("ChartArea1");
			chart1.ChartAreas["ChartArea1"].AxisX.Minimum = 1;
			chart1.ChartAreas["ChartArea1"].AxisX.Maximum = coreClass.annularNum;
			for (int i = 0; i < coreClass.annularNum; i++)
			{
				chart1.Series["Series1"].Points.AddXY(i, obj.annularFeature[toolStripComboBox1.SelectedIndex, i]);
			}
		}

		private void imageBox1_DoubleClick(object sender, EventArgs e)
		{
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Filter = "bmp files (*.bmp)|*.bmp";
			if (sfd.ShowDialog() == DialogResult.OK)
			{
				imageBox1.Image.Save(sfd.FileName);
			}
		}
	}
}
