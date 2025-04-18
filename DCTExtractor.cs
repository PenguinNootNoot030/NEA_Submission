using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing_WPF
{
    public class DCTExtractor : Extractor
    {
        private int coefRow;
        private int coefCol;
        private int strength;

        public DCTExtractor(int coefRow, int coefCol, int strength)
        {
            this.coefRow = coefRow;
            this.coefCol = coefCol;
            this.strength = strength;
        }

        public override string ExtractWatermark()
        {
            if (blocks == null || traversalOrder == null)
                throw new InvalidOperationException("Block grid not set.");

            List<bool> bitstream = new List<bool>();
            int size = blocks[0,0].GetBlockSize();
            Matrix dctBasis = Methods.GenerateDCTMatrix(size);

            foreach (var (row, col) in traversalOrder)
            {
                Block block = blocks[row, col];
                block.UpdateYCbCr();
                
                Matrix Y = block.GetChannel("Y");
                Matrix freq = Methods.ApplyDCT(Y, dctBasis);
                
                double val = freq.GetElement(coefRow, coefCol);
                int intVal = (int)Math.Round(val);
                bool bit = (intVal % 2 != 0); // parity: 1 = odd = true

                bitstream.Add(bit);
            }

            return Watermark.ToMessage(bitstream);
        }

        public override string GetExtractorName()
        {
            return "DCT Extractor";
        }

        public override bool SupportsColor()
        {
            return false; // only operates on luminance
        }
    }
}
