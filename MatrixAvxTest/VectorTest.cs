using MatrixAvxLib;
using MatrixAvxTest.OriginalMath;

namespace MatrixAvxTest
{
	[TestClass]
	public class VectorFTest
	{
		private Random rnd = new Random();


		[TestMethod]
		public void ShouldAddAsLightVersion()
		{
			VectorF v1 = _CreateRandomVector();
			VectorF v2 = _CreateRandomVector(v1.Length);
			VectorF vActual = new VectorF(v1.Length);
			MatrixMath.Add(v1, v2, vActual);

			VectorF vExpected = SimpleMath.Add(v1, v2);

			Assert.AreEqual(vExpected, vActual);
		}


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


		private VectorF _CreateRandomVector(int length = 0)
		{
			length = length == 0 ? rnd.Next(1, 20) : length;
			var res = new VectorF(length);

			for (int i = 0; i < length; i++)
			{
				res[i] = rnd.Next(-9, 10);
			}

			return res;
		}
	}
}