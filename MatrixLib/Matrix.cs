using System.Numerics;

namespace MatrixLib
{
	public class Matrix<T> : IMatrix<T> where T : INumber<T>
	{
		private readonly T[,] _array;
		public int Width => _array.GetLength(0);
		public int Height => _array.GetLength(1);


		public Matrix(int width, int height)
		{
			_array = new T[width, height];
		}


		public override bool Equals(object? obj)
		{
			if (obj is Matrix<T> m)
			{
				if (Width != m.Width || Height != m.Height)
				{
					return false;
				}

				if (_array == m._array)
				{
					return true;
				}

				return AreEqual(m, this);
			}

			return false;
		}


		public T this[int i, int j]
		{
			get => _array[i, j];
			set => _array[i, j] = value;
		}


		public override int GetHashCode()
		{
			return base.GetHashCode();
		}


		public static IMatrix<T> operator *(Matrix<T> a, IMatrix<T> b)
		{
			return a.Mul(b);
		}

		public IMatrix<T> Mul(IMatrix<T> b)
		{
			// Ширина матрицы A равна высоте B
			if (this.Width != b.Height)
			{
				throw new Exception("Cannot execute multiplication of matrixes, because their sizes are not suitable");
			}

			int width = b.Width;
			int height = this.Height;
			IMatrix<T> c = new Matrix<T>(width, height);
			Mul(this, b, c);
			return c;
		}


		public static void Mul(IMatrix<T> a, IMatrix<T> b, IMatrix<T> c)
		{
			if (a.Width != b.Height || b.Width != c.Width || a.Height != c.Height)
			{
				throw new Exception("Cannot execute multiplication of matrixes, because their sizes are not suitable");
			}

			int commonWidth = a.Width;
			int width = b.Width;
			int height = a.Width;

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					T sum = T.Zero;
					for (int i = 0; i < commonWidth; i++)
					{
						sum += a[i, y] * b[x, i];
					}
					c[x, y] = sum;
				}
			}
		}


		public IMatrix<T> Add(IMatrix<T> b)
		{
			if (this.Width != b.Width || this.Height != b.Height)
			{
				throw new Exception($"Cannot add two matrixes: A({Width}, {Height}) and B({b.Width}, {b.Height})");
			}

			var c = new Matrix<T>(Width, Height);
			Add(this, b, c);
			return c;
		}


		public void AddUp(IMatrix<T> b)
		{
			if (this.Width != b.Width || this.Height != b.Height)
			{
				throw new Exception($"Cannot add two matrixes: A({Width}, {Height}) and B({b.Width}, {b.Height})");
			}

			Add(this, b, this);
		}


		public static void Add(IMatrix<T> a, IMatrix<T> b, IMatrix<T> c)
		{
			if (
				a.Width != b.Width || a.Width != c.Width
				|| a.Height != b.Height || a.Height != c.Height
			)
			{
				throw new ArgumentException("Matrixes have different sizes");
			}

			for (int i = 0; i < a.Width; i++)
			{
				for (int j = 0; j < a.Height; j++)
				{
					c[i, j] = a[i, j] + b[i, j];
				}
			}
		}


		public static bool AreEqual(IMatrix<T> a, IMatrix<T> b)
		{
			if (
				a.Width != b.Width || a.Height != b.Height
			)
			{
				return false;
			}

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
	}
}