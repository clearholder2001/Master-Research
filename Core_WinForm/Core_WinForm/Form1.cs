/*
功能：
1. 製作range image

return值：
-1代表讀檔失敗
-2產生影像失敗
+1代表成功

*/

using System;
using System.IO;
// using System.Collections.Generic;
// using System.ComponentModel;
// using System.Data;
using System.Drawing;
using System.Linq;
// using System.Text;
// using System.Diagnostics;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using ImageTool;
using Emgu.CV;
using Emgu.CV.Structure;

using MathWorks.MATLAB.NET.Arrays;
using xyzMatlabTool;


namespace Core_WinForm
{
	public partial class Form1 : Form
	{
		//Global variable
		//--------------------------
		private string objFileName, xyzFileName;
		private double[,,] obj2dData, xyz2dData;
		private double gridSize;
		private int featureDimention;
		private Bitmap objRangeImg, xyzRangeImg;
		private StructuringElementEx StructEle;
		private int structEleSize, MorIterNum;
		private Emgu.CV.CvEnum.CV_ELEMENT_SHAPE structEleShape;
		//--------------------------


		public Form1()
		{
			InitializeComponent();
		}


		private struct PointCloud
		{
			public double x, y, z;
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			featureDimention = 4;
			structEleSize = 5;
			structEleShape = Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_RECT;
			MorIterNum = 1;
		}

		//Custom function
		public static DialogResult InputBox(string title, string promptText, out double value)
		{
			Form form = new Form();
			Label label = new Label();
			TextBox textBox = new TextBox();
			Button buttonOk = new Button();
			Button buttonCancel = new Button();

			form.Text = title;
			label.Text = promptText;
			textBox.Text = String.Empty;

			buttonOk.Text = "OK";
			buttonCancel.Text = "Cancel";
			buttonOk.DialogResult = DialogResult.OK;
			buttonCancel.DialogResult = DialogResult.Cancel;

			label.SetBounds(9, 20, 372, 13);
			textBox.SetBounds(12, 36, 372, 20);
			buttonOk.SetBounds(228, 72, 75, 23);
			buttonCancel.SetBounds(309, 72, 75, 23);

			label.AutoSize = true;
			textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
			buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

			form.ClientSize = new Size(396, 107);
			form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
			form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
			form.FormBorderStyle = FormBorderStyle.FixedDialog;
			form.StartPosition = FormStartPosition.CenterScreen;
			form.MinimizeBox = false;
			form.MaximizeBox = false;
			form.AcceptButton = buttonOk;
			form.CancelButton = buttonCancel;

			DialogResult dialogResult = form.ShowDialog();
			try
			{
				value = Convert.ToDouble(textBox.Text);
			}
			catch (Exception)
			{
				value = -1.0;
				dialogResult = DialogResult.Cancel;
			}

			return dialogResult;
		}

		private Bitmap Data2Bitmap(double[,,] data, int featureIndex)
		{
			if (featureIndex >= data.GetLength(2) || featureIndex < 0)
				return null;

			int width = data.GetLength(0);
			int height = data.GetLength(1);

			byte[,,] imgData = new byte[width, height, 1];

			double zMax = Enumerable.Range(0, width * height).Select(i => data[i % width, i % height, featureIndex]).Max();
			double zMin = Enumerable.Range(0, width * height).Select(i => data[i % width, i % height, featureIndex]).Min();
			double zRange = zMax - zMin;

			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					imgData[i, j, 0] = (byte)((data[i, j, featureIndex] - zMin) / zRange * 255);
				}
			}

			Bitmap outputImg = BitmapTool.Array2Bitmap(imgData, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

			return outputImg;
		}

		private bool obj_do_work(double gridSize, string fileName, out Bitmap outputImg)
		{

			ObjMesh model;
			float[] pixels;

			double xMax, xMin;
			double yMax, yMin;
			double zMax, zMin, zRange;
			int width, height;
			Vector3d eye, center, up;

			try
			{
				model = new ObjMesh(fileName);
			}
			catch (Exception)
			{
				outputImg = null;
				return false;
			}

			ObjMesh.ObjVertex[] v = model.Vertices;


// 			xMax = v[0].Vertex.X;
// 			xMin = v[0].Vertex.X;
// 			yMax = v[0].Vertex.Y;
// 			yMin = v[0].Vertex.Y;
// 			zMax = v[0].Vertex.Z;
// 			zMin = v[0].Vertex.Z;

			xMax = Enumerable.Range(0, v.Length).Select(i => v[i].Vertex.X).Max();
			xMin = Enumerable.Range(0, v.Length).Select(i => v[i].Vertex.X).Min();
			yMax = Enumerable.Range(0, v.Length).Select(i => v[i].Vertex.Y).Max();
			yMin = Enumerable.Range(0, v.Length).Select(i => v[i].Vertex.Y).Min();
			zMax = Enumerable.Range(0, v.Length).Select(i => v[i].Vertex.Z).Max();
			zMin = Enumerable.Range(0, v.Length).Select(i => v[i].Vertex.Z).Min();
			zRange = zMax - zMin;

// 			for (int i = 0; i < v.Length; i++)
// 			{
// 				if (v[i].Vertex.X > xMax)
// 					xMax = v[i].Vertex.X;
// 				else if (v[i].Vertex.X < xMin)
// 					xMin = v[i].Vertex.X;
// 				if (v[i].Vertex.Y > yMax)
// 					yMax = v[i].Vertex.Y;
// 				else if (v[i].Vertex.Y < yMin)
// 					yMin = v[i].Vertex.Y;
// 				if (v[i].Vertex.Z > zMax)
// 					zMax = v[i].Vertex.Z;
// 				else if (v[i].Vertex.Z < zMin)
// 					zMin = v[i].Vertex.Z;
// 			}


			width = (int)Math.Ceiling((Math.Ceiling(xMax) - Math.Floor(xMin)) / gridSize);
			height = (int)Math.Ceiling((Math.Ceiling(yMax) - Math.Floor(yMin)) / gridSize);

			eye = new Vector3d(0, 0, zMax);
			center = new Vector3d(0, 0, 0);
			up = new Vector3d(0, 1, 0);

			

			using (var game = new GameWindow(width, height))
			{
				GL.Enable(EnableCap.DepthTest);
				GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

				GL.DepthRange(0.0f, 1.0f);
				GL.ClearDepth(1.0f);


				GL.Viewport(0, 0, width, height);


				GL.MatrixMode(MatrixMode.Projection);
				GL.LoadIdentity();
				GL.Ortho(Math.Floor(xMin), Math.Ceiling(xMax), Math.Floor(yMin), Math.Ceiling(yMax), 0, zMax - zMin);


				GL.MatrixMode(MatrixMode.Modelview);
				GL.LoadIdentity();
				Matrix4d mat = Matrix4d.LookAt(eye.X, eye.Y, eye.Z, center.X, center.Y, center.Z, up.X, up.Y, up.Z);
				GL.LoadMatrix(ref mat);


				model.Render();


				//draw depth
				pixels = new float[width * height];
				obj2dData = new double[width, height, featureDimention];
				GL.ReadPixels(0, 0, width, height, PixelFormat.DepthComponent, PixelType.Float, pixels);



				for (int i = 0; i < pixels.Length; i++)
					pixels[i] = 1.0f - pixels[i];
				

				for (int i = 0; i < width; i++)
				{
					for (int j = 0; j < height; j++)
					{
						obj2dData[i, j, 0] = pixels[(height - 1 - j) * width + i] * zRange;
					}
				}

				for (int i = 0; i < width; i++)
				{
					for (int j = 0; j < height; j++)
					{
						obj2dData[i, j, 1] = obj2dData[i, j, 0] * i;
						obj2dData[i, j, 2] = obj2dData[i, j, 0] * j;
					}
				}

				double zSum = Enumerable.Range(0, width * height).Select(i => obj2dData[i % width, i % height, 0]).Sum();
				int xAvg = (int)Math.Round(Enumerable.Range(0, width * height).Select(i => obj2dData[i % width, i % height, 1]).Sum() / zSum);
				int yAvg = (int)Math.Round(Enumerable.Range(0, width * height).Select(i => obj2dData[i % width, i % height, 2]).Sum() / zSum);

				for (int i = xAvg - 1; i <= xAvg + 1; i++)
				{
					for (int j = yAvg - 1; j <= yAvg + 1; j++)
					{
						obj2dData[i, j, 0] = 0.0;
					}
				}


				outputImg = Data2Bitmap(obj2dData, 0);


				writeConsole("Model file: " + fileName);
				writeConsole("GridSize: " + gridSize);
				writeConsole("Width: " + width);
				writeConsole("Height: " + height);
				writeConsole("xMax: " + xMax + "\txMin: " + xMin);
				writeConsole("yMax: " + yMax + "\tyMin: " + yMin);
				writeConsole("zMax: " + zMax + "\tzMin: " + zMin);
				writeConsole("xCenter: " + (double)xAvg / (double)width);
				writeConsole("yCenter: " + (double)yAvg / (double)height);
				writeConsole("-----End-----");

				return true;
			}
		}

		private bool xyz_do_work(double gridSize, string fileName, out Bitmap outputImg)
		{
			double[,] pcArray; //點雲陣列
			double xMax, xMin;
			double yMax, yMin;
			double zMax, zMin, zRange;
			int width, height;

			//點雲總數
			int pcNum = 0;



			//計算總行數
			using (StreamReader sr = new StreamReader(fileName))
			{
				while (sr.ReadLine() != null)
					pcNum++;

				pcArray = new double[pcNum, 3];
			}

			//讀點雲到pcArray
			using (StreamReader sr = new StreamReader(fileName))
			{
				string[] sArray;

				for (int i = 0; i < pcNum; i++)
				{
					sArray = sr.ReadLine().Split(' ');
					pcArray[i, 0] = Convert.ToDouble(sArray[0]);
					pcArray[i, 1] = Convert.ToDouble(sArray[1]);
					pcArray[i, 2] = Convert.ToDouble(sArray[2]);
				}
			}



			//找出bounding box的範圍
			xMax = Enumerable.Range(0, pcArray.GetLength(0)).Select(i => pcArray[i, 0]).Max();
			xMin = Enumerable.Range(0, pcArray.GetLength(0)).Select(i => pcArray[i, 0]).Min();
			yMax = Enumerable.Range(0, pcArray.GetLength(0)).Select(i => pcArray[i, 1]).Max();
			yMin = Enumerable.Range(0, pcArray.GetLength(0)).Select(i => pcArray[i, 1]).Min();
			zMax = Enumerable.Range(0, pcArray.GetLength(0)).Select(i => pcArray[i, 2]).Max();
			zMin = Enumerable.Range(0, pcArray.GetLength(0)).Select(i => pcArray[i, 2]).Min();
			zRange = zMax - zMin;

			//計算range image的長寬
			width = (int)Math.Ceiling((Math.Ceiling(xMax) - Math.Floor(xMin)) / gridSize);
			height = (int)Math.Ceiling((Math.Ceiling(yMax) - Math.Floor(yMin)) / gridSize);



			//宣告range image的z值最大的點雲陣列
			PointCloud[,] zMaxArray = new PointCloud[width, height];

			//初始化zMaxArray
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					zMaxArray[i, j].x = 0;
					zMaxArray[i, j].y = 0;
					zMaxArray[i, j].z = zMin;
				}
			}



			//找出range image中每個pixel最高的點雲
			int pcX, pcY;
			for (int i = 0; i < pcNum; i++)
			{
				pcX = (int)Math.Floor((pcArray[i, 0] - xMin) / gridSize);
				pcY = (int)Math.Floor((pcArray[i, 1] - yMin) / gridSize);
				if (pcArray[i, 2] > zMaxArray[pcX, pcY].z)
				{
					zMaxArray[pcX, pcY].x = pcArray[i, 0];
					zMaxArray[pcX, pcY].y = pcArray[i, 1];
					zMaxArray[pcX, pcY].z = pcArray[i, 2];
				}
			}


			//xyz2dData初始化並填值
			xyz2dData = new double[width, height, featureDimention];
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					xyz2dData[i, j, 0] = zMaxArray[i, height - j - 1].z - zMin;
				}
			}



			//EmguCV的Morphology處理
			using (Image<Gray, Double> emguImg = new Image<Gray, Double>(width, height))
			{
				for (int i = 0; i < width; i++)
				{
					for (int j = 0; j < height; j++)
					{
						emguImg.Data[j, i, 0] = xyz2dData[i, j, 0];
					}
				}


				StructEle = new StructuringElementEx(structEleSize, structEleSize, (structEleSize - 1) / 2, (structEleSize - 1) / 2, structEleShape);
				CvInvoke.cvDilate(emguImg, emguImg, StructEle, MorIterNum);
				CvInvoke.cvErode(emguImg, emguImg, StructEle, MorIterNum);
				 

				for (int i = 0; i < width; i++)
				{
					for (int j = 0; j < height; j++)
					{
						xyz2dData[i, j, 0] = emguImg.Data[j, i, 0];
					}
				}


			}


			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					xyz2dData[i, j, 1] = xyz2dData[i, j, 0] * i;
					xyz2dData[i, j, 2] = xyz2dData[i, j, 0] * j;
				}
			}

			double zSum = Enumerable.Range(0, width * height).Select(i => xyz2dData[i % width, i % height, 0]).Sum();
			int xAvg = (int)Math.Round(Enumerable.Range(0, width * height).Select(i => xyz2dData[i % width, i % height, 1]).Sum() / zSum);
			int yAvg = (int)Math.Round(Enumerable.Range(0, width * height).Select(i => xyz2dData[i % width, i % height, 2]).Sum() / zSum);

			for (int i = xAvg - 1; i <= xAvg + 1; i++)
			{
				for (int j = yAvg - 1; j <= yAvg + 1; j++)
				{
					xyz2dData[i, j, 0] = 0.0;
				}
			}




			writeConsole("Point cloud file: " + fileName);
			writeConsole("GridSize: " + gridSize);
			writeConsole("Width: " + width);
			writeConsole("Height: " + height);
			writeConsole("xMax: " + xMax + "\txMin: " + xMin);
			writeConsole("yMax: " + yMax + "\tyMin: " + yMin);
			writeConsole("zMax: " + zMax + "\tzMin: " + zMin);
			writeConsole("xCenter: " + (double)xAvg / (double)width);
			writeConsole("yCenter: " + (double)yAvg / (double)height);
			writeConsole("-----End-----");




			outputImg = Data2Bitmap(xyz2dData, 0);
			return true;

			/*
			//計算在range image中在建築物範圍內的pixel數量
			pcNum = 0;
			foreach (PointCloud i in zMaxArray)
			{
				if (i.z != zMin)
					pcNum++;
			}



			//將pcArray點雲陣列替換成在pixel中最高的點雲
			int count = 0;
			pcArray = new double[pcNum, 3];
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					if (zMaxArray[i, j].z != zMin)
					{
						pcArray[count, 0] = zMaxArray[i, j].x;
						pcArray[count, 1] = zMaxArray[i, j].y;
						pcArray[count, 2] = zMaxArray[i, j].z;
						count++;
					}
				}
			}

// 			try
// 			{
// 				xyzClass matlab = new xyzClass();
// 				MWNumericArray pcMatlabArray = new MWNumericArray(pcArray);
// 				MWArray[] result = null;
// 				result = matlab.xyzPostProcessing(2, pcMatlabArray, gridSize, outputName);
// 			}
// 			catch (Exception)
// 			{
// 				return false;
// 			}

			// 			BitmapTool.Array2Bitmap(rangeImgData, System.Drawing.Imaging.PixelFormat.Format8bppIndexed).Save(outputName, System.Drawing.Imaging.ImageFormat.Jpeg);

			return true;

			*/



		}

		private void writeConsole(string message)
		{
			richTextBox1.Text += message + "\n";
		}





		private void richTextBox1_TextChanged(object sender, EventArgs e)
		{
			richTextBox1.SelectionStart = richTextBox1.TextLength;
			richTextBox1.ScrollToCaret();
		}

		private void pictureBox1_DoubleClick(object sender, EventArgs e)
		{
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Filter = "bmp files (*.bmp)|*.bmp";
			if (sfd.ShowDialog() == DialogResult.OK)
			{
				pictureBox1.Image.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
			}
		}

		private void pictureBox2_DoubleClick(object sender, EventArgs e)
		{
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Filter = "bmp files (*.bmp)|*.bmp";
			if (sfd.ShowDialog() == DialogResult.OK)
			{
				pictureBox2.Image.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
			}
		}

		private void modelToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "obj files (*.obj)|*.obj";
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				if (Form1.InputBox("Grid Size?", "Please enter grid size (m):", out gridSize) == DialogResult.Cancel || gridSize <= 0.0)
				{
					MessageBox.Show("Fail!");
					return;
				}

				if (gridSize * 100 > 100)
					hScrollBar1.Value = hScrollBar1.Maximum;
				else if (gridSize * 100 < 0)
					hScrollBar1.Value = hScrollBar1.Minimum;
				else
					hScrollBar1.Value = (int)(gridSize * 100);

				if (obj_do_work(gridSize, ofd.FileName, out objRangeImg))
					pictureBox1.Image = objRangeImg;

				objFileName = ofd.FileName;
			}
		}

		private void pointCloudToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "xyz files (*.xyz)|*.xyz";
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				if (Form1.InputBox("Grid Size?", "Please enter grid size (m):", out gridSize) == DialogResult.Cancel || gridSize <= 0.0)
				{
					MessageBox.Show("Fail!");
					return;
				}

				if (gridSize * 100 > 100)
					hScrollBar1.Value = hScrollBar1.Maximum;
				else if (gridSize * 100 < 0)
					hScrollBar1.Value = hScrollBar1.Minimum;
				else
					hScrollBar1.Value = (int)(gridSize * 100);

				if (xyz_do_work(gridSize, ofd.FileName, out xyzRangeImg))
					pictureBox2.Image = xyzRangeImg;

				xyzFileName = ofd.FileName;
			}
		}

		private void regenerateRangeImageToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (objFileName == null || xyzFileName == null)
			{
				MessageBox.Show("Please load file first!");
				return;
			}

			gridSize = (double)hScrollBar1.Value / 100.0;

			if (obj_do_work(gridSize, objFileName, out objRangeImg))
				pictureBox1.Image = objRangeImg;

			if (xyz_do_work(gridSize, xyzFileName, out xyzRangeImg))
				pictureBox2.Image = xyzRangeImg;

		}

	}
}
