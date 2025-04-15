using System;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace Testing_WPF
{
    public class ImageRotation : Attack 
    {
        /*
        double angleInDegrees;
        double angleInRadians;
        double cos;
        double sin;
        Matrix rotationMatrix;
        public ImageRotation(double angleInDegrees) 
        {
            this.angleInDegrees = angleInDegrees;
            angleInRadians = angleInDegrees*Math.PI/180;
            cos = Math.Cos(angleInRadians);
            sin = Math.Sin(angleInRadians);
            rotationMatrix = new Matrix(new double[,] { { cos, -sin }, { sin, cos } });
        }

        public override Bitmap Apply(Bitmap bmp)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            double cx = width / 2;
            double cy = height / 2;

            int newWidth = (int)Math.Ceiling(Math.Abs(width * cos + Math.Abs(height * sin)));
            int newHeight = (int)Math.Ceiling(Math.Abs(width * sin) + Math.Abs(height * cos));
            double ncx = newWidth / 2.0;
            double ncy = newHeight / 2.0;

            // Build transformation matrices using homogeneous coordinates
            // T1: Translate the source image so its center is at the origin
            double[,] T1Array = new double[,] { { 1, 0, -cx }, { 0, 1, -cy }, { 0, 0, 1 } };
            Matrix T1 = new Matrix(T1Array);

            // R: Rotation matrix (counter-clockwise)
            double[,] RArray = new double[,] { { cos, -sin, 0 }, { sin, cos, 0 }, { 0, 0, 1 } };
            Matrix RMat = new Matrix(RArray);

            // T2: Translate back so that the rotated image is centered in the new canvas
            double[,] T2Array = new double[,] { { 1, 0, ncx }, { 0, 1, ncy }, { 0, 0, 1 } };
            Matrix T2 = new Matrix(T2Array);

            // Overall transformation matrix: T = T2 * R * T1
            Matrix T = T2 * RMat * T1;

            // For backward mapping we need the inverse transformation:
            Matrix TInv = T.Inverse();

            Bitmap result = new Bitmap(newWidth, newHeight);

            // For each pixel in the result image, determine the source pixel using TInv.
            for (int y = 0; y < newHeight; y++)
            {
                for (int x = 0; x < newWidth; x++)
                {
                    // Represent the destination coordinate as a 3x1 homogeneous vector [x; y; 1]
                    double[,] coordArray = new double[,] { { x }, { y }, { 1 } };
                    Matrix coord = new Matrix(coordArray);

                    // Multiply by the inverse transformation matrix to get the source coordinate.
                    Matrix srcCoord = TInv * coord;
                    double srcX = srcCoord.GetElement(0, 0);
                    double srcY = srcCoord.GetElement(1, 0);

                    // For simplicity, use nearest-neighbor interpolation.
                    int ix = (int)Math.Round(srcX);
                    int iy = (int)Math.Round(srcY);

                    if (ix >= 0 && ix < width && iy >= 0 && iy < height)
                    {
                        Color pixel = bmp.GetPixel(ix, iy);
                        result.SetPixel(x, y, pixel);
                    }
                    else
                    {
                        // Set transparent if outside the bounds of the source image.
                        result.SetPixel(x, y, Color.Transparent);
                    }
                }
            }
            return result;
        }
        */
        private float angle;

        /// <summary>
        /// Constructs a rotation attack with a specific angle in degrees
        /// </summary>
        public ImageRotation(float angleDegrees = 30f)
        {
            angle = ClampAngle(angleDegrees);
        }

        /// <summary>
        /// Apply rotation attack to image
        /// </summary>
        public override Bitmap ApplyAttack(Bitmap image)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));

            // Calculate new dimensions
            float radians = angle * (float)(Math.PI / 180.0);
            double sin = Math.Abs(Math.Sin(radians));
            double cos = Math.Abs(Math.Cos(radians));

            int newWidth = (int)(image.Width * cos + image.Height * sin);
            int newHeight = (int)(image.Width * sin + image.Height * cos);

            Bitmap rotated = new Bitmap(newWidth, newHeight, PixelFormat.Format32bppArgb);
            rotated.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (Graphics g = Graphics.FromImage(rotated))
            {
                g.Clear(Color.Transparent);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.TranslateTransform(newWidth / 2f, newHeight / 2f);
                g.RotateTransform(angle);
                g.TranslateTransform(-image.Width / 2f, -image.Height / 2f);
                g.DrawImage(image, new Point(0, 0));
            }

            attackedImage = rotated;
            return attackedImage;
        }

        /// <summary>
        /// Returns display name of this attack
        /// </summary>
        public override string GetAttackName()
        {
            return $"Image Rotation [{angle}°]";
        }

        private float ClampAngle(float angle)
        {
            if (angle < -360f) return -360f;
            if (angle > 360f) return 360f;
            return angle;
        }
    }
}

