using System;
using System.Drawing;

namespace Testing_WPF
{

    //Todo: instead of returning 4 matrices 
    //Todo: Block/ImageMatrix class
    //added alpha back in - enable less significant changes with transparency
	public class ImageHandling
	{
        const int max_size = 1024;
        //using filepath, load image and return as a matrix
        public (Matrix, Matrix, Matrix, Matrix) LoadImageToMatrix(string filePath)
        {
            Bitmap bmp = new Bitmap(filePath);
            return ConvertImageToMatrix(bmp);
        }

        //convert a matrix back into an image
        public Bitmap ConvertMatrixToImage(Matrix source_Amat, Matrix source_Rmat, Matrix source_Gmat, Matrix source_Bmat)
        {
            int width = source_Amat.GetRow();
            int height = source_Amat.GetColumn();
            Bitmap res_bmp = new Bitmap(width, height);
            for (int i = 0; i < width; i++)
            { 
                for (int j = 0; j < width; j++)
                {
                    int alpha = (int)(source_Amat.GetElement(i, j) * 255);
                    int red = (int) (source_Rmat.GetElement(i, j) * 255);
                    int green = (int)(source_Gmat.GetElement(i, j) * 255);
                    int blue = (int)(source_Bmat.GetElement(i, j) * 255);
                    Color pixel = Color.FromArgb(alpha, red, green, blue);
                    res_bmp.SetPixel(i, j, pixel);
                }
            }
            return res_bmp;
        }
        public (Matrix,Matrix,Matrix,Matrix) ConvertImageToMatrix(Bitmap source_bmp)
        { 
            int rows = source_bmp.Width;
            int columns = source_bmp.Height;
            //Matrix res_mat = new Matrix(rows,columns);
            Matrix A_scale = new Matrix(rows,columns);
            Matrix R_scale = new Matrix(rows,columns);
            Matrix G_scale = new Matrix(rows,columns);
            Matrix B_scale = new Matrix(rows,columns);
            for (int i = 0; i< rows; i++)
            { 
                for (int j = 0; j < columns; j++)
                { 
                    Color pixel = source_bmp.GetPixel(i, j);
                    A_scale.SetElement(i, j, pixel.A / 255);
                    R_scale.SetElement(i, j, pixel.R / 255);
                    G_scale.SetElement(i, j, pixel.G / 255);
                    B_scale.SetElement(i, j, pixel.B / 255);
                }
            }
            return (A_scale, R_scale, G_scale, B_scale);
        }


        //debugging
        /*
        public void DisplayImage(Matrix source_Amat, Matrix source_Rmat, Matrix source_Gmat, Matrix source_Bmat)
        {
            ConvertMatrixToImage(source_Amat, source_Rmat, source_Gmat, source_Bmat);
        }
        */

        //save the image to a given filepath
        public void SaveImage(string filePath, Bitmap bmp) => bmp.Save(filePath);

        //Check if the image is still following the origianl/ provided resolution
        private bool ValidateImage(Bitmap bmp) => bmp.Width < max_size && bmp.Height < max_size;

	}
}

