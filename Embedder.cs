using System;
namespace Testing_WPF
{
    //public abstract class Embedder
    public abstract class Embedder
    {
		protected byte[] watermarkBinary;
        protected Watermark watermark;
        
        public Embedder(Watermark watermark)
        {
            this.watermark = watermark;
            watermarkBinary = watermark.GetBinaryRepresentation();
        }
       
        public virtual void GenerateWatermark()
        { watermarkBinary = watermark.GetBinaryRepresentation(); }
		public virtual void EmbedWatermark()
        { }

    }
}

