/*
功能：
1. 製作range image

return值：
-1代表讀檔失敗
-2產生影像失敗
+1代表成功

range image band：
0 => adjusted z value (zMinAdj ~ zMaxAdj) (meter)
1 => index of annular (0 ~ annularNum, annularNum = outlier) (no define)
2 => edge
*/

using System;
//using System.Collections.Generic;
using System.Linq;
using System.IO;
//using System.Text;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using ImageTool;

using Emgu.CV;
using Emgu.CV.Structure;

// using MathWorks.MATLAB.NET.Arrays;
// using xyzMatlabTool;

namespace Core
{
	//Base class
	class coreClass
	{
		public string fileName { get; set; }
		public double gridSize { get; set; }
		public int annularNum { get; set; }

		public double[,,] data2D { get; protected set; }
		public bool[,] data2DMask { get; protected set; }
		public double[,] annularFeature { get; protected set; }
		public Bitmap rangeImg { get; protected set; }
		public Bitmap featureImg { get; protected set; }
		
		public double xMax { get; protected set; }
		public double xMin { get; protected set; }
		public double yMax { get; protected set; }
		public double yMin { get; protected set; }
		public double zMax { get; protected set; }
		public double zMin { get; protected set; }
		public double zMaxAdj { get; protected set; }
		public double zMinAdj { get; protected set; }
		public double zRange { get; protected set; }
		public int width { get; protected set; }
		public int height { get; protected set; }
		public int xAvg { get; protected set; } //起始值為0
		public int yAvg { get; protected set; } //起始值為0
		public double zAvg { get; protected set; }

		//Features
		public double totalArea { get; protected set; }
		public double totalVol { get; protected set; }
		public double radiusMax { get; protected set; }

		//Options
		public const int featureDimension = 3;
		public static readonly string[] featureDimensionName = new string[featureDimension] { "Height", "Annular", "Edge" }; //記得增修
		public const int annularfeatureDimension = 2;
		public const bool ifDisplayCenter = false;


		public coreClass(string fileName)
		{
			this.fileName = fileName;
		}

		public virtual bool GenerateRangeImage(double gridSize)
		{
			return false;
		}

		public string OutputConsole()
		{
			string message = string.Empty;

			if (Path.GetExtension(fileName) == ".obj")
				message += "Type: model\n";
			else
				message += "Type: point cloud\n";

			message += "File path: " + fileName + "\n";
			message += "GridSize: " + gridSize + "\n";
			message += "Width: " + width + "\n";
			message += "Height: " + height + "\n";
			message += "xMax: " + xMax + "\txMin: " + xMin + "\n";
			message += "yMax: " + yMax + "\tyMin: " + yMin + "\n";
			message += "zMax: " + zMax + "\tzMin: " + zMin + "\n";
			message += "zMaxAdj: " + zMaxAdj + "\tzMinAdj: " + zMinAdj + "\n";
			message += "xCenter: " + (double)xAvg / (double)(width - 1) + "\n";
			message += "yCenter: " + (double)yAvg / (double)(height - 1) + "\n";
			message += "zCenter: " + zAvg + "\n";
			message += "radiusMax: " + radiusMax + " m\n";
			message += "Total area: " + totalArea + " m2\n";
			message += "Total volume: " + totalVol + " m3\n";
			message += "-----End-----";

			return message;
		}

		public string OutputSeries()
		{
			string series = string.Empty;

			series += zAvg + "\n";
			series += totalArea +"\n";
			series += totalVol +"\n";
			series += radiusMax + "\n";


			for (int i = 0; i < annularfeatureDimension; i++)
			{
				for (int j = 0; j < annularNum; j++)
				{
					series += annularFeature[j, i] + "\n";	
				}
			}

			return series;
		}

		public bool GenerateFeature(int annularNum)
		{
			if (data2D == null)
				return false;

			this.annularNum = annularNum;

			CalArea();
			CalVol();
			Cal2DFeature();

			CalAnnularFeature();

			return true;
		}

		private void CalArea()
		{
			totalArea = Enumerable.Range(0, width * height).Where(i => data2DMask[i % width, i / width] == true).Count() * Math.Pow(gridSize, 2);
		}

		private void CalVol()
		{
			totalVol = Enumerable.Range(0, width * height).Where(i => data2DMask[i % width, i / width] == true).Select(i => data2D[i % width, i / width, 0]).Sum();
		}

		private void CalAnnularFeature()
		{
			radiusMax = Enumerable.Range(0, width * height).Where(i => data2DMask[i % width, i / width] == true).Select(i => i).Select(i => Math.Sqrt(Math.Pow((i % width) - xAvg, 2) + Math.Pow((i / width) - yAvg, 2))).Max() * gridSize;

			double annularRangeDistance = radiusMax / (double)annularNum;
			annularFeature = new double[annularNum, annularfeatureDimension];
			
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					data2D[i, j, 1] = Math.Sqrt(Math.Pow(i - xAvg, 2) + Math.Pow(j - yAvg, 2)) * gridSize;
					data2D[i, j, 1] = Math.Floor(data2D[i, j, 1] / annularRangeDistance);

					if (data2D[i, j, 1] < annularNum)
					{
						annularFeature[(int)data2D[i, j, 1], 0] += data2D[i, j, 0];
						annularFeature[(int)data2D[i, j, 1], 1] += data2D[i, j, 2];
					}
				}
			}

		}

		private void Cal2DFeature()
		{
			//EmguCV
			using (Image<Gray, byte> emguImg = new Image<Gray, byte>(width, height))
			{
				for (int i = 0; i < width; i++)
				{
					for (int j = 0; j < height; j++)
					{
						emguImg.Data[j, i, 0] = (byte)data2D[i, j, 0];
					}
				}

				Image<Gray, byte> emguCannyImg = emguImg.Canny(10.0, 15.0);


				for (int i = 0; i < width; i++)
				{
					for (int j = 0; j < height; j++)
					{
						data2D[i, j, 2] = emguCannyImg.Data[j, i, 0];
					}
				}
			}
		}

		protected void CalAvg()
		{
			double[,,] data2DAvg = new double[width, height, 2];

			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					data2DAvg[i, j, 0] = data2D[i, j, 0] * (i + 1);
					data2DAvg[i, j, 1] = data2D[i, j, 0] * (j + 1);
				}
			}

			//計算中心點(z因為要算中心而不是表面，所以要除以2)
			double zSum = Enumerable.Range(0, width * height).Select(i => data2D[i % width, i / width, 0]).Sum();
			xAvg = (int)Math.Round(Enumerable.Range(0, width * height).Select(i => data2DAvg[i % width, i / width, 0]).Sum() / zSum) - 1;
			yAvg = (int)Math.Round(Enumerable.Range(0, width * height).Select(i => data2DAvg[i % width, i / width, 1]).Sum() / zSum) - 1;
			zAvg = (Enumerable.Range(0, width * height).Select(i => data2D[i % width, i / width, 0]).Sum() / 2) / Enumerable.Range(0, width * height).Where(i => data2DMask[i % width, i / width] == true).Count();
		}

		protected void DisplayCenter()
		{
			for (int i = xAvg - 1; i <= xAvg + 1; i++)
			{
				for (int j = yAvg - 1; j <= yAvg + 1; j++)
				{
					data2D[i, j, 0] = zMinAdj;
				}
			}
		}

	}

	//derived from coreClass
	class objClass : coreClass
	{
		public ObjMesh model { get; private set; }
		public float[] pixels { get; private set; }

		public objClass(string fileName) : base(fileName)
		{
		}

		public override bool GenerateRangeImage(double gridSize)
		{
			this.gridSize = gridSize;


			Vector3d eye, center, up;

			try
			{
				model = new ObjMesh(fileName);
			}
			catch (Exception)
			{
				rangeImg = null;
				return false;
			}



			ObjMesh.ObjVertex[] v = model.Vertices;

			xMax = Enumerable.Range(0, v.Length).Select(i => v[i].Vertex.X).Max();
			xMin = Enumerable.Range(0, v.Length).Select(i => v[i].Vertex.X).Min();
			yMax = Enumerable.Range(0, v.Length).Select(i => v[i].Vertex.Y).Max();
			yMin = Enumerable.Range(0, v.Length).Select(i => v[i].Vertex.Y).Min();
			zMax = Enumerable.Range(0, v.Length).Select(i => v[i].Vertex.Z).Max();
			zMin = Enumerable.Range(0, v.Length).Select(i => v[i].Vertex.Z).Min();
			zMaxAdj = zMax - zMin;
			zMinAdj = zMin - zMin;
			zRange = zMax - zMin;



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
				data2D = new double[width, height, featureDimension];
				data2DMask = new bool[width, height];
				GL.ReadPixels(0, 0, width, height, PixelFormat.DepthComponent, PixelType.Float, pixels);



				for (int i = 0; i < pixels.Length; i++)
					pixels[i] = 1.0f - pixels[i];


				for (int i = 0; i < width; i++)
				{
					for (int j = 0; j < height; j++)
					{
						data2D[i, j, 0] = pixels[(height - 1 - j) * width + i] * zRange;
						if (pixels[(height - 1 - j) * width + i] != zMinAdj)
							data2DMask[i, j] = true;
						else
							data2DMask[i, j] = false;						
					}
				}

				CalAvg();

				if (ifDisplayCenter)
				{
					DisplayCenter();
				}

				rangeImg = utilClass.Data2Bitmap(data2D, 0);

				return true;
			}

		}
	}
	
	//derived from coreClass
	class xyzClass : coreClass
	{
		public StructuringElementEx StructEle { get; set; }
		public int structEleSize { get; set; }
		public int MorIterNum { get; set; }
		public Emgu.CV.CvEnum.CV_ELEMENT_SHAPE structEleShape { get; set; }

		public double[,] pcArray { get; private set; } //點雲陣列

		private struct PointCloud
		{
			public double x, y, z;
		}


		public xyzClass(string fileName) : base(fileName)
		{
			structEleSize = 7;
			MorIterNum = 2;
			structEleShape = Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_RECT;
		}

		public override bool GenerateRangeImage(double gridSize)
		{
			this.gridSize = gridSize;



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
			zMaxAdj = zMax - zMin;
			zMinAdj = zMin - zMin;
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
			data2D = new double[width, height, featureDimension];
			data2DMask = new bool[width, height];
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					data2D[i, j, 0] = zMaxArray[i, height - j - 1].z - zMin;
				}
			}



			//EmguCV的Morphology處理
			using (Image<Gray, Double> emguImg = new Image<Gray, Double>(width, height))
			{
				for (int i = 0; i < width; i++)
				{
					for (int j = 0; j < height; j++)
					{
						emguImg.Data[j, i, 0] = data2D[i, j, 0];
					}
				}


				StructEle = new StructuringElementEx(structEleSize, structEleSize, (structEleSize - 1) / 2, (structEleSize - 1) / 2, structEleShape);
				CvInvoke.cvDilate(emguImg, emguImg, StructEle, MorIterNum);
				CvInvoke.cvErode(emguImg, emguImg, StructEle, MorIterNum);


				for (int i = 0; i < width; i++)
				{
					for (int j = 0; j < height; j++)
					{
						data2D[i, j, 0] = emguImg.Data[j, i, 0];
					}
				}
			}

			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					if (data2D[i, j, 0] != zMinAdj)
						data2DMask[i, j] = true;
					else
						data2DMask[i, j] = false;
				}
			}

			CalAvg();

			if (ifDisplayCenter)
			{
				DisplayCenter();
			}

			rangeImg = utilClass.Data2Bitmap(data2D, 0);

			return true;
		}
	}

	class utilClass
	{
		static public Bitmap Data2Bitmap(double[,,] data2D, int featureIndex)
		{
			if (featureIndex >= data2D.GetLength(2) || featureIndex < 0)
				return null;

			int width = data2D.GetLength(0);
			int height = data2D.GetLength(1);

			byte[,,] imgData = new byte[width, height, 1];

			double zMax = Enumerable.Range(0, width * height).Select(i => data2D[i % width, i / width, featureIndex]).Max();
			double zMin = Enumerable.Range(0, width * height).Select(i => data2D[i % width, i / width, featureIndex]).Min();
			double zRange = zMax - zMin;

			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					imgData[i, j, 0] = (byte)((data2D[i, j, featureIndex] - zMin) / zRange * 255.0);
				}
			}

			Bitmap outputImg = BitmapTool.Array2Bitmap(imgData, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

			return outputImg;
		}

	}
}
