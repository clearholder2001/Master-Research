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
using System.Linq;
using System.Drawing;
// using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using ImageTool;

using MathWorks.MATLAB.NET.Arrays;
using xyzMatlabTool;


namespace Core
{
	class Program
	{
		public struct PointCloud
		{
			public double x, y, z;
		}

		static int Main(string[] args)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();


			double gridSize;
			bool result = false;

	  		args = new string[3] { "0.2", @"model\model2.obj", "mode2_obj_0.2" };

			
			if (args.Length != 3 || !double.TryParse(args[0], out gridSize) || gridSize <= 0.0 || !File.Exists(args[1]))
			{
				Console.WriteLine("Parameters error!");
				Console.ReadKey();
				return -1;
			}

			string fileName = args[1];
			string outputName = args[2] + ".bmp";


			if (Path.GetExtension(args[1]) == ".obj")
			{
				result = obj_do_work(gridSize, fileName, outputName);
			}
			else if (Path.GetExtension(args[1]) == ".xyz")
			{
				result = xyz_do_work(gridSize, fileName, outputName);
			}


			sw.Stop();


			if (result)
			{
				Console.WriteLine("Success!");
				Console.WriteLine(sw.Elapsed.TotalSeconds.ToString() + "s");
				Console.ReadKey();
				return 1;
			}
			else
			{
				Console.WriteLine("Fail!");
				Console.WriteLine(sw.Elapsed.TotalSeconds.ToString() + "s");
				Console.ReadKey();
				return -2;
			}

		}

		static bool xyz_do_work(double gridSize, string fileName, string outputName)
		{
			double[,] pcArray;
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


// 			Bitmap img = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
// 			byte[,,] rangeImgData = BitmapTool.Bitmap2Array(img);



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

			pcNum = 0;
			foreach (PointCloud i in zMaxArray)
			{
				if (i.z != zMin)
					pcNum++;	
			}

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

			try
			{
				xyzClass matlab = new xyzClass();
				MWNumericArray pcMatlabArray = new MWNumericArray(pcArray);
				MWArray[] result = null;
				result = matlab.xyzPostProcessing(2, pcMatlabArray, gridSize, outputName);
			}
			catch (Exception)
			{
				return false;
			}

// 			BitmapTool.Array2Bitmap(rangeImgData, System.Drawing.Imaging.PixelFormat.Format8bppIndexed).Save(outputName, System.Drawing.Imaging.ImageFormat.Jpeg);

			return true;
		}

		static bool obj_do_work(double gridSize, string fileName, string outputName)
		{

			ObjMesh model;
			float[] pixels;
			byte[,,] imgData;

			double xMax, xMin;
			double yMax, yMin;
			double zMax, zMin;
			int width, height;
			Vector3d eye, center, up;

			try
			{
				model = new ObjMesh(fileName);
			}
			catch (Exception)
			{
				return false;
			}

			ObjMesh.ObjVertex[] v = model.Vertices;


			xMax = v[0].Vertex.X;
			xMin = v[0].Vertex.X;
			yMax = v[0].Vertex.Y;
			yMin = v[0].Vertex.Y;
			zMax = v[0].Vertex.Z;
			zMin = v[0].Vertex.Z;

			for (int i = 0; i < v.Length; i++)
			{
				if (v[i].Vertex.X > xMax)
					xMax = v[i].Vertex.X;
				else if (v[i].Vertex.X < xMin)
					xMin = v[i].Vertex.X;
				if (v[i].Vertex.Y > yMax)
					yMax = v[i].Vertex.Y;
				else if (v[i].Vertex.Y < yMin)
					yMin = v[i].Vertex.Y;
				if (v[i].Vertex.Z > zMax)
					zMax = v[i].Vertex.Z;
				else if (v[i].Vertex.Z < zMin)
					zMin = v[i].Vertex.Z;
			}


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
				imgData = new byte[width, height, 3];
				GL.ReadPixels(0, 0, width, height, PixelFormat.DepthComponent, PixelType.Float, pixels);


// 				float dMax = pixels[0];
// 				float dMin = pixels[0];
// 				for (int i = 0; i < pixels.Length; i++)
// 
// 				{
// 					if (pixels[i] > dMax)
// 						dMax = pixels[i];
// 					else if (pixels[i] < dMin)
// 						dMin = pixels[i];
// 				}

				for (int i = 0; i < pixels.Length; i++)
					pixels[i] = 255.0f - (pixels[i] * 255.0f);
// 						pixels[i] = (pixels[i] - dMin) / (dMax - dMin) * 255;


				Bitmap img = new Bitmap(width, height);

				for (int i = 0; i < width; i++)
				{
					for (int j = 0; j < height; j++)
					{
						imgData[i, j, 0] = (byte)pixels[(height - 1 - j) * width + i];
// 						img.SetPixel(i, j, Color.FromArgb(imgData[i, j, 0], imgData[i, j, 0], imgData[i, j, 0]));
					}
				}

				BitmapTool.Array2Bitmap(imgData, System.Drawing.Imaging.PixelFormat.Format8bppIndexed).Save(outputName, System.Drawing.Imaging.ImageFormat.Bmp);

// 				img.Save(outputName, System.Drawing.Imaging.ImageFormat.Tiff);


				return true;
			}
		}
	}
}
