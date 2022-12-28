namespace MatrixAvxLib
{
	public static unsafe class MatrixMath
	{
		public static MatrixF Mul(MatrixF A, MatrixF B)
		{
			if (A.Width != B.Height)
			{
				var e = new ArgumentException("Ширина матрицы A должна равняться высоте матрицы B");
				e.Data.Add("Matrix A", A);
				e.Data.Add("Matrix B", B);
				throw e;
			}

			int Heigth = A.Height;
			int Width = B.Width;
			int K = A.Width;

			var res = new MatrixF(Width, Heigth);
			gemm_v1(Heigth, Width, K, (float*)A, (float*)B, (float*)res);

			return res;
		}

		private static void gemm_v1(int Height, int Width, int K, float* A, float* B, float* C)
		{
			for (int i = 0; i < Height; ++i)
			{
				float* c = C + i * Width;

				for (int j = 0; j < Width; ++j)
				{
					c[j] = 0;
				}

				for (int k = 0; k < K; ++k)
				{
					float* b = B + k * Width;
					float a = A[i * K + k];
					for (int j = 0; j < Width; ++j)
					{
						c[j] += a * b[j];
					}
				}
			}
		}
	}
}
