using System.Numerics;

namespace MatrixLib
{
	public interface IMatrix<T> where T : INumber<T>
	{
		int Width { get; }
		int Height { get; }

		T this[int i, int j] { get; set; }

		IMatrix<T> Mul(IMatrix<T> b);
		IMatrix<T> Add(IMatrix<T> b);
		void AddUp(IMatrix<T> b);
	}
}