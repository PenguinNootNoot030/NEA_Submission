using System;
namespace Testing_WPF
{
	//rotation
	//cropping
	//blurring
	//lossy compression
	public abstract class Attack
	{
        // Attributes
        protected Matrix RedChannel;
        protected Matrix GreenChannel;
        protected Matrix BlueChannel;

        // Constructor
        protected Attack(Matrix red, Matrix green, Matrix blue)
        {
            RedChannel = red;
            GreenChannel = green;
            BlueChannel = blue;
        }

        // Abstract method to apply the attack
        public abstract void Apply();

        // Get the attacked red channel
        public Matrix GetRedChannel()
        {
            return RedChannel;
        }

        // Get the attacked green channel
        public Matrix GetGreenChannel()
        {
            return GreenChannel;
        }

        // Get the attacked blue channel
        public Matrix GetBlueChannel()
        {
            return BlueChannel;
        }

        // Utility method for clamping pixel values to [0, 1]
        protected double Clamp(double value)
        {
            return Math.Min(1.0, Math.Max(0.0, value));
        }


    }
}

