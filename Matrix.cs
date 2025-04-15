using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text;

namespace Testing_WPF
{
	//MAJOR FUNCTIONALITY COMPLETE AND TESTED
	//Todo: fix the submatrix() function!!!
    //test : 3*3 (same dimension) matrix multiplication & addition & transposition tested + worked
    //test : 2*4 and 4*2 (diff.dimension matrix) multiplication & transposition tested + worked
	
	//Todo: return row/column of the matrix 
    public class Matrix
	{
		int column;
		int row;
		double[,] matrix; //matrix = row x column
		double[,] t_matrix; //transposed matrix

		//constructor
		public Matrix(int row, int column)
		{
			this.column = column;
			this.row = row;
			matrix = new double[row, column];
			t_matrix = new double[column, row];
		}
		public Matrix(double[,] p_matrix)
		{
			row = p_matrix.GetLength(0);
			column = p_matrix.GetLength(1);
            matrix = new double[row, column] ;
            t_matrix = new double[column, row];
			matrix = p_matrix;
        }

		public int GetColumn() => column;
		public int GetRow() => row;
		public Matrix GetTMatrix() => new Matrix(t_matrix);

        public Matrix GetSubMatrix(int startRow, int startCol, int blockSize)
        {
            if (startRow + blockSize > row || startCol + blockSize > column)
                throw new ArgumentOutOfRangeException("Submatrix exceeds bounds.");

            Matrix sub = new Matrix(blockSize, blockSize);
            for (int i = 0; i < blockSize; i++)
            {
                for (int j = 0; j < blockSize; j++)
                {
                    double val = matrix[startRow + i, startCol + j];
                    sub.SetElement(i, j, val);
                }
            }
            return sub;
        }

        //return specific double element
        //Todo: Validate the row/column index
        public double GetElement(int rowIndex, int columnIndex)
			=> matrix[rowIndex, columnIndex];


        //Set double value to specific element
        //Todo: Validate the row/column index
        public void SetElement(int rowIndex, int columnIndex, double element)
			=> matrix[rowIndex, columnIndex] = element;

		public void Set2DArrayToMatrix(double[,] source)
		{
			for (int i = 0; i < source.GetLength(0); i++)
			{
				for (int j = 0; j < source.GetLength(1); j++)
				{ SetElement(i, j, source[i, j]); } 
			}
		}

		// create transpose of the given matrix : row becomes columns, columns becomes rows
		public void TransposeMatrix()
		{
			for (int i = 0; i < row; i++) //looping through the rows within the o_matrix
			{
				for (int j = 0; j < column; j++) //looping through the items on the rows of the o_matrix
				{
                    Console.WriteLine(matrix[i, j]);
                    t_matrix[j, i] = matrix[i, j];
				}
			}
		}
        public Matrix Inverse()
        {
            if (row != column)
                throw new InvalidOperationException("Inverse requires a square matrix.");

            int n = row;
            double[,] A = (double[,])matrix.Clone();
            double[,] I = new double[n, n];

            // Initialize identity matrix
            for (int i = 0; i < n; i++)
                I[i, i] = 1.0;

            for (int i = 0; i < n; i++)
            {
                double diag = A[i, i];
                if (Math.Abs(diag) < 1e-10)
                    throw new InvalidOperationException("Matrix is singular and cannot be inverted.");

                // Scale row to make pivot = 1
                for (int j = 0; j < n; j++)
                {
                    A[i, j] /= diag;
                    I[i, j] /= diag;
                }

                // Eliminate other rows
                for (int k = 0; k < n; k++)
                {
                    if (k == i) continue;

                    double factor = A[k, i];
                    for (int j = 0; j < n; j++)
                    {
                        A[k, j] -= factor * A[i, j];
                        I[k, j] -= factor * I[i, j];
                    }
                }
            }

            return new Matrix(I);
        }

        //Add matrix and return sum matrix
        public static Matrix operator + (Matrix a, Matrix b)
		{
			if (a.RowColumnValidatation(b, "+"))
			{
				Matrix result = new Matrix(a.GetRow(), a.GetColumn());
				for (int i = 0; i < a.GetRow(); i++)
				{
					for (int j = 0; j < a.GetColumn(); j++)
					{
						result.SetElement(i, j, a.GetElement(i, j) + b.GetElement(i, j));
					}
				}
				return result;
			}
			else throw new ArgumentException("False Action: dimension error");
			
		}

        //multiply two given matrix in form of AxB, return matrix
        public static Matrix operator *(Matrix a, Matrix b) 
        {
			double res_element = 0;
			if (a.RowColumnValidatation(b, "*"))
			{
                //int counter = 0;
                Matrix result = new Matrix(a.GetRow(), b.GetColumn());
				//each row has to repeat a.row or b.column times with the cumalative timsing
                for (int z = 0; z < b.GetColumn(); z++) //z indicates the current row on a 
                {
					for (int j = 0; j < b.GetColumn(); j++) //j indicates the current column of b
					{
                        for (int i = 0; i < a.GetColumn(); i++)  
                        {
                            res_element += a.GetElement(z, i) * b.GetElement(i, j);
                            Console.WriteLine($"{z},{i},{j}::{a.GetElement(z, i)},{b.GetElement(i,j)},{a.GetElement(z, i) * b.GetElement(i,j)}");
                        }
                        result.SetElement(z, j, res_element);
                        Console.WriteLine(res_element);
                        res_element = 0;
                    }
                }
                return result;
            }
            else throw new ArgumentException("False Action: dimension error");
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    sb.Append(matrix[i, j].ToString().PadLeft(4)); 
                }
                sb.AppendLine(); 
            }

            return sb.ToString();
        }

        //validate whether if the matrix mu;tiplication could take place.
        private bool RowColumnValidatation(Matrix suppliedMatrix, string action)
		{
			switch (action)
			{
				case "+":
					return (suppliedMatrix.GetColumn() == column)&&(suppliedMatrix.row == row);
				case "*":
					return suppliedMatrix.GetRow() == column; //default matrix * supplied matrix
				default:
					return false;
			}
		}
        public Matrix Clone()
        {
            Matrix clone = new Matrix(row, column);
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    clone.SetElement(i, j, matrix[i, j]);
                }
            }
            return clone;
        }
        
        public static Matrix Identity(int size)
        {
            Matrix identity = new Matrix(size, size);
            for (int i = 0; i < size; i++)
            {
                identity.SetElement(i, i, 1.0);
            }
            return identity;
        }
    }
}

