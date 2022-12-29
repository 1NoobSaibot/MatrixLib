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
			MatrixF matrix = new MatrixF(100, 200);
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
			MatrixF matrix = new MatrixF(10, 20);
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
			float[,] AO = _GetRandomMatrix();
			float[,] BO = _GetRandomMatrix(height: AO.GetWidth());
			float[,] CO = MatrixMathF.Mul(AO, BO);

			MatrixF AN = _CopyAsMatrix(AO);
			MatrixF BN = _CopyAsMatrix(BO);
			MatrixF CNExpected = _CopyAsMatrix(CO);
			MatrixF CNActual = MatrixMath.Mul(AN, BN);

			Assert.AreEqual(CNExpected, CNActual);
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
				CO = MatrixMathF.Mul(AO, BO);
			}
			TimeSpan originalTime = DateTime.Now - start;

			MatrixF AN = _CopyAsMatrix(AO);
			MatrixF BN = _CopyAsMatrix(BO);
			MatrixF CN;

			start = DateTime.Now;
			for (int i = 0; i < 1; i++)
			{
				CN = MatrixMath.Mul(AN, BN);
			}
			TimeSpan newTime = DateTime.Now - start;

			Assert.IsTrue(newTime < originalTime);
		}


		private float[,] _GetRandomMatrix(int width = 0, int height = 0)
		{
			width = width == 0 ? rnd.Next(3, 77) : width;
			height = height == 0 ? rnd.Next(3, 77) : height;

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


		private MatrixF _CopyAsMatrix(float[,] proto)
		{
			var res = new MatrixF(proto.GetWidth(), proto.GetHeight());

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