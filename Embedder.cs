using System;
namespace Testing_WPF
{
    //public abstract class Embedder
    public abstract class Embedder
    {
        protected Watermark watermark;             // Original message
        protected List<bool> bitstream;            // Message in binary
        protected int blockSize;                   // Size of each block (e.g., 8)
        protected Block[,] blocks;                 // Full image in block form
        protected ImageHandling.QuadTreeNode treeRoot;
        protected List<(int row, int col)> traversalOrder;

        public Embedder() { }

        /// <summary>
        /// Sets the watermark and prepares the bitstream
        /// </summary>
        public virtual void GenerateWatermark(Watermark wm)
        {
            watermark = wm;
            bitstream = Watermark.GetBitStream();
        }

        /// <summary>
        /// Main method to override – embeds the watermark into the image
        /// </summary>
        public abstract void EmbedWatermark(Watermark watermark);

        /// <summary>
        /// Display name of the embedder (for GUI/logging purposes)
        /// </summary>
        public virtual string GetEmbedderName() => "Base Embedder";

        /// <summary>
        /// True if this embedder touches RGB channels, false if luminance-only
        /// </summary>
        public virtual bool SupportsColor() => false;

        /// <summary>
        /// Replaces a modified block back into the image grid
        /// </summary>
        protected void ReplaceBlock(Block newBlock, int rowIndex, int colIndex)
        {
            if (blocks != null &&
                rowIndex >= 0 && rowIndex < blocks.GetLength(0) &&
                colIndex >= 0 && colIndex < blocks.GetLength(1))
            {
                blocks[rowIndex, colIndex] = newBlock;
            }
            else
            {
                throw new ArgumentOutOfRangeException("Invalid block index during replacement.");
            }
        }

        /// <summary>
        /// Optional – Set the current working blocks (e.g., from ImageHandling)
        /// </summary>
        public virtual void SetBlockGrid(Block[,] blockGrid)
        {
            blocks = blockGrid;
            if (blocks != null && blocks.Length > 0)
            { blockSize = blocks[0, 0].GetBlockSize(); }
            ImageHandling handler = new ImageHandling();
            treeRoot = handler.BuildQuadTree(blocks);
            traversalOrder = handler.TraverseTreeDFS(treeRoot);
        }

        /// <summary>
        /// Gets the block grid after embedding (useful for output or testing)
        /// </summary>
        public virtual Block[,] GetBlockGrid()
        {
            return blocks;
        }
    }
}

