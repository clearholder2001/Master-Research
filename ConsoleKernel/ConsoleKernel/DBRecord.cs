using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBQuery
{
	public class DBRecord
	{
		public enum feature : byte { Height, Edge, Linearity, Planarity, Sphericity }
		public enum sizeFeature : byte { zRange, zAvg, totalArea, totalVol, radiusMax }
		public double zRange, zAvg, totalArea, totalVol, radiusMax;
		public double[,] featureValue;

		public const int featureHeaderNum = 5;
		public const int featureDimension = 5;
		public const int annularNum = 30;
		public const int recordLength = featureHeaderNum + featureDimension * annularNum;

		public DBRecord()
		{
			featureValue = new double[featureDimension, annularNum];
		}

		public bool LoadFeature(double[] doubleArray)
		{
			//檢查長度
			if (doubleArray.Length != recordLength)
				return false;

			zRange = doubleArray[0];
			zAvg = doubleArray[1];
			totalArea = doubleArray[2];
			totalVol = doubleArray[3];
			radiusMax = doubleArray[4];

			for (int i = 0; i < featureDimension; i++)
			{
				for (int j = 0; j < annularNum; j++)
				{
					featureValue[i, j] = doubleArray[featureHeaderNum + i * annularNum + j];
				}
			}

			return true;
		}

		public void NormalizeFeature()
		{
			double sum;
			double[,] newFeatureValue = new double[featureDimension, annularNum];

			for (int i = 0; i < featureDimension; i++)
			{
				sum = Enumerable.Range(0, annularNum).AsParallel().Select(a => featureValue[i, a]).Sum();

				for (int j = 0; j < annularNum; j++)
				{
					if (sum != 0.0)
						newFeatureValue[i, j] = featureValue[i, j] / sum;
					else
						newFeatureValue[i, j] = 0.0;
				}
			}

			featureValue = newFeatureValue;
		}

		public string OutputSeries()
		{
			string series = string.Empty;

			series += zRange + "\n";
			series += zAvg + "\n";
			series += totalArea + "\n";
			series += totalVol + "\n";
			series += radiusMax + "\n";


			for (int i = 0; i < featureDimension; i++)
			{
				for (int j = 0; j < annularNum; j++)
				{
					series += featureValue[i, j] + "\n";
				}
			}

			return series;
		}
	}
}
