using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms.DataVisualization.Charting;
using System.Numerics;

namespace ImageTool
{
	class BitmapTool
	{
		public static byte[, ,] Bitmap2Array(Bitmap myBitmap) //轉換Bitmap為Array
		{
			byte[, ,] imgData = new byte[myBitmap.Width, myBitmap.Height, 3];
			int LayerNumber = 0;
			PixelFormat Format = new PixelFormat();
			ColorPalette tempPalette;
			if (myBitmap.PixelFormat == PixelFormat.Format24bppRgb)
			{
				LayerNumber = 3;
				Format = PixelFormat.Format24bppRgb;
				imgData = new byte[myBitmap.Width, myBitmap.Height, LayerNumber];

				BitmapData bmpData = myBitmap.LockBits(new Rectangle(0, 0, myBitmap.Width, myBitmap.Height), ImageLockMode.ReadWrite, Format);
				int byteOfSkip = bmpData.Stride - bmpData.Width * LayerNumber;
				unsafe　　//專案－＞屬性－＞建置－＞容許Unsafe程式碼須選取。           
				{
					byte* imgPtr = (byte*)(bmpData.Scan0);
					for (int y = 0; y < bmpData.Height; y++)
					{
						for (int x = 0; x < bmpData.Width; x++)
						{
							for (int k = 0; k < LayerNumber; k++)
							{
								imgData[x, y, k] = *(imgPtr + k);
							}
							imgPtr += LayerNumber;
						}
						imgPtr += byteOfSkip;
					}
				}
				myBitmap.UnlockBits(bmpData);
			}//判斷24位元彩色影像(R,G,B)

			if (myBitmap.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				LayerNumber = 1;
				Format = PixelFormat.Format8bppIndexed;
				tempPalette = myBitmap.Palette;
				byte[] newDN = new byte[256];
				Color c;
				for (int i = 0; i < 256; i++)
				{
					c = tempPalette.Entries[i];
					newDN[i] = Convert.ToByte(c.R * 0.299 + c.G * 0.587 + c.B * 0.114);
				}
				imgData = new byte[myBitmap.Width, myBitmap.Height, LayerNumber];

				BitmapData bmpData = myBitmap.LockBits(new Rectangle(0, 0, myBitmap.Width, myBitmap.Height), ImageLockMode.ReadWrite, Format);
				int byteOfSkip = bmpData.Stride - bmpData.Width * LayerNumber;
				unsafe　　//專案－＞屬性－＞建置－＞容許Unsafe程式碼須選取。           
				{
					byte tmpDN;
					byte* imgPtr = (byte*)(bmpData.Scan0);
					for (int y = 0; y < bmpData.Height; y++)
					{
						for (int x = 0; x < bmpData.Width; x++)
						{
							for (int k = 0; k < LayerNumber; k++)
							{
								tmpDN = *(imgPtr + k);
								imgData[x, y, k] = newDN[tmpDN];
							}
							imgPtr += LayerNumber;
						}
						imgPtr += byteOfSkip;
					}
				}
				myBitmap.UnlockBits(bmpData);
			}//判斷8位元灰階影像

			if (myBitmap.PixelFormat == PixelFormat.Format1bppIndexed)
			{
				LayerNumber = 1;
				Format = PixelFormat.Format8bppIndexed;
				tempPalette = myBitmap.Palette;
				byte[] newDN = new byte[2];
				Color c;
				for (int i = 0; i < 2; i++)
				{
					c = tempPalette.Entries[i];
					newDN[i] = Convert.ToByte(c.R * 0.299 + c.G * 0.587 + c.B * 0.114);
				}
				imgData = new byte[myBitmap.Width, myBitmap.Height, LayerNumber];

				BitmapData bmpData = myBitmap.LockBits(new Rectangle(0, 0, myBitmap.Width, myBitmap.Height), ImageLockMode.ReadOnly, Format);
				int byteOfSkip = bmpData.Stride - bmpData.Width * LayerNumber;
				unsafe　　//專案－＞屬性－＞建置－＞容許Unsafe程式碼須選取。           
				{
					byte tmpDN;
					byte* imgPtr = (byte*)(bmpData.Scan0);
					for (int y = 0; y < bmpData.Height; y++)
					{
						for (int x = 0; x < bmpData.Width; x++)
						{
							for (int k = 0; k < LayerNumber; k++)
							{
								tmpDN = *(imgPtr + k);
								imgData[x, y, k] = newDN[tmpDN];
								//  ImgData[x, y, k] = *(imgPtr);
							}
							imgPtr += LayerNumber;
						}
						imgPtr += byteOfSkip;
					}
				}
				myBitmap.UnlockBits(bmpData);
			}//判斷2位元灰階影像

			return imgData;
		}
		public static Bitmap Array2Bitmap(byte[, ,] imgData, PixelFormat format) //轉換Array為Bitmap
		{
			int Width = imgData.GetLength(0);
			int Height = imgData.GetLength(1);
			Bitmap myBitmap = new Bitmap(Width, Height, format);
			BitmapData bmpData = myBitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, format);
			ColorPalette tempPalette;
			int LayerNumber = 0;
			if (format == PixelFormat.Format24bppRgb)
			{
				LayerNumber = 3;
			}//判斷24位元彩色影像(R,G,B)
			if (format == PixelFormat.Format8bppIndexed)
			{
				LayerNumber = 1;
				using (Bitmap tempBmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed))
				{
					tempPalette = tempBmp.Palette;
				}
				for (int i = 0; i < 256; i++)
				{
					tempPalette.Entries[i] = Color.FromArgb(i, i, i);
				}
				myBitmap.Palette = tempPalette;
			}//判斷8位元灰階影像
			if (format == PixelFormat.Format1bppIndexed)
			{
				LayerNumber = 1;
				for (int y = 0; y < Height; y++)
				{
					for (int x = 0; x < Width; x++)
					{
						if (imgData[x, y, 0] > 0)
						{
							imgData[x, y, 0] = 1;
						}
					}
				}
			}//判斷2位元灰階影像

			if (format == PixelFormat.Format24bppRgb || format == PixelFormat.Format8bppIndexed)
			{
				int byteOfSkip = bmpData.Stride - bmpData.Width * LayerNumber;
				unsafe　　//專案－＞屬性－＞建置－＞容許Unsafe程式碼須選取。           
				{
					byte* imgPtr = (byte*)(bmpData.Scan0);
					for (int y = 0; y < bmpData.Height; y++)
					{
						for (int x = 0; x < bmpData.Width; x++)
						{
							for (int k = 0; k < LayerNumber; k++)
							{
								*(imgPtr + k) = imgData[x, y, k];
							}
							imgPtr += LayerNumber;
						}
						imgPtr += byteOfSkip;
					}
				}
				myBitmap.UnlockBits(bmpData);
			}
			else
			{
				unsafe　　//專案－＞屬性－＞建置－＞容許Unsafe程式碼須選取。           
				{
					for (int y = 0; y < Height; y++)
					{
						for (int x = 0; x < Width; x++)
						{
							byte* p = (byte*)bmpData.Scan0;
							int index = y * bmpData.Stride + (x >> 3);
							byte mask = (byte)(0x80 >> (x & 0x7));
							if (imgData[x, y, 0] == 1)
								p[index] |= mask;
							else
								p[index] &= (byte)(mask ^ 0xff);
						}
					}
				}
				myBitmap.UnlockBits(bmpData);
			}

			return myBitmap;
		}
		public static void ChartAddSeries(byte[, ,] imgData, Chart chart1) //在Chart元件上加入直方圖資料
		{
			int Width = imgData.GetLength(0);
			int Height = imgData.GetLength(1);
			int[] imgDNCount = new int[256];
			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					imgDNCount[imgData[x, y, 0]]++;
				}
			}
			int countMax = 0;
			for (int i = 0; i < imgDNCount.Length; i++)
			{
				if (imgDNCount[i] > countMax)
					countMax = imgDNCount[i];
			}
			chart1.Series.Clear();
			chart1.Series.Add("Series1");
			chart1.Series["Series1"].ChartType = SeriesChartType.Column;
			chart1.ChartAreas["ChartArea1"].AxisX.Minimum = 0;
			chart1.ChartAreas["ChartArea1"].AxisX.Maximum = 255;
			chart1.ChartAreas["ChartArea1"].AxisY.Minimum = 0;
			chart1.ChartAreas["ChartArea1"].AxisY.Maximum = Math.Ceiling((double)countMax / 100) * 100;
			for (int i = 0; i < 256; i++)
			{
				chart1.Series["Series1"].Points.AddXY(i, imgDNCount[i]);
			}
		}
		public static byte[, ,] HistEqualization(byte[, ,] imgData) //Histogram Equalization funtion
		{
			int Width = imgData.GetLength(0);
			int Height = imgData.GetLength(1);
			byte[, ,] tmpData = new byte[Width, Height, 1];
			int[] imgDN = new int[256];
			for (int i = 0; i < 256; i++)
				imgDN[i] = 0;
			double constant = 255 / (double)(Width * Height);
			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					imgDN[imgData[x, y, 0]]++;
				}
			}
			for (int i = 1; i < 256; i++)
			{
				imgDN[i] = imgDN[i] + imgDN[i - 1];
			}
			for (int i = 0; i < 256; i++)
			{
				imgDN[i] = (byte)Math.Round(((double)imgDN[i] * constant));
			}
			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					tmpData[x, y, 0] = (byte)imgDN[imgData[x, y, 0]];
				}
			}
			return tmpData;
		}
		public static Bitmap ResizeBitmap(Bitmap sourceBitmap, int width, int height, bool resize2SquareAndPowerOf2)
		{
			if (resize2SquareAndPowerOf2)
			{
				double p;
				int length;
				if (width <= height)
				{
					p = Math.Log(width, 2);
					length = (int)Math.Pow(2, Math.Truncate(p));
					Rectangle srcRect = new Rectangle(0, (height - width) / 2, width, width);
					Rectangle destRect = new Rectangle(0, 0, length, length);
					Bitmap result = new Bitmap(length, length);
					using (Graphics g = Graphics.FromImage(result))
						g.DrawImage(sourceBitmap, destRect, srcRect, GraphicsUnit.Pixel);
					return result;
				}
				else
				{
					p = Math.Log(height, 2);
					length = (int)Math.Pow(2, Math.Truncate(p));
					Rectangle srcRect = new Rectangle((width - height) / 2, 0, height, height);
					Rectangle destRect = new Rectangle(0, 0, length, length);
					Bitmap result = new Bitmap(length, length);
					using (Graphics g = Graphics.FromImage(result))
						g.DrawImage(sourceBitmap, destRect, srcRect, GraphicsUnit.Pixel);
					return result;
				}
			}
			else
			{
				Bitmap result = new Bitmap(width, height);
				using (Graphics g = Graphics.FromImage(result))
					g.DrawImage(sourceBitmap, 0, 0, width, height);
				return result;
			}
		}
		public static byte[, ,] Complex2Array(Complex[,] comData,bool logDisplay,double logCoef)
		{
			int M = comData.GetLength(0);
			int N = comData.GetLength(1);

			byte[, ,] byteData = new byte[M, N, 1];
			double[,] magnitudeData = new double[M, N];

			if (logDisplay)
			{
				for (int u = 0; u < M; u++)
				{
					for (int v = 0; v < N; v++)
					{
						magnitudeData[u, v] = Math.Log10(1 + logCoef * comData[u, v].Magnitude);
					}
				}
			}
			else 
			{
				for (int u = 0; u < M; u++)
				{
					for (int v = 0; v < N; v++)
					{
						magnitudeData[u, v] = comData[u, v].Magnitude;
					}
				}
			}

			double max = magnitudeData[0, 0];
			double min = magnitudeData[0, 0];

			for (int u = 0; u < M; u++)
			{
				for (int v = 0; v < N; v++)
				{
					if (magnitudeData[u, v] > max)
						max = magnitudeData[u, v];
					if (magnitudeData[u, v] < min)
						min = magnitudeData[u, v];
				}
			}
			if (max == min)
			{
				for (int u = 0; u < M; u++)
				{
					for (int v = 0; v < N; v++)
					{
						byteData[u, v, 0] = 255;
					}
				}
			}
			else
			{
				for (int u = 0; u < M; u++)
				{
					for (int v = 0; v < N; v++)
					{
						byteData[u, v, 0] = Convert.ToByte((magnitudeData[u, v] - min) / (max - min) * 255.0);
					}
				}
			}

			return byteData;
		}
		public static Complex[,] Array2Complex(byte[, ,] byteData)
		{
			int M = byteData.GetLength(0);
			int N = byteData.GetLength(1);

			Complex[,] comData = new Complex[M, N];

			for (int u = 0; u < M; u++)
			{
				for (int v = 0; v < N; v++)
				{
					comData[u, v] = new Complex(byteData[u, v, 0],0);
				}
			}

			return comData;
		}
	}
}
