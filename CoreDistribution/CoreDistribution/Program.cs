using System;
using System.IO;
using System.Collections;
using System.Diagnostics;

using Core;

namespace coreDistribution
{
	class Program
	{
		static void Main(string[] args)
		{
			Stopwatch sw = new Stopwatch();
			//sw.Start();


			string path1, path2, fileName;
			coreClass core;

			double timeSum = 0;

			//設定參數
			coreClass.gridSize = 0.1;
			coreClass.annularNum = 20;
			coreClass.laplacianAperture = 7;
			coreClass.gaussianAperture = 7;
			coreClass.eigenFeatureDiameterMultiple = 4;
			coreClass.borderAdditionWidth = 10;
			coreClass.thumbnailImgMaxSize = 100;
			xyzClass.structEleSize = 7;
			xyzClass.MorIterNum = 3;
			xyzClass.structEleShape = Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_RECT;


			//啟用GPU加速
			coreClass.Cudafy_initialization();


			//刪除目錄所有的圖檔
			Process.Start("CMD.exe", "/c del *.bmp");
			//Process process = new Process();
			//ProcessStartInfo startInfo = new ProcessStartInfo();
			//startInfo.WindowStyle = ProcessWindowStyle.Hidden;
			//startInfo.FileName = "cmd.exe";
			//startInfo.Arguments = "/c del *.bmp";
			//process.StartInfo = startInfo;
			//process.Start();


			//設定初始目錄
			try
			{
				path1 = @"C:\Users\Yi-Chen\CloudStation\Experiment\All";
				path2 = @"C:\Users\Yi-Chen\CloudStation\test";
			}
			catch (Exception)
			{
				Console.WriteLine("Incorrect path.");
				Console.ReadKey();
				return;
			}


			ArrayList files = new ArrayList();

			if (Directory.Exists(path1) && Directory.Exists(path2))
			{
				files.AddRange(Directory.GetFiles(path1));
				files.AddRange(Directory.GetFiles(path2));
			}

			foreach (var file in files)
			{
				sw.Restart();

				//取得檔名
				try
				{
					fileName = file.ToString();
					//fileName = args[0];
					//fileName = @"C:\Users\Yi-Chen\CloudStation\Experiment\7. Living Mall\7-1.obj";
				}
				catch (Exception)
				{
					Console.WriteLine("Incorrect file name.");
					return;
				}


				if (Path.GetExtension(fileName) == ".xyz")
				{
					core = new xyzClass(fileName);
				}
				else if (Path.GetExtension(fileName) == ".obj")
				{
					core = new objClass(fileName);
				}
				else
				{
					Console.WriteLine("File format error.");
					return;
				}


				//產生range image並存檔
				if (core.GenerateRangeImage())
				{
					//core.rangeImg.Save(Path.GetFileNameWithoutExtension(fileName) + "_range_image.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
					///Console.WriteLine("Range image saved.");
				}
				else
				{
					Console.WriteLine("Range image error.");
					return;
				}



				//產生annular feature
				if (core.GenerateFeature())
				{
					//utilClass.Data2Bitmap(core.data2D, 2).Save(Path.GetFileNameWithoutExtension(fileName) + "_eigen_feature_1.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
					//utilClass.Data2Bitmap(core.data2D, 3).Save(Path.GetFileNameWithoutExtension(fileName) + "_eigen_feature_2.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
					//utilClass.Data2Bitmap(core.data2D, 4).Save(Path.GetFileNameWithoutExtension(fileName) + "_eigen_feature_3.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
					//Console.WriteLine("Feature generated.");
				}
				else
				{
					Console.WriteLine("Feature error.");
					return;
				}


				sw.Stop();
				timeSum += sw.Elapsed.TotalSeconds;
				Console.WriteLine("{0}. {1}: {2}s", files.IndexOf(file) + 1, Path.GetFileName(fileName), sw.Elapsed.TotalSeconds.ToString());
			}


			Console.WriteLine("--------------------------------");
			Console.WriteLine("Total files: {0}", files.Count);
			Console.WriteLine("Average time per file: {0}s", timeSum / files.Count);
			Console.ReadKey();
			Console.ReadKey();
			Console.ReadKey();
			Console.ReadKey();
			Console.ReadKey();
			Console.ReadKey();
			Console.ReadKey();
			Console.ReadKey();
			Console.ReadKey();
			Console.ReadKey();

			/*
			Console.WriteLine("--------------------------------");
			Console.Write(core.OutputConsole());
			Console.WriteLine("--------------------------------");
			Console.Write(core.OutputSeries());
			Console.WriteLine("-------------Finish-------------");
			*/
			//Console.ReadKey();
		}
	}
}
