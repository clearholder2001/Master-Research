using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using MySql.Data.MySqlClient;

namespace DBQuery
{
	class DBQuery
	{
		public DBCreator dbCreator;
		public DBRecord dbInput;
		public DBRecord[] dbRecordArray;
		public string sqlQueryTableName;
		public string[] sqlTableNames;

		public bool[] usedFeature { get; set; }
		public double[] featureWeight { get; set; }
		public bool sizeWeight { get; set; }
		public double[] rankingDistance { get; private set; }
		public int[] rankingIndex { get; private set; }
		
		//先讀database分辨是否為3 feature格式
		public bool is3FeatureFormat { get; private set; }

		private MySqlConnection dbConn;
		private bool sqlDBReady;

		public DBQuery()
		{
			featureWeight = new double[DBRecord.featureDimension];

			usedFeature = Enumerable.Repeat(true, DBRecord.featureDimension).ToArray();
			featureWeight = Enumerable.Repeat(1.0, DBRecord.featureDimension).ToArray();
			GetSQLTables();
		}

		public bool LoadDatabase()
		{
			if (dbCreator == null)
				return false;

			dbRecordArray = dbCreator.GetDBRecordArray();
			return true;
		}
		
		/*
		public bool LoadDatabase(string fileName)
		{
			try
			{
				FileStream fs = new FileStream(fileName, FileMode.Open);
				BinaryReader bir = new BinaryReader(fs);

				//read header (total length)
				uint fileCount = bir.ReadUInt32();
				
				if (fs.Length == (sizeof(uint) + sizeof(double) * (DBRecord.sizeFeatureNum + DBRecord.annularNum * 5) * fileCount))
				{
					//all feature
					is3FeatureFormat = false;
				}
				else if (fs.Length == (sizeof(uint) + sizeof(double) * (DBRecord.sizeFeatureNum + DBRecord.annularNum * 3) * fileCount))
				{
					//3 feature
					is3FeatureFormat = true;
				}
				else
				{
					return false;
				}
				

				double[] doubleArray = new double[DBRecord.recordLength];

				dbRecordArray = new DBRecord[fileCount];


				//read record
				for (int i = 0; i < fileCount; i++)
				{
					for (int j = 0; j < DBRecord.sizeFeatureNum; j++)
					{
						doubleArray[j] = bir.ReadDouble();
					}

					for (int j = 0; j < DBRecord.featureDimension; j++)
					{
						if (is3FeatureFormat && (j == 2 || j == 4))
							continue;

						for (int k = 0; k < DBRecord.annularNum; k++)
						{
							doubleArray[DBRecord.sizeFeatureNum + (j * DBRecord.annularNum) + k] = bir.ReadDouble();
						}
					}

					dbRecordArray[i] = new DBRecord();
					dbRecordArray[i].LoadFeature(doubleArray);
				}


				bir.Close();
				fs.Close();
				bir.Dispose();
				fs.Dispose();

				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
		*/

		public bool LoadDatabase(string fileName)
		{
			try
			{
				byte[] fileByteArray = File.ReadAllBytes(fileName);

				FileStream fs = new FileStream(fileName, FileMode.Open);

				//read header (total length)
				uint fileCount = BitConverter.ToUInt32(fileByteArray, 0);

				if (fs.Length == (sizeof(uint) + sizeof(double) * (DBRecord.sizeFeatureNum + DBRecord.annularNum * 5) * fileCount))
				{
					//all feature
					is3FeatureFormat = false;
				}
				else if (fs.Length == (sizeof(uint) + sizeof(double) * (DBRecord.sizeFeatureNum + DBRecord.annularNum * 3) * fileCount))
				{
					//3 feature
					is3FeatureFormat = true;
				}
				else
				{
					return false;
				}


				double[] doubleArray = new double[DBRecord.recordLength];

				dbRecordArray = new DBRecord[fileCount];

				//read record
				for (int i = 0; i < fileCount; i++)
				{
					//以byte為單位
					Buffer.BlockCopy(fileByteArray, sizeof(UInt32) + i * DBRecord.recordLength * sizeof(double), doubleArray, 0, DBRecord.recordLength * sizeof(double));

					dbRecordArray[i] = new DBRecord();
					dbRecordArray[i].LoadFeature(doubleArray);
				}


				fs.Close();
				fs.Dispose();

				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public bool LoadInput(string fileName)
		{
			FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate);

			int lineCount = 0;
			string line;
			double featureValue;
			double[] doubleArray = new double[DBRecord.recordLength];

			try
			{
				using (StreamReader sr = new StreamReader(fs))
				{
					while ((line = sr.ReadLine()) != null)
					{
						//此程式假設只有3 feature
						if (false)
						//if (is3FeatureFormat)
						{
							if (Double.TryParse(line, out featureValue))
							{
								if (lineCount >= (DBRecord.sizeFeatureNum + DBRecord.annularNum * 2))
									doubleArray[DBRecord.annularNum + lineCount++] = featureValue;
								else
									doubleArray[lineCount++] = featureValue;
							}
							else
							{
								featureValue = 0;
								doubleArray[lineCount++] = featureValue;
							}
						}
						else
						{
							if (Double.TryParse(line, out featureValue))
							{
								doubleArray[lineCount++] = featureValue;
							}
							else
							{
								featureValue = 0;
								doubleArray[lineCount++] = featureValue;
							}
						}
					}
				}

				dbInput = new DBRecord();
				dbInput.LoadFeature(doubleArray);
			}
			catch (Exception)
			{
				fs.Close();
				return false;
			}

			dbInput.NormalizeFeature();

			fs.Close();
			return true;
		}

		public bool LoadInput(int queryId)
		{
			if (queryId >= 0 && queryId < dbRecordArray.Length)
			{
				dbInput = dbRecordArray[queryId];
				return true;
			}
			else
			{
				return false;
			}
			
		}

		public bool Query()
		{
			if (dbInput == null || dbRecordArray == null)
				return false;

			rankingDistance = new double[dbRecordArray.Length];

			//for (int i = 0; i < dbRecordArray.Length; i++)
			Parallel.For(0, dbRecordArray.Length, i =>
			{
				double distSum = 0.0;
				for (int j = 0; j < DBRecord.featureDimension; j++)
				{
					if (usedFeature[j])
						distSum += Enumerable.Range(0, DBRecord.annularNum).Select(a => Math.Abs(dbInput.featureValue[j, a] - dbRecordArray[i].featureValue[j, a])).Sum() * featureWeight[j];
					else
						continue;
				}
				rankingDistance[i] = distSum;

				//weight
				if (sizeWeight)
				{
					double zRangeWeight = Math.Abs((dbRecordArray[i].zRange - dbInput.zRange) / dbInput.zRange) + 1;
					double zAvgWeight = Math.Abs((dbRecordArray[i].zAvg - dbInput.zAvg) / dbInput.zAvg) + 1;
					double radiusWeight = Math.Abs((dbRecordArray[i].radiusMax - dbInput.radiusMax) / dbInput.radiusMax) + 1;
					double areaWeight = Math.Abs((dbRecordArray[i].totalArea - dbInput.totalArea) / dbInput.totalArea) + 1;

					rankingDistance[i] = rankingDistance[i] * zRangeWeight * zAvgWeight * Math.Pow(radiusWeight, 2) * areaWeight;
				}

				//remove cases of NaN
				if (double.IsNaN(rankingDistance[i]))
					rankingDistance[i] = double.MaxValue;
			}
			);

			rankingIndex = rankingDistance.Select((value, index) => new { value, index }).AsParallel().OrderBy(a => a.value).Select(a => a.index).ToArray();

			return true;
		}

		public MySqlDataReader QueryDetail(int id)
		{
			if (!sqlDBReady)
				return null;

			using (MySqlCommand dbCommand = dbConn.CreateCommand())
			{
				dbCommand.CommandText = string.Format("SELECT * FROM {0} WHERE id = {1}", sqlQueryTableName, id);
				MySqlDataReader dbReader = dbCommand.ExecuteReader();
				return dbReader;
			}			
		}

		public bool QueryResult2Table(int totalNum)
		{
			if (!sqlDBReady)
				return false;

			int rankingId;

			try
			{
				using (MySqlCommand dbCommand = dbConn.CreateCommand())
				{
					//清空query result資料表
					dbCommand.CommandText = "TRUNCATE query_result";
					dbCommand.ExecuteNonQuery();

					for (int i = 0; i < totalNum; i++)
					{
						rankingId = rankingIndex[i] + 1;

						dbCommand.CommandText = string.Format("INSERT INTO `query_result`(`ranking`, `distance`, `id`, `mid`, `mod4`, `state`, `longitude`, `latitude`, `txt00`) SELECT {0}, {1}, `id`, `mid`, `mod4`, `state`, `longitude`, `latitude`, `txt00` FROM `{2}` WHERE `id` = {3}", i + 1, rankingDistance[rankingIndex[i]], sqlQueryTableName, rankingIndex[i] + 1);
						dbCommand.ExecuteNonQuery();
					}
				}

				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public bool ConnectMariaDB()
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
				sqlDBReady = true;
				return true;
			}
			catch (Exception)
			{
				sqlDBReady = false;
				return false;
			}
		}

		public bool ConnectMariaDB(string dbHost, string dbUser, string dbPass, string dbName)
		{
			string connStr = "server=" + dbHost + ";uid=" + dbUser + ";pwd=" + dbPass + ";database=" + dbName/* + ";CharSet=utf8mb4_unicode_ci"*/;
			dbConn = new MySqlConnection(connStr);

			try
			{
				dbConn.Open();
				sqlDBReady = true;
				return true;
			}
			catch (Exception)
			{
				sqlDBReady = false;
				return false;
			}
		}

		public bool DisconnectMariaDB()
		{
			if (sqlDBReady)
			{
				try
				{
					dbConn.Close();
					return true;
				}
				catch (Exception)
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}

		private bool GetSQLTables()
		{
			if (ConnectMariaDB())
			{
				try
				{
					using (MySqlCommand dbCommand = dbConn.CreateCommand())
					{
						dbCommand.CommandText = "SHOW TABLES";
						MySqlDataReader dbReader = dbCommand.ExecuteReader();
						List<string> nameList = new List<string>();
						while (dbReader.Read())
							nameList.Add(dbReader["Tables_in_model_retrieval"].ToString());
						sqlTableNames = nameList.ToArray();
					}
					DisconnectMariaDB();
					return true;
				}
				catch (Exception)
				{
					DisconnectMariaDB();
					return false;
				}
			}
			else
			{
				return false;
			}
		}
	}
}
