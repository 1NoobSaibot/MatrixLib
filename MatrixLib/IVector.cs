using System.Numerics;
using System.Runtime.Intrinsics.X86;

namespace MatrixLib
{
	public interface IVector<T> where T : INumber<T>
	{
		int Length { get; }
		T this[int i] { get; set; }
		void AddUp(IVector<T> vector);
		IVector<T> Add(IVector<T> vector);
		void Mul(T value);
	}
}
