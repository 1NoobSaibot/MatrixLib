using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace MatrixAvxLib
{
	public unsafe class VectorF
	{
		private readonly float* _array;
		public readonly int Length;

		private VectorF(int length, bool setZero)
		{
			Length = length;
			nuint size = (nuint)(length * sizeof(float));

			if (setZero) {
				_array = (float*)NativeMemory.AllocZeroed(size);
			}
			else
			{
				_array = (float*)NativeMemory.Alloc(size);
			}
		}


		public VectorF(int length)
			: this(length, true)
		{ }


		~VectorF() {
			NativeMemory.Free(_array);
		}


		/// <summary>
		/// Creates a vector without filling it zero
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		public static VectorF CreateDirty(int length)
		{
			return new VectorF(length, false);
		}


		public static implicit operator float*(VectorF v)
		{
			return v._array;
		}


		public static implicit operator Vector256<float>*(VectorF v)
		{
			return (Vector256<float>*)v._array;
		}


		public float this[int i]
		{
			get
			{
				if (i < 0 || i >= Length)
				{
					throw new IndexOutOfRangeException();
				}
				return _array[i];
			}
			set
			{
				if (i < 0 || i >= Length)
				{
					throw new IndexOutOfRangeException();
				}
				_array[i] = value;
			}
		}
	}
}
