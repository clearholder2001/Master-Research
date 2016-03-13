using System;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
// using System.Text;

namespace Core
{
	public class Project
	{
		/// <summary>
		/// default value
		/// gridSize: 0.1m
		/// annularNum: 20
		/// laplacian aperture: 7
		/// gaussian aperture: 7
		/// eigen feature diameter multiple: 5
		/// structure element size: 5
		/// morphology iteration number: 3
		/// display center: false
		/// structure element shape: rectangle
		/// </summary>
		public enum parameterName : byte { Grid_Size, Annular_Number, LoG_Laplacian_Aperture, LoG_Gaussian_Aperture, Eigen_Feature_Diameter_Multiple, Structure_Element_Size, Morphology_Iteration_Number, Display_Center, Structure_Element_Shape }
		public double[] parameter = new double[Enum.GetValues(typeof(parameterName)).Length];

		private xyzClass pointCloud;
		private List<string> polygonalModel_fileName;
		private List<objClass> polygonalModel_list;
		private double[,] pointCloud_annulerFeature;
		private List<double[,]> annulerFeatrue_list;

		public bool ifGenerateFeature { get; set; }
		public bool[] usedFeature { get; set; }
		public double[] featureWeight { get; set; }
		public bool sizeWeight { get; set; }
		public double[] rankingDistance { get; private set; }
		public int[] rankingIndex { get; private set; }


		public Project()
		{
			polygonalModel_fileName = new List<string>();
			polygonalModel_list = new List<objClass>();
			//Set_gridSize();
			//Set_annularNum(10);

			parameter[(int)parameterName.Grid_Size] = 0.1;
			parameter[(int)parameterName.Annular_Number] = 20;
			parameter[(int)parameterName.LoG_Laplacian_Aperture] = 7;
			parameter[(int)parameterName.LoG_Gaussian_Aperture] = 7;
			parameter[(int)parameterName.Eigen_Feature_Diameter_Multiple] = 5;
			parameter[(int)parameterName.Structure_Element_Size] = 5;
			parameter[(int)parameterName.Morphology_Iteration_Number] = 3;
			parameter[(int)parameterName.Display_Center] = 0;
			parameter[(int)parameterName.Structure_Element_Shape] = (double)Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_RECT;

			ifGenerateFeature = false;

			usedFeature = Enumerable.Repeat(true, coreClass.featureDimension).ToArray();
			featureWeight = Enumerable.Repeat(1.0, coreClass.featureDimension).ToArray();
		}

		/// <summary>
		/// set parameter to class of both point cloud and polygonal model 
		/// </summary>
		public bool Set_parameter()
		{
			try
			{
				if (((int)parameter[(int)parameterName.LoG_Laplacian_Aperture] % 2) == 0)
					parameter[(int)parameterName.LoG_Laplacian_Aperture] += 1;
				if (((int)parameter[(int)parameterName.LoG_Gaussian_Aperture] % 2) == 0)
					parameter[(int)parameterName.LoG_Gaussian_Aperture] += 1;
				if ((int)parameter[(int)parameterName.Display_Center] == 0)
				{
					parameter[(int)parameterName.Display_Center] = 0;
					coreClass.ifDisplayCenter = false;
				}
				else
				{
					parameter[(int)parameterName.Display_Center] = 1;
					coreClass.ifDisplayCenter = true;
				}

				coreClass.gridSize = parameter[(int)parameterName.Grid_Size];
				coreClass.annularNum = (int)parameter[(int)parameterName.Annular_Number];
				coreClass.laplacianAperture = (int)parameter[(int)parameterName.LoG_Laplacian_Aperture];
				coreClass.gaussianAperture = (int)parameter[(int)parameterName.LoG_Gaussian_Aperture];
				coreClass.eigenFeatureDiameterMultiple = (int)parameter[(int)parameterName.Eigen_Feature_Diameter_Multiple];
				xyzClass.structEleSize = (int)parameter[(int)parameterName.Structure_Element_Size];
				xyzClass.MorIterNum = (int)parameter[(int)parameterName.Morphology_Iteration_Number];
				xyzClass.structEleShape = (Emgu.CV.CvEnum.CV_ELEMENT_SHAPE)parameter[(int)parameterName.Structure_Element_Shape];
			}
			catch (Exception)
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// reload all point cloud and polygonal model
		/// </summary>
		public void Reload_pointCloud()
		{
			if (pointCloud != null)
			{
				Load_pointCloud(pointCloud.fileName);
			}
		}

		/// <summary>
		/// set grid size
		/// </summary>
		/// <param name="gridSize"></param>
		public void Set_gridSize()
		{
			if (parameter[(int)parameterName.Grid_Size] < 0.0)
			{
				parameter[(int)parameterName.Grid_Size] = 0.1;
			}

			if (pointCloud != null)
			{
				Load_pointCloud(pointCloud.fileName);
			}
			if (polygonalModel_list.Count != 0)
			{
				List<string> fileName_List = new List<string>();
				foreach (objClass model in polygonalModel_list)
				{
					fileName_List.Add(model.fileName);
					Delete_polygonalModel(model.fileName);
				}
				foreach (string fileName in fileName_List)
				{
					Add_polygonalModel(fileName);
				}
			}
		}

		/// <summary>
		/// set anuular num
		/// </summary>
		/// <param name="annularNum"></param>
		public void Set_annularNum()
		{
			if (parameter[(int)parameterName.Annular_Number] < 1)
			{
				parameter[(int)parameterName.Annular_Number] = 10;
			}

			if (pointCloud != null && polygonalModel_list.Count != 0)
			{
				Generate_feature();
			}
		}

		/// <summary>
		/// load point cloud
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public bool Load_pointCloud(string fileName)
		{
			pointCloud = new xyzClass(fileName);
			return pointCloud.GenerateRangeImage();
		}

		/// <summary>
		/// add file name to polygonal model file name list
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public void Add_polygonalModel_fileName(string fileName)
		{
			polygonalModel_fileName.Add(fileName);
		}

		/// <summary>
		/// load all polygonal model
		/// </summary>
		/// <returns></returns>
		public bool Load_allPolygonalModel()
		{
			polygonalModel_list.Clear();

			foreach (string fileName in polygonalModel_fileName)
			{
				if (Add_polygonalModel(fileName))
				{

				}
				else
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// load all polygonal model with progress
		/// </summary>
		/// <param name="bw"></param>
		/// <returns></returns>
		public bool Load_allPolygonalModel_withProgress(BackgroundWorker bw, DoWorkEventArgs e)
		{
			int count = 0;
			bool allCorrect = true;

			polygonalModel_list.Clear();

// 			foreach (string fileName in polygonalModel_fileName)
// 			{
// 				if (!Add_polygonalModel(fileName))
// 				{
// 					return false;
// 				}
// 				bw.ReportProgress(++count);
// 			}

			objClass[] polygonModelArray = new objClass[polygonalModel_fileName.Count];

			Parallel.ForEach(polygonalModel_fileName, (fileName, state, index) =>
			{
				try
				{
					if (bw.CancellationPending != false)
					{
						throw new Exception();
					}
					objClass polygonModel = new objClass(fileName);
					polygonModel.GenerateRangeImage();
					polygonModelArray[index] = polygonModel;
				}
				catch (Exception)
				{
					allCorrect = false;
					e.Cancel = true;
					state.Stop();
				}
				bw.ReportProgress(++count);
			});

			if (bw.CancellationPending != false)
				return allCorrect;

			foreach (objClass polygonModel in polygonModelArray)
			{
				polygonalModel_list.Add(polygonModel);
			}

			return allCorrect;
		}

		/// <summary>
		/// add polygonal model to list
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		private bool Add_polygonalModel(string fileName)
		{
			objClass polygonModel = new objClass(fileName);

			if (polygonModel.GenerateRangeImage())
			{
				polygonalModel_list.Add(polygonModel);
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// delete polygonal model from list
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public bool Delete_polygonalModel(string fileName)
		{
			if (polygonalModel_fileName.Count == 0)
				return false;

			foreach (string modelName in polygonalModel_fileName)
			{
				if (modelName == fileName)
				{
					if (polygonalModel_list.Count != 0)
						polygonalModel_list.RemoveAt(polygonalModel_fileName.IndexOf(modelName));
					polygonalModel_fileName.Remove(modelName);
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// return polygonal model file name list
		/// </summary>
		/// <returns></returns>
		public List<string> Return_polygonalModelFileName()
		{
			return new List<string>(polygonalModel_fileName);
		}

		/// <summary>
		/// return point cloud data
		/// </summary>
		/// <returns></returns>
		public xyzClass Return_pointCloudData()
		{
			return pointCloud;
		}

		/// <summary>
		/// return polygonal model data
		/// </summary>
		/// <param name="selectedIndex"></param>
		/// <returns></returns>
		public objClass Return_polygonalModelData(int selectedIndex)
		{
			return polygonalModel_list[selectedIndex];
		}

		/// <summary>
		/// return type of selected Emgu.CV.CvEnum.CV_ELEMENT_SHAPE
		/// </summary>
		/// <returns></returns>
		public string Reture_SturctureElementShape()
		{
			if (parameter[(int)parameterName.Structure_Element_Shape] == (double)Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_CROSS)
				return "CV_SHAPE_CROSS";
			else if (parameter[(int)parameterName.Structure_Element_Shape] == (double)Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_CUSTOM)
				return "";
			else if (parameter[(int)parameterName.Structure_Element_Shape] == (double)Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE)
				return "CV_SHAPE_ELLIPSE";
			else if (parameter[(int)parameterName.Structure_Element_Shape] == (double)Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_RECT)
				return "CV_SHAPE_RECT";
			else
				return null;
		}

		/// <summary>
		/// generate feature of both point cloud and polygonal model
		/// </summary>
		/// <returns></returns>
		public bool Generate_feature()
		{
			if (!pointCloud.GenerateFeature())
			{
				return false;
			}

			foreach (objClass model in polygonalModel_list)
			{
				if (!model.GenerateFeature())
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// generate feature of only point cloud
		/// </summary>
		/// <returns></returns>
		public bool Generate_feature_onlyPointCloud()
		{
			if (!pointCloud.GenerateFeature())
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// generate feature of both point cloud and polygonal model with progress report
		/// </summary>
		/// <param name="bw"></param>
		/// <returns></returns>
		public bool Generate_feature_withProgress(BackgroundWorker bw, DoWorkEventArgs e)
		{
			int count = 0;
			bool allCorrect = true;

			if (!pointCloud.GenerateFeature())
			{
				return false;
			}
			bw.ReportProgress(++count);

			foreach (objClass model in polygonalModel_list)
			{
				if (!model.GenerateFeature() || bw.CancellationPending != false)
				{
					e.Cancel = true;
					return false;
				}
				bw.ReportProgress(++count);
			}

// 			Parallel.ForEach(polygonalModel_list, (model, state) =>
// 			{
// 				if (!model.GenerateFeature() || bw.CancellationPending != false)
// 				{
// 					allCorrect = false;
// 					e.Cancel = true;
// 					state.Stop();
// 				}
// 				bw.ReportProgress(++count);
// 			});

			return allCorrect;
		}

		/// <summary>
		/// return thumbnail range image of point cloud
		/// </summary>
		/// <returns></returns>
		public Bitmap Get_thumbnailImage()
		{
			if (pointCloud == null)
			{
				return null;
			}
			else
			{
				return pointCloud.rangeThumbnailImg;
			}
		}

		/// <summary>
		/// return thumbnail range image of selectedIndex polygonal model
		/// </summary>
		/// <param name="selectedIndex"></param>
		/// <returns></returns>
		public Bitmap Get_thumbnailImage(int selectedIndex)
		{
			if (polygonalModel_list.Count == 0 || selectedIndex > polygonalModel_list.Count - 1)
			{
				return null;
			}
			else
			{
				return polygonalModel_list[selectedIndex].rangeThumbnailImg;
			}
		}

		/// <summary>
		/// return range image of point cloud
		/// </summary>
		/// <returns></returns>
		public Bitmap Get_rangeImage()
		{
			if (pointCloud == null)
			{
				return null;
			}
			else
			{
				return pointCloud.rangeImg;
			}
		}

		/// <summary>
		/// return range image of selectedIndex polygonal model
		/// </summary>
		/// <param name="selectedIndex"></param>
		/// <returns></returns>
		public Bitmap Get_rangeImage(int selectedIndex)
		{
			if (polygonalModel_list.Count == 0 || selectedIndex > polygonalModel_list.Count - 1)
			{
				return null;
			}
			else
			{
				return polygonalModel_list[selectedIndex].rangeImg;
			}
		}

		/// <summary>
		/// reture feature image of point cloud
		/// </summary>
		/// <param name="featureIndex"></param>
		/// <returns></returns>
		public Bitmap Get_featureImage(int featureIndex)
		{
			if (pointCloud == null)
			{
				return null;
			}
			else
			{
				return utilClass.Data2Bitmap(pointCloud.data2D, featureIndex);
			}
		}

		/// <summary>
		/// reture feature image of polygonal model
		/// </summary>
		/// <param name="selectedIndex"></param>
		/// <param name="featureIndex"></param>
		/// <returns></returns>
		public Bitmap Get_featureImage(int selectedIndex, int featureIndex)
		{
			if (polygonalModel_list.Count == 0 || selectedIndex > polygonalModel_list.Count - 1)
			{
				return null;
			}
			else
			{
				return utilClass.Data2Bitmap(polygonalModel_list[selectedIndex].data2D, featureIndex);
			}
		}

		/// <summary>
		/// return annular feature of point cloud
		/// </summary>
		/// <param name="featureIndex"></param>
		/// <returns></returns>
		public double[] Get_annularFeature(int featureIndex)
		{
			if (pointCloud == null)
			{
				return null;
			}
			else
			{
				double[] annularFeature = new double[coreClass.annularNum];
				for (int i = 0; i < coreClass.annularNum; i++)
				{
					annularFeature[i] = pointCloud.annularFeature[featureIndex, i];
				}
				return annularFeature;
			}
		}

		/// <summary>
		/// return annular feature of polygonal model
		/// </summary>
		/// <param name="selectedIndex"></param>
		/// <param name="featureIndex"></param>
		/// <returns></returns>
		public double[] Get_annularFeature(int selectedIndex, int featureIndex)
		{
			if (polygonalModel_list.Count == 0 || selectedIndex > polygonalModel_list.Count - 1)
			{
				return null;
			}
			else
			{
				double[] annularFeature = new double[coreClass.annularNum];
				for (int i = 0; i < coreClass.annularNum; i++)
				{
					annularFeature[i] = polygonalModel_list[selectedIndex].annularFeature[featureIndex, i];
				}
				return annularFeature;
			}
		}


		/// <summary>
		/// retrieve and rank
		/// </summary>
		/// <returns></returns>
		public void Retrieve_and_Rank()
		{
			//for point cloud
			pointCloud_annulerFeature = new double[xyzClass.featureDimension, coreClass.annularNum];

			for (int i = 0; i < xyzClass.featureDimension; i++)
			{
				if (usedFeature[i])
				{
					double sum = Enumerable.Range(0, coreClass.annularNum).Select(a => pointCloud.annularFeature[i, a]).Sum();
					
					for (int j = 0; j < coreClass.annularNum; j++)
					{
						if (sum != 0)
							pointCloud_annulerFeature[i, j] = pointCloud.annularFeature[i, j] / sum;
						else
							pointCloud_annulerFeature[i, j] = 0.0;
					}
				}
			}

			//for polygonal model
			annulerFeatrue_list = new List<double[,]>();

			foreach (objClass model in polygonalModel_list)
			{
				double[,] annulerFeature = new double[objClass.featureDimension, coreClass.annularNum];

				for (int i = 0; i < objClass.featureDimension; i++)
				{
					if (usedFeature[i])
					{
						double sum = Enumerable.Range(0, coreClass.annularNum).Select(a => model.annularFeature[i, a]).Sum();

						for (int j = 0; j < coreClass.annularNum; j++)
						{
							if (sum != 0.0)
								annulerFeature[i, j] = model.annularFeature[i, j] / sum;
							else
								annulerFeature[i, j] = 0.0;
						}
					}
				}

				annulerFeatrue_list.Add(annulerFeature);
			}

			//rank
			rankingDistance = new double[polygonalModel_list.Count];

			for (int i = 0; i < polygonalModel_list.Count; i++)
			{
				double distSum = 0.0;
				for (int j = 0; j < coreClass.featureDimension; j++)
				{
					distSum += Enumerable.Range(0, coreClass.annularNum).Select(a => Math.Abs(pointCloud_annulerFeature[j, a] - annulerFeatrue_list[i][j, a])).Sum() * featureWeight[j];
				}
				rankingDistance[i] = distSum;

				//weight
				if (sizeWeight)
				{
					double heightWeight = Math.Abs((polygonalModel_list[i].zRange - pointCloud.zRange) / pointCloud.zRange) + 1; 
					double radiusWeight = Math.Abs((polygonalModel_list[i].radiusMax - pointCloud.radiusMax) / pointCloud.radiusMax) + 1;
					double areaWeight = Math.Abs((polygonalModel_list[i].totalArea - pointCloud.totalArea) / pointCloud.totalArea) + 1;

					rankingDistance[i] = rankingDistance[i] * heightWeight * Math.Pow(radiusWeight, 2) * areaWeight;
				}
			}

			rankingIndex = rankingDistance.Select((value, index) => new { value, index }).OrderBy(a => a.value).Select(a => a.index).ToArray();
		}
	}
}
