using System;
using System.Windows;
namespace Testing_WPF
{
	public class LSBWEmbedder: Embedder
	{

        private string embedChannel; // one of RGB

        public LSBWEmbedder(string channel)
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

            foreach (var (i, j) in traversalOrder)
            {
                Block block = blocks[i, j];

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

                    ReplaceBlock(block, i, j);
                
            }

            if (bitIndex < bitstream.Count)
                throw new Exception($"Bitstream too long for image. Embedded {bitIndex}/{bitstream.Count} bits.");
        }

        private double Clamp(double val)
        {
            return Math.Min(1.0, Math.Max(0.0, val));
        }

        public override string GetEmbedderName() => $"LSBW Embedder [{embedChannel}]";

        public override bool SupportsColour()
        {
            return embedChannel != "Y";
        }
    }

}

