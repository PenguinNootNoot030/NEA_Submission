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

            foreach (var(i,j) in traversalOrder)
            {
                if (bitIndex >= bitstream.Count) { break;}
                Block block = blocks[i, j];

                // Update Y from RGB if needed
                Matrix tr1 = block.GetChannel("R").Clone();
                Matrix tg1 = block.GetChannel("G").Clone();
                Matrix tb1 = block.GetChannel("B").Clone();

                block.UpdateYCbCr();

                // Apply DCT to the block
                Methods.DCT(block);

                // Embed a bit in a safe coefficient (e.g., (4,3))
                EmbedBitInDCT(ref block, bitstream[bitIndex], coefRow, coefCol);
                bitIndex++;

                // Apply IDCT to return to spatial domain
                Methods.IDCT(block);

                // Update RGB from YCbCr
                block.UpdateARGB();
                Matrix tr2 = block.GetChannel("R").Clone();
                tr2.SMultiply(-1.0);
                Matrix tg2 = block.GetChannel("G").Clone();
                tg2.SMultiply(-1.0);
                Matrix tb2= block.GetChannel("B").Clone();
                tb2.SMultiply(-1.0);
                tr1=tr1+ tr2;
                tg1=tg1+ tg2;
                tb1=tb1+ tb2;

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
            if ((intVal % 2 == 0 && bit) || (intVal % 2 == 1 && !bit))
            { intVal += (bit ? strength : -strength); }

            Y.SetElement(row, col, intVal);
            block.SetChannel("Y", Y);
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

