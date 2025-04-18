using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing_WPF
{
    public class LSBWExtractor : Extractor
    {
        private string channel;

        public LSBWExtractor(string targetChannel)
        {
            channel = targetChannel.ToUpper();
        }

        public override string ExtractWatermark()
        {
            if (blocks == null || traversalOrder == null)
                throw new InvalidOperationException("Block grid not set.");

            List<bool> bitstream = new List<bool>();

            foreach (var (row, col) in traversalOrder)
            {
                Block block = blocks[row, col];

                if (channel == "Y")
                    block.UpdateYCbCr();

                Matrix target = block.GetChannel(channel);

                for (int i = 0; i < blockSize; i++)
                {
                    for (int j = 0; j < blockSize; j++)
                    {
                        double val = target.GetElement(i, j);
                        int intVal = (int)(val * 255);
                        bool bit = (intVal % 2 != 0); // LSB = 1 → true
                        bitstream.Add(bit);
                    }
                }
            }

            return Watermark.ToMessage(bitstream);
        }

        public override string GetExtractorName()
        {
            return $"LSBW Extractor [{channel}]";
        }

        public override bool SupportsColor()
        {
            return channel != "Y";
        }
    }
}
