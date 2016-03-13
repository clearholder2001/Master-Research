/*
功能：
1. 製作range image
2. 製作feature：height, edge(LoG), Eigen-feature(Linearity, Planarity, Sphericity)

return值：
-1代表讀檔失敗
-2產生影像失敗
+1代表成功

data2DAnnularIndex：
index of annular (0 ~ annularNum, annularNum = outliers) (no define)

data2D：
0 => adjusted z value (zMinAdj ~ zMaxAdj) (meter)
1 => edge
2 => eigen feature A
3 => eigen feature P
4 => eigen feature S
*/

using System;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Numerics;
using System.Threading.Tasks;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using ImageTool;

using Emgu.CV;
using Emgu.CV.Structure;

using MathNet.Numerics.LinearAlgebra;

namespace Core
{
	//Base class
	public class coreClass
	{
		public string fileName { get; private set; }
		public double eigenFeatureDiameter { get; private set; }
		

		public double[,,] data2D { get; protected set; }
		public bool[,] data2DMask { get; protected set; }
		public byte[,] data2DAnnularIndex { get; protected set; }
		public double[,] annularFeature { get; protected set; }
		public Bitmap rangeImg { get; protected set; }
		public Bitmap rangeThumbnailImg { get; protected set; }
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
		public static double gridSize { get; set; }
		public static int annularNum { get; set; }
		public enum feature : byte { Height, Edge, Linearity, Planarity, Sphericity }
		public static int featureDimension = Enum.GetValues(typeof(feature)).Length;
		public static int laplacianAperture;
		public static int gaussianAperture;
		public static int eigenFeatureDiameterMultiple;
		public static bool ifDisplayCenter;
		public static int borderAdditionWidth;
		public static int thumbnailImgMaxSize;


		public coreClass(string fileName)
		{
			this.fileName = fileName;
			//gridSize = 0.1;
			//annularNum = 10;
			//eigenFeatureDiameterMultiple = 3;
			//ifDisplayCenter = false;
			borderAdditionWidth = 10;
			thumbnailImgMaxSize = 100;
		}

		public virtual bool GenerateRangeImage()
		{
			return false;
		}

		public string OutputConsole()
		{
			string message = string.Empty;

			if (Path.GetExtension(fileName) == ".obj")
				message += "Type: polygonal model\n";
			else if (Path.GetExtension(fileName) == ".xyz")
				message += "Type: point cloud\n";

			message += "File path: " + fileName + "\n";
			message += "GridSize: " + gridSize + "\n";
			message += "AnnularNum: " + annularNum + "\n";
			message += "LaplacianAperture: " + laplacianAperture + "\n";
			message += "GaussianAperture: " + gaussianAperture + "\n";
			message += "EigenFeatureDiameterMultiple: " + eigenFeatureDiameterMultiple + "\n";
			message += "Width: " + width + "\n";
			message += "Height: " + height + "\n";
			message += "xMax: " + String.Format("{0:0.000}", xMax) + "\t\txMin: " + String.Format("{0:0.000}", xMin) + "\n";
			message += "yMax: " + String.Format("{0:0.000}", yMax) + "\t\tyMin: " + String.Format("{0:0.000}", yMin) + "\n";
			message += "zMax: " + String.Format("{0:0.000}", zMax) + "\t\tzMin: " + String.Format("{0:0.000}", zMin) + "\n";
			message += "zMaxAdj: " + String.Format("{0:0.000}", zMaxAdj) + "\t\tzMinAdj: " + String.Format("{0:0.000}", zMinAdj) + "\n";
			message += "xCenter: " + String.Format("{0:0.000}", (double)xAvg / (double)(width - 1)) + "\n";
			message += "yCenter: " + String.Format("{0:0.000}", (double)yAvg / (double)(height - 1)) + "\n";
			message += "zCenter: " + String.Format("{0:0.000}", zAvg) + "\n";
			message += "radiusMax: " + String.Format("{0:0.000}", radiusMax) + " m\n";
			message += "totalArea: " + String.Format("{0:0.000}", totalArea) + " m2\n";
			message += "totalVol: " + String.Format("{0:0.000}", totalVol) + " m3\n";


			return message;
		}

		public string OutputSeries()
		{
			string series = string.Empty;

			series += zAvg + "\n";
			series += totalArea + "\n";
			series += totalVol + "\n";
			series += radiusMax + "\n";


			for (int i = 0; i < featureDimension; i++)
			{
				for (int j = 0; j < annularNum; j++)
				{
					series += annularFeature[i, j] + "\n";
				}
			}

			return series;
		}

		public bool GenerateFeature()
		{
			if (data2D == null)
				return false;

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
			totalVol = Enumerable.Range(0, width * height).Where(i => data2DMask[i % width, i / width] == true).Select(i => data2D[i % width, i / width, (byte)feature.Height]).Sum();
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
						emguImg.Data[j, i, 0] = (byte)data2D[i, j, (byte)feature.Height];
					}
				}

				//Canny
				//Image<Gray, byte> emguCannyImg = emguImg.Canny(10.0, 15.0);

				//Loplacian of Gaussian (LoG)
				Image<Gray, float> emguLoGImg = emguImg.SmoothGaussian(gaussianAperture).Laplace(laplacianAperture);

				for (int i = 0; i < width; i++)
				{
					for (int j = 0; j < height; j++)
					{
						data2D[i, j, 1] = Math.Abs(emguLoGImg.Data[j, i, (byte)feature.Height]);
					}
				}
			}



			//Eigen feature
			if (eigenFeatureDiameterMultiple >= 1)
			{
				eigenFeatureDiameter = gridSize * eigenFeatureDiameterMultiple;
			}
			else
			{
				for (int i = 0; i < width; i++)
				{
					for (int j = 0; j < height; j++)
					{
						data2D[i, j, (byte)feature.Linearity] = 0.0;
						data2D[i, j, (byte)feature.Planarity] = 0.0;
						data2D[i, j, (byte)feature.Sphericity] = 0.0;
					}
				}
				return;
			}
			//List<double[]> neighborList = new List<double[]>();
			//List<List<double[]>> tt = new List<List<double[]>>();
			//List<Vector<Complex>> ttp = new List<Vector<Complex>>();
			//for (int i = 0; i < width; i++)
			Parallel.For(0, width - 1, i =>
			{
				for (int j = 0; j < height; j++)
				{
					if (data2DMask[i, j])
					{
						if (i < eigenFeatureDiameterMultiple || i > width - 1 - eigenFeatureDiameterMultiple || j < eigenFeatureDiameterMultiple || j > height - 1 - eigenFeatureDiameterMultiple)
						{

						}
						else
						{
							List<double[]> neighborList = new List<double[]>();
							//neighborList.Clear();

							for (int k = i - eigenFeatureDiameterMultiple; k <= i + eigenFeatureDiameterMultiple; k++)
							{
								for (int l = j - eigenFeatureDiameterMultiple; l <= j + eigenFeatureDiameterMultiple; l++)
								{
									if (data2DMask[k, l] && Math.Sqrt(Math.Pow((k - i) * gridSize, 2) + Math.Pow((l - j) * gridSize, 2) + Math.Pow(data2D[i, j, (byte)feature.Height] - data2D[k, l, (byte)feature.Height], 2)) < eigenFeatureDiameter)
									{
										neighborList.Add(new double[3] { k * gridSize, l * gridSize, data2D[k, l, (byte)feature.Height]});
									}
								}
							}

							if (neighborList.Count < 2)
							{
								data2D[i, j, (byte)feature.Linearity] = 0;
								data2D[i, j, (byte)feature.Planarity] = 0;
								data2D[i, j, (byte)feature.Sphericity] = 0;
							}
							else
							{
								double[] mean = new double[3] { neighborList.Select(a => a[0]).Average(), neighborList.Select(a => a[1]).Average(), neighborList.Select(a => a[2]).Average() };

								foreach (double[] pc in neighborList)
								{
									pc[0] -= mean[0];
									pc[1] -= mean[1];
									pc[2] -= mean[2];
								}

								MathNet.Numerics.LinearAlgebra.Matrix<double> matrixP = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build.DenseOfColumnArrays(neighborList.ToArray());

								//MathNet.Numerics.LinearAlgebra.Matrix<double> matrixCovariance = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build.DenseOfColumnArrays(neighborList.ToArray());
								//matrixCovariance = matrixCovariance * matrixCovariance.Transpose() / neighborList.Count;

								MathNet.Numerics.LinearAlgebra.Matrix<double> matrixCovariance = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build.Dense(3, 3, 0.0);

								for (int n = 0; n < neighborList.Count; n++)
								{
									matrixCovariance += matrixP.SubMatrix(0, 3, n, 1) * matrixP.SubMatrix(0, 3, n, 1).Transpose();
								}

								MathNet.Numerics.LinearAlgebra.Matrix<double> matrixTT = matrixCovariance;


								matrixCovariance = matrixCovariance.Divide(neighborList.Count);

								var eigenResult = matrixCovariance.Evd();

								Vector<Complex> eigenValues = eigenResult.EigenValues;

								/*
								if (eigenValues[0].Real < 0 || eigenValues[1].Real < 0 || eigenValues[2].Real < 0)
								{
									tt.Add(neighborList);
									ttp.Add(eigenValues);
								}
								*/

								double Linearity = (eigenValues[2].Magnitude - eigenValues[1].Magnitude) / eigenValues[2].Magnitude;
								double Planarity = (eigenValues[1].Magnitude - eigenValues[0].Magnitude) / eigenValues[2].Magnitude;
								double Sphericity = eigenValues[0].Magnitude / eigenValues[2].Magnitude;

								if (!double.IsNaN(Linearity))
									data2D[i, j, (byte)feature.Linearity] = Linearity;
								else
									data2D[i, j, (byte)feature.Linearity] = 0;

								if (!double.IsNaN(Planarity))
									data2D[i, j, (byte)feature.Planarity] = Planarity;
								else
									data2D[i, j, (byte)feature.Planarity] = 0;

								if (!double.IsNaN(Sphericity))
									data2D[i, j, (byte)feature.Sphericity] = Sphericity;
								else
									data2D[i, j, (byte)feature.Sphericity] = 0;
							}

						}
					}
				}
			}
			);

		}

		private void CalAnnularFeature()
		{
			radiusMax = Enumerable.Range(0, width * height).Where(i => data2DMask[i % width, i / width] == true).Select(i => Math.Sqrt(Math.Pow((i % width) - xAvg, 2) + Math.Pow((i / width) - yAvg, 2))).Max() * gridSize;

			double annularRangeDistance = radiusMax / (double)annularNum;

			/*
			//equal area
			double[] annularRangeDistance = new double[annularNum + 1];
			double equalArea = Math.Pow(radiusMax, 2) / annularNum;

			annularRangeDistance[0] = 0;
			annularRangeDistance[1] = Math.Sqrt(equalArea);

			for (int i = 2; i < annularRangeDistance.Length; i++)
			{
				annularRangeDistance[i] = Math.Sqrt(equalArea + Math.Pow(annularRangeDistance[i - 1], 2));
			}
			*/

			data2DAnnularIndex = new byte[width, height];
			annularFeature = new double[featureDimension, annularNum];

			double[,] data2DDistance = new double[width, height];

			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					//計算annular index
					data2DDistance[i, j] = Math.Sqrt(Math.Pow(i - xAvg, 2) + Math.Pow(j - yAvg, 2)) * gridSize;
					data2DAnnularIndex[i, j] = (byte)Math.Floor(data2DDistance[i, j] / annularRangeDistance);

					/*
					//equal area
					if (data2DDistance[i, j] <= radiusMax)
					{
						for (int interval = 0; interval < annularRangeDistance.Length - 1; interval++)
						{
							double smallRadius = annularRangeDistance[interval];
							double largeRadius = annularRangeDistance[interval + 1];

							if (data2DDistance[i, j] >= smallRadius && data2DDistance[i, j] < largeRadius)
							{
								data2DAnnularIndex[i, j] = (byte)interval;
								break;
							}
						}
					}
					else
					{
						data2DAnnularIndex[i, j] = (byte)annularNum;
					}
					*/


					//計算annular統計值
					if (data2DAnnularIndex[i, j] < annularNum)
					{
						//annularFeature[0, data2DAnnularIndex[i, j]] += data2D[i, j, (byte)feature.Height];
						if (data2D[i, j, (byte)feature.Height] != 0)
							annularFeature[(byte)feature.Height, data2DAnnularIndex[i, j]] += (zMaxAdj - data2D[i, j, (byte)feature.Height]);

						annularFeature[(byte)feature.Edge, data2DAnnularIndex[i, j]] += data2D[i, j, (byte)feature.Edge];
						annularFeature[(byte)feature.Linearity, data2DAnnularIndex[i, j]] += data2D[i, j, (byte)feature.Linearity];
						annularFeature[(byte)feature.Planarity, data2DAnnularIndex[i, j]] += data2D[i, j, (byte)feature.Planarity];
						annularFeature[(byte)feature.Sphericity, data2DAnnularIndex[i, j]] += data2D[i, j, (byte)feature.Sphericity];
					}
				}
			}

			//apply annular weight
			for (int i = 0; i < coreClass.featureDimension; i++)
			{
				for (int j = 1; j <= coreClass.annularNum; j++)
				{
					annularFeature[i, j - 1] /= ((j + (j - 1)) * (j - (j - 1)));
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
					data2DAvg[i, j, 0] = data2D[i, j, (byte)feature.Height] * (i + 1);
					data2DAvg[i, j, 1] = data2D[i, j, (byte)feature.Height] * (j + 1);
				}
			}

			//計算中心點(z因為要算中心而不是表面，所以要除以2)
			double zSum = Enumerable.Range(0, width * height).Select(i => data2D[i % width, i / width, (byte)feature.Height]).Sum();
			xAvg = (int)Math.Round(Enumerable.Range(0, width * height).Select(i => data2DAvg[i % width, i / width, 0]).Sum() / zSum) - 1;
			yAvg = (int)Math.Round(Enumerable.Range(0, width * height).Select(i => data2DAvg[i % width, i / width, 1]).Sum() / zSum) - 1;
			zAvg = (Enumerable.Range(0, width * height).Select(i => data2D[i % width, i / width, (byte)feature.Height]).Sum() / 2) / Enumerable.Range(0, width * height).Where(i => data2DMask[i % width, i / width] == true).Count();
		}

		protected void DisplayCenter()
		{
			for (int i = xAvg - 1; i <= xAvg + 1; i++)
			{
				for (int j = yAvg - 1; j <= yAvg + 1; j++)
				{
					data2D[i, j, (byte)feature.Height] = zMinAdj;
				}
			}
		}

	}

	//derived from coreClass
	public class objClass : coreClass
	{
		public objClass(string fileName) : base(fileName)
		{
		}

		public override bool GenerateRangeImage()
		{
			ObjMesh model;
			float[] pixels;
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



			width = (int)Math.Ceiling((Math.Ceiling(xMax) - Math.Floor(xMin)) / gridSize) + (borderAdditionWidth * 2);
			height = (int)Math.Ceiling((Math.Ceiling(yMax) - Math.Floor(yMin)) / gridSize) + (borderAdditionWidth * 2);

			eye = new Vector3d(0, 0, zMax);
			center = new Vector3d(0, 0, 0);
			up = new Vector3d(0, 1, 0);



			using (GameWindow window = new GameWindow(1, 1))
			{
				uint FboHandle;
				uint DepthRenderbuffer;

				GL.Ext.GenRenderbuffers(1, out DepthRenderbuffer);
				GL.Ext.BindRenderbuffer(RenderbufferTarget.RenderbufferExt, DepthRenderbuffer);
				GL.Ext.RenderbufferStorage(RenderbufferTarget.RenderbufferExt, (RenderbufferStorage)All.DepthComponent32, width, height);

				GL.Ext.GenFramebuffers(1, out FboHandle);
				GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, FboHandle);
				GL.Ext.FramebufferRenderbuffer(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachmentExt, RenderbufferTarget.RenderbufferExt, DepthRenderbuffer);

				GL.Enable(EnableCap.DepthTest);
				GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

				GL.DepthRange(0.0f, 1.0f);
				GL.ClearDepth(1.0f);


				GL.Viewport(0, 0, width, height);


				GL.MatrixMode(MatrixMode.Projection);
				GL.LoadIdentity();
				GL.Ortho(Math.Floor(xMin - borderAdditionWidth * gridSize), Math.Ceiling(xMax + borderAdditionWidth * gridSize), Math.Floor(yMin - borderAdditionWidth * gridSize), Math.Ceiling(yMax + borderAdditionWidth * gridSize), 0, zMax - zMin);


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
						data2D[i, j, (byte)feature.Height] = pixels[(height - 1 - j) * width + i] * zRange;
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

				int thumbnailImgWidth, thumbnailImgHeight;
				if (width >= height)
				{
					thumbnailImgWidth = thumbnailImgMaxSize;
					thumbnailImgHeight = (int)((double)height / ((double)width / (double)thumbnailImgMaxSize));
				}
				else
				{
					thumbnailImgWidth = (int)((double)width / ((double)height / (double)thumbnailImgMaxSize));
					thumbnailImgHeight = thumbnailImgMaxSize;
				}
				rangeThumbnailImg = (Bitmap)rangeImg.GetThumbnailImage(thumbnailImgWidth, thumbnailImgHeight, null, IntPtr.Zero);


				return true;
			}
		}
	}

	//derived from coreClass
	public class xyzClass : coreClass
	{
		public static int structEleSize { get; set; }
		public static int MorIterNum { get; set; }
		public static Emgu.CV.CvEnum.CV_ELEMENT_SHAPE structEleShape { get; set; }

		private struct PointCloud
		{
			public double x, y, z;
		}


		public xyzClass(string fileName) : base(fileName)
		{
			// 			structEleSize = 7;
			// 			MorIterNum = 2;
			// 			structEleShape = Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_RECT;
		}

		public override bool GenerateRangeImage()
		{
			int pcNum = 0;//點雲總數
			double[,] pcArray;//點雲陣列


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
			width = (int)Math.Ceiling((Math.Ceiling(xMax) - Math.Floor(xMin)) / gridSize) + (borderAdditionWidth * 2);
			height = (int)Math.Ceiling((Math.Ceiling(yMax) - Math.Floor(yMin)) / gridSize) + (borderAdditionWidth * 2);



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
				pcX = (int)Math.Floor((pcArray[i, 0] - (xMin - borderAdditionWidth * gridSize)) / gridSize);
				pcY = (int)Math.Floor((pcArray[i, 1] - (yMin - borderAdditionWidth * gridSize)) / gridSize);
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
					data2D[i, j, (byte)feature.Height] = zMaxArray[i, height - j - 1].z - zMin;
				}
			}



			//EmguCV的Morphology處理
			using (Image<Gray, Double> emguImg = new Image<Gray, Double>(width, height))
			{
				for (int i = 0; i < width; i++)
				{
					for (int j = 0; j < height; j++)
					{
						emguImg.Data[j, i, 0] = data2D[i, j, (byte)feature.Height];
					}
				}


				StructuringElementEx StructEle = new StructuringElementEx(structEleSize, structEleSize, (structEleSize - 1) / 2, (structEleSize - 1) / 2, structEleShape);
				CvInvoke.cvDilate(emguImg, emguImg, StructEle, MorIterNum);
				CvInvoke.cvErode(emguImg, emguImg, StructEle, MorIterNum);


				for (int i = 0; i < width; i++)
				{
					for (int j = 0; j < height; j++)
					{
						data2D[i, j, (byte)feature.Height] = emguImg.Data[j, i, 0];
					}
				}
			}

			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					if (data2D[i, j, (byte)feature.Height] != zMinAdj)
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

			int thumbnailImgWidth, thumbnailImgHeight;
			if (width >= height)
			{
				thumbnailImgWidth = thumbnailImgMaxSize;
				thumbnailImgHeight = (int)((double)height / ((double)width / (double)thumbnailImgMaxSize));
			}
			else
			{
				thumbnailImgWidth = (int)((double)width / ((double)height / (double)thumbnailImgMaxSize));
				thumbnailImgHeight = thumbnailImgMaxSize;
			}
			rangeThumbnailImg = (Bitmap)rangeImg.GetThumbnailImage(thumbnailImgWidth, thumbnailImgHeight, null, IntPtr.Zero);

			return true;
		}
	}

	public class utilClass
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

		static public Bitmap ResizeBitmap2Square(Bitmap originalImage)
		{
			int width = originalImage.Width;
			int height = originalImage.Height;
			int newWidth, newHeight, xCorner, yCorner;

			if (width >= height)
			{
				newWidth = newHeight = width;
				xCorner = 0;
				yCorner = (newWidth - height) / 2;
			}
			else
			{
				newWidth = newHeight = height;
				xCorner = (newHeight - width) / 2;
				yCorner = 0;
			}

			Bitmap squareImage = new Bitmap(newWidth, newHeight);
			Graphics g = Graphics.FromImage(squareImage);
			g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
			g.Clear(Color.White);
			g.DrawImage(originalImage, xCorner, yCorner);
			return squareImage;
		}
	}
}
