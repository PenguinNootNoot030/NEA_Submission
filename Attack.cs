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
        // Stores the result after attack is applied
        protected Bitmap attackedImage;

        /// <summary>
        /// Returns a human-readable name of the attack
        /// </summary>
        public abstract string GetAttackName();

        /// <summary>
        /// Applies the attack to the input image and returns the result
        /// </summary>
        public abstract Bitmap ApplyAttack(Bitmap image);

        /// <summary>
        /// Returns the attacked image (optional use, e.g., in GUI)
        /// </summary>
        public virtual Bitmap GetAttackedImage()
        {
            return attackedImage;
        }
    }
}

