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

			for (int i = 0; i < A.Length; i++)
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
			gemm_v1(Heigth, Width, K, (float*)A, (float*)B, (float*)res);

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

			gemm_v1(M, N, K, (float*)A, (float*)B, (float*)C);
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

			gemm_v1(M, N, K, (float*)A, (float*)B, (float*)C);
		}


		private static void gemm_v1(int M, int N, int K, float* A, float* B, float* C) {
			if ((N & 7) != 0)
			{
				gemm_v1_NU(M, N, K, A, B, C);
			}
			else if ((N & 15) != 0)
			{
				gemm_v2_N8(M, N, K, A, B, C);
			}
			else
			{
				gemm_v2_N16(M, N, K, A, B, C);
			}
		}


		private static void gemm_v1_NU(int M, int N, int K, float* A, float* B, float* C)
		{
			for (int i = 0; i < M; ++i)
			{
				float* c = C + i * N;

				for (int j = 0; j < N; ++j)
				{
					c[j] = 0;
				}

				for (int k = 0; k < K; ++k)
				{
					float* b = B + k * N;
					float a = A[i * K + k];
					for (int j = 0; j < N; ++j)
					{
						c[j] += a * b[j];
					}
				}
			}
		}


		/// <summary>
		/// TODO: Process Edge and make it Universal
		/// </summary>
		/// <param name="M"></param>
		/// <param name="N">N % 16 == 0</param>
		/// <param name="K"></param>
		/// <param name="A"></param>
		/// <param name="B"></param>
		/// <param name="C"></param>
		public static void gemm_v2_N16(int M, int N, int K, float* A, float* B, float* C)
		{
			Vector256<float> zero = Vector256.Create<float>(0);
			// NativeMemory.Fill(C, )

			for (int i = 0; i < M; ++i)
			{
				float* c = C + i * N;
				for (int j = 0; j < N; j += 8) {
					Avx2.Store(c + j, zero);
					// _mm256_storeu_ps(c + j, _mm256_setzero_ps());
				}

				for (int k = 0; k < K; ++k)
				{
					float* b = B + k * N;
					Vector256<float> a = Vector256.Create<float>(A[i * K + k]);
					// __m256 a = _mm256_set1_ps(A[i * K + k]);
					for (int j = 0; j < N; j += 16)
					{
						Avx2.Store(c + j + 0, Fma.MultiplyAdd(a, Avx2.LoadVector256(b + j + 0), Avx2.LoadVector256(c + j + 0)));
						// _mm256_storeu_ps(c + j + 0, _mm256_fmadd_ps(a, _mm256_loadu_ps(b + j + 0), _mm256_loadu_ps(c + j + 0)));
						Avx2.Store(c + j + 8, Fma.MultiplyAdd(a, Avx2.LoadVector256(b + j + 8), Avx2.LoadVector256(c + j + 8)));
						// _mm256_storeu_ps(c + j + 8, _mm256_fmadd_ps(a, _mm256_loadu_ps(b + j + 8), _mm256_loadu_ps(c + j + 8)));
					}
				}
			}
		}


		public static void gemm_v2_N8(int M, int N, int K, float* A, float* B, float* C)
		{
			Vector256<float> zero = Vector256.Create<float>(0);
			// NativeMemory.Fill(C, )

			for (int i = 0; i < M; ++i)
			{
				float* c = C + i * N;
				for (int j = 0; j < N; j += 8) {
					Avx2.Store(c + j, zero);
					// _mm256_storeu_ps(c + j, _mm256_setzero_ps());
				}

				for (int k = 0; k < K; ++k)
				{
					float* b = B + k * N;
					Vector256<float> a = Vector256.Create<float>(A[i * K + k]);
					// __m256 a = _mm256_set1_ps(A[i * K + k]);
					for (int j = 0; j < N; j += 8)
					{
						Avx2.Store(c + j + 0, Fma.MultiplyAdd(a, Avx2.LoadVector256(b + j + 0), Avx2.LoadVector256(c + j + 0)));
						// _mm256_storeu_ps(c + j + 0, _mm256_fmadd_ps(a, _mm256_loadu_ps(b + j + 0), _mm256_loadu_ps(c + j + 0)));
						// Avx2.Store(c + j + 8, Fma.MultiplyAdd(a, Avx2.LoadVector256(b + j + 8), Avx2.LoadVector256(c + j + 8)));
						// _mm256_storeu_ps(c + j + 8, _mm256_fmadd_ps(a, _mm256_loadu_ps(b + j + 8), _mm256_loadu_ps(c + j + 8)));
					}
				}
			}
		}
	}
}
