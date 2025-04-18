using System;
using System.Drawing;
namespace Testing_WPF
{
    public class ImageCropping : Attack
    {
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

