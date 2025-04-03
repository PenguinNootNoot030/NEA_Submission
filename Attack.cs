using System;
using System.Drawing;
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
        protected Attack()
        {
            
        }

        // Abstract method to apply the attack
        public abstract Bitmap Apply(Bitmap bmp);
    }
}

