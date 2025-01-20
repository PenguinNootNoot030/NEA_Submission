using System;
namespace Testing_WPF
{
	public class LSBWEmbedder: Embedder
	{
        private Matrix RedChannel;
        private Matrix GreenChannel;
        private Matrix BlueChannel;
        private string WatermarkMessage;
        private int BitPlane;
        
        public LSBWEmbedder(Matrix red, Matrix green, Matrix blue, string message, int bitPlane)
        {
            RedChannel = red;
            GreenChannel = green;
            BlueChannel = blue;
            WatermarkMessage = message;
            BitPlane = bitPlane;
        }

        /// <summary>
        /// Embed the watermark into the specified bit plane of the red channel
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void EmbedWatermark()
        {
            byte[] watermarkBits = ConvertMessageToBits(WatermarkMessage);
            int bitIndex = 0;
            int rows = RedChannel.GetRow();
            int cols = RedChannel.GetColumn();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (bitIndex >= watermarkBits.Length)
                        return; // Stop embedding if we've used all the bits

                    // Get the current pixel value
                    double pixelValue = RedChannel.GetElement(i, j);

                    // Convert the pixel value to an integer for bit manipulation
                    int pixelInt = (int)(pixelValue * 255.0);

                    // Modify the bit in the specified plane
                    int mask = ~(1 << BitPlane); // Mask to clear the target bit
                    pixelInt &= mask; // Clear the bit at BitPlane
                    pixelInt |= (watermarkBits[bitIndex] << BitPlane); // Set the new bit

                    // Update the pixel value back to the matrix
                    RedChannel.SetElement(i, j, pixelInt / 255.0);

                    // Move to the next bit in the watermark
                    bitIndex++;
                }
            }

            if (bitIndex < watermarkBits.Length)
            {
                throw new InvalidOperationException("The image is too small to hold the entire watermark.");
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

        // Getter for the watermarked red channel
        public Matrix GetWatermarkedRedChannel()
        {
            return RedChannel;
        }
    }
}

