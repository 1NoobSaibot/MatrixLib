using System.Numerics;

namespace MatrixLib
{
	public class Vector<T> : IVector<T> where T : INumber<T>
	{
		private readonly T[] _array;
		public int Length => _array.Length;


		public Vector(int length)
		{
			_array = new T[length];
		}


		public static implicit operator T[] (Vector<T> v)
		{
			return v._array;
		}


		public T this[int i]
		{
			get => _array[i];
			set => _array[i] = value;
		}


		public override bool Equals(object? obj)
		{
			if (obj is Vector<T> v)
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


		public void AddUp(IVector<T> b)
		{
			if (this.Length != b.Length)
			{
				throw new ArgumentException($"Cannot add two vectors: this({Length}) and B({b.Length})");
			}

			Add(this, b, this);
		}


		public IVector<T> Add(IVector<T> b)
		{
			if (this.Length != b.Length)
			{
				throw new ArgumentException($"Cannot add two vectors: this({Length}) and B({b.Length})");
			}

			var c = new Vector<T>(this.Length);
			Add(this, b, c);
			return c;
		}


		public void Mul(T value)
		{
			for (int i = 0; i < Length; i++)
			{
				_array[i] *= value;
			}
		}


		private static void Add(IVector<T> a, IVector<T> b, IVector<T> c)
		{
			for (int i = 0; i < a.Length; i++)
			{
				c[i] = a[i] + b[i];
			}
		}


		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
