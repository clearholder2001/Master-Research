using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Forms.DataVisualization.Charting;


namespace DBQuery
{
	public partial class Form2 : Form
	{
		private DBRecord dbInput, dbResult;
		private string dbResultFileName;

		private static int selectedIndex = 0;

		private void showFeature()
		{
			comboBox1.SelectedIndex = selectedIndex;

			Series series1, series2;
			ChartArea chartArea1;
			Legend legend1;

			chart1.Series.Clear();

			series1 = chart1.Series.Add("Input");
			series1.ChartType = SeriesChartType.Column;

			series2 = chart1.Series.Add("Result");
			series2.ChartType = SeriesChartType.Column;

			chart1.ChartAreas.Clear();
			chartArea1 = chart1.ChartAreas.Add("ChartArea1");

			chart1.Legends.Clear();
			legend1 = chart1.Legends.Add("Legend1");

			if (selectedIndex < DBRecord.featureDimension)
			{
				chartArea1.AxisX.Minimum = 0;
				chartArea1.AxisX.Maximum = DBRecord.annularNum + 1;
				chartArea1.AxisX.Interval = 1;
				//chartArea1.AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;

				for (int i = 0; i < DBRecord.annularNum; i++)
				{
					series1.Points.AddXY(i + 1, dbInput.featureValue[comboBox1.SelectedIndex, i]);
					series2.Points.AddXY(i + 1, dbResult.featureValue[comboBox1.SelectedIndex, i]);
				}
			}
			else
			{
				series1.IsValueShownAsLabel = true;
				series2.IsValueShownAsLabel = true;

				chartArea1.AxisX.Minimum = 0;
				chartArea1.AxisX.Maximum = 2;
				chartArea1.AxisX.Interval = 1;
				//chartArea1.AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;

				switch (selectedIndex)
				{
					case 5:
						series1.Points.AddXY(1, dbInput.zRange);
						series2.Points.AddXY(1, dbResult.zRange);
						break;
					case 6:
						series1.Points.AddXY(1, dbInput.zAvg);
						series2.Points.AddXY(1, dbResult.zAvg);
						break;
					case 7:
						series1.Points.AddXY(1, dbInput.totalArea);
						series2.Points.AddXY(1, dbResult.totalArea);
						break;
					case 8:
						series1.Points.AddXY(1, dbInput.totalVol);
						series2.Points.AddXY(1, dbResult.totalVol);
						break;
					case 9:
						series1.Points.AddXY(1, dbInput.radiusMax);
						series2.Points.AddXY(1, dbResult.radiusMax);
						break;
				}
			}
		}

		public Form2(DBRecord dbInput,DBRecord dbResult, string dbResultFileName)
		{
			InitializeComponent();

			this.dbInput = dbInput;
			this.dbResult = dbResult;
			this.dbResultFileName = dbResultFileName;
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			selectedIndex = comboBox1.SelectedIndex;

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

		private void Form2_Load(object sender, EventArgs e)
		{
			textBox1.Text = dbResultFileName;

			for (int i = 0; i < DBRecord.featureDimension; i++)
			{
				comboBox1.Items.Add(Enum.GetName(typeof(DBRecord.feature), i));
			}

			for (int i = 0; i < DBRecord.recordLength - DBRecord.featureDimension * DBRecord.annularNum; i++)
			{
				comboBox1.Items.Add(Enum.GetName(typeof(DBRecord.sizeFeature), i));
			}

			comboBox1.SelectedIndex = selectedIndex;

			richTextBox1.Text = dbInput.OutputSeries();
			richTextBox2.Text = dbResult.OutputSeries();
		}

		private void richTextBox1_DoubleClick(object sender, EventArgs e)
		{
			SaveFileDialog sfd = new SaveFileDialog();
			if (sfd.ShowDialog() == DialogResult.OK)
			{
				richTextBox1.SaveFile(sfd.FileName, RichTextBoxStreamType.PlainText);
			}
		}

		private void richTextBox2_DoubleClick(object sender, EventArgs e)
		{
			SaveFileDialog sfd = new SaveFileDialog();
			if (sfd.ShowDialog() == DialogResult.OK)
			{
				richTextBox2.SaveFile(sfd.FileName, RichTextBoxStreamType.PlainText);
			}
		}
	}
}
