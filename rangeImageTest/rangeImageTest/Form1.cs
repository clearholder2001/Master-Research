using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using Emgu.CV;
using Emgu.CV.UI;
using Emgu.CV.Structure;
using Emgu.Util;

using ImageTool;

namespace rangeImageTest
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		//定義point cloud結構
		public struct PointCloud
		{
			public double x;
			public double y;
			public double z;
		}

		//global variables
		public double[,] pcArray;
		public PointCloud[,] maxHeightPcArray;
		public int pcNum;
		public double[] maxBoundary, minBoundary;
		public double gridSize;


		private void button1_Click(object sender, EventArgs e)
		{
			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				try
				{
					richTextBox1.LoadFile(openFileDialog1.FileName, RichTextBoxStreamType.PlainText);
					pcNum = richTextBox1.Lines.Length - 1;

					//宣告陣列大小
					pcArray = new double[pcNum, 3];
					maxBoundary = new double[3];
					minBoundary = new double[3];

					//用StreamReader讀檔
					StreamReader sr = new StreamReader(openFileDialog1.FileName);
					String[] sArray;

					//定最大最小的初始值
					sArray = sr.ReadLine().Split(' ');
					pcArray[0, 0] = Convert.ToDouble(sArray[0]);
					pcArray[0, 1] = Convert.ToDouble(sArray[1]);
					pcArray[0, 2] = Convert.ToDouble(sArray[2]);
					maxBoundary[0] = minBoundary[0] = pcArray[0, 0];
					maxBoundary[1] = minBoundary[1] = pcArray[0, 1];
					maxBoundary[2] = minBoundary[2] = pcArray[0, 2];


					//切割String並轉成double
					for (int i = 1; i < pcNum; i++)
					{
						sArray = sr.ReadLine().Split(' ');
						pcArray[i, 0] = Convert.ToDouble(sArray[0]);
						pcArray[i, 1] = Convert.ToDouble(sArray[1]);
						pcArray[i, 2] = Convert.ToDouble(sArray[2]);
						if (pcArray[i, 0] > maxBoundary[0])
							maxBoundary[0] = pcArray[i, 0];
						else if (pcArray[i, 0] < minBoundary[0])
							minBoundary[0] = pcArray[i, 0];
						if (pcArray[i, 1] > maxBoundary[1])
							maxBoundary[1] = pcArray[i, 1];
						else if (pcArray[i, 1] < minBoundary[1])
							minBoundary[1] = pcArray[i, 1];
						if (pcArray[i, 2] > maxBoundary[2])
							maxBoundary[2] = pcArray[i, 2];
						else if (pcArray[i, 2] < minBoundary[2])
							minBoundary[2] = pcArray[i, 2];
					}

					//將邊界轉為整數
					for (int i = 0; i < 3; i++)
					{
						maxBoundary[i] = Math.Ceiling(maxBoundary[i]);
						minBoundary[i] = Math.Floor(minBoundary[i]);
					}
				}
				catch (Exception)
				{
					richTextBox1.Clear();
					MessageBox.Show("Fail!");
				}

			}
		}

		private void richTextBox1_TextChanged(object sender, EventArgs e)
		{
			label1.Text = "Total: " + (richTextBox1.Lines.Length - 1).ToString();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			if (pcArray == null || maxBoundary == null || minBoundary == null)
				return;
			
			//計算range image的size
			int width = (int)((maxBoundary[0] - minBoundary[0]) / gridSize) + 1;
			int height = (int)((maxBoundary[1] - minBoundary[1]) / gridSize) + 1;
			double xMin = minBoundary[0];
			double yMin = minBoundary[1];
			double zMin = minBoundary[2];
			double zRange = maxBoundary[2] - minBoundary[2];
			label3.Text = "Width: " + width.ToString() + "\n" + "Height: " + height.ToString();

			//宣告range image紀錄最高點的array並初始化
			maxHeightPcArray = new PointCloud[width, height];
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					maxHeightPcArray[i, j].x = double.MaxValue;
					maxHeightPcArray[i, j].y = double.MaxValue;
					maxHeightPcArray[i, j].z = double.MaxValue;
				}
			}

			//宣告range image，將bitmap轉成array
			Bitmap rangeImg = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
			byte[, ,] rangeImgData = BitmapTool.Bitmap2Array(rangeImg);

			//計算range image
			int pcX, pcY, pcZ;
			for (int i = 0; i < pcNum; i++)
			{
				pcX = (int)((pcArray[i, 0] - xMin) / gridSize);
				pcY = (int)((pcArray[i, 1] - yMin) / gridSize);
				pcZ = (int)((pcArray[i, 2] - zMin) / zRange * 254) + 1;
				if (pcZ > rangeImgData[pcX, height - pcY - 1, 0])
				{
					rangeImgData[pcX, height - pcY - 1, 0] = (byte)pcZ;
					maxHeightPcArray[pcX, pcY].x = pcArray[i, 0];
					maxHeightPcArray[pcX, pcY].y = pcArray[i, 1];
					maxHeightPcArray[pcX, pcY].z = pcArray[i, 2];
				}
			}


			//將Bitmap秀出來
			rangeImg = BitmapTool.Array2Bitmap(rangeImgData, PixelFormat.Format8bppIndexed);

			Image<Gray, Byte> img = new Image<Gray, Byte>(rangeImg);
			imageBox1.Image = img;
		}

		private void hScrollBar1_ValueChanged(object sender, EventArgs e)
		{
			//改變Label的顯示
			gridSize = (double)hScrollBar1.Value / 100.0;
			label2.Text = "Grid Size: " + String.Format("{0:0.00}", gridSize);

			if (imageBox1.Image == null)
				return;

			//套用改變的網格大小
			button2_Click(sender, e);
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			//網格大小初始化
			hScrollBar1_ValueChanged(sender, e);
		}

		private void button3_Click(object sender, EventArgs e)
		{
			if (maxHeightPcArray == null)
				return;

			//將range image紀錄最高點的array存檔
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Filter = "xyz files (*.xyz)|*.xyz|All files (*.*)|*.*";
			if (sfd.ShowDialog() == DialogResult.OK)
			{
				StreamWriter sw = new StreamWriter(sfd.FileName);
				for (int i = 0; i < maxHeightPcArray.GetLength(0); i++)
				{
					for (int j = 0; j < maxHeightPcArray.GetLength(1); j++)
					{
						if (maxHeightPcArray[i, j].x != double.MaxValue && maxHeightPcArray[i, j].y != double.MaxValue && maxHeightPcArray[i, j].z != double.MaxValue)
						{
							sw.WriteLine(String.Format("{0:0.0000}", maxHeightPcArray[i, j].x) + " " + String.Format("{0:0.0000}", maxHeightPcArray[i, j].y) + " " + String.Format("{0:0.0000}", maxHeightPcArray[i, j].z));
						}
					}
				}
				sw.Close();
				MessageBox.Show("Range data have been save to\n" + sfd.FileName);
			}
		}

	}
}
