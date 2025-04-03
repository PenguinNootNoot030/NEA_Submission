using System;
namespace Testing_WPF
{
	public class LSBWEmbedder: Embedder
	{
        
        private string WatermarkMessage;
        private int BitPlane;
        
        public LSBWEmbedder(string message, int bitPlane)
        {
            WatermarkMessage = message;
            BitPlane = bitPlane;
        }

       
    }

}

