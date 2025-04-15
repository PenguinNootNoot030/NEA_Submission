using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing_WPF
{
    public class DCTExtractor : Extractor
    {
        public override string ExtractWatermark()
        {
            if (blocks == null || traversalOrder == null)
                throw new InvalidOperationException("Block grid not set.");

            List<bool> bitstream = new List<bool>();

            foreach (var (row, col) in traversalOrder)
            {
                Block block = blocks[row, col];
                block.UpdateYCbCr();
                Methods.DCT(block);

                Matrix Y = block.GetChannel("Y");
                int x = 4, y = 3;

                double val = Y.GetElement(x, y);
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
