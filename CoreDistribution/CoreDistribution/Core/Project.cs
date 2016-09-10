using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Core
{
	public class Project
	{
		public string initPath, rootPath, targetPath;
		public bool enableCUDA, CUDAGenerateDebug, overWrite, saveRangeImage, RunWithoutYes;
		public int startIndex, threadMaxNum, gcFrequency, degreeOfParallelism, ePlatform, CUDADeviceId;
		public long fileMaxSize;
		public string pathDirRegex, sourceExtension, outputExtension;

		public double gridSize { get; private set; }
		public int annularNum { get; private set; }
		public int laplacianAperture { get; private set; }
		public int gaussianAperture { get; private set; }
		public int eigenFeatureDiameterMultiple { get; private set; }
		public int borderAdditionWidth { get; private set; }
		public int thumbnailImgMaxSize { get; private set; }
		public int structEleSize { get; private set; }
		public int morIterNum { get; private set; }
		public int structEleShape { get; private set; }

		public List<string> dirList, fileList;
		public string[] dirListArray, fileListArray;


		public Project()
		{
			dirList = new List<string>();
			fileList = new List<string>();

			startIndex = 0;

			objClass.InitOpenGL();
		}

		public bool LoadConfig()
		{
			if (SetConfig("conf.txt"))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool LoadConfig(string configFileName)
		{
			if (SetConfig(configFileName))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool GetfileList()
		{
			if (Directory.Exists(initPath))
			{
				if (pathDirRegex != null)
				{
					dirListArray = Directory.GetDirectories(initPath);
					fileList = new List<string>();

					for (int i = 0; i < dirListArray.Length; i++)
					{
						if (Regex.IsMatch(dirListArray[i], pathDirRegex))
						{
							IEnumerable<string> enumFileNames = from dir in Directory.EnumerateFiles(dirListArray[i], "*.*", SearchOption.AllDirectories)
																select dir;

							ParallelQuery<string> queryFileNames = from file in enumFileNames.AsParallel().WithDegreeOfParallelism(degreeOfParallelism)
																   where Path.GetExtension(file) == "." + sourceExtension
																   select file;

							fileList.AddRange(queryFileNames.ToArray());
						}
					}

					fileListArray = fileList.ToArray();
				}
				else
				{
					IEnumerable<string> enumFileNames = from dir in Directory.EnumerateFiles(initPath, "*.*", SearchOption.AllDirectories)
														select dir;

					ParallelQuery<string> queryFileNames = from file in enumFileNames.AsParallel().WithDegreeOfParallelism(degreeOfParallelism)
														   where Path.GetExtension(file) == "." + sourceExtension
														   select file;

					fileListArray = queryFileNames.ToArray();
				}

				return true;
			}
			else
			{
				return false;
			}
		}

		public string ShowInfo()
		{
			string s = string.Empty;

			s += "################################Configure##################################" + "\n";
			s += "initPath: " + initPath + "\n";
			s += "rootPath: " + rootPath + "\n";
			s += "targetPath: " + targetPath + "\n";
			s += "startIndex: " + startIndex + "\n";
			s += "pathDirRegex: " + pathDirRegex + "\n";
			s += "overWrite: " + overWrite.ToString() + "\n";
			s += "gcFrequency: " + gcFrequency + "\n";
			s += "fileMaxSize: " + fileMaxSize.ToString() + " bytes\n";
			s += "sourceExtenstion: " + sourceExtension + "\n";
			s += "outputExtension: " + outputExtension + "\n";
			s += "saveRangeImage: " + saveRangeImage.ToString() + "\n";
			s += "RunWithoutYes: " + RunWithoutYes.ToString() + "\n";
			s += "degreeOfParallelism: " + degreeOfParallelism + "\n";
			s += "enableCUDA: " + enableCUDA.ToString() + "\n";
			s += "ePlatform: " + ePlatform + "\n";
			s += "CUDAGenerateDebug: " + CUDAGenerateDebug.ToString() + "\n";
			s += "CUDADeviceId: " + CUDADeviceId + "\n";
			s += "threadMaxNum: " + threadMaxNum + "\n";
			s += "gridSize: " + gridSize + "\n";
			s += "annularNum: " + annularNum + "\n";
			s += "laplacianAperture: " + laplacianAperture + "\n";
			s += "gaussianAperture: " + gaussianAperture + "\n";
			s += "eigenFeatureDiameterMultiple: " + eigenFeatureDiameterMultiple + "\n";
			s += "borderAdditionWidth: " + borderAdditionWidth + "\n";
			s += "thumbnailImgMaxSize: " + thumbnailImgMaxSize + "\n";
			s += "structEleSize: " + structEleSize + "\n";
			s += "morIterNum: " + morIterNum + "\n";
			s += "structEleShape: " + structEleShape + "\n";
			s += "###########################################################################" + "\n";

			return s;
		}

		private bool SetConfig(string configFileName)
		{
			if (!File.Exists(configFileName))
			{
				return false;
			}

			using (StreamReader sr = new StreamReader(configFileName))
			{
				string line;
				string[] paramArray;
				while ((line = sr.ReadLine()) != null)
				{
					if (line.StartsWith("#"))
					{
						continue;
					}
					else
					{
						try
						{
							paramArray = line.Split(' ');
							switch (paramArray[0])
							{
								case "initPath":
									initPath = paramArray[1];
									break;
								case "rootPath":
									rootPath = paramArray[1];
									break;
								case "targetPath":
									targetPath = paramArray[1];
									break;
								case "startIndex":
									startIndex = Convert.ToInt32(paramArray[1]);
									break;
								case "pathDirRegex":
									pathDirRegex = paramArray[1];
									break;
								case "overWrite":
									overWrite = Convert.ToInt32(paramArray[1]) != 0 ? true : false;
									break;
								case "gcFrequency":
									gcFrequency = Convert.ToInt32(paramArray[1]);
									break;
								case "fileMaxSize":
									fileMaxSize = Convert.ToInt64(paramArray[1]);
									break;
								case "sourceExtenstion":
									sourceExtension = paramArray[1];
									break;
								case "outputExtension":
									outputExtension = paramArray[1];
									break;
								case "saveRangeImage":
									saveRangeImage = Convert.ToInt32(paramArray[1]) != 0 ? true : false;
									break;
								case "RunWithoutYes":
									RunWithoutYes = Convert.ToInt32(paramArray[1]) != 0 ? true : false;
									break;
								case "degreeOfParallelism":
									degreeOfParallelism = Convert.ToInt32(paramArray[1]);
									break;
								case "enableCUDA":
									enableCUDA = Convert.ToInt32(paramArray[1]) != 0 ? true : false;
									break;
								case "ePlatform":
									ePlatform = Convert.ToInt32(paramArray[1]);
									break;
								case "CUDAGenerateDebug":
									CUDAGenerateDebug = Convert.ToInt32(paramArray[1]) != 0 ? true : false;
									break;
								case "CUDADeviceId":
									CUDADeviceId = Convert.ToInt32(paramArray[1]);
									break;
								case "threadMaxNum":
									threadMaxNum = Convert.ToInt32(paramArray[1]);
									break;	
								case "gridSize":
									gridSize = Convert.ToDouble(paramArray[1]);
									break;
								case "annularNum":
									annularNum = Convert.ToInt32(paramArray[1]);
									break;
								case "laplacianAperture":
									laplacianAperture = Convert.ToInt32(paramArray[1]);
									break;
								case "gaussianAperture":
									gaussianAperture = Convert.ToInt32(paramArray[1]);
									break;
								case "eigenFeatureDiameterMultiple":
									eigenFeatureDiameterMultiple = Convert.ToInt32(paramArray[1]);
									break;
								case "borderAdditionWidth":
									borderAdditionWidth = Convert.ToInt32(paramArray[1]);
									break;
								case "thumbnailImgMaxSize":
									thumbnailImgMaxSize = Convert.ToInt32(paramArray[1]);
									break;
								case "structEleSize":
									structEleSize = Convert.ToInt32(paramArray[1]);
									break;
								case "morIterNum":
									morIterNum = Convert.ToInt32(paramArray[1]);
									break;
								case "structEleShape":
									structEleShape = Convert.ToInt32(paramArray[1]);
									break;
								default:
									break;
							}
						}
						catch (Exception)
						{
							return false;
						}
					}
				}
			}


			coreClass.gridSize = gridSize;
			coreClass.annularNum = annularNum;
			coreClass.laplacianAperture = laplacianAperture;
			coreClass.gaussianAperture = gaussianAperture;
			coreClass.eigenFeatureDiameterMultiple = eigenFeatureDiameterMultiple;
			coreClass.borderAdditionWidth = borderAdditionWidth;
			coreClass.thumbnailImgMaxSize = thumbnailImgMaxSize;
			coreClass.generateImage = saveRangeImage;
			xyzClass.structEleSize = structEleSize;
			xyzClass.morIterNum = morIterNum;
			xyzClass.structEleShape = (Emgu.CV.CvEnum.CV_ELEMENT_SHAPE)structEleShape;

			//GPU加速
			coreClass.threadMaxNum = threadMaxNum;

			if (enableCUDA)
				coreClass.Cudafy_initialization(ePlatform, CUDADeviceId, CUDAGenerateDebug);

			//CPU加速
			coreClass.degreeOfParallelism = degreeOfParallelism;
			coreClass.parallelOptions = new System.Threading.Tasks.ParallelOptions();
			coreClass.parallelOptions.MaxDegreeOfParallelism = degreeOfParallelism;


			return true;
		}

		public string GetSaveFilePath(string fileName)
		{
			string saveFilePath = string.Empty;

			saveFilePath = fileName.Replace(rootPath, targetPath);
			saveFilePath = saveFilePath.Replace("." + sourceExtension, "." + outputExtension);

			return saveFilePath;
		}
	}
}
