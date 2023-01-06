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

			_array = (float*)NativeMemory.AlignedAlloc(size, 32);
			if (setZero) {
				NativeMemory.Fill(_array, size, 0);
			}
		}


		public VectorF(int length)
			: this(length, true)
		{ }


		~VectorF() {
			NativeMemory.AlignedFree(_array);
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


		public override bool Equals(object? obj)
		{
			if (obj is VectorF v)
			{
				if (v.Length != Length)
				{
					return false;
				}

				for (int i = 0; i < Length; i++)
				{
					if (v[i] != this[i])
					{
						return false;
					}
				}

				return true;
			}

			return false;
		}
	}
}
