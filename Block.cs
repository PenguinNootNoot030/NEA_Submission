using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Testing_WPF
{
    public class Block
    {
        private int BlockSize;

        private Matrix A, R, G, B;
        private Matrix Y, Cb, Cr;

        // Constructor from ARGB
        public Block(Matrix a, Matrix r, Matrix g, Matrix b)
        {
            A = a;
            R = r;
            G = g;
            B = b;
            BlockSize = a.GetRow(); // assuming square blocks
            ARGB_YCbCr_Converter();
        }

        // Constructor from YCbCr (optional)
        public Block(Matrix y, Matrix cb, Matrix cr)
        {
            Y = y;
            Cb = cb;
            Cr = cr;
            BlockSize = y.GetRow();
            YCbCr_ARGB_Converter();
        }

        // Empty block
        public Block(int size)
        {
            BlockSize = size;
            A = new Matrix(size, size);
            R = new Matrix(size, size);
            G = new Matrix(size, size);
            B = new Matrix(size, size);
            Y = new Matrix(size, size);
            Cb = new Matrix(size, size);
            Cr = new Matrix(size, size);
        }

        // Convert ARGB → YCbCr
        public void ARGB_YCbCr_Converter()
        {
            
            Y = new Matrix(BlockSize, BlockSize);
            Cb = new Matrix(BlockSize, BlockSize);
            Cr = new Matrix(BlockSize, BlockSize);
            

            for (int i = 0; i < BlockSize; i++)
            {
                for (int j = 0; j < BlockSize; j++)
                {
                    double r = R.GetElement(i, j);
                    double g = G.GetElement(i, j);
                    double b = B.GetElement(i, j);
                    /*
                    double y = 0.257 * r + 0.564 * g + 0.098 * b + 16 / 255.0;//0.299 * r + 0.587 * g + 0.114 * b;
                    double cb = -0.148 * r - 0.291 * g + 0.439 * b + 128 / 255.0;//-0.168736 * r - 0.331264 * g + 0.5 * b + 0.5;
                    double cr = 0.439 * r - 0.368 * g - 0.071 * b + 128 / 255.0;//0.5 * r - 0.418688 * g - 0.081312 * b + 0.5;
                    */
                    double y =0.299 * r + 0.587 * g + 0.114 * b;
                    double cb = -0.168736 * r - 0.331264 * g + 0.5 * b + 0.5;
                    double cr = 0.5 * r - 0.418688 * g - 0.081312 * b + 0.5;

                    Y.SetElement(i, j, Clamp(y));
                    Cb.SetElement(i, j, Clamp(cb));
                    Cr.SetElement(i, j, Clamp(cr));
                }
            }
        }

        // Convert YCbCr → ARGB
        public void YCbCr_ARGB_Converter()
        {
            
            R = new Matrix(BlockSize, BlockSize);
            G = new Matrix(BlockSize, BlockSize);
            B = new Matrix(BlockSize, BlockSize);
            

            for (int i = 0; i < BlockSize; i++)
            {
                for (int j = 0; j < BlockSize; j++)
                {
                    double y = Y.GetElement(i, j);  
                    double cb = Cb.GetElement(i, j);
                    double cr = Cr.GetElement(i, j);

                    /*
                    double r = 1.164 * (y - 16 / 255.0) + 1.596 * (cr - 128 / 255.0);//y + 1.403 * cr;
                    double g = 1.164 * (y - 16 / 255.0) - 0.392 * (cb - 128 / 255.0) - 0.813 * (cr - 128 / 255.0); //y - 0.344 * cb - 0.714 * cr;
                    double b = 1.164 * (y - 16 / 255.0) + 2.017 * (cb - 128 / 255.0);//y + 1.772 * cb;
                    */
                    double r = y + 1.403 * (cr - 0.5);
                    double g = y - 0.344 * (cb - 0.5) - 0.714 * (cr - 0.5);
                    double b = y + 1.772 * (cb - 0.5);

                    R.SetElement(i, j, Clamp(r));
                    G.SetElement(i, j, Clamp(g));
                    B.SetElement(i, j, Clamp(b));
                }
            }
        }

        // Sync YCbCr from ARGB
        public void UpdateYCbCr() => ARGB_YCbCr_Converter();

        // Sync ARGB from YCbCr
        public void UpdateARGB() => YCbCr_ARGB_Converter();

        // Channel Accessor
        public Matrix GetChannel(string channel)
        {
            return channel.ToUpper() switch
            {
                "A" => A,
                "R" => R,
                "G" => G,
                "B" => B,
                "Y" => Y,
                "CB" => Cb,
                "CR" => Cr,
                _ => throw new ArgumentException($"Invalid channel: {channel}")
            };
        }

        // Channel Mutator
        public void SetChannel(string channel, Matrix matrix)
        {
            switch (channel.ToUpper())
            {
                case "A": A = matrix; break;
                case "R": R = matrix; break;
                case "G": G = matrix; break;
                case "B": B = matrix; break;
                case "Y": Y = matrix; break;
                case "CB": Cb = matrix; break;
                case "CR": Cr = matrix; break;
                default: throw new ArgumentException($"Invalid channel: {channel}");
            }
        }

        public int GetBlockSize() => BlockSize;

        // Deep copy
        public Block Clone()
        {
            return new Block(
                A.Clone(), R.Clone(), G.Clone(), B.Clone()
            );
        }

        public override string ToString()
        {
            return $"Block Size: {BlockSize}x{BlockSize}";
        }

        private double Clamp(double val)
        {
            return Math.Max(0.0, Math.Min(1.0, val));
        }
        /*
        private void ClampYChannel()
        {
            for (int i = 0; i < BlockSize; i++) 
            {
                for (int j = 0; j < BlockSize; j++) 
                { double val = Y.GetElement(i, j);
                    val = Math.Max(0.0, Math.Min(1.0, val));
                    Y.SetElement(i, j, val); } 
            }
        }
        */
    }
}
