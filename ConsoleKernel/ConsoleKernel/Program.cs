/*
回傳值:
1	成功
0	條件不符不處理
-1	參數檔錯誤
-2	產生range image錯誤
-3	產生feature錯誤
-4	載入db檔錯誤
-5	載入input錯誤
-6	query錯誤
-7	SQL server錯誤
-8	寫入query result錯誤

input模式:
1	需要encoding
2	讀txt00
3	database re-query
*/

using System;
using System.IO;
using System.Diagnostics;

using Core;

namespace ConsoleKernel
{
	class Program
	{
		static int Main(string[] args)
		{
			//args = new string[1];
			//args[0] = "66c84c634e4a50637b74dbf49fa9bf7e.obj";
			//args[0] = "1_2.obj";
			//args[0] = "Far Eastern Department Store.xyz";
			//args[0] = "Dept. of Geomatics.xyz";
			//args[0] = "Living Mall.xyz";
			//args[0] = "1-1.obj";
			//args[0] = "test.txt00";
			//args[0] = "187054";


			if (!Directory.Exists("query"))
				Directory.CreateDirectory("query");


			string logName = DateTime.Now.ToString().Replace('/', '-').Replace(' ', '-').Replace(':', '-') + ".clog";
			ConsoleCopy consoleCopy = new ConsoleCopy("query/" + logName);



			Project project = new Project();
			Stopwatch sw = new Stopwatch();
			string fileName, configName;
			int queryMode;
			int queryId = -1;


			#region Configure
			if (args.Length < 1)
			{
				Console.WriteLine("Error: not enough arguments");
				return 0;
			}
			else if (args.Length == 1)
			{
				fileName = args[0];
				if (!project.LoadConfig())
				{
					Console.WriteLine("Error: configuration");
					return -1;
				}

				//紀錄log
				Console.WriteLine("File: {0}, Configuration: conf.txt", args[0]);
				Console.Write(project.ShowInfo());
			}
			else
			{
				fileName = args[0];
				configName = args[1];
				if (project.LoadConfig(configName))
				{
					Console.WriteLine("Error: configuration");
					return -1;
				}

				//紀錄log
				Console.WriteLine("File: {0}, Configuration: {1}", args[0], args[1]);
				Console.Write(project.ShowInfo());
			} 
			#endregion



			#region Check & Initialize

			//檢查檔案是否存在
			if (!File.Exists(fileName) && !int.TryParse(fileName, out queryId))
			{
				Console.WriteLine("Error: file not found");
				return 0;
			}


			//物件初始化
			coreClass core;

			if (Path.GetExtension(fileName) == ".xyz" || Path.GetExtension(fileName) == ".XYZ")
			{
				queryMode = 1;
				core = new xyzClass(fileName);
			}
			else if (Path.GetExtension(fileName) == ".obj" || Path.GetExtension(fileName) == ".OBJ")
			{
				queryMode = 1;
				core = new objClass(fileName);
			}
			else if (Path.GetExtension(fileName) == "." + project.outputExtension || Path.GetExtension(fileName) == "." + project.outputExtension.ToUpper())
			{
				queryMode = 2;
				core = null;
			}
			else if (queryId != -1)
			{
				queryMode = 3;
				core = null;
			}
			else
			{
				Console.WriteLine("Error: not supported file format");
				return 0;
			}

			#endregion



			#region Encoding


			if (queryMode == 1)
			{
				sw.Start();
				

				if (core.GenerateRangeImage()) //產生range image
				{
					if (core.GenerateFeature()) //產生feature
					{
						//Encoding完成！
					}
					else
					{
						Console.WriteLine("Error: feature generation");
						return -3;
					}
				}
				else
				{
					Console.WriteLine("Error: range image generation");
					return -2;
				}


				//range image存檔
				if (project.saveRangeImage)
				{
					core.rangeImg.Save("query/query_range_image.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
				}

				//feature存檔
				core.SaveResult("query/query_encoding_result." + project.outputExtension, true);

				sw.Stop();

				Console.WriteLine("Generate range image & feature done. {0}ms", sw.ElapsedMilliseconds);
			}

			#endregion



			#region Query

			sw.Restart();

			//宣告dbQuery物件
			DBQuery.DBQuery dbQuery = new DBQuery.DBQuery();


			//query參數初始化
			dbQuery.sizeWeight = true;

			dbQuery.usedFeature[0] = true;
			dbQuery.usedFeature[1] = true;
			dbQuery.usedFeature[2] = true;

			dbQuery.featureWeight[0] = (1 - project.WeightOfEdgeFeature) / 2;
			dbQuery.featureWeight[1] = project.WeightOfEdgeFeature;
			dbQuery.featureWeight[2] = (1 - project.WeightOfEdgeFeature) / 2;
			//dbQuery.featureWeight[0] = 1;
			//dbQuery.featureWeight[1] = 1;
			//dbQuery.featureWeight[2] = 1;


			dbQuery.sqlQueryTableName = project.sqlQueryTableName;


			//讀入database
			if (dbQuery.LoadDatabase(project.dbName))
			{
				//準備query
			}
			else
			{
				Console.WriteLine("Error: load database file");
				return -4;
			}


			//讀入input query
			if (queryMode == 1)
			{
				dbQuery.dbInput = utilClass.Core2DBRecord(ref core);
			}
			else if (queryMode == 2)
			{
				if (!dbQuery.LoadInput(fileName))
				{
					Console.WriteLine("Error: load input");
					return -5;
				}
			}
			else
			{
				if (!dbQuery.LoadInput(queryId))
				{
					Console.WriteLine("Error: load input");
					return -5;
				}
			}

			sw.Stop();

			Console.WriteLine("Load database done. {0}ms", sw.ElapsedMilliseconds);


			//query
			sw.Restart();

			if (dbQuery.Query())
			{
				if (dbQuery.ConnectMariaDB(project.sqlDBHost, project.sqlDBUser, project.sqlDBPass, project.sqlDBName))
				{
					if (dbQuery.QueryResult2Table(project.rankingResultNum))
					{
						dbQuery.DisconnectMariaDB();
					}
					else
					{
						dbQuery.DisconnectMariaDB();
						Console.WriteLine("Error: query result to table");
						return -8;
					}
				}
				else
				{
					dbQuery.DisconnectMariaDB();
					Console.WriteLine("Error: connecting MySQL server");
					return -7;
				}
			}
			else
			{
				Console.WriteLine("Error: query");
				return -6;
			}

			sw.Stop();

			Console.WriteLine("Query done. {0}ms", sw.ElapsedMilliseconds);

			#endregion


			consoleCopy.Dispose();

			return 1;
		}
	}
}
