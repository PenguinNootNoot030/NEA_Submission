using System;
using System.Drawing;
namespace Testing_WPF
{
	public class ImageCompression: Attack
	{
        private float compressionStrength; // 0 = no loss, higher = more info lost
        private int blockSize = 8; // JPEG-style DCT blocks

        public ImageCompression(float strength = 2.0f)
        {
            compressionStrength = Clamp(strength, 0.1f, 10f);
        }

        public override Bitmap ApplyAttack(Bitmap image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            ImageHandling handler = new ImageHandling();
            Bitmap trimmed = handler.TrimToBlockSize(image, blockSize);
            Matrix[] argb = Matrix.FromBitmap(trimmed); // A, R, G, B

            Block[,] blocks = handler.SplitToBlocks(argb[0], argb[1], argb[2], argb[3], blockSize);

            int rows = blocks.GetLength(0);
            int cols = blocks.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Block block = blocks[i, j];

                    // Convert RGB → YCbCr
                    block.UpdateYCbCr();

                    // Apply DCT on Y channel
                    Methods.DCT(block);
                    Matrix Y = block.GetChannel("Y");

                    // Quantize: zero out small coefficients
                    ApplyLossyQuantization(Y, compressionStrength);

                    // Restore spatial domain
                    Methods.IDCT(block);
                    block.UpdateARGB();

                    blocks[i, j] = block;
                }
            }

            attackedImage = handler.MergeBlocksToImage(blocks);
            return attackedImage;
        }

        public override string GetAttackName()
        {
            return $"DCT Compression [Strength {compressionStrength:F1}]";
        }

        private void ApplyLossyQuantization(Matrix freq, float strength)
        {
            int size = freq.GetRow();
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    double val = freq.GetElement(x, y);
                    if (Math.Abs(val) < strength)
                        freq.SetElement(x, y, 0.0); // Drop low-magnitude components
                    else
                        freq.SetElement(x, y, Math.Round(val)); // Optional rounding for realism
                }
            }
        }

        private float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}

