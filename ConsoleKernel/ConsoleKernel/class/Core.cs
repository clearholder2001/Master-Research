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

CPU加速：
在執行物件方法前，先呼叫CUDA的初始化函式
coreClass.Cudafy_initialization();
*/

using System;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Numerics;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using ImageTool;

using Emgu.CV;
using Emgu.CV.Structure;

using MN = MathNet.Numerics;
using MNL = MathNet.Numerics.LinearAlgebra;

using Cudafy;
using Cudafy.Host;
using Cudafy.Translator;

using DBQuery;

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
		public static bool generateImage;

		public static GPGPU gpu { get; private set; }
		public static CudafyModule km { get; private set; }
		public static bool cudaReady = false;
		public static int threadMaxNum = 32;
		//public static Queue<byte> cudaStreamQueue { get; private set; }



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

		public bool SaveResult(string path, bool overwrite)
		{
			if (File.Exists(path))
			{
				if (!overwrite)
				{
					return false;
				}
				else
				{
					File.Delete(path);
				}
			}


			//default: overwrite is false
			//Directory.CreateDirectory(Path.GetDirectoryName(path));
			using (StreamWriter sw = new StreamWriter(path, false))
			{
				sw.WriteLine(string.Format("{0:0.000000}", zRange));
				sw.WriteLine(string.Format("{0:0.000000}", zAvg));
				sw.WriteLine(string.Format("{0:0.000000}", totalArea));
				sw.WriteLine(string.Format("{0:0.000000}", totalVol));
				sw.WriteLine(string.Format("{0:0.000000}", radiusMax));

				double sum;
				for (int i = 0; i < featureDimension; i++)
				{
					if (i == 2 || i == 4)
						continue;


					sum = Enumerable.Range(0, annularNum).AsParallel().Select(a => annularFeature[i, a]).Sum();

					for (int j = 0; j < annularNum; j++)
					{
						if (sum != 0.0)
							sw.WriteLine(string.Format("{0:0.000000}", annularFeature[i, j] / sum));
						else
							sw.WriteLine(string.Format("{0:0.000000}", 0.0));
					}
				}

				//輸出原始數值
				/*
				for (int i = 0; i < featureDimension; i++)
				{
					for (int j = 0; j < annularNum; j++)
					{
						sw.WriteLine(string.Format("{0:0.######}", annularFeature[i, j]));
					}
				}
				*/

				return true;
			}
		}

		private void CalArea()
		{
			totalArea = Enumerable.Range(0, width * height).AsParallel().Where(i => data2DMask[i % width, i / width] == true).Count() * Math.Pow(gridSize, 2);
		}

		private void CalVol()
		{
			totalVol = Enumerable.Range(0, width * height).AsParallel().Where(i => data2DMask[i % width, i / width] == true).Select(i => data2D[i % width, i / width, (byte)feature.Height]).Sum();
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
				using (Image<Gray, float> emguLoGImg = emguImg.SmoothGaussian(gaussianAperture).Laplace(laplacianAperture))
				{
					Parallel.For(0, width - 1, i =>
					//for (int i = 0; i < width; i++)
					{
						for (int j = 0; j < height; j++)
						{
							data2D[i, j, 1] = Math.Abs(emguLoGImg.Data[j, i, (byte)feature.Height]);
						}
					}
					);
				}
			}



			//Eigen feature
			if (eigenFeatureDiameterMultiple >= 1)
			{
				eigenFeatureDiameter = gridSize * eigenFeatureDiameterMultiple;
			}
			else
			{
				Parallel.For(0, width - 1, i =>
				//for (int i = 0; i < width; i++)
				{
					for (int j = 0; j < height; j++)
					{
						data2D[i, j, (byte)feature.Linearity] = 0.0;
						data2D[i, j, (byte)feature.Planarity] = 0.0;
						data2D[i, j, (byte)feature.Sphericity] = 0.0;
					}
				}
				);
				return;
			}



			//if (!cudaReady)
			//	Cudafy_initialization();

			
			if (cudaReady)
			{
				double[,,] data2DHeight = new double[width, height, 1];
				byte[,] data2DMask = new byte[width, height];
				double[,,] eigenFeature = new double[width, height, 3];

				Parallel.For(0, width - 1, i =>
				//for (int i = 0; i < width; i++)
				{
					for (int j = 0; j < height; j++)
					{
						data2DHeight[i, j, 0] = data2D[i, j, (byte)feature.Height];

						if (this.data2DMask[i, j])
							data2DMask[i, j] = 1;
						else
							data2DMask[i, j] = 0;
					}
				}
				);


				int streamid = 0;
				//gpu.CreateStream(streamid);


				//int threadMaxNum = 1;

				dim3 gridDim = new dim3(((width * height) / threadMaxNum) + 1, 1, 1);
				dim3 blockDim = new dim3(threadMaxNum, 1, 1);

				//dim3 gridDim = new dim3(width, height, 1);
				//dim3 blockDim = new dim3(1, 1, 1);

				
				IntPtr ptr_imgData = gpu.HostAllocate<double>(width, height, 1);
				IntPtr ptr_data2DMask = gpu.HostAllocate<bool>(width, height);
				IntPtr ptr_eigenFeature = gpu.HostAllocate<double>(width, height, 3);

				ptr_imgData.Write<double>(data2DHeight);
				ptr_data2DMask.Write<byte>(data2DMask);

				double[,,] dev_imgData = gpu.Allocate<double>(width, height, 1);
				byte[,] dev_data2DMask = gpu.Allocate<byte>(width, height);
				double[,,] dev_eigenFeature = gpu.Allocate<double>(width, height, 3);

				gpu.CopyToDeviceAsync<double>(ptr_imgData, 0, dev_imgData, 0, width * height, streamid);
				gpu.CopyToDeviceAsync<byte>(ptr_data2DMask, 0, dev_data2DMask, 0, width * height, streamid);
				gpu.LaunchAsync(gridDim, blockDim, streamid, "GenerateEigenFeature", eigenFeatureDiameterMultiple, (float)gridSize, dev_imgData, dev_data2DMask, dev_eigenFeature);
				gpu.CopyFromDeviceAsync<double>(dev_eigenFeature, 0, ptr_eigenFeature, 0, width * height * 3, streamid);

				gpu.SynchronizeStream(streamid);
				//gpu.DestroyStream(streamid);

				ptr_eigenFeature.Read<double>(eigenFeature);


				gpu.HostFree(ptr_imgData);
				gpu.HostFree(ptr_data2DMask);
				gpu.HostFree(ptr_eigenFeature);
				gpu.Free(dev_imgData);
				gpu.Free(dev_data2DMask);
				gpu.Free(dev_eigenFeature);


				Parallel.For(0, width - 1, i =>
				//for (int i = 0; i < width; i++)
				{
					for (int j = 0; j < height; j++)
					{
						data2D[i, j, (byte)feature.Linearity] = eigenFeature[i, j, 0];
						data2D[i, j, (byte)feature.Planarity] = eigenFeature[i, j, 1];
						data2D[i, j, (byte)feature.Sphericity] = eigenFeature[i, j, 2];
					}
				}
				);
			}
			else
			{
				//MN.Control.UseNativeCUDA();
				MN.Control.UseNativeMKL();
				//MN.Control.UseManaged();
				//CudaLinearAlgebraProvider cu = new CudaLinearAlgebraProvider();



				Parallel.For(0, width - 1, i =>
				//for (int i = 0; i < width; i++)
				{
					for (int j = 0; j < height; j++)
					{
						if (this.data2DMask[i, j])
						{
							if (i < eigenFeatureDiameterMultiple || i > width - 1 - eigenFeatureDiameterMultiple || j < eigenFeatureDiameterMultiple || j > height - 1 - eigenFeatureDiameterMultiple)
							{
								break;
							}
							else
							{
								int neighborCount = 0;
								bool[,] isNeighbor = new bool[2 * eigenFeatureDiameterMultiple + 1, 2 * eigenFeatureDiameterMultiple + 1];

								for (int k = 0; k < isNeighbor.GetLength(0); k++)
								{
									for (int l = 0; l < isNeighbor.GetLength(1); l++)
									{
										isNeighbor[k, l] = false;
									}
								}

								double xSum = 0;
								double ySum = 0;
								double zSum = 0;


								for (int k = i - eigenFeatureDiameterMultiple; k <= i + eigenFeatureDiameterMultiple; k++)
								{
									for (int l = j - eigenFeatureDiameterMultiple; l <= j + eigenFeatureDiameterMultiple; l++)
									{
										if (this.data2DMask[k, l] && Math.Sqrt(Math.Pow((k - i) * gridSize, 2) + Math.Pow((l - j) * gridSize, 2) + Math.Pow(data2D[i, j, (byte)feature.Height] - data2D[k, l, (byte)feature.Height], 2)) < eigenFeatureDiameter)
										{
											isNeighbor[k - (i - eigenFeatureDiameterMultiple), l - (j - eigenFeatureDiameterMultiple)] = true;

											xSum += k * gridSize;
											ySum += l * gridSize;
											zSum += data2D[k, l, 0];

											neighborCount++;
										}
									}
								}

								if (neighborCount < 2)
								{
									data2D[i, j, (byte)feature.Linearity] = 0;
									data2D[i, j, (byte)feature.Planarity] = 0;
									data2D[i, j, (byte)feature.Sphericity] = 0;
								}
								else
								{
									double[] mean = new double[3] { xSum / neighborCount, ySum / neighborCount, zSum / neighborCount};
									MNL.Matrix<double> eigenMatrix = MNL.Matrix<double>.Build.Dense(3, 3, 0.0);

									for (int k = i - eigenFeatureDiameterMultiple; k <= i + eigenFeatureDiameterMultiple; k++)
									{
										for (int l = j - eigenFeatureDiameterMultiple; l <= j + eigenFeatureDiameterMultiple; l++)
										{
											if (isNeighbor[k - (i - eigenFeatureDiameterMultiple), l - (j - eigenFeatureDiameterMultiple)])
											{
												double x = k * gridSize - mean[0];
												double y = l * gridSize - mean[1];
												double z = data2D[k, l, 0] - mean[2];

												eigenMatrix[0, 0] += x * x;
												eigenMatrix[0, 1] += x * y;
												eigenMatrix[0, 2] += x * z;
												eigenMatrix[1, 0] += y * x;
												eigenMatrix[1, 1] += y * y;
												eigenMatrix[1, 2] += y * z;
												eigenMatrix[2, 0] += z * x;
												eigenMatrix[2, 1] += z * y;
												eigenMatrix[2, 2] += z * z;
											}
										}
									}

									eigenMatrix /= neighborCount;
									var eigenResult = eigenMatrix.Evd();
									MNL.Vector<Complex> eigenValues = eigenResult.EigenValues;


									double eigenValues1 = eigenValues[2].Magnitude;
									double eigenValues2 = eigenValues[1].Magnitude;
									double eigenValues3 = eigenValues[0].Magnitude;


									if (eigenValues1 < 0)
										eigenValues1 = 0;
									if (eigenValues2 < 0)
										eigenValues2 = 0;
									if (eigenValues3 < 0)
										eigenValues3 = 0;


									double Linearity = 0;
									double Planarity = 0;
									double Sphericity = 0;


									if (eigenValues1 > 0)
									{
										Linearity = (eigenValues1 - eigenValues2) / eigenValues1;
										Planarity = (eigenValues2 - eigenValues3) / eigenValues1;
										Sphericity = eigenValues3 / eigenValues1;
									}


									data2D[i, j, (byte)feature.Linearity] = Linearity;
									data2D[i, j, (byte)feature.Planarity] = Planarity;
									data2D[i, j, (byte)feature.Sphericity] = Sphericity;
								}

							}
						}
					}
				}
				);

			}
		}

		private void CalAnnularFeature()
		{
			radiusMax = Enumerable.Range(0, width * height).AsParallel().Where(i => data2DMask[i % width, i / width] == true).Select(i => Math.Sqrt(Math.Pow((i % width) - xAvg, 2) + Math.Pow((i / width) - yAvg, 2))).Max() * gridSize;

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

			Parallel.For(0, width - 1, i =>
			//for (int i = 0; i < width; i++)
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
			);

			//apply annular weight
			Parallel.For(0, coreClass.featureDimension - 1, i =>
			//for (int i = 0; i < coreClass.featureDimension; i++)
			{
				for (int j = 1; j <= coreClass.annularNum; j++)
				{
					annularFeature[i, j - 1] /= ((j + (j - 1)) * (j - (j - 1)));
				}
			}
			);
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
			double zSum = Enumerable.Range(0, width * height).AsParallel().Select(i => data2D[i % width, i / width, (byte)feature.Height]).Sum();
			xAvg = (int)Math.Round(Enumerable.Range(0, width * height).AsParallel().Select(i => data2DAvg[i % width, i / width, 0]).Sum() / zSum) - 1;
			yAvg = (int)Math.Round(Enumerable.Range(0, width * height).AsParallel().Select(i => data2DAvg[i % width, i / width, 1]).Sum() / zSum) - 1;
			zAvg = (Enumerable.Range(0, width * height).AsParallel().Select(i => data2D[i % width, i / width, (byte)feature.Height]).Sum() / 2) / Enumerable.Range(0, width * height).Where(i => data2DMask[i % width, i / width] == true).Count();
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

		public static void Cudafy_initialization(int ePlatform, int CUDADeviceId, int eArchitecture, bool CUDAGenerateDebug)
		{
			try
			{
				km = CudafyModule.TryDeserialize();
				if (km == null || !km.TryVerifyChecksums())
				{
					km = CudafyTranslator.Cudafy((ePlatform)ePlatform, (eArchitecture)eArchitecture);
					km.Serialize();
				}

				CudafyTranslator.GenerateDebug = CUDAGenerateDebug;

				gpu = CudafyHost.GetDevice(eGPUType.Cuda, CUDADeviceId);

				gpu.UnloadModules();
				gpu.FreeAll();

				gpu.LoadModule(km);

				cudaReady = true;				
			}
			catch (Exception)
			{ 
				cudaReady = false;
				return;
			}
		}

		#region Cudafy_bak
		//		[Cudafy1]
		//		public static void GenerateEigenFeature(GThread thread, double[,,] data2D, byte[,] data2DMask, double[,,] eigenFeature)
		//		{
		//			int i = (int)Math.Floor((double)(thread.blockIdx.x * thread.blockDim.x + thread.threadIdx.x) / data2D.GetLength(1));
		//			int j = (thread.blockIdx.x * thread.blockDim.x + thread.threadIdx.x) % data2D.GetLength(1);
		//			int t = thread.threadIdx.x;
		//
		//			int width = data2D.GetLength(0);
		//			int height = data2D.GetLength(1);
		//
		//
		//			int eigenFeatureDiameterMultiple = 5;
		//			double gridSize = 0.1;
		//			double eigenFeatureDiameter = gridSize * eigenFeatureDiameterMultiple;
		//
		//
		//			if (data2DMask[i, j] == 1)
		//			{
		//				if (i < eigenFeatureDiameterMultiple || i > width - 1 - eigenFeatureDiameterMultiple || j < eigenFeatureDiameterMultiple || j > height - 1 - eigenFeatureDiameterMultiple)
		//				{
		//					eigenFeature[i, j, 0] = 0;
		//					eigenFeature[i, j, 1] = 0;
		//					eigenFeature[i, j, 2] = 0;
		//				}
		//				else
		//				{
		//					int indexCount = 0;
		//					int neighborCount = 0;
		//
		//
		//					double[,,] neighborList = thread.AllocateShared<double>("neighborList", threadMaxNum, 121, 3);
		//
		//					for (int k = i - eigenFeatureDiameterMultiple; k <= i + eigenFeatureDiameterMultiple; k++)
		//					{
		//						for (int l = j - eigenFeatureDiameterMultiple; l <= j + eigenFeatureDiameterMultiple; l++)
		//						{
		//							if (data2DMask[k, l] == 1)
		//							{
		//								double xDis = Math.Pow((k - i) * gridSize, 2);
		//								double yDis = Math.Pow((l - j) * gridSize, 2);
		//								double zDis = Math.Pow(data2D[i, j, 0] - data2D[k, l, 0], 2);
		//								double totalDis = Math.Sqrt(xDis + yDis + zDis);
		//
		//								if (totalDis < eigenFeatureDiameter)
		//								{
		//									neighborList[t, indexCount, 0] = k * gridSize;
		//									neighborList[t, indexCount, 1] = l * gridSize;
		//									neighborList[t, indexCount, 2] = data2D[k, l, 0];
		//
		//									neighborCount++;
		//								}
		//								else
		//								{
		//									neighborList[t, indexCount, 0] = 0.0;
		//									neighborList[t, indexCount, 1] = 0.0;
		//									neighborList[t, indexCount, 2] = 0.0;
		//								}
		//							}
		//							else
		//							{
		//								neighborList[t, indexCount, 0] = 0.0;
		//								neighborList[t, indexCount, 1] = 0.0;
		//								neighborList[t, indexCount, 2] = 0.0;
		//							}
		//							indexCount++;
		//						}
		//					}
		//
		//
		//
		//					if (neighborCount < 2)
		//					{
		//						eigenFeature[i, j, 0] = 0;
		//						eigenFeature[i, j, 1] = 0;
		//						eigenFeature[i, j, 2] = 0;
		//					}
		//					else
		//					{
		//						double xSum = 0.0;
		//						double ySum = 0.0;
		//						double zSum = 0.0;
		//
		//						for (int k = 0; k < indexCount; k++)
		//						{
		//							xSum += neighborList[t, k, 0];
		//							ySum += neighborList[t, k, 1];
		//							zSum += neighborList[t, k, 2];
		//						}
		//
		//						double xAverage = xSum / neighborCount;
		//						double yAverage = ySum / neighborCount;
		//						double zAverage = zSum / neighborCount;
		//
		//
		//						for (int k = 0; k < indexCount; k++)
		//						{
		//							if (neighborList[t, k, 0] != 0 && neighborList[t, k, 1] != 0 && neighborList[t, k, 2] != 0)
		//							{
		//								neighborList[t, k, 0] -= xAverage;
		//								neighborList[t, k, 1] -= yAverage;
		//								neighborList[t, k, 2] -= zAverage;
		//							}
		//						}
		//
		//
		//
		//						double[,,] eigenMatrix = thread.AllocateShared<double>("eigenMatrix", threadMaxNum, 3, 3);
		//
		//						for (int k = 0; k < 3; k++)
		//						{
		//							for (int l = 0; l < 3; l++)
		//							{
		//								eigenMatrix[t, k, l] = 0.0f;
		//								for (int m = 0; m < indexCount; m++)
		//								{
		//									eigenMatrix[t, k, l] += neighborList[t, m, k] * neighborList[t, m, l];
		//								}
		//								eigenMatrix[t, k, l] /= neighborCount;
		//							}
		//						}
		//
		//
		//
		//						double[,] eigenValues = thread.AllocateShared<double>("eigenValue", threadMaxNum, 3);
		//
		//						double p1 = Math.Pow(eigenMatrix[t, 0, 1], 2) + Math.Pow(eigenMatrix[t, 0, 2], 2) + Math.Pow(eigenMatrix[t, 1, 2], 2);
		//						if (p1 == 0)
		//						{
		//							double[] v = thread.AllocateShared<double>("v", 3);
		//
		//							v[0] = eigenMatrix[t, 0, 0];
		//							v[1] = eigenMatrix[t, 1, 1];
		//							v[2] = eigenMatrix[t, 2, 2];
		//
		//							if (v[0] >= v[1] && v[0] >= v[2])
		//							{
		//								eigenValues[t, 0] = v[0];
		//
		//								if (v[1] >= v[2])
		//								{
		//									eigenValues[t, 1] = v[1];
		//									eigenValues[t, 2] = v[2];
		//								}
		//								else
		//								{
		//									eigenValues[t, 1] = v[2];
		//									eigenValues[t, 2] = v[1];
		//								}
		//							}
		//							else if (v[1] >= v[0] && v[1] >= v[2])
		//							{
		//								eigenValues[t, 0] = v[1];
		//
		//								if (v[0] >= v[2])
		//								{
		//									eigenValues[t, 1] = v[0];
		//									eigenValues[t, 2] = v[2];
		//								}
		//								else
		//								{
		//									eigenValues[t, 1] = v[2];
		//									eigenValues[t, 2] = v[0];
		//								}
		//							}
		//							else if (v[2] >= v[0] && v[2] >= v[1])
		//							{
		//								eigenValues[t, 0] = v[2];
		//
		//								if (v[0] >= v[1])
		//								{
		//									eigenValues[t, 1] = v[0];
		//									eigenValues[t, 2] = v[1];
		//								}
		//								else
		//								{
		//									eigenValues[t, 1] = v[1];
		//									eigenValues[t, 2] = v[0];
		//								}
		//							}
		//						}
		//						else
		//						{
		//							double q = (eigenMatrix[t, 0, 0] + eigenMatrix[t, 1, 1] + eigenMatrix[t, 2, 2]) / 3;
		//							double p2 = Math.Pow(eigenMatrix[t, 0, 0] - q, 2) + Math.Pow(eigenMatrix[t, 1, 1] - q, 2) + Math.Pow(eigenMatrix[t, 2, 2] - q, 2) + 2 * p1;
		//							double p = Math.Sqrt(p2 / 6);
		//
		//							double[,,] B = thread.AllocateShared<double>("B", threadMaxNum, 3, 3);
		//
		//							for (int k = 0; k < 3; k++)
		//							{
		//								for (int l = 0; l < 3; l++)
		//								{
		//									float I = 0.0f;
		//									if (k == l)
		//										I = 1;
		//									B[t, k, l] = (1 / p) * (eigenMatrix[t, k, l] - q * I);
		//								}
		//							}
		//
		//							double r = (B[t, 0, 0] * B[t, 1, 1] * B[t, 2, 2] + B[t, 0, 1] * B[t, 1, 2] * B[t, 2, 0] + B[t, 0, 2] * B[t, 1, 0] * B[t, 2, 1] - B[t, 0, 2] * B[t, 1, 1] * B[t, 2, 0] - B[t, 0, 0] * B[t, 1, 2] * B[t, 2, 1] - B[t, 0, 1] * B[t, 1, 0] * B[t, 2, 2]) / 2;
		//
		//							double phi;
		//
		//							if (r <= -1)
		//								phi = Math.PI / 3;
		//							else if (r >= 1)
		//								phi = 0;
		//							else
		//								phi = Math.Acos(r) / 3;
		//
		//							eigenValues[t, 0] = q + 2 * p * Math.Cos(phi);
		//							eigenValues[t, 2] = q + 2 * p * Math.Cos(phi + (2 * Math.PI / 3));
		//							eigenValues[t, 1] = 3 * q - eigenValues[t, 0] - eigenValues[t, 2];
		//						}
		//
		//
		//						for (int k = 0; k < 3; k++)
		//						{
		//							if (eigenValues[t, k] < 0)
		//								eigenValues[t, k] = 0;
		//						}
		//
		//
		//						double Linearity = 0.0;
		//						double Planarity = 0.0;
		//						double Sphericity = 0.0;
		//
		//
		//						if (eigenValues[t, 0] != 0)
		//						{
		//							Linearity = (eigenValues[t, 0] - eigenValues[t, 1]) / eigenValues[t, 0];
		//							Planarity = (eigenValues[t, 1] - eigenValues[t, 2]) / eigenValues[t, 0];
		//							Sphericity = eigenValues[t, 2] / eigenValues[t, 0];
		//						}
		//
		//
		//						eigenFeature[i, j, 0] = Linearity;
		//						eigenFeature[i, j, 1] = Planarity;
		//						eigenFeature[i, j, 2] = Sphericity;
		//
		//
		//						/*
		//						double Linearity = (eigenValues[0] - eigenValues[1]) / eigenValues[0];
		//						double Planarity = (eigenValues[1] - eigenValues[2]) / eigenValues[0];
		//						double Sphericity = eigenValues[2] / eigenValues[0];
		//
		//						if (!double.IsNaN(Linearity) && !double.IsInfinity(Linearity))
		//							eigenFeature[i, j, 0] = Linearity;
		//						else
		//							eigenFeature[i, j, 0] = 0;
		//
		//						if (!double.IsNaN(Planarity) && !double.IsInfinity(Planarity))
		//							eigenFeature[i, j, 1] = Planarity;
		//						else
		//							eigenFeature[i, j, 1] = 0;
		//
		//						if (!double.IsNaN(Sphericity) && !double.IsInfinity(Sphericity))
		//							eigenFeature[i, j, 2] = Sphericity;
		//						else
		//							eigenFeature[i, j, 2] = 0;
		//						*/
		//
		//
		//						//eigenFeature[i, j, 0] = 1;
		//						//eigenFeature[i, j, 1] = 1;
		//						//eigenFeature[i, j, 2] = 1;
		//					}
		//				}
		//			}
		//			else
		//			{
		//				eigenFeature[i, j, 0] = 0;
		//				eigenFeature[i, j, 1] = 0;
		//				eigenFeature[i, j, 2] = 0;
		//			}
		//
		//		}


		//		[Cudafy2]
		//		public static void GenerateEigenFeature(GThread thread, double[,,] data2D, byte[,] data2DMask, double[,,] eigenFeature)
		//		{
		//			int i = (int)GMath.Floor((thread.blockIdx.x * thread.blockDim.x + thread.threadIdx.x) / data2D.GetLength(1));
		//			int j = (thread.blockIdx.x * thread.blockDim.x + thread.threadIdx.x) % data2D.GetLength(1);
		//			int t = thread.threadIdx.x;
		//
		//			int width = data2D.GetLength(0);
		//			int height = data2D.GetLength(1);
		//
		//			
		//			int eigenFeatureDiameterMultiple = 4;
		//			float gridSize = 0.1f;
		//			float eigenFeatureDiameter = gridSize * eigenFeatureDiameterMultiple;
		//
		//
		//			if (data2DMask[i, j] == 1)
		//			{
		//				if (i < eigenFeatureDiameterMultiple || i > width - 1 - eigenFeatureDiameterMultiple || j < eigenFeatureDiameterMultiple || j > height - 1 - eigenFeatureDiameterMultiple)
		//				{
		//					return;
		//				}
		//				else
		//				{
		//					int indexCount = 0;
		//					int neighborCount = 0;
		//
		//
		//					float[,,] neighborList = thread.AllocateShared<float>("neighborList", threadMaxNum, 81, 3);
		//
		//					for (int k = i - eigenFeatureDiameterMultiple; k <= i + eigenFeatureDiameterMultiple; k++)
		//					{
		//						for (int l = j - eigenFeatureDiameterMultiple; l <= j + eigenFeatureDiameterMultiple; l++)
		//						{
		//							if (data2DMask[k, l] == 1)
		//							{
		//								float xDis = GMath.Pow((k - i) * gridSize, 2);
		//								float yDis = GMath.Pow((l - j) * gridSize, 2);
		//								float zDis = GMath.Pow((float)(data2D[i, j, 0] - data2D[k, l, 0]), 2);
		//								float totalDis = GMath.Sqrt(xDis + yDis + zDis);
		//
		//								if (totalDis < eigenFeatureDiameter)
		//								{
		//									neighborList[t, indexCount, 0] = k * gridSize;
		//									neighborList[t, indexCount, 1] = l * gridSize;
		//									neighborList[t, indexCount, 2] = (float)data2D[k, l, 0];
		//
		//									neighborCount++;
		//								}
		//								else
		//								{
		//									neighborList[t, indexCount, 0] = 0;
		//									neighborList[t, indexCount, 1] = 0;
		//									neighborList[t, indexCount, 2] = 0;
		//								}
		//							}
		//							else
		//							{
		//								neighborList[t, indexCount, 0] = 0;
		//								neighborList[t, indexCount, 1] = 0;
		//								neighborList[t, indexCount, 2] = 0;
		//							}
		//							indexCount++;
		//						}
		//					}
		//
		//
		//
		//					if (neighborCount >= 2)
		//					{
		//						float xSum = 0;
		//						float ySum = 0;
		//						float zSum = 0;
		//
		//						for (int k = 0; k < indexCount; k++)
		//						{
		//							xSum += neighborList[t, k, 0];
		//							ySum += neighborList[t, k, 1];
		//							zSum += neighborList[t, k, 2];
		//						}
		//
		//						float xAverage = xSum / neighborCount;
		//						float yAverage = ySum / neighborCount;
		//						float zAverage = zSum / neighborCount;
		//
		//
		//						for (int k = 0; k < indexCount; k++)
		//						{
		//							if (neighborList[t, k, 0] != 0 && neighborList[t, k, 1] != 0 && neighborList[t, k, 2] != 0)
		//							{
		//								neighborList[t, k, 0] -= xAverage;
		//								neighborList[t, k, 1] -= yAverage;
		//								neighborList[t, k, 2] -= zAverage;
		//							}
		//						}
		//
		//
		//
		//						float[,,] eigenMatrix = thread.AllocateShared<float>("eigenMatrix", threadMaxNum, 3, 3);
		//
		//						for (int k = 0; k < 3; k++)
		//						{
		//							for (int l = 0; l < 3; l++)
		//							{
		//								eigenMatrix[t, k, l] = 0.0f;
		//								for (int m = 0; m < indexCount; m++)
		//								{
		//									eigenMatrix[t, k, l] += neighborList[t, m, k] * neighborList[t, m, l];
		//								}
		//								eigenMatrix[t, k, l] /= neighborCount;
		//							}
		//						}
		//
		//
		//
		//						//float[,] eigenValues = thread.AllocateShared<float>("eigenValue", threadMaxNum, 3);
		//						float eigenValues1 = 0;
		//						float eigenValues2 = 0;
		//						float eigenValues3 = 0;
		//
		//						float p1 = GMath.Pow(eigenMatrix[t, 0, 1], 2) + GMath.Pow(eigenMatrix[t, 0, 2], 2) + GMath.Pow(eigenMatrix[t, 1, 2], 2);
		//						if (p1 == 0)
		//						{
		//							//float[] v = thread.AllocateShared<float>("v", 3);
		//							float v1, v2, v3;
		//
		//							v1 = eigenMatrix[t, 0, 0];
		//							v2 = eigenMatrix[t, 1, 1];
		//							v3 = eigenMatrix[t, 2, 2];
		//
		//							if (v1 >= v2 && v1 >= v3)
		//							{
		//								eigenValues1 = v1;
		//
		//								if (v2 >= v3)
		//								{
		//									eigenValues2 = v2;
		//									eigenValues3 = v3;
		//								}
		//								else
		//								{
		//									eigenValues2 = v3;
		//									eigenValues3 = v2;
		//								}
		//							}
		//							else if (v2 >= v1 && v2 >= v3)
		//							{
		//								eigenValues1 = v2;
		//
		//								if (v1 >= v3)
		//								{
		//									eigenValues2 = v1;
		//									eigenValues3 = v3;
		//								}
		//								else
		//								{
		//									eigenValues2 = v3;
		//									eigenValues3 = v1;
		//								}
		//							}
		//							else if (v3 >= v1 && v3 >= v2)
		//							{
		//								eigenValues1 = v3;
		//
		//								if (v1 >= v2)
		//								{
		//									eigenValues2 = v1;
		//									eigenValues3 = v2;
		//								}
		//								else
		//								{
		//									eigenValues2 = v2;
		//									eigenValues3 = v1;
		//								}
		//							}
		//						}
		//						else
		//						{
		//							float q = (eigenMatrix[t, 0, 0] + eigenMatrix[t, 1, 1] + eigenMatrix[t, 2, 2]) / 3;
		//							float p2 = GMath.Pow(eigenMatrix[t, 0, 0] - q, 2) + GMath.Pow(eigenMatrix[t, 1, 1] - q, 2) + GMath.Pow(eigenMatrix[t, 2, 2] - q, 2) + 2 * p1;
		//							float p = GMath.Sqrt(p2 / 6);
		//
		//							//float[,] B = thread.AllocateShared<float>("B", 3, 3);
		//							float B1, B2, B3, B4, B5, B6, B7, B8, B9;
		//
		//							/*
		//							for (int k = 0; k < 3; k++)
		//							{
		//								for (int l = 0; l < 3; l++)
		//								{
		//									float I = 0.0f;
		//									if (k == l)
		//										I = 1;
		//									B[k, l] = (1 / p) * (eigenMatrix[t, k, l] - q * I);
		//								}
		//							}
		//							*/
		//							
		//							B1 = (1 / p) * (eigenMatrix[t, 0, 0] - q);
		//							B2 = (1 / p) * (eigenMatrix[t, 0, 1]);
		//							B3 = (1 / p) * (eigenMatrix[t, 0, 2]);
		//							B4 = (1 / p) * (eigenMatrix[t, 1, 0]);
		//							B5 = (1 / p) * (eigenMatrix[t, 1, 1] - q);
		//							B6 = (1 / p) * (eigenMatrix[t, 1, 2]);
		//							B7 = (1 / p) * (eigenMatrix[t, 2, 0]);
		//							B8 = (1 / p) * (eigenMatrix[t, 2, 1]);
		//							B9 = (1 / p) * (eigenMatrix[t, 2, 2] - q);
		//
		//							float r = (B1 * B5 * B9 + B2 * B6 * B7 + B3 * B4 * B8 - B3 * B5 * B7 - B1 * B6 * B8 - B2 * B4 * B9) / 2;
		//
		//							float phi;
		//
		//							if (r <= -1)
		//								phi = GMath.PI / 3;
		//							else if (r >= 1)
		//								phi = 0;
		//							else
		//								phi = GMath.Acos(r) / 3;
		//
		//							eigenValues1 = q + 2 * p * GMath.Cos(phi);
		//							eigenValues3 = q + 2 * p * GMath.Cos(phi + (2 * GMath.PI / 3));
		//							eigenValues2 = 3 * q - eigenValues1 - eigenValues3;
		//						}
		//
		//
		//						//for (int k = 0; k < 3; k++)
		//						//{
		//						//	if (eigenValues[t, k] < 0)
		//						//		eigenValues[t, k] = 0;
		//						//}
		//
		//
		//						/*
		//						if (eigenValues1 < 0.0000001)
		//							eigenValues1 = 0;
		//						if (eigenValues2 < 0.0000001)
		//							eigenValues2 = 0;
		//						if (eigenValues3 < 0.0000001)
		//							eigenValues3 = 0;
		//						*/
		//
		//						/*
		//						double Linearity = 0;
		//						double Planarity = 0;
		//						double Sphericity = 0;
		//						
		//						
		//						if (eigenValues1 != 0)
		//						{
		//							Linearity = (eigenValues1 - eigenValues2) / eigenValues1;
		//							Planarity = (eigenValues2 - eigenValues3) / eigenValues1;
		//							Sphericity = eigenValues3 / eigenValues1;
		//						}
		//
		//
		//						eigenFeature[i, j, 0] = Linearity;
		//						eigenFeature[i, j, 1] = Planarity;
		//						eigenFeature[i, j, 2] = Sphericity;
		//						*/
		//
		//						
		//						double Linearity = (eigenValues1 - eigenValues2) / eigenValues1;
		//						double Planarity = (eigenValues2 - eigenValues3) / eigenValues1;
		//						double Sphericity = eigenValues3 / eigenValues1;
		//						
		//						if (!double.IsNaN(Linearity) && !double.IsInfinity(Linearity))
		//							eigenFeature[i, j, 0] = Linearity;
		//						else
		//							eigenFeature[i, j, 0] = 0;
		//
		//						if (!double.IsNaN(Planarity) && !double.IsInfinity(Planarity))
		//							eigenFeature[i, j, 1] = Planarity;
		//						else
		//							eigenFeature[i, j, 1] = 0;
		//
		//						if (!double.IsNaN(Sphericity) && !double.IsInfinity(Sphericity))
		//							eigenFeature[i, j, 2] = Sphericity;
		//						else
		//							eigenFeature[i, j, 2] = 0;
		//
		//
		//
		//						//eigenFeature[i, j, 0] = xSum;
		//						//eigenFeature[i, j, 1] = ySum;
		//						//eigenFeature[i, j, 2] = zSum;
		//					}
		//				}
		//			}
		//		} 
		#endregion

		#region Cudafy
		[Cudafy]
		public static void GenerateEigenFeature(GThread thread, int eigenFeatureDiameterMultiple, float gridSize, double[,,] data2D, byte[,] data2DMask, double[,,] eigenFeature)
		{
			int i = (int)GMath.Floor((thread.blockIdx.x * thread.blockDim.x + thread.threadIdx.x) / data2D.GetLength(1));
			int j = (thread.blockIdx.x * thread.blockDim.x + thread.threadIdx.x) % data2D.GetLength(1);
			int t = thread.threadIdx.x;

			int width = data2D.GetLength(0);
			int height = data2D.GetLength(1);

			if (i < 0 || i > width - 1)
				return;
			if (j < 0 || j > height - 1)
				return;

			//int eigenFeatureDiameterMultiple = 4;
			//float gridSize = 0.1f;
			float eigenFeatureDiameter = gridSize * eigenFeatureDiameterMultiple;


			if (data2DMask[i, j] == 1)
			{
				if (i < eigenFeatureDiameterMultiple || i > width - 1 - eigenFeatureDiameterMultiple || j < eigenFeatureDiameterMultiple || j > height - 1 - eigenFeatureDiameterMultiple)
				{
					return;
				}
				else
				{
					int indexCount = 0;
					int neighborCount = 0;

					float xSum = 0;
					float ySum = 0;
					float zSum = 0;



					for (int k = i - eigenFeatureDiameterMultiple; k <= i + eigenFeatureDiameterMultiple; k++)
					{
						for (int l = j - eigenFeatureDiameterMultiple; l <= j + eigenFeatureDiameterMultiple; l++)
						{
							if (data2DMask[k, l] == 1)
							{
								float xDis = GMath.Pow((k - i) * gridSize, 2);
								float yDis = GMath.Pow((l - j) * gridSize, 2);
								float zDis = GMath.Pow((float)(data2D[i, j, 0] - data2D[k, l, 0]), 2);
								float totalDis = GMath.Sqrt(xDis + yDis + zDis);

								if (totalDis < eigenFeatureDiameter)
								{
									xSum += k * gridSize;
									ySum += l * gridSize;
									zSum += (float)data2D[k, l, 0];

									neighborCount++;
								}
							}
							indexCount++;
						}
					}



					if (neighborCount >= 2)
					{
						float xAverage = xSum / neighborCount;
						float yAverage = ySum / neighborCount;
						float zAverage = zSum / neighborCount;

						float eigenMatrix11 = 0;
						float eigenMatrix12 = 0;
						float eigenMatrix13 = 0;
						float eigenMatrix21 = 0;
						float eigenMatrix22 = 0;
						float eigenMatrix23 = 0;
						float eigenMatrix31 = 0;
						float eigenMatrix32 = 0;
						float eigenMatrix33 = 0;



						for (int k = i - eigenFeatureDiameterMultiple; k <= i + eigenFeatureDiameterMultiple; k++)
						{
							for (int l = j - eigenFeatureDiameterMultiple; l <= j + eigenFeatureDiameterMultiple; l++)
							{
								if (data2DMask[k, l] == 1)
								{
									float xDis = GMath.Pow((k - i) * gridSize, 2);
									float yDis = GMath.Pow((l - j) * gridSize, 2);
									float zDis = GMath.Pow((float)(data2D[i, j, 0] - data2D[k, l, 0]), 2);
									float totalDis = GMath.Sqrt(xDis + yDis + zDis);

									if (totalDis < eigenFeatureDiameter)
									{
										float x = k * gridSize - xAverage;
										float y = l * gridSize - yAverage;
										float z = (float)data2D[k, l, 0] - zAverage;

										eigenMatrix11 += x * x;
										eigenMatrix12 += x * y;
										eigenMatrix13 += x * z;
										eigenMatrix21 += y * x;
										eigenMatrix22 += y * y;
										eigenMatrix23 += y * z;
										eigenMatrix31 += z * x;
										eigenMatrix32 += z * y;
										eigenMatrix33 += z * z;
									}
								}
							}
						}



						eigenMatrix11 /= neighborCount;
						eigenMatrix12 /= neighborCount;
						eigenMatrix13 /= neighborCount;
						eigenMatrix21 /= neighborCount;
						eigenMatrix22 /= neighborCount;
						eigenMatrix23 /= neighborCount;
						eigenMatrix31 /= neighborCount;
						eigenMatrix32 /= neighborCount;
						eigenMatrix33 /= neighborCount;

						float eigenValues1 = 0;
						float eigenValues2 = 0;
						float eigenValues3 = 0;



						float p1 = GMath.Pow(eigenMatrix12, 2) + GMath.Pow(eigenMatrix13, 2) + GMath.Pow(eigenMatrix23, 2);
						if (p1 == 0)
						{
							float v1, v2, v3;

							v1 = eigenMatrix11;
							v2 = eigenMatrix22;
							v3 = eigenMatrix33;

							if (v1 >= v2 && v1 >= v3)
							{
								eigenValues1 = v1;

								if (v2 >= v3)
								{
									eigenValues2 = v2;
									eigenValues3 = v3;
								}
								else
								{
									eigenValues2 = v3;
									eigenValues3 = v2;
								}
							}
							else if (v2 >= v1 && v2 >= v3)
							{
								eigenValues1 = v2;

								if (v1 >= v3)
								{
									eigenValues2 = v1;
									eigenValues3 = v3;
								}
								else
								{
									eigenValues2 = v3;
									eigenValues3 = v1;
								}
							}
							else if (v3 >= v1 && v3 >= v2)
							{
								eigenValues1 = v3;

								if (v1 >= v2)
								{
									eigenValues2 = v1;
									eigenValues3 = v2;
								}
								else
								{
									eigenValues2 = v2;
									eigenValues3 = v1;
								}
							}
						}
						else
						{
							float q = (eigenMatrix11 + eigenMatrix22 + eigenMatrix33) / 3;
							float p2 = GMath.Pow(eigenMatrix11 - q, 2) + GMath.Pow(eigenMatrix22 - q, 2) + GMath.Pow(eigenMatrix33 - q, 2) + 2 * p1;
							float p = GMath.Sqrt(p2 / 6);

							float B11, B12, B13, B21, B22, B23, B31, B32, B33;

							B11 = (1 / p) * (eigenMatrix11 - q);
							B12 = (1 / p) * (eigenMatrix12);
							B13 = (1 / p) * (eigenMatrix13);
							B21 = (1 / p) * (eigenMatrix21);
							B22 = (1 / p) * (eigenMatrix22 - q);
							B23 = (1 / p) * (eigenMatrix23);
							B31 = (1 / p) * (eigenMatrix31);
							B32 = (1 / p) * (eigenMatrix32);
							B33 = (1 / p) * (eigenMatrix33 - q);

							float r = (B11 * B22 * B33 + B12 * B23 * B31 + B13 * B21 * B32 - B13 * B22 * B31 - B11 * B23 * B32 - B12 * B21 * B33) / 2;

							float phi;

							if (r <= -1)
								phi = GMath.PI / 3;
							else if (r >= 1)
								phi = 0;
							else
								phi = GMath.Acos(r) / 3;

							eigenValues1 = q + 2 * p * GMath.Cos(phi);
							eigenValues3 = q + 2 * p * GMath.Cos(phi + (2 * GMath.PI / 3));
							eigenValues2 = 3 * q - eigenValues1 - eigenValues3;
						}



						if (eigenValues1 < 0)
							eigenValues1 = 0;
						if (eigenValues2 < 0)
							eigenValues2 = 0;
						if (eigenValues3 < 0)
							eigenValues3 = 0;



						double Linearity = 0;
						double Planarity = 0;
						double Sphericity = 0;


						if (eigenValues1 > 0)
						{
							Linearity = (eigenValues1 - eigenValues2) / eigenValues1;
							Planarity = (eigenValues2 - eigenValues3) / eigenValues1;
							Sphericity = eigenValues3 / eigenValues1;
						}


						eigenFeature[i, j, 0] = Linearity;
						eigenFeature[i, j, 1] = Planarity;
						eigenFeature[i, j, 2] = Sphericity;


						//eigenFeature[i, j, 0] = xSum;
						//eigenFeature[i, j, 1] = ySum;
						//eigenFeature[i, j, 2] = zSum;
					}
				}
			}
		}
		#endregion
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



			if (model == null || model.Vertices == null || model.Vertices.Length == 0)
			{
				return false;
			}



			ObjMesh.ObjVertex[] v = model.Vertices;

			xMax = Enumerable.Range(0, v.Length).AsParallel().Select(i => v[i].Vertex.X).Max();
			xMin = Enumerable.Range(0, v.Length).AsParallel().Select(i => v[i].Vertex.X).Min();
			yMax = Enumerable.Range(0, v.Length).AsParallel().Select(i => v[i].Vertex.Y).Max();
			yMin = Enumerable.Range(0, v.Length).AsParallel().Select(i => v[i].Vertex.Y).Min();
			zMax = Enumerable.Range(0, v.Length).AsParallel().Select(i => v[i].Vertex.Z).Max();
			zMin = Enumerable.Range(0, v.Length).AsParallel().Select(i => v[i].Vertex.Z).Min();
			zMaxAdj = zMax - zMin;
			zMinAdj = zMin - zMin;
			zRange = zMax - zMin;


			double dWidth = (xMax - xMin) / gridSize;
			double dHeight = (yMax - yMin) / gridSize;


			//out of size
			if (dWidth <= 0 || dHeight <= 0)
			{
				return false;
			}
			else if (dWidth * dHeight > 3000 * 3000)//if too large, resize
			{
				double aspectRatio = dWidth / dHeight;

				if (aspectRatio >= 1)
				{
					width = 3000;
					height = (int)Math.Ceiling(3000 / aspectRatio);
				}
				else
				{
					width = (int)Math.Ceiling(3000 * aspectRatio);
					height = 3000;
				}

				width += (borderAdditionWidth * 2);
				height += (borderAdditionWidth * 2);
			}
			else
			{
				width = (int)Math.Ceiling((Math.Ceiling(xMax) - Math.Floor(xMin)) / gridSize) + (borderAdditionWidth * 2);
				height = (int)Math.Ceiling((Math.Ceiling(yMax) - Math.Floor(yMin)) / gridSize) + (borderAdditionWidth * 2);
			}


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

				Parallel.For(0, width - 1, i =>
				//for (int i = 0; i < width; i++)
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
				);

				CalAvg();

				if (ifDisplayCenter)
				{
					DisplayCenter();
				}


				if (generateImage)
				{
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
				}


				GL.DeleteBuffer(FboHandle);
				GL.DeleteBuffer(DepthRenderbuffer);

				model.Dispose();

				return true;
			}
		}
	}

	//derived from coreClass
	public class xyzClass : coreClass
	{
		public static int structEleSize { get; set; }
		public static int morIterNum { get; set; }
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


			if (pcArray.GetLength(0) == 0)
			{
				return false;
			}


			//找出bounding box的範圍
			xMax = Enumerable.Range(0, pcArray.GetLength(0)).AsParallel().Select(i => pcArray[i, 0]).Max();
			xMin = Enumerable.Range(0, pcArray.GetLength(0)).AsParallel().Select(i => pcArray[i, 0]).Min();
			yMax = Enumerable.Range(0, pcArray.GetLength(0)).AsParallel().Select(i => pcArray[i, 1]).Max();
			yMin = Enumerable.Range(0, pcArray.GetLength(0)).AsParallel().Select(i => pcArray[i, 1]).Min();
			zMax = Enumerable.Range(0, pcArray.GetLength(0)).AsParallel().Select(i => pcArray[i, 2]).Max();
			zMin = Enumerable.Range(0, pcArray.GetLength(0)).AsParallel().Select(i => pcArray[i, 2]).Min();
			zMaxAdj = zMax - zMin;
			zMinAdj = zMin - zMin;
			zRange = zMax - zMin;



			double dWidth = (xMax - xMin) / gridSize;
			double dHeight = (yMax - yMin) / gridSize;


			//out of size
			if (dWidth <= 0 || dHeight <= 0)
			{
				return false;
			}
			else
			{
				//計算range image的長寬
				width = (int)Math.Ceiling((Math.Ceiling(xMax) - Math.Floor(xMin)) / gridSize) + (borderAdditionWidth * 2);
				height = (int)Math.Ceiling((Math.Ceiling(yMax) - Math.Floor(yMin)) / gridSize) + (borderAdditionWidth * 2);
			}



			//宣告range image的z值最大的點雲陣列
			PointCloud[,] zMaxArray = new PointCloud[width, height];

			//初始化zMaxArray
			Parallel.For(0, width - 1, i =>
			//for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					zMaxArray[i, j].x = 0;
					zMaxArray[i, j].y = 0;
					zMaxArray[i, j].z = zMin;
				}
			}
			);



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
			Parallel.For(0, width - 1, i =>
			//for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					data2D[i, j, (byte)feature.Height] = zMaxArray[i, height - j - 1].z - zMin;
				}
			}
			);



			//EmguCV的Morphology處理
			using (Image<Gray, Double> emguImg = new Image<Gray, Double>(width, height))
			{
				Parallel.For(0, width - 1, i =>
				//for (int i = 0; i < width; i++)
				{
					for (int j = 0; j < height; j++)
					{
						emguImg.Data[j, i, 0] = data2D[i, j, (byte)feature.Height];
					}
				}
				);


				StructuringElementEx StructEle = new StructuringElementEx(structEleSize, structEleSize, (structEleSize - 1) / 2, (structEleSize - 1) / 2, structEleShape);
				CvInvoke.cvDilate(emguImg, emguImg, StructEle, morIterNum);
				CvInvoke.cvErode(emguImg, emguImg, StructEle, morIterNum);

				Parallel.For(0, width - 1, i =>
				//for (int i = 0; i < width; i++)
				{
					for (int j = 0; j < height; j++)
					{
						data2D[i, j, (byte)feature.Height] = emguImg.Data[j, i, 0];
					}
				}
				);
			}

			Parallel.For(0, width - 1, i =>
			//for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					if (data2D[i, j, (byte)feature.Height] != zMinAdj)
						data2DMask[i, j] = true;
					else
						data2DMask[i, j] = false;
				}
			}
			);

			CalAvg();

			if (ifDisplayCenter)
			{
				DisplayCenter();
			}


			if (generateImage)
			{
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
			}

			return true;
		}
	}

	public class utilClass
	{
		public static Bitmap Data2Bitmap(double[,,] data2D, int featureIndex)
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

		public static Bitmap ResizeBitmap2Square(ref Bitmap originalImage)
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

		public static DBRecord Core2DBRecord(ref coreClass core)
		{
			if (core == null || core.annularFeature == null)
				return null;


			DBRecord dbRecord = new DBRecord();

			dbRecord.zRange = core.zRange;
			dbRecord.zAvg = core.zAvg;
			dbRecord.totalArea = core.totalArea;
			dbRecord.totalVol = core.totalVol;
			dbRecord.radiusMax = core.radiusMax;

			//copy features {Height(0), Edge(1), Planarity(3)}
			Array.Copy(core.annularFeature, dbRecord.featureValue, coreClass.annularNum * 2);
			//Array.Copy(core.annularFeature, coreClass.annularNum * 3, dbRecord.featureValue, coreClass.annularNum * 3, coreClass.annularNum * 1);
			Array.Copy(core.annularFeature, coreClass.annularNum * 3, dbRecord.featureValue, coreClass.annularNum * 2, coreClass.annularNum * 1);

			dbRecord.NormalizeFeature();

			return dbRecord;
		}
	}
}
