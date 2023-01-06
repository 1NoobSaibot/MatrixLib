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
			gemm(Heigth, Width, K, (float*)A, (float*)B, (float*)res);

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

			gemm(M, N, K, (float*)A, (float*)B, (float*)C);
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

			gemm(M, N, K, (float*)A, (float*)B, (float*)C);
		}


		public static void gemm(int Height, int Widht, int K, float* A, float* B, float* C)
		{
			// The commented method is slower than Universal
			// gemm_v3(Height, Widht, K, A, B, C);

			gemm_v2_Universal(Height, Widht, K, A, B, C);

			/* if (Height % 6 == 0 && Widht % 16 == 0)
			{
				gemm_v3(Height, Widht, K, A, B, C);
			}
			else
			{
				gemm_v2_Universal(Height, Widht, K, A, B, C);
			} */
		}


		public static void gemm_v2_Universal(int M, int N, int K, float* A, float* B, float* C)
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


		/// <param name="M">M % 6 == 0 !!!</param>
		/// <param name="N">N % 16 == 0 !!!</param>
		private static void gemm_v3(int M, int N, int K, float* A, float* B, float* C)
		{
			// init_c doesn't seem to be faster than Memory.Fill
			NativeMemory.Fill(C, (nuint)(M * N * sizeof(float)), 0);

			for (int i = 0; i < M; i += 6)
			{
				for (int j = 0; j < N; j += 16)
				{
					// init_c_6x16(C + i * N + j, N);
					micro_6x16(
						K,
						A + i * K,
						// K,
						// 1,
						B + j,
						N,
						C + i * N + j// ,
						// N
					);
				}
			}
		}


		private static void micro_6x16(
			int K,
			float* A,
			// int lda,
			// int step,
			float* B,
			int N,
			float* C //,
			// int ldc
		)
		{
			Vector256<float> c00 = Vector256.Create<float>(0);
			Vector256<float> c10 = Vector256.Create<float>(0);
			Vector256<float> c20 = Vector256.Create<float>(0);
			Vector256<float> c30 = Vector256.Create<float>(0);
			Vector256<float> c40 = Vector256.Create<float>(0);
			Vector256<float> c50 = Vector256.Create<float>(0);
			Vector256<float> c01 = Vector256.Create<float>(0);
			Vector256<float> c11 = Vector256.Create<float>(0);
			Vector256<float> c21 = Vector256.Create<float>(0);
			Vector256<float> c31 = Vector256.Create<float>(0);
			Vector256<float> c41 = Vector256.Create<float>(0);
			Vector256<float> c51 = Vector256.Create<float>(0);//

			int offset0 = K * 0;
			int offset1 = K * 1;
			int offset2 = K * 2;
			int offset3 = K * 3;
			int offset4 = K * 4;
			int offset5 = K * 5;

			Vector256<float>b0, b1, a0, a1;
			for (int k = 0; k < K; k++)
			{
				b0 = Avx2.LoadVector256(B + 0);
				b1 = Avx2.LoadVector256(B + 8);

				a0 = Vector256.Create<float>(A[offset0]);
				a1 = Vector256.Create<float>(A[offset1]);
				c00 = Fma.MultiplyAdd(a0, b0, c00);
				c01 = Fma.MultiplyAdd(a0, b1, c01);
				c10 = Fma.MultiplyAdd(a1, b0, c10);
				c11 = Fma.MultiplyAdd(a1, b1, c11);

				a0 = Vector256.Create<float>(A[offset2]);
				a1 = Vector256.Create<float>(A[offset3]);
				c20 = Fma.MultiplyAdd(a0, b0, c20);
				c21 = Fma.MultiplyAdd(a0, b1, c21);
				c30 = Fma.MultiplyAdd(a1, b0, c30);
				c31 = Fma.MultiplyAdd(a1, b1, c31);

				a0 = Vector256.Create<float>(A[offset4]);
				a1 = Vector256.Create<float>(A[offset5]);
				c40 = Fma.MultiplyAdd(a0, b0, c40);
				c41 = Fma.MultiplyAdd(a0, b1, c41);
				c50 = Fma.MultiplyAdd(a1, b0, c50);
				c51 = Fma.MultiplyAdd(a1, b1, c51);

				B += N; A += 1;// A += step;
			}

			Avx2.Store(C + 0, Avx2.Add(c00, Avx2.LoadVector256(C + 0)));
			Avx2.Store(C + 8, Avx2.Add(c01, Avx2.LoadVector256(C + 8)));
			C += N;
			Avx2.Store(C + 0, Avx2.Add(c10, Avx2.LoadVector256(C + 0)));
			Avx2.Store(C + 8, Avx2.Add(c11, Avx2.LoadVector256(C + 8)));
			C += N;
			Avx2.Store(C + 0, Avx2.Add(c20, Avx2.LoadVector256(C + 0)));
			Avx2.Store(C + 8, Avx2.Add(c21, Avx2.LoadVector256(C + 8)));
			C += N;
			Avx2.Store(C + 0, Avx2.Add(c30, Avx2.LoadVector256(C + 0)));
			Avx2.Store(C + 8, Avx2.Add(c31, Avx2.LoadVector256(C + 8)));
			C += N;
			Avx2.Store(C + 0, Avx2.Add(c40, Avx2.LoadVector256(C + 0)));
			Avx2.Store(C + 8, Avx2.Add(c41, Avx2.LoadVector256(C + 8)));
			C += N;
			Avx2.Store(C + 0, Avx2.Add(c50, Avx2.LoadVector256(C + 0)));
			Avx2.Store(C + 8, Avx2.Add(c51, Avx2.LoadVector256(C + 8)));
		}


		private static void init_c_6x16(float* C, int ldc)
		{
			for (int i = 0; i < 6; ++i, C += ldc)
			{
				for (int j = 0; j < 16; j += 8)
				{
					Avx2.Store(C + j, Vector256.Create<float>(0));
				}
			}
		}
	}
}
