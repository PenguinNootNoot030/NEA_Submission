using System;
using System.Drawing;
namespace Testing_WPF
{
    public class ImageCropping : Attack
    {
        /*
        private readonly int cropX;
        private readonly int cropY;
        private readonly int cropWidth;
        private readonly int cropHeight;

        /// <summary>
        /// Initializes a new instance of CropAttack.
        /// </summary>
        /// <param name="cropX">X-coordinate of the top-left corner of the crop region.</param>
        /// <param name="cropY">Y-coordinate of the top-left corner of the crop region.</param>
        /// <param name="cropWidth">Width of the crop region.</param>
        /// <param name="cropHeight">Height of the crop region.</param>
        public ImageCropping(int cropX, int cropY, int cropWidth, int cropHeight)
        {
            this.cropX = cropX;
            this.cropY = cropY;
            this.cropWidth = cropWidth;
            this.cropHeight = cropHeight;
        }

        /// <summary>
        /// Applies the cropping operation on the source bitmap.
        /// </summary>
        /// <param name="source">The original bitmap to be cropped.</param>
        /// <returns>A new bitmap that is the cropped region of the source image.</returns>
        public override Bitmap Apply(Bitmap source)
        {
            Bitmap cropped = new Bitmap(cropWidth, cropHeight);

            for (int y = 0; y < cropHeight; y++)
            {
                for (int x = 0; x < cropWidth; x++)
                {
                    int srcX = cropX + x;
                    int srcY = cropY + y;
                    Color color;

                    // Check that the source coordinates are within the source image bounds.
                    if (srcX >= 0 && srcX < source.Width && srcY >= 0 && srcY < source.Height)
                    {
                        color = source.GetPixel(srcX, srcY);
                    }
                    else
                    {
                        // Use transparent pixel if the crop region exceeds the image boundaries.
                        color = Color.Transparent;
                    }

                    cropped.SetPixel(x, y, color);
                }
            }

            return cropped;
        }
        */
        private float cropRatio; // Between 0.0 and 0.5

        public ImageCropping(float ratio = 0.1f)
        {
            cropRatio = ClampRatio(ratio);
        }

        public override Bitmap ApplyAttack(Bitmap image)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));

            int width = image.Width;
            int height = image.Height;

            int cropX = (int)(width * cropRatio);
            int cropY = (int)(height * cropRatio);

            int newWidth = width - 2 * cropX;
            int newHeight = height - 2 * cropY;

            if (newWidth <= 0 || newHeight <= 0)
                throw new Exception("Cropping ratio too aggressive — results in zero-size image.");

            Rectangle cropRect = new Rectangle(cropX, cropY, newWidth, newHeight);
            Bitmap cropped = new Bitmap(newWidth, newHeight);

            using (Graphics g = Graphics.FromImage(cropped))
            {
                g.DrawImage(image, new Rectangle(0, 0, newWidth, newHeight), cropRect, GraphicsUnit.Pixel);
            }

            attackedImage = cropped;
            return attackedImage;
        }

        public override string GetAttackName()
        {
            return $"Image Cropping [{cropRatio * 100:F0}%]";
        }

        private float ClampRatio(float ratio)
        {
            if (ratio < 0f) return 0f;
            if (ratio > 0.45f) return 0.45f; // Safety limit
            return ratio;
        }
    }
}

