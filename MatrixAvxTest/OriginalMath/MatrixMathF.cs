namespace MatrixAvxTest.OriginalMath
{
	public static class MatrixMathF
	{
		public static float[,] Add(float[,] a, float[,] b)
		{
			int width = a.GetWidth();
			int height = a.GetHeight();
			if (width != b.GetWidth() || height != b.GetHeight())
			{
				throw new ArgumentException("Matrixes have different size");
			}

			float[,] result = new float[width, height];
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					result[i, j] = a[i, j] + b[i, j];
				}
			}

			return result;
		}


		/* public static float GetDeterminant(this float[,] matrix)
		{
			QuadMatrixDeterminantCalculatorF calc = new QuadMatrixDeterminantCalculatorF(matrix);
			return calc.Determinant;
		} */


		public static float[,] Transpose(this float[,] matrixA)
		{
			int width = matrixA.GetLength(0);
			int height = matrixA.GetLength(1);
			float[,] res = new float[height, width];

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					res[y, x] = matrixA[x, y];
				}
			}

			return res;
		}


		public static bool EqualsTo(this float[,] a, float[,] b)
		{
			if (a.GetLength(0) != b.GetLength(0) || a.GetLength(1) != b.GetLength(1))
			{
				return false;
			}

			int width = a.GetLength(0);
			int height = a.GetLength(1);

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					if (a[x, y] != b[x, y])
					{
						return false;
					}
				}
			}

			return true;
		}


		/// <summary>
		/// Returns identity matrix width*width
		/// </summary>
		/// <param name="width"></param>
		/// <returns>Quad Identity Matrix</returns>
		public static float[,] CreateIdentity(int width)
		{
			float[,] m = new float[width, width];
			for (int i = 0; i < width; i++)
			{
				m[i, i] = 1;
			}
			return m;
		}


		public static float[,] Mul(this float[,] a, float b)
		{
			int width = a.GetWidth();
			int height = a.GetHeight();
			float[,] c = new float[width, height];

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					c[x, y] = a[x, y] * b;
				}
			}

			return c;
		}


		public static float[,] Mul(this float[,] a, float[,] b)
		{
			// Ширина матрицы A равна высоте B
			if (a.GetWidth() != b.GetHeight())
			{
				throw new Exception("Cannot execute multiplication of matrixes, because their sizes are not suitable");
			}

			int commonWidth = a.GetWidth();
			int width = b.GetWidth();
			int height = a.GetHeight();
			float[,] c = new float[width, height];

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					float sum = 0.0f;
					for (int i = 0; i < commonWidth; i++)
					{
						sum += a[i, y] * b[x, i];
					}
					c[x, y] = sum;
				}
			}

			return c;
		}


		public static float[,] Extend(float[,] sourceMatrix, int newWidth, int newHeight)
		{
			float[,] res = new float[newWidth, newHeight];
			for (int i = 0; i < sourceMatrix.GetWidth(); i++)
			{
				for (int j = 0; j < sourceMatrix.GetHeight(); j++)
				{
					res[i, j] = sourceMatrix[i, j];
				}
			}
			return res;
		}


		public static int GetWidth(this float[,] a)
		{
			return a.GetLength(0);
		}

		public static int GetHeight(this float[,] a)
		{
			return a.GetLength(1);
		}
	}
}
