using System.Runtime.InteropServices;

namespace MatrixAvxLib
{
	public unsafe class MatrixF
	{
		private readonly float* _array;
		public readonly int Width;
		public readonly int Height;


		private MatrixF(int width, int height, bool setZero)
		{
			if (width < 0)
			{
				throw new ArgumentException("Width must be greater than or equal to zero");
			}

			if (height < 0)
			{
				throw new ArgumentException("Height must be greater than or equal to zero");
			}

			Width = width;
			Height = height;

			if (setZero)
			{
				_array = (float*)NativeMemory.AllocZeroed((nuint)(width * height * sizeof(float)));
			}
			else
			{
				_array = (float*)NativeMemory.Alloc((nuint)(width * height * sizeof(float)));
			}
		}


		public MatrixF(int width, int height)
			: this(width, height, true)
		{ }


		public static MatrixF CreateDirty(int width, int height)
		{
			return new MatrixF(width, height, false);
		}


		~MatrixF()
		{
			NativeMemory.Free(_array);
		}


		public override bool Equals(object obj)
		{
			if (obj is MatrixF m)
			{
				if (Width != m.Width || Height != m.Height)
				{
					return false;
				}

				if (_array == m._array)
				{
					return true;
				}

				return _AreEqual(m._array, _array, Width * Height);
			}

			return false;
		}


		public static implicit operator float*(MatrixF m)
		{
			return m._array;
		}


		public float this[int i, int j]
		{
			get
			{
				if (i < 0 || i >= Width || j < 0 || j >= Height)
				{
					throw new IndexOutOfRangeException();
				}
				return _array[j * Width + i];
			}
			set
			{
				if (i < 0 || i >= Width || j < 0 || j >= Height)
				{
					throw new IndexOutOfRangeException();
				}
				_array[j * Width + i] = value;
			}
		}


		private bool _AreEqual(float* p1, float* p2, int length)
		{
			for (int i = 0; i < length; i++)
			{
				if (p1[i] != p2[i])
				{
					return false;
				}
			}

			return true;
		}
	}
}