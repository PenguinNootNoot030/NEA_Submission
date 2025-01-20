using System;
namespace Testing_WPF
{
    //public abstract class Embedder
    public abstract class Embedder
    {
		public byte[] watermarkBinary;
		public Matrix ImageMatrix;
        /*
        public Embedder(Watermark watermark)
        {
            watermarkBinary = watermark.GetBinaryRepresentation();
        }
        */
        public virtual void GenerateWatermark()
        { }
		public virtual void EmbedWatermark()
        { }

    }
}

