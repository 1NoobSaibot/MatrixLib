using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixAvxLib
{
  public static unsafe class MatrixMath
  {
    public static MatrixF Mul(MatrixF A, MatrixF B)
    {
      if (A.Width != B.Height)
      {
        var e = new ArgumentException("Ширина матрицы A должна равняться высоте матрицы B");
        e.Data.Add("Matrix A", A);
        e.Data.Add("Matrix B", B);
        throw e;
			}

      int M = A.Height;
      int N = B.Width;
      int K = A.Width;

      var res = new MatrixF(N, M);
      gemm_v1(M, N, K, (float*)A, (float*)B, (float*)res);

      return res;
    }

    private static void gemm_v1(int M, int N, int K, float* A, float* B, float* C)
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
	}
}
