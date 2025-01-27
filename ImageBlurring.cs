using System;
namespace Testing_WPF
{
    public class ImageBlurring : Attack
    {
        public ImageBlurring(Matrix red, Matrix green, Matrix blue) : base(red, green, blue)
        {
        }

        public override void Apply()
        {
            throw new NotImplementedException();
        }

       
    }
}

