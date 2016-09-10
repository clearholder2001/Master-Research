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
			string logName = DateTime.Now.ToString().Replace('/', '-').Replace(' ', '_').Replace(':', '-') + ".clog";
			ConsoleCopy consoleCopy = new ConsoleCopy(logName);


			double timeSum = 0;
			int okCount = 0;
			int failCount = 0;
			int skipCount = 0;
			int emptCount = 0;
			int bigCount = 0;
			int stateIndex;
			string stateMsg = string.Empty;
			string coreSizeMsg = string.Empty;

			string filePath;
			FileInfo fileInfo;


			Stopwatch sw = new Stopwatch();
			Project project = new Project();


			//讀取設定
			Console.Write("Load configure: ");

			if (args.Length == 0)
			{
				if (project.LoadConfig())
				{
					Console.WriteLine("OK");
				}
				else
				{
					Console.WriteLine("Fail");
					Console.WriteLine("Program has been canceled.");
					Console.WriteLine("Please press any key to close this program...");
					Console.ReadKey();
					Console.ReadKey();
					Console.ReadKey();
					consoleCopy.Dispose();
					return;
				}
			}
			else
			{
				if (project.LoadConfig(args[0]))
				{
					Console.WriteLine("OK");
				}
				else
				{
					Console.WriteLine("Fail");
					Console.WriteLine("Program has been canceled.");
					Console.WriteLine("Please press any key to close this program...");
					Console.ReadKey();
					Console.ReadKey();
					Console.ReadKey();
					consoleCopy.Dispose();
					return;
				}
			}



			//顯示設定
			Console.Write(project.ShowInfo());


			//搜尋檔案
			Console.Write("Search file: ");
			if (project.GetfileList())
			{
				Console.WriteLine("OK");
			}
			else
			{
				Console.WriteLine("Fail");
				consoleCopy.Dispose();
				return;
			}



			//確認開始
			Console.WriteLine("Total file: {0}", project.fileListArray.Length);
			if (!project.RunWithoutYes)
			{
				Console.Write("Start? (y or n) ");
				string s = Console.ReadLine();
				//string s = "Y";
				if (s == "Y" || s == "y")
				{
					//start
				}
				else
				{
					Console.WriteLine("Program has been canceled.");
					Console.WriteLine("Please press any key to close this program...");
					Console.ReadKey();
					Console.ReadKey();
					Console.ReadKey();
					Console.ReadKey();
					Console.ReadKey();

					consoleCopy.Dispose();
					return;
				}
			}


			//開始處理檔案
			for (int i = project.startIndex; i < project.fileListArray.Length; i++)
			{
				filePath = project.fileListArray[i];
				fileInfo = new FileInfo(filePath);

				sw.Restart();


				Console.Write("({0}/{1}) {2} {3}: ", i + 1, project.fileListArray.Length, DateTime.Now.ToShortTimeString(), Path.GetFileName(filePath).PadRight(38));

				coreClass core = new objClass(filePath);


				if (File.Exists(project.GetSaveFilePath(filePath)) && !project.overWrite) //檔案已存在且不允許覆寫
				{
					stateIndex = 0;
				}
				else if (fileInfo.Length == 0) //檔案為空不處理
				{
					stateIndex = -2;
				}
				else if (fileInfo.Length > project.fileMaxSize) //檔案太大不處理
				{
					stateIndex = -3;
				}
				else
				{
					if (core.GenerateRangeImage())
					{
						stateIndex = 1;
						if (core.GenerateFeature())
						{
							stateIndex = 1;
						}
						else
						{
							stateIndex = -1;
						}
					}
					else
					{
						stateIndex = -1;
					}
				}


				sw.Stop();
				timeSum += sw.Elapsed.TotalSeconds;


				switch (stateIndex)
				{
					case 0:
						stateMsg = "Skip";
						coreSizeMsg = string.Empty;
						skipCount++;
						break;
					case 1:
						stateMsg = "OK";
						coreSizeMsg = string.Format("{0} x {1}", core.width, core.height).PadRight(15);
						okCount++;

						//存檔
						core.SaveResult(project.GetSaveFilePath(filePath), project.overWrite);

						//要存影像的話，要把objClass產生rangeImg的註解拿掉
						if (project.saveRangeImage)
							core.rangeImg.Save(project.GetSaveFilePath(filePath).Replace("." + project.outputExtension, "_range_image.bmp"), System.Drawing.Imaging.ImageFormat.Bmp);
						break;
					case -1:
						stateMsg = "Fail";
						coreSizeMsg = string.Format("{0} x {1}", core.width, core.height).PadRight(15);
						failCount++;
						break;
					case -2:
						stateMsg = "Empt";
						failCount++;
						emptCount++;
						break;
					case -3:
						stateMsg = "Big";
						failCount++;
						bigCount++;
						break;
				}

				Console.WriteLine("{0} {1:0.0000}s Avg.:{2:0.0000}s {3}", stateMsg.PadRight(4), sw.Elapsed.TotalSeconds, timeSum / okCount, coreSizeMsg);


				if (okCount % project.gcFrequency == 0)
					GC.Collect(2);
			}



			Console.WriteLine();
			Console.WriteLine("All done.");
			Console.WriteLine("Total time: {0:0.0000}", timeSum);
			Console.WriteLine("OK: {0}, Fail: {1}, Out of Size: {2}, Skip: {3}", okCount, failCount, emptCount + bigCount, skipCount);
			Console.WriteLine("Please press any key to close this program...");

			consoleCopy.Dispose();

			Console.ReadKey();
			Console.ReadKey();
			Console.ReadKey();
			Console.ReadKey();
			Console.ReadKey();


			consoleCopy.Dispose();
			return;



			string path1, path2, fileName;





			//啟用GPU加速
			//coreClass.Cudafy_initialization();


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
				path1 = @"\\140.116.80.133\model_retrieval";
				//path2 = @"C:\Users\Yi-Chen\CloudStation\test";
			}
			catch (Exception)
			{
				Console.WriteLine("Incorrect path.");
				Console.ReadKey();
				return;
			}


			ArrayList files = new ArrayList();

			if (Directory.Exists(path1)/* && Directory.Exists(path2)*/)
			{
				files.AddRange(Directory.GetFiles(path1));
				//files.AddRange(Directory.GetFiles(path2));
			}
			/*
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
					Console.WriteLine("{0}. {1}: Not Support File", files.IndexOf(file) + 1, Path.GetFileName(fileName));
					continue;
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
			*/

			Console.WriteLine("--------------------------------");
			Console.WriteLine("Total files: {0}", files.Count);
			Console.WriteLine("Average time per file: {0}s", timeSum / files.Count);
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
