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

      int M = A.Height;
      int N = B.Width;
      int K = A.Width;

      var res = new MatrixF(N, M);
      gemm_v1(M, N, K, (float*)A, (float*)B, (float*)res);

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
