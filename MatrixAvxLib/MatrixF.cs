﻿using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace MatrixAvxLib
{
	public unsafe struct MatrixF
	{
		private readonly float* _array;
		public readonly int Width;
		public readonly int Height;


		public MatrixF(int width, int height)
		{
			if (width < 0)
			{
				throw new ArgumentException("Width must be greater than zero");
			}

			if (height < 0)
			{
				throw new ArgumentException("Height must be greater than zero");
			}

			Width = width;
			Height = height;
			_array = (float*)NativeMemory.Alloc((nuint)(width * height * sizeof(float)));
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

				for (int i = 0; i < Width; i++)
				{
					for (int j = 0; j < Height; j++)
					{
						if (m[i, j] != this[i, j])
						{
							return false;
						}
					}
				}

				return true;
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
				if (i < 0 || i > Width || j < 0 || j > Height)
				{
					throw new IndexOutOfRangeException();
				}
				return _array[j * Width + i];
			}
			set
			{
				if (i < 0 || i > Width || j < 0 || j > Height)
				{
					throw new IndexOutOfRangeException();
				}
				_array[j * Width + i] = value;
			}
		}
	}
}