using System;
namespace Testing_WPF
{
    //public abstract class Embedder
    public abstract class Embedder
    {
		protected byte[] watermarkBinary;
        protected Watermark watermark;
        
        public Embedder()
        {
        }
       
        public virtual void GenerateWatermark()
        { watermarkBinary = watermark.GetBinaryRepresentation(); }
		public virtual void EmbedWatermark(Watermark watermark)
        { }
        public void Embed(Embedder embedder, Watermark watermark)
        { embedder.EmbedWatermark(watermark); }
        // Replace a block in the original matrix
        private void ReplaceBlock(Block block, int rowStart, int colStart)
        {
            
        }
    }
}

