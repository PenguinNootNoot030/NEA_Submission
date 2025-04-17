using System;
using System.Drawing;
namespace Testing_WPF
{
    public class ImageBlurring : Attack
    {
        private int kernelSize;
        private double blurSigma;

        public ImageBlurring(int size = 3, double blurSigma = 1.0)
        {
            kernelSize = ClampToOdd(size);
            this.blurSigma = ClmapSigma(blurSigma);
        }

        public override Bitmap ApplyAttack(Bitmap image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            attackedImage = ApplyGaussianBlur(image, kernelSize, blurSigma);
            return attackedImage;
        }

        public override string GetAttackName()
        {
            return $"Blurring [{kernelSize}x{kernelSize}], sigma = {blurSigma}";
        }

        private int ClampToOdd(int size)
        {
            if (size < 1) return 3;
            if (size % 2 == 0) size += 1; // Force odd
            if (size > 11) return 11;     // Safety limit
            return size;
        }

        private double ClmapSigma(double sigma)
        { 
            if (sigma < 0.1) { return 0.1; }
            if (sigma > 10) { return 10; }
            return sigma;
        }

        private Bitmap ApplyGaussianBlur(Bitmap src, int kernelSize, double sigma)
        {
            Bitmap blurred = new Bitmap(src.Width, src.Height);
            double[,] kernel = GenerateGaussianKernel(kernelSize, sigma);
            int radius = kernelSize / 2;

            for (int y = 0; y < src.Height; y++)
            {
                for (int x = 0; x < src.Width; x++)
                {
                    double r = 0, g = 0, b = 0;
                    double weightSum = 0;

                    for (int ky = -radius; ky <= radius; ky++)
                    {
                        for (int kx = -radius; kx <= radius; kx++)
                        {
                            int px = Clamp(x + kx, 0, src.Width - 1);
                            int py = Clamp(y + ky, 0, src.Height - 1);
                            Color pixel = src.GetPixel(px, py);

                            double weight = kernel[ky + radius, kx + radius];
                            r += pixel.R * weight;
                            g += pixel.G * weight;
                            b += pixel.B * weight;
                            weightSum += weight;
                        }
                    }

                    int rr = Clamp((int)(r / weightSum), 0, 255);
                    int gg = Clamp((int)(g / weightSum), 0, 255);
                    int bb = Clamp((int)(b / weightSum), 0, 255);

                    blurred.SetPixel(x, y, Color.FromArgb(rr, gg, bb));
                }
            }

            return blurred;
        }

        private double[,] GenerateGaussianKernel(int size, double sigma)
        {
            double[,] kernel = new double[size, size];
            int radius = size / 2;
            double sum = 0;
            double sigma2 = 2 * sigma * sigma;

            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    double value = Math.Exp(-(x * x + y * y) / sigma2);
                    kernel[y + radius, x + radius] = value;
                    sum += value;
                }
            }

            // Normalize
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                    kernel[y, x] /= sum;

            return kernel;
        }

        private int Clamp(int val, int min, int max)
        {
            if (val < min) return min;
            if (val > max) return max;
            return val;
        }
    }
}

