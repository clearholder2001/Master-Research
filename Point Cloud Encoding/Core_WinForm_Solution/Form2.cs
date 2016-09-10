using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
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
		private static byte selectedIndex = 0;
		

		public void showFeature()
		{
			toolStripComboBox1.SelectedIndex = selectedIndex;

			Bitmap img = utilClass.Data2Bitmap(obj.data2D, toolStripComboBox1.SelectedIndex);
			imageBox1.Image = new Image<Gray, byte>(img);

			chart1.Series.Clear();
			chart1.Series.Add("Series1");
			chart1.Series["Series1"].ChartType = SeriesChartType.Column;

			chart1.ChartAreas.Clear();
			chart1.ChartAreas.Add("ChartArea1");
			chart1.ChartAreas["ChartArea1"].AxisX.Minimum = 1;
			chart1.ChartAreas["ChartArea1"].AxisX.Maximum = coreClass.annularNum;
			chart1.ChartAreas["ChartArea1"].AxisX.Interval = 1;
			chart1.ChartAreas["ChartArea1"].AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;
			for (int i = 0; i < coreClass.annularNum; i++)
			{
				chart1.Series["Series1"].Points.AddXY(i + 1, obj.annularFeature[toolStripComboBox1.SelectedIndex, i]);
			}
		}

		public Form2(coreClass obj)
		{
			InitializeComponent();
			this.obj = obj;
		}

		private void Form2_Load(object sender, EventArgs e)
		{
			this.Text = "Feature: " + obj.fileName;

			for (int i = 0; i < coreClass.featureDimension; i++)
			{
				toolStripComboBox1.Items.Add(Enum.GetName(typeof(coreClass.feature), i));
			}

			richTextBox1.Text = obj.OutputSeries();
			richTextBox2.Text = obj.OutputConsole();

			toolStripComboBox1.SelectedIndex = selectedIndex;
		}

		private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			selectedIndex = (byte)toolStripComboBox1.SelectedIndex;

			List<Form> fList = new List<Form>();

			foreach (Form f in Application.OpenForms)
			{
				if (f.Name == "Form2")
					fList.Add(f);
			}

			foreach (Form2 f in fList)
			{
				f.showFeature();
			}
		}

		private void imageBox1_DoubleClick(object sender, EventArgs e)
		{
			SaveFileDialog sfd = new SaveFileDialog();
			//sfd.Filter = "bmp files (*.bmp)|*.bmp";
			sfd.Filter = "tif files (*.tif)|*.tif";
			if (sfd.ShowDialog() == DialogResult.OK)
			{
				imageBox1.Image.Save(sfd.FileName);
			}
		}
	}
}
