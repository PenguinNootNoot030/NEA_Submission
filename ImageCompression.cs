using System;
namespace Testing_WPF
{
	public class ImageCompression: Attack
	{
        public ImageCompression(Matrix red, Matrix green, Matrix blue) : base(red, green, blue)
        {
        }

        public override void Apply()
        {
            throw new NotImplementedException();
        }
    }
}

