using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ConsoleApp1
{
    internal static class MatrixOperations
    {
        public static Matrix Transpose(Matrix matrix)
        {
            double[,] m = matrix.GetMatrix();
            double[,] t = new double[m.GetLength(1), m.GetLength(0)];
            for (int i = 0; i < m.GetLength(0); i++)
            {
                for (int j = 0; j < m.GetLength(1); j++)
                {
                    t[j, i] = m[i, j];
                }
            }
            return new Matrix(t);
        }

        public static Matrix MultiplyByNum(Matrix matrix, double n)
        {
            double[,] m = matrix.GetMatrix();
            for (int i = 0; i < m.GetLength(0); i++)
            {
                for (int j = 0; j < m.GetLength(1); j++)
                {
                    m[i, j] *= n;
                }
            }
            return new Matrix(m);
        }

        public static Matrix SumMatrices(Matrix matrix, Matrix matrix2)
        {
            if (matrix.r != matrix2.r || matrix.c != matrix2.c)
            {
                throw new Exception();
            }
            double[,] m = matrix.GetMatrix();
            double[,] m2 = matrix2.GetMatrix();
            for (int i = 0; i < m.GetLength(0); i++)
            {
                for (int j = 0; j < m.GetLength(1); j++)
                {
                    m[i, j] += m2[i, j];
                }
            }
            return new Matrix(m);
        }

        public static Matrix SubtractMatrices(Matrix matrix, Matrix matrix2)
        {
            if (matrix.r != matrix2.r || matrix.c != matrix2.c)
            {
                throw new Exception();
            }
            double[,] m = matrix.GetMatrix();
            double[,] m2 = matrix2.GetMatrix();
            for (int i = 0; i < m.GetLength(0); i++)
            {
                for (int j = 0; j < m.GetLength(1); j++)
                {
                    m[i, j] -= m2[i, j];
                }
            }
            return new Matrix(m);
        }

        public static Matrix DotMatrix(Matrix matrix, Matrix matrix2)
        {
            if (matrix.r != matrix2.r || matrix.c != matrix2.c)
            {
                throw new Exception();
            }
            double[,] m = new double[matrix.r, matrix2.r];
            double[,] m1 = matrix.GetMatrix();
            double[,] m2 = Transpose(matrix2).GetMatrix();

            Parallel.For(0, m1.GetLength(0),
                (i, loopState) =>
                {
                    for (int j = 0; j < m2.GetLength(1); j++)
                    {
                        double dotProduct = 0;
                        for (int k = 0; k < m1.GetLength(1); k++)
                        {
                            dotProduct += m1[i, k] * m2[k, j];
                        }
                        m[i, j] = dotProduct;
                    }
                }
            );

            return Transpose(new Matrix(m));
        }
    }
}
