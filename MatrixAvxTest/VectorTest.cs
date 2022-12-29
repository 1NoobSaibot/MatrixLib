using MatrixAvxLib;
using MatrixAvxTest.OriginalMath;

namespace MatrixAvxTest
{
	[TestClass]
	public class VectorFTest
	{
		private Random rnd = new Random();


		[TestMethod]
		public void ShouldNotThrowAnErrorWhenIndexIsRight()
		{
			VectorF v = new VectorF(4);
			v[0] = 1;
			v[1] = 2;
			v[2] = 3;
			v[3] = 4;
		}


		[TestMethod]
		public void ShouldThrowAnErrorWhenOutOfRange()
		{
			VectorF v = new VectorF(4);
			Assert.ThrowsException<IndexOutOfRangeException>(() =>
			{
				v[-1] = 1;
			});
			Assert.ThrowsException<IndexOutOfRangeException>(() =>
			{
				v[4] = 1;
			});
		}


		private float[,] _GetRandomMatrix(int width = 0, int height = 0)
		{
			width = width == 0 ? rnd.Next(3, 7) : width;
			height = height == 0 ? rnd.Next(3, 7) : height;

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