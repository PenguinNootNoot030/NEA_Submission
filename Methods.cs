using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing_WPF
{
    public static class Methods
    {
        private static Dictionary<int, Matrix> DCTMatrixCache = new Dictionary<int, Matrix>();

        /// <summary>
        /// Generates and caches the DCT basis matrix of a given size
        /// </summary>
        public static Matrix GenerateDCTMatrix(int size)
        {
            if (DCTMatrixCache.ContainsKey(size))
                return DCTMatrixCache[size];

            Matrix dct = new Matrix(size, size);
            double c = Math.PI / (2.0 * size);

            for (int u = 0; u < size; u++)
            {
                for (int x = 0; x < size; x++)
                {
                    double factor = (u == 0) ? Math.Sqrt(1.0 / size) : Math.Sqrt(2.0 / size);
                    double value = factor * Math.Cos((2 * x + 1) * u * c);
                    dct.SetElement(u, x, value);
                }
            }

            DCTMatrixCache[size] = dct;
            return dct;
        }

        /// <summary>
        /// Applies DCT: D * M * D^T
        /// </summary>
        public static Matrix ApplyDCT(Matrix spatialBlock, Matrix dctBasis)
        {
            Matrix temp = dctBasis * spatialBlock;
            Matrix trans = dctBasis.TransposeMatrix(); // D^T
            return temp * trans;
        }

        /// <summary>
        /// Applies inverse DCT: D^T * F * D
        /// </summary>
        public static Matrix ApplyIDCT(Matrix freqBlock, Matrix dctBasis)
        {
            Matrix trans = dctBasis.TransposeMatrix(); // D^T
            Matrix temp = trans * freqBlock;
            return temp * dctBasis;
        }

        /// <summary>
        /// Applies DCT to a Block’s Y matrix
        /// </summary>
        public static void DCT(Block block)
        {
            int size = block.GetBlockSize();
            Matrix dctBasis = GenerateDCTMatrix(size);
            Matrix Y = block.GetChannel("Y");
            Matrix freq = ApplyDCT(Y, dctBasis);
            block.SetChannel("Y", freq);
        }

        /// <summary>
        /// Applies inverse DCT to a Block’s Y matrix
        /// </summary>
        public static void IDCT(Block block)
        {
            int size = block.GetBlockSize();
            Matrix dctBasis = GenerateDCTMatrix(size);
            Matrix freq = block.GetChannel("Y");
            Matrix spatial = ApplyIDCT(freq, dctBasis);
            block.SetChannel("Y", spatial);
        }
    }
}
