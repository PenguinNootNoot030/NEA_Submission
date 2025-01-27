using System;
namespace Testing_WPF
{
	public class DCTEmbedder: Embedder
	{

        private Matrix RedChannel;
        private Matrix GreenChannel;
        private Matrix BlueChannel;
        private string WatermarkMessage;
        private int BitPlane;
        private int BlockSize;
        private double EmbeddingStrength;

        // Constructor
        public DCTEmbedder(Matrix red, Matrix green, Matrix blue, string message, int blockSize, double strength)
            //: base(red, green, blue, message)
        {
            BlockSize = blockSize;
            EmbeddingStrength = strength;
        }

        public override void EmbedWatermark()
        {
            byte[] watermarkBits = ConvertMessageToBits(WatermarkMessage);
            int bitIndex = 0;

            List<Matrix> redBlocks = SplitIntoBlocks(RedChannel, BlockSize);

            foreach (var block in redBlocks)
            {
                if (bitIndex >= watermarkBits.Length)
                    break; // Stop embedding when all bits are used

                // Apply DCT to the block
                Matrix dctBlock = ApplyDCT(block);

                // Embed a single bit in the DCT coefficients
                EmbedBitInDCT(dctBlock, watermarkBits[bitIndex]);

                // Apply inverse DCT to reconstruct the block
                Matrix idctBlock = ApplyIDCT(dctBlock);

                // Replace the block in the original matrix
                ReplaceBlock(RedChannel, idctBlock, block.GetRow(), block.GetColumn());

                bitIndex++;
            }

            if (bitIndex < watermarkBits.Length)
            {
                throw new InvalidOperationException("The image is too small to hold the entire watermark.");
            }
        }

        // Embed a single bit into the DCT coefficients
        private void EmbedBitInDCT(Matrix dctBlock, byte bit)
        {
            // Choose a coefficient to modify (e.g., the second-lowest frequency)
            int row = 1;
            int col = 1;

            double coefficient = dctBlock.GetElement(row, col);

            // Modify the coefficient based on the bit
            if (bit == 1)
            {
                coefficient += EmbeddingStrength;
            }
            else
            {
                coefficient -= EmbeddingStrength;
            }

            dctBlock.SetElement(row, col, coefficient);
        }

        // Apply the Discrete Cosine Transform to a block
        private Matrix ApplyDCT(Matrix block)
        {
            // Implement or call a DCT algorithm here
            return block; // Placeholder
        }

        // Apply the Inverse Discrete Cosine Transform to a block
        private Matrix ApplyIDCT(Matrix block)
        {
            // Implement or call an IDCT algorithm here
            return block; // Placeholder
        }

        // Split a matrix into smaller blocks
        private List<Matrix> SplitIntoBlocks(Matrix matrix, int blockSize)
        {
            List<Matrix> blocks = new List<Matrix>();
            int rows = matrix.GetRow();
            int cols = matrix.GetRow();

            for (int i = 0; i < rows; i += blockSize)
            {
                for (int j = 0; j < cols; j += blockSize)
                {
                    Matrix block = matrix.GetSubMatrix(i, j, blockSize);
                    blocks.Add(block);
                }
            }

            return blocks;
        }

        // Replace a block in the original matrix
        private void ReplaceBlock(Matrix matrix, Matrix block, int rowStart, int colStart)
        {
            int blockRows = block.GetRow();
            int blockCols = block.GetColumn();

            for (int i = 0; i < blockRows; i++)
            {
                for (int j = 0; j < blockCols; j++)
                {
                    matrix.SetElement(rowStart + i, colStart + j, block.GetElement(i, j));
                }
            }
        }

        // Convert a string message to an array of bits
        private byte[] ConvertMessageToBits(string message)
        {
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(message);
            byte[] bits = new byte[bytes.Length * 8];

            for (int i = 0; i < bytes.Length; i++)
            {
                for (int bit = 0; bit < 8; bit++)
                {
                    bits[i * 8 + bit] = (byte)((bytes[i] >> bit) & 1);
                }
            }

            return bits;
        }


    }
}

