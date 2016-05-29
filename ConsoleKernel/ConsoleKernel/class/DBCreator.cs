using System;
using System.IO;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;

namespace DBQuery
{
	class DBCreator
	{
		public string initPath, pathDirRegex, sourceExtension, dbFileName;
		public string[] fileListArray;
		public int failCount;

		private DBRecord[] dbRecordArray;
		private MySqlConnection dbConn;
		private MySqlCommand dbCommand;
		private const int binMerge = 4;
		private const string sqlTableName = "models_tt";


		public DBCreator(string initPath, string sourceExtension, string pathDirRegex)
		{
			this.initPath = initPath;
			this.sourceExtension = sourceExtension;
			this.pathDirRegex = pathDirRegex;
		}

		public bool GetFileList(BackgroundWorker bw)
		{
			if (Directory.Exists(initPath))
			{
				bw.ReportProgress(0);

				if (pathDirRegex != string.Empty)
				{
					string[] dirListArray = Directory.GetDirectories(initPath);
					List<string> fileList = new List<string>();

					for (int i = 0; i < dirListArray.Length; i++)
					{
						if (Regex.IsMatch(dirListArray[i], pathDirRegex))
						{
							IEnumerable<string> enumFileNames = from dir in Directory.EnumerateFiles(dirListArray[i], "*.*", SearchOption.AllDirectories)
																select dir;

							ParallelQuery<string> queryFileNames = from file in enumFileNames.AsParallel()
																   where Path.GetExtension(file) == "." + sourceExtension
																   select file;

							fileList.AddRange(queryFileNames.ToArray());

							bw.ReportProgress((int)((double)(i + 1) / (double)dirListArray.Length * 100.0));
						}
					}

					fileListArray = fileList.ToArray();
				}
				else
				{
					IEnumerable<string> enumFileNames = from dir in Directory.EnumerateFiles(initPath, "*.*", SearchOption.AllDirectories)
														select dir;

					ParallelQuery<string> queryFileNames = from file in enumFileNames.AsParallel()
														   where Path.GetExtension(file) == "." + sourceExtension
														   select file;

					fileListArray = queryFileNames.ToArray();
					bw.ReportProgress(100);
				}
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool CreateDatabaseFile(BackgroundWorker bw)
		{
			int progressCount = 0;
			failCount = 0;
			dbRecordArray = new DBRecord[fileListArray.Length];


			//connect MariaDB
			bool sqlDBReady = ConnectMariaDB();
			if (sqlDBReady)
			{
				dbCommand = dbConn.CreateCommand();
				dbCommand.CommandText = string.Format("TRUNCATE {0}", sqlTableName);
				dbCommand.ExecuteNonQuery();
			}

			try
			{
				//read file and add to dbRecordArray
				//Parallel.For(0, fileListArray.Length, i =>
				for (int i = 0; i < fileListArray.Length; i++)
				{
					int lineCount = 0;
					double binCount = 0.0;
					string line;
					bool isAllOK = true;
					double featureValue;
					double[] doubleArray = new double[DBRecord.recordLength];

					using (StreamReader sr = new StreamReader(fileListArray[i]))
					{
						while ((line = sr.ReadLine()) != null)
						{
							if (lineCount < DBRecord.sizeFeatureNum)
							{
								if (Double.TryParse(line, out featureValue))
								{
									doubleArray[lineCount] = featureValue;
								}
								else if (line == "不是一個數字")
								{
									featureValue = 0;
									doubleArray[lineCount] = featureValue;
									isAllOK = false;
								}
								else
								{
									featureValue = 0;
									doubleArray[lineCount] = featureValue;
									isAllOK = false;
								}
							}
							else
							{
								if (Double.TryParse(line, out featureValue))
								{
									binCount += featureValue;
								}
								else if (line == "不是一個數字")
								{
									featureValue = 0;
									binCount += featureValue;
									isAllOK = false;
								}
								else
								{
									featureValue = 0;
									binCount += featureValue;
									isAllOK = false;
								}

								if ((lineCount - DBRecord.sizeFeatureNum + 1) % binMerge == 0)
								{
									doubleArray[DBRecord.sizeFeatureNum - 1 + (lineCount - DBRecord.sizeFeatureNum + 1) / binMerge] = binCount;
									binCount = 0;
								}
							}

							lineCount++;
						}
					}

					dbRecordArray[i] = new DBRecord();
					dbRecordArray[i].LoadFeature(doubleArray);

					if (!isAllOK)
						failCount++;
					
					progressCount++;
					if (progressCount % Math.Ceiling(fileListArray.Length * 0.005) == 0)
						bw.ReportProgress(progressCount / 2);
				}
				//);
			}
			catch (Exception)
			{
				return false;
			}

			progressCount = 0;

			if (File.Exists(dbFileName))
				File.Delete(dbFileName);

			FileStream fs = new FileStream(dbFileName, FileMode.CreateNew);
			BinaryWriter biw = new BinaryWriter(fs);

			//write header (total length)
			biw.Write((uint)fileListArray.Length);


			//write feature values of each model
			for (int i = 0; i < fileListArray.Length; i++)
			{
				biw.Write(dbRecordArray[i].zRange);
				biw.Write(dbRecordArray[i].zAvg);
				biw.Write(dbRecordArray[i].totalArea);
				biw.Write(dbRecordArray[i].totalVol);
				biw.Write(dbRecordArray[i].radiusMax);
				for (int j = 0; j < DBRecord.featureDimension; j++)
				{
					for (int k = 0; k < DBRecord.annularNum; k++)
					{
						biw.Write(dbRecordArray[i].featureValue[j, k]);
					}
				}

				string mid = Path.GetFileNameWithoutExtension(fileListArray[i]);
				byte[] asciiBytes = Encoding.ASCII.GetBytes(mid);
				int mod4 = Enumerable.Range(0, asciiBytes.Length).Select(a => (int)asciiBytes[a]).Sum() % 4;


				if (sqlDBReady)
				{
					//dbCommand.CommandText = string.Format("INSERT INTO {0} (mid, mod4, state) VALUES ('{1}', {2}, {3})", sqlTableName, mid, mod4, 1);
					dbCommand.CommandText = string.Format("INSERT INTO {0} (mid, mod4, state, txt00) VALUES ('{1}', {2}, {3}, @blobFile)", sqlTableName, mid, mod4, 1);

					dbCommand.Parameters.Clear();
					dbCommand.Parameters.AddWithValue("@blobFile", dbRecordArray[i].ConvertToByteArray());

					dbCommand.ExecuteNonQuery();
				}

				progressCount++;
				if (progressCount % Math.Ceiling(fileListArray.Length * 0.005) == 0)
					bw.ReportProgress((progressCount / 2) + (fileListArray.Length / 2));
			}

			if (sqlDBReady)
				dbConn.Close();

			biw.Flush();
			fs.Flush();

			biw.Close();
			fs.Close();

			return true;
		}

		public bool CreateFilelistFile(string fileName)
		{
			FileInfo fi;

			using (StreamWriter sw = new StreamWriter(fileName))
			{
				//Parallel.ForEach(fileListArray, file =>
				foreach (string file in fileListArray)
				{
					//mid
					string mid = Path.GetFileNameWithoutExtension(file);

					//mod4
					byte[] asciiBytes = Encoding.ASCII.GetBytes(mid);
					int mod4 = Enumerable.Range(0, asciiBytes.Length).Select(a => (int)asciiBytes[a]).Sum() % 4;

					//directory path
					string dirPath = string.Format(@"\{0}\{1}\{2}\", mod4, mid.Substring(0, 2), mid.Substring(2, 2));

					//model size in bytes
					fi = new FileInfo(file);
					long modelSize = fi.Length;

					//final string
					string fileInfoString = String.Format(",{0},{1},{2},{3},,,,\n", mid, mod4, dirPath, modelSize);

					//write
					sw.Write(fileInfoString);
				}
				//);
			}
			return true;
		}

		public DBRecord[] GetDBRecordArray()
		{
			return dbRecordArray;
		}

		private bool ConnectMariaDB()
		{
			string dbHost = "140.116.80.133";
			string dbUser = "encoding";
			string dbPass = "encoding";
			string dbName = "model_retrieval";

			string connStr = "server=" + dbHost + ";uid=" + dbUser + ";pwd=" + dbPass + ";database=" + dbName/* + ";CharSet=utf8mb4_unicode_ci"*/;
			dbConn = new MySqlConnection(connStr);

			try
			{
				dbConn.Open();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
