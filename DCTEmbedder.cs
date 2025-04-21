using System;
using System.Windows;
namespace Testing_WPF
{
    public class DCTEmbedder : Embedder
    {
        private int coefRow;
        private int coefCol;
        private int strength;

        public DCTEmbedder(int coefRow, int coefCol, int strength)
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

            foreach (var(i,j) in traversalOrder)
            {
                if (bitIndex >= bitstream.Count) { break;}
                Block block = blocks[i, j];

                // Update Y from RGB 
                block.UpdateYCbCr();

                // Apply DCT to the block
                Methods.DCT(block);

                // Embed a bit 
                EmbedBitInDCT(ref block, bitstream[bitIndex], coefRow, coefCol);
                bitIndex++;

                // Apply IDCT to return to spatial domain
                Methods.IDCT(block);

                // Update RGB from YCbCr
                block.UpdateARGB();

                // Save back the modified block
                ReplaceBlock(block, i, j);
            }

            if (bitIndex < bitstream.Count)
                throw new Exception($"Bitstream too long for image capacity. Embedded {bitIndex}/{bitstream.Count} bits.");
        }

        private void EmbedBitInDCT(ref Block block, bool bit, int row, int col)
        {
            Matrix Y = block.GetChannel("Y");
            double val = Y.GetElement(row, col);

            // Embed by adjusting LSB of the integer value
            int intVal = (int)Math.Round(val);
            int parity = intVal % 2;
            int desiredPArity = bit ? 1 : 0; ;

            if (parity != desiredPArity)
            { //ensure stregth is odd to filp the parity bit
                int safeStrength = (strength%2 ==0)? strength +1 : strength;
                intVal += (bit ? safeStrength : -safeStrength);
            }

            Y.SetElement(row, col, intVal);
            block.SetChannel("Y", Y);
        }

        public override string GetEmbedderName() => "DCT Embedder";

        public override bool SupportsColour() => false; // works on Y channel
    }
}

