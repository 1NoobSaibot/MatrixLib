using MatrixLib;
using System.Runtime.InteropServices;

namespace MatrixAvxLib
{
	public unsafe class MatrixAvxF : IMatrix<float>
	{
		private readonly float* _array;
		public int Width { get; private set; }
		public int Height { get; private set; }


		private MatrixAvxF(int width, int height, bool setZero)
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
			var size = (nuint)(width * height * sizeof(float));
			_array = (float*)NativeMemory.AlignedAlloc(size, 32);

			if (setZero)
			{
				NativeMemory.Fill(_array, size, 0);
			}
		}


		public MatrixAvxF(int width, int height)
			: this(width, height, true)
		{ }


		public static MatrixAvxF CreateDirty(int width, int height)
		{
			return new MatrixAvxF(width, height, false);
		}


		~MatrixAvxF()
		{
			NativeMemory.AlignedFree(_array);
		}


		public override bool Equals(object? obj)
		{
			if (obj == null)
			{
				return false;
			}

			if ((object)this == obj)
			{
				return true;
			}

			if (obj is IMatrix<float> m)
			{
				if (Width != m.Width || Height != m.Height)
				{
					return false;
				}

				if (m is MatrixAvxF mf)
				{
					return AreEqual(this._array, mf._array, Width * Height);
				}

				return AreEqual(this, m);
			}

			return false;
		}


		public static implicit operator float*(MatrixAvxF m)
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


		private bool AreEqual(IMatrix<float> a, IMatrix<float> b)
		{
			for (int i = 0; i < a.Width; i++)
			{
				for (int j = 0; j < a.Height; j++)
				{
					if (a[i, j] != b[i, j])
					{
						return false;
					}
				}
			}

			return true;
		}


		private bool AreEqual(float* p1, float* p2, int length)
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


		public override int GetHashCode()
		{
			return base.GetHashCode();
		}


		public IMatrix<float> Mul(IMatrix<float> b)
		{
			if (b is MatrixAvxF mf)
			{
				return MatrixMathAvx.Mul(this, mf);
			}

			if (this.Width != b.Height)
			{
				throw new Exception($"Cannot mul matrixes: A({this.Width}, {this.Height}) and B({b.Width}, {b.Height})");
			}

			var c = new MatrixAvxF(b.Width, this.Height);
			Matrix<float>.Mul(this, b, c);
			return c;
		}


		public IMatrix<float> Add(IMatrix<float> b)
		{
			var c = new MatrixAvxF(Width, Height);
			Matrix<float>.Add(this, b, c);
			return c;
		}

		public void AddUp(IMatrix<float> b)
		{
			Matrix<float>.Add(this, b, this);
		}
	}
}