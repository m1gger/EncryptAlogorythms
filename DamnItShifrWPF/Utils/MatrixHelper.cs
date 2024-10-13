using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DamnItShifrWPF.Utils
{
    public static class MatrixHelper
    {
        public static Matrix<double> Inverse(Matrix<double> matrix,int mod)
        {
            
            var outputMatrix = DenseMatrix.Build.DenseDiagonal(matrix.RowCount, matrix.ColumnCount, 0);
            var matrixDet = (int)Math.Round(matrix.Determinant());

            while (matrixDet < 0)
            {
                matrixDet += 26;
            }

            for (int i = 0; i < matrix.RowCount; i++)
            {
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    var tempMatrix = matrix.RemoveRow(i).RemoveColumn(j);
                    var insertedValue = Math.Pow(-1, i + j) * tempMatrix.Determinant();

                    while (insertedValue < 0)
                    {
                        insertedValue += mod;
                    }

                    while (insertedValue % matrixDet != 0)
                    {
                        insertedValue += mod;
                    }
                    outputMatrix.At(j, i, Math.Round(insertedValue / matrixDet) % 26);
                }
            }
            return (Matrix)outputMatrix;
        }

        // Метод для нахождения обратного элемента по модулю
        private static int ModularInverse(int a, int mod)
{
    a = a % mod;
    for (int x = 1; x < mod; x++)
    {
        if ((a * x) % mod == 1)
        {
            return x;
        }
    }
    throw new InvalidOperationException("Обратный элемент не существует.");
}


        public static bool CheckConstraints(Matrix<double> matrix,string alphabet)
        {
            var determinant = (long)Math.Round(matrix.Determinant(), 0);

            var firstCondition = determinant != 0;
            var secondCondition = Euclid.GreatestCommonDivisor(determinant, alphabet.Length) == 1;

            return firstCondition && secondCondition;
        }

        public static Matrix<double> GetRandomMatrix(int size)
        {
            if (size < 0 || size > 5) return null;

            var outputMatrix = DenseMatrix.CreateRandom(size, size, new Chi(100));

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    outputMatrix.At(i, j, Math.Round(outputMatrix.At(i, j), 0));
                }
            }

            return outputMatrix;
        }

        public static Matrix<double> GetMatrixFromString(string text,string alphabet)
        {
            var size = (int)Math.Sqrt(text.Length);
            // var size = int.Parse(Math.Sqrt(text.Length).ToString());
            var matrix = DenseMatrix.Build.DenseDiagonal(size, size, 0);

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    matrix.At(i, j, alphabet.IndexOf(text[i * size + j]));
                }
            }

            return matrix;
        }
    }
}
