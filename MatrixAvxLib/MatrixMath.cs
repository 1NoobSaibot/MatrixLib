using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace MatrixAvxLib
{
	public static unsafe class MatrixMath
	{
		public static void Add(VectorF A, VectorF B, VectorF Result)
		{
			if (A.Length != B.Length || A.Length != Result.Length)
			{
				throw new ArgumentException("Lengths of vectors must be the same");
			}

			var Ap = (float*)A;
			var Bp = (float*)B;
			var Cp = (float*)Result;

			int N8 = A.Length - (A.Length % 8);
			int i = 0;
			for (; i < N8; i += 8)
			{
				Avx2.Store(Cp + i, Avx2.Add(Avx2.LoadVector256(Ap + i), Avx2.LoadVector256(Bp + i)));
			}

			for (; i < A.Length; i++)
			{
				Cp[i] = Ap[i] + Bp[i];
			}
		}


		public static MatrixF Mul(MatrixF A, MatrixF B)
		{
			if (A.Width != B.Height)
			{
				var e = new ArgumentException("Ширина матрицы A должна равняться высоте матрицы B");
				e.Data.Add("Matrix A", A);
				e.Data.Add("Matrix B", B);
				throw e;
			}

			int Heigth = A.Height;
			int Width = B.Width;
			int K = A.Width;

			var res = new MatrixF(Width, Heigth);
			gemm_v2_UniversalN(Heigth, Width, K, (float*)A, (float*)B, (float*)res);

			return res;
		}


		public static void Mul (VectorF A, MatrixF B, VectorF C)
		{
			if (A.Length != B.Height)
			{
				var e = new ArgumentException("Ширина матрицы A должна равняться высоте матрицы B");
				e.Data.Add("Vector A", A);
				e.Data.Add("Matrix B", B);
				throw e;
			}

			if (B.Width != C.Length)
			{
				var e = new ArgumentException("Ширина матрицы B должна равняться высоте матрицы C");
				e.Data.Add("Matrix B", B);
				e.Data.Add("Vector C", C);
				throw e;
			}

			int M = 1;
			int N = B.Width;
			int K = A.Length;

			gemm_v2_UniversalN(M, N, K, (float*)A, (float*)B, (float*)C);
		}


		public static void Mul(MatrixF A, VectorF B, VectorF C)
		{
			if (A.Width != B.Length)
			{
				var e = new ArgumentException("Ширина матрицы A должна равняться высоте матрицы B");
				e.Data.Add("Vector A", A);
				e.Data.Add("Matrix B", B);
				throw e;
			}

			if (A.Height != C.Length)
			{
				var e = new ArgumentException("Высота матрицы A должна равняться высоте матрицы С");
				e.Data.Add("Matrix B", B);
				e.Data.Add("Vector C", C);
				throw e;
			}

			int M = A.Height;
			int N = 1;
			int K = A.Width;

			gemm_v2_UniversalN(M, N, K, (float*)A, (float*)B, (float*)C);
		}


		public static void gemm_v2_UniversalN(int M, int N, int K, float* A, float* B, float* C)
		{
			int N16 = N - (N % 16);
			int N8 = N - (N % 8);

			NativeMemory.Fill(C, (nuint)(M * N * sizeof(float)), 0);
			for (int i = 0; i < M; ++i)
			{
				float* c = C + i * N;

				for (int k = 0; k < K; ++k)
				{
					float* b = B + k * N;
					float aValue = A[i * K + k];
					Vector256<float> a = Vector256.Create<float>(aValue);
					int j = 0;
					for (; j < N16; j += 16)
					{
						Avx2.Store(c + j + 0, Fma.MultiplyAdd(a, Avx2.LoadVector256(b + j + 0), Avx2.LoadVector256(c + j + 0)));
						Avx2.Store(c + j + 8, Fma.MultiplyAdd(a, Avx2.LoadVector256(b + j + 8), Avx2.LoadVector256(c + j + 8)));
					}

					for (; j < N8; j += 8)
					{
						Avx2.Store(c + j + 0, Fma.MultiplyAdd(a, Avx2.LoadVector256(b + j + 0), Avx2.LoadVector256(c + j + 0)));
					}

					for (; j < N; ++j)
					{
						c[j] += aValue * b[j];
					}
				}
			}
		}
	}
}
