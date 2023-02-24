using MatrixAvxLib;
using MatrixAvxTest.OriginalMath;

namespace MatrixAvxTest
{
	[TestClass]
	public class MatrixTest
	{
		private Random rnd = new Random();


		[TestMethod]
		public void ShouldProvideValuesByIndexer()
		{
			MatrixAvxF matrix = new MatrixAvxF(100, 200);
			for (int i = 0; i < matrix.Width; i++)
			{
				for (int j = 0; j < matrix.Height; j++)
				{
					matrix[i, j] = i * j;
				}
			}

			for (int i = 0; i < matrix.Width; i++)
			{
				for (int j = 0; j < matrix.Height; j++)
				{
					Assert.AreEqual(i * j, matrix[i, j]);
				}
			}
		}


		[TestMethod]
		public void ShouldThrowOutOfRangeException()
		{
			MatrixAvxF matrix = new MatrixAvxF(10, 20);
			CheckIndexes(-1, 0);
			CheckIndexes(11, 0);
			CheckIndexes(0, -1);
			CheckIndexes(0, 21);

			void CheckIndexes(int i, int j)
			{
				Assert.ThrowsException<IndexOutOfRangeException>(() =>
				{
					float f = matrix[-1, 0];
				});
			}
		}


		[TestMethod]
		public void ShouldMulAsLightVersion()
		{
			float[,] AO = _GetRandomMatrix(48, 48);
			float[,] BO = _GetRandomMatrix(48, 48);
			float[,] CO = SimpleMath.Mul(AO, BO);

			MatrixAvxF AN = _CopyAsMatrix(AO);
			MatrixAvxF BN = _CopyAsMatrix(BO);
			MatrixAvxF CNExpected = _CopyAsMatrix(CO);
			MatrixAvxF CNActual = MatrixMathAvx.Mul(AN, BN);

			Assert.AreEqual(CNExpected, CNActual);
		}


		[TestMethod]
		public void ShouldWorkFast()
		{
			MatrixAvxF A = _GetRandomMatrixF(1152, 1152);
			MatrixAvxF B = _GetRandomMatrixF(1152, 1152);
			MatrixAvxF C = MatrixMathAvx.Mul(A, B);
		}


		[TestMethod]
		public void ShouldMulFasterThanOriginalMatrixMul()
		{
			float[,] AO = _GetRandomMatrix(1152, 1152);
			float[,] BO = _GetRandomMatrix(1152, 1152);
			float[,] CO;
			
			DateTime start = DateTime.Now;
			for (int i = 0; i < 1; i++)
			{
				CO = SimpleMath.Mul(AO, BO);
			}
			TimeSpan originalTime = DateTime.Now - start;

			MatrixAvxF AN = _CopyAsMatrix(AO);
			MatrixAvxF BN = _CopyAsMatrix(BO);
			MatrixAvxF CN;

			start = DateTime.Now;
			for (int i = 0; i < 1; i++)
			{
				CN = MatrixMathAvx.Mul(AN, BN);
			}
			TimeSpan newTime = DateTime.Now - start;

			Assert.IsTrue(newTime < originalTime);
		}


		private float[,] _GetRandomMatrix(int width = 0, int height = 0)
		{
			width = width == 0 ? rnd.Next(1, 40) : width;
			height = height == 0 ? rnd.Next(1, 40) : height;

			var res = new float[width, height];

			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					res[i, j] = rnd.Next(-9, 10);
				}
			}

			return res;
		}


		private MatrixAvxF _GetRandomMatrixF(int width = 0, int height = 0)
		{
			width = width == 0 ? rnd.Next(1990, 2000) : width;
			height = height == 0 ? rnd.Next(1990, 2000) : height;

			var res = new MatrixAvxF(width, height);

			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					res[i, j] = rnd.Next(-9, 10);
				}
			}

			return res;
		}


		private MatrixAvxF _CopyAsMatrix(float[,] proto)
		{
			var res = new MatrixAvxF(proto.GetWidth(), proto.GetHeight());

			for (int i = 0; i < res.Width; i++)
			{
				for (int j = 0; j < res.Height; j++)
				{
					res[i, j] = proto[i, j];
				}
			}

			return res;
		}
	}
}