using System;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Factorization;

namespace Program
{
    [Serializable]
    public class Matrix
    {
        public int nRow;
        public int nCol;
        public double[] W;
        public double[] gradW;
        public double[] rmPropStepCache;
        public double[] rmPropStepCache2;
        public double[] rmPropStepCache3;

        public Matrix(int dim)
        {
            this.nRow = dim;
            this.nCol = 1;
            this.W = new double[nRow * nCol];
            this.gradW = new double[nRow * nCol];
            this.rmPropStepCache = new double[nRow * nCol];
            this.rmPropStepCache2 = new double[nRow * nCol];
            this.rmPropStepCache3 = new double[nRow * nCol];
        }

        public Matrix(int rows, int cols)
        {
            this.nRow = rows;
            this.nCol = cols;
            this.W = new double[rows * cols];
            this.gradW = new double[rows * cols];
            this.rmPropStepCache = new double[rows * cols];
            this.rmPropStepCache2 = new double[nRow * nCol];
            this.rmPropStepCache3 = new double[nRow * nCol];
        }

        public Matrix(double[] vector)
        {
            this.nRow = vector.Length;
            this.nCol = 1;
            this.W = vector;
            this.gradW = new double[vector.Length];
            this.rmPropStepCache = new double[vector.Length];
            this.rmPropStepCache2 = new double[vector.Length];
            this.rmPropStepCache3 = new double[vector.Length];
        }


        public static Matrix newMatrix_random(int rows, int cols, double upbound)
        {
            Matrix result = new Matrix(rows, cols);
            for (int i = 0; i < result.W.Length; i++)
            {
                result.W[i] = Global.randn.Sample() * upbound;
            }
            return result;
        }
        public static Matrix newMatrix_randomorthor(int rows, int cols, double upbound)
        {
            Matrix result = new Matrix(rows, cols);
            double[,] xdata = new double[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    xdata[i, j] = Global.randn.Sample() * upbound;
                }
            }
            DenseMatrix storage = DenseMatrix.OfArray(xdata);

            xdata = storage.Svd().U.ToArray();  //Thse method Svd() doesn't seem to be available in v2.4 
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result.W[i * cols + j] = xdata[i, j];
                }
            }

            return result;
        }

        public static Matrix newMatrix_val(int rows, int cols, double v)
        {
            Matrix result = new Matrix(rows, cols);
            for (int i = 0; i < result.W.Length; i++)
            {
                result.W[i] = v;
            }
            return result;
        }

        public static Matrix newMatrix_1(int rows, int cols)
        {
            return newMatrix_val(rows, cols, 1.0);
        }

        public static Matrix newMatrix_0(int rows, int cols)
        {
            return new Matrix(rows, cols);
        }



    }
}
