using System;
namespace Testing_WPF
{
    public class DCTEmbedder : Embedder
    {
        private int coefRow;
        private int coefCol;
        private int strength;
        public DCTEmbedder(int coefRow = 4, int coefCol = 3, int strength = 1)
        {
            this.coefRow = coefRow;
            this.coefCol = coefCol;
            this.strength = strength;
        }

        public override void EmbedWatermark(Watermark wm)
        {
            // Prepare watermark and bitstream
            GenerateWatermark(wm);

            if (blocks == null)
                throw new InvalidOperationException("No block grid provided to DCTEmbedder.");

            int rows = blocks.GetLength(0);
            int cols = blocks.GetLength(1);
            int bitIndex = 0;

            for (int i = 0; i < rows && bitIndex < bitstream.Count; i++)
            {
                for (int j = 0; j < cols && bitIndex < bitstream.Count; j++)
                {
                    Block block = blocks[i, j];

                    // Update Y from RGB if needed
                    block.UpdateYCbCr();

                    // Apply DCT to the block
                    Methods.DCT(block);

                    // Embed a bit in a safe coefficient (e.g., (4,3))
                    EmbedBitInDCT(block, bitstream[bitIndex], coefRow, coefCol);
                    bitIndex++;

                    // Apply IDCT to return to spatial domain
                    Methods.IDCT(block);

                    // Update RGB from YCbCr
                    block.UpdateARGB();

                    // Save back the modified block
                    ReplaceBlock(block, i, j);
                }
            }

            if (bitIndex < bitstream.Count)
                throw new Exception($"Bitstream too long for image capacity. Embedded {bitIndex}/{bitstream.Count} bits.");
        }

        private void EmbedBitInDCT(Block block, bool bit, int row, int col)
        {
            Matrix Y = block.GetChannel("Y");
            double val = Y.GetElement(row, col);

            // Embed by adjusting LSB of the integer value
            int intVal = (int)Math.Round(val);
            if ((intVal % 2 == 0 && bit) || (intVal % 2 == 1 && !bit))
            { intVal += (bit ? strength : -strength); }

            Y.SetElement(row, col, intVal);
            /*
            if ((intVal % 2 == 0 && bit) || (intVal % 2 == 1 && !bit))
                intVal += (bit ? 1 : -1);

            Y.SetElement(row, col, intVal);
            */
        }

        public override string GetEmbedderName() => "DCT Embedder";

        public override bool SupportsColor() => false; // works on Y channel
    }
}

