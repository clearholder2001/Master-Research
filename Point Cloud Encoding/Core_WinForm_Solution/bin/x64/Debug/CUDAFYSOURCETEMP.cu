
// Core.coreClass
extern "C" __global__  void GenerateEigenFeature(int eigenFeatureDiameterMultiple, float gridSize,  double* data2D, int data2DLen0, int data2DLen1, int data2DLen2,  unsigned char* data2DMask, int data2DMaskLen0, int data2DMaskLen1,  double* eigenFeature, int eigenFeatureLen0, int eigenFeatureLen1, int eigenFeatureLen2);

// Core.coreClass
extern "C" __global__  void GenerateEigenFeature(int eigenFeatureDiameterMultiple, float gridSize,  double* data2D, int data2DLen0, int data2DLen1, int data2DLen2,  unsigned char* data2DMask, int data2DMaskLen0, int data2DMaskLen1,  double* eigenFeature, int eigenFeatureLen0, int eigenFeatureLen1, int eigenFeatureLen2)
{
	int num = (int)floorf((float)((blockIdx.x * blockDim.x + threadIdx.x) / data2DLen1));
	int num2 = (blockIdx.x * blockDim.x + threadIdx.x) % data2DLen1;
	int x = threadIdx.x;
	int length = data2DLen0;
	int length2 = data2DLen1;
	float num3 = gridSize * (float)eigenFeatureDiameterMultiple;
	bool flag = data2DMask[(num) * data2DMaskLen1 + ( num2)] == 1;
	if (flag)
	{
		bool flag2 = num < eigenFeatureDiameterMultiple || num > length - 1 - eigenFeatureDiameterMultiple || num2 < eigenFeatureDiameterMultiple || num2 > length2 - 1 - eigenFeatureDiameterMultiple;
		if (!flag2)
		{
			int num4 = 0;
			int num5 = 0;
			float num6 = 0.0f;
			float num7 = 0.0f;
			float num8 = 0.0f;
			for (int num9 = num - eigenFeatureDiameterMultiple; num9 <= num + eigenFeatureDiameterMultiple; num9++)
			{
				for (int num10 = num2 - eigenFeatureDiameterMultiple; num10 <= num2 + eigenFeatureDiameterMultiple; num10++)
				{
					bool flag3 = data2DMask[(num9) * data2DMaskLen1 + ( num10)] == 1;
					if (flag3)
					{
						float num11 = powf((float)(num9 - num) * gridSize, 2.0f);
						float num12 = powf((float)(num10 - num2) * gridSize, 2.0f);
						float num13 = powf((float)(data2D[(num) * data2DLen1 * data2DLen2 + ( num2) * data2DLen2 + (0)] - data2D[(num9) * data2DLen1 * data2DLen2 + ( num10) * data2DLen2 + (0)]), 2.0f);
						float num14 = sqrtf(num11 + num12 + num13);
						bool flag4 = num14 < num3;
						if (flag4)
						{
							num6 += (float)num9 * gridSize;
							num7 += (float)num10 * gridSize;
							num8 += (float)data2D[(num9) * data2DLen1 * data2DLen2 + ( num10) * data2DLen2 + (0)];
							num5++;
						}
					}
					num4++;
				}
			}
			bool flag5 = num5 >= 2;
			if (flag5)
			{
				float num15 = num6 / (float)num5;
				float num16 = num7 / (float)num5;
				float num17 = num8 / (float)num5;
				float num18 = 0.0f;
				float num19 = 0.0f;
				float num20 = 0.0f;
				float num21 = 0.0f;
				float num22 = 0.0f;
				float num23 = 0.0f;
				float num24 = 0.0f;
				float num25 = 0.0f;
				float num26 = 0.0f;
				for (int num27 = num - eigenFeatureDiameterMultiple; num27 <= num + eigenFeatureDiameterMultiple; num27++)
				{
					for (int num28 = num2 - eigenFeatureDiameterMultiple; num28 <= num2 + eigenFeatureDiameterMultiple; num28++)
					{
						bool flag6 = data2DMask[(num27) * data2DMaskLen1 + ( num28)] == 1;
						if (flag6)
						{
							float num29 = powf((float)(num27 - num) * gridSize, 2.0f);
							float num30 = powf((float)(num28 - num2) * gridSize, 2.0f);
							float num31 = powf((float)(data2D[(num) * data2DLen1 * data2DLen2 + ( num2) * data2DLen2 + (0)] - data2D[(num27) * data2DLen1 * data2DLen2 + ( num28) * data2DLen2 + (0)]), 2.0f);
							float num32 = sqrtf(num29 + num30 + num31);
							bool flag7 = num32 < num3;
							if (flag7)
							{
								float num33 = (float)num27 * gridSize - num15;
								float num34 = (float)num28 * gridSize - num16;
								float num35 = (float)data2D[(num27) * data2DLen1 * data2DLen2 + ( num28) * data2DLen2 + (0)] - num17;
								num18 += num33 * num33;
								num19 += num33 * num34;
								num20 += num33 * num35;
								num21 += num34 * num33;
								num22 += num34 * num34;
								num23 += num34 * num35;
								num24 += num35 * num33;
								num25 += num35 * num34;
								num26 += num35 * num35;
							}
						}
					}
				}
				num18 /= (float)num5;
				num19 /= (float)num5;
				num20 /= (float)num5;
				num21 /= (float)num5;
				num22 /= (float)num5;
				num23 /= (float)num5;
				num24 /= (float)num5;
				num25 /= (float)num5;
				num26 /= (float)num5;
				float num36 = 0.0f;
				float num37 = 0.0f;
				float num38 = 0.0f;
				float num39 = powf(num19, 2.0f) + powf(num20, 2.0f) + powf(num23, 2.0f);
				bool flag8 = num39 == 0.0f;
				if (flag8)
				{
					float num40 = num18;
					float num41 = num22;
					float num42 = num26;
					bool flag9 = num40 >= num41 && num40 >= num42;
					if (flag9)
					{
						num36 = num40;
						bool flag10 = num41 >= num42;
						if (flag10)
						{
							num37 = num41;
							num38 = num42;
						}
						else
						{
							num37 = num42;
							num38 = num41;
						}
					}
					else
					{
						bool flag11 = num41 >= num40 && num41 >= num42;
						if (flag11)
						{
							num36 = num41;
							bool flag12 = num40 >= num42;
							if (flag12)
							{
								num37 = num40;
								num38 = num42;
							}
							else
							{
								num37 = num42;
								num38 = num40;
							}
						}
						else
						{
							bool flag13 = num42 >= num40 && num42 >= num41;
							if (flag13)
							{
								num36 = num42;
								bool flag14 = num40 >= num41;
								if (flag14)
								{
									num37 = num40;
									num38 = num41;
								}
								else
								{
									num37 = num41;
									num38 = num40;
								}
							}
						}
					}
				}
				else
				{
					float num43 = (num18 + num22 + num26) / 3.0f;
					float num44 = powf(num18 - num43, 2.0f) + powf(num22 - num43, 2.0f) + powf(num26 - num43, 2.0f) + 2.0f * num39;
					float num45 = sqrtf(num44 / 6.0f);
					float num46 = 1.0f / num45 * (num18 - num43);
					float num47 = 1.0f / num45 * num19;
					float num48 = 1.0f / num45 * num20;
					float num49 = 1.0f / num45 * num21;
					float num50 = 1.0f / num45 * (num22 - num43);
					float num51 = 1.0f / num45 * num23;
					float num52 = 1.0f / num45 * num24;
					float num53 = 1.0f / num45 * num25;
					float num54 = 1.0f / num45 * (num26 - num43);
					float num55 = (num46 * num50 * num54 + num47 * num51 * num52 + num48 * num49 * num53 - num48 * num50 * num52 - num46 * num51 * num53 - num47 * num49 * num54) / 2.0f;
					bool flag15 = num55 <= -1.0f;
					float num56;
					if (flag15)
					{
						num56 = 1.047198f;
					}
					else
					{
						bool flag16 = num55 >= 1.0f;
						if (flag16)
						{
							num56 = 0.0f;
						}
						else
						{
							num56 = acosf(num55) / 3.0f;
						}
					}
					num36 = num43 + 2.0f * num45 * cosf(num56);
					num38 = num43 + 2.0f * num45 * cosf(num56 + 2.094395f);
					num37 = 3.0f * num43 - num36 - num38;
				}
				bool flag17 = num36 < 0.0f;
				if (flag17)
				{
					num36 = 0.0f;
				}
				bool flag18 = num37 < 0.0f;
				if (flag18)
				{
					num37 = 0.0f;
				}
				bool flag19 = num38 < 0.0f;
				if (flag19)
				{
					num38 = 0.0f;
				}
				double num57 = 0.0;
				double num58 = 0.0;
				double num59 = 0.0;
				bool flag20 = num36 > 0.0f;
				if (flag20)
				{
					num57 = (double)((num36 - num37) / num36);
					num58 = (double)((num37 - num38) / num36);
					num59 = (double)(num38 / num36);
				}
				eigenFeature[(num) * eigenFeatureLen1 * eigenFeatureLen2 + ( num2) * eigenFeatureLen2 + (0)] = num57;
				eigenFeature[(num) * eigenFeatureLen1 * eigenFeatureLen2 + ( num2) * eigenFeatureLen2 + (1)] = num58;
				eigenFeature[(num) * eigenFeatureLen1 * eigenFeatureLen2 + ( num2) * eigenFeatureLen2 + (2)] = num59;
			}
		}
	}
}
