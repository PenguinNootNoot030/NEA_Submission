using System;
namespace Testing_WPF
{
	public class DCTEmbedder: Embedder
	{

        private Block block;
        private Watermark watermark;
        private int BitPlane;
        private int BlockSize;
        private double EmbeddingStrength;

        // Constructor
        public DCTEmbedder(): base()
        {
            
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
        public Block ApplyDCT()
        {
            Methods.DCT(block);
            return block; // Placeholder
        }

        // Apply the Inverse Discrete Cosine Transform to a block
        public Block ApplyIDCT()
        {
            Methods.IDCT(block);
            return block; // Placeholder
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

