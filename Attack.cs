using System;
namespace Testing_WPF
{
    //rotation
    //cropping

    //compression (technique)
    //blurring (visual)

    public abstract class Attack
	{
        // Attributes
        //public Block b;

        // Constructor
        protected Attack(Matrix red, Matrix green, Matrix blue)
        {
            
        }

        // Abstract method to apply the attack
        public abstract void Apply();

        // Get the attacked red channel
        

        // Utility method for clamping pixel values to [0, 1]
        protected double Clamp(double value)
        {
            return Math.Min(1.0, Math.Max(0.0, value));
        }


    }
}

