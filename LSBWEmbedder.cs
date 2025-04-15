using System;
namespace Testing_WPF
{
	public class LSBWEmbedder: Embedder
	{

        private string embedChannel; // "R", "G", "B", or "Y"

        public LSBWEmbedder(string channel = "B")
        {
            embedChannel = channel.ToUpper();
        }

        public override void EmbedWatermark(Watermark wm)
        {
            GenerateWatermark(wm);

            if (blocks == null)
                throw new InvalidOperationException("No block grid provided to LSBWEmbedder.");

            int rows = blocks.GetLength(0);
            int cols = blocks.GetLength(1);
            int bitIndex = 0;

            for (int i = 0; i < rows && bitIndex < bitstream.Count; i++)
            {
                for (int j = 0; j < cols && bitIndex < bitstream.Count; j++)
                {
                    Block block = blocks[i, j];

                    if (embedChannel == "Y")
                        block.UpdateYCbCr(); // If embedding in luminance

                    Matrix target = block.GetChannel(embedChannel);

                    for (int x = 0; x < blockSize && bitIndex < bitstream.Count; x++)
                    {
                        for (int y = 0; y < blockSize && bitIndex < bitstream.Count; y++)
                        {
                            double val = target.GetElement(x, y);
                            int intVal = (int)(val * 255);

                            if ((intVal % 2 == 0 && bitstream[bitIndex]) || (intVal % 2 == 1 && !bitstream[bitIndex]))
                                intVal += (bitstream[bitIndex] ? 1 : -1);

                            target.SetElement(x, y, Clamp(intVal / 255.0));
                            bitIndex++;
                        }
                    }

                    if (embedChannel == "Y")
                        block.UpdateARGB(); // Restore RGB from YCbCr

                    ReplaceBlock(block, i, j);
                }
            }

            if (bitIndex < bitstream.Count)
                throw new Exception($"Bitstream too long for image. Embedded {bitIndex}/{bitstream.Count} bits.");
        }

        private double Clamp(double val)
        {
            return Math.Min(1.0, Math.Max(0.0, val));
        }

        public override string GetEmbedderName() => $"LSBW Embedder [{embedChannel}]";

        public override bool SupportsColor()
        {
            return embedChannel != "Y";
        }
    }

}

