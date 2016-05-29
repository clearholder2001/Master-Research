/// <summary>
/// 本版本的陣列維度為[width, height, band]
/// 使用前須先設定專案能容許unsafe操作
/// 專案－＞屬性－＞建置－＞選取 "容許Unsafe程式碼" 
/// 修改日期：2015/12/17
/// </summary>


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;


namespace ImageTool
{
	class BitmapTool
	{
		//轉換Bitmap為Array
		public static byte[,,] Bitmap2Array(ref Bitmap myBitmap)
		{
			byte[,,] imgData = new byte[myBitmap.Width, myBitmap.Height, 3];
			int LayerNumber;
			PixelFormat Format = new PixelFormat();
			ColorPalette tempPalette;

			//判斷24位元彩色影像(R,G,B)
			if (myBitmap.PixelFormat == PixelFormat.Format24bppRgb)
			{
				LayerNumber = 3;
				Format = PixelFormat.Format24bppRgb;
				imgData = new byte[myBitmap.Width, myBitmap.Height, LayerNumber];

				BitmapData bmpData = myBitmap.LockBits(new Rectangle(0, 0, myBitmap.Width, myBitmap.Height), ImageLockMode.ReadWrite, Format);
				int byteOfSkip = bmpData.Stride - bmpData.Width * LayerNumber;

				unsafe
				{
					byte* imgPtr = (byte*)(bmpData.Scan0);
					for (int j = 0; j < bmpData.Height; j++)
					{
						for (int i = 0; i < bmpData.Width; i++)
						{
							for (int k = 0; k < LayerNumber; k++)
							{
								imgData[i, j, k] = *(imgPtr + k);
							}
							imgPtr += LayerNumber;
						}
						imgPtr += byteOfSkip;
					}
				}
				myBitmap.UnlockBits(bmpData);
			}

			//判斷8位元灰階影像
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

				unsafe
				{
					byte tmpDN;
					byte* imgPtr = (byte*)(bmpData.Scan0);
					for (int j = 0; j < bmpData.Height; j++)
					{
						for (int i = 0; i < bmpData.Width; i++)
						{
							for (int k = 0; k < LayerNumber; k++)
							{
								tmpDN = *(imgPtr + k);
								imgData[i, j, k] = newDN[tmpDN];
							}
							imgPtr += LayerNumber;
						}
						imgPtr += byteOfSkip;
					}
				}
				myBitmap.UnlockBits(bmpData);
			}

			return imgData;
		}

		//轉換Array為Bitmap
		public static Bitmap Array2Bitmap(byte[,,] imgData, PixelFormat format)
		{
			int Width = imgData.GetLength(0);
			int Height = imgData.GetLength(1);
			Bitmap myBitmap = new Bitmap(Width, Height, format);
			BitmapData bmpData = myBitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, format);
			ColorPalette tempPalette;
			int LayerNumber = 0;

			//判斷24位元彩色影像(R,G,B)
			if (format == PixelFormat.Format24bppRgb)
			{
				LayerNumber = 3;
			}

			//判斷8位元灰階影像
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
			}

			int byteOfSkip = bmpData.Stride - bmpData.Width * LayerNumber;
			unsafe
			{
				byte* imgPtr = (byte*)(bmpData.Scan0);
				for (int j = 0; j < bmpData.Height; j++)
				{
					for (int i = 0; i < bmpData.Width; i++)
					{
						for (int k = 0; k < LayerNumber; k++)
						{
							*(imgPtr + k) = imgData[i, j, k];
						}
						imgPtr += LayerNumber;
					}
					imgPtr += byteOfSkip;
				}
			}

			myBitmap.UnlockBits(bmpData);
			return myBitmap;
		}
	}
}
