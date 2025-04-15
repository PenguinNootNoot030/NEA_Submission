using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing_WPF
{
    public abstract class Extractor
    {
        protected Block[,] blocks;
        protected int blockSize;
        protected ImageHandling.QuadTreeNode treeRoot;
        protected List<(int row, int col)> traversalOrder;

        public void SetBlockGrid(Block[,] inputBlocks)
        {
            if (inputBlocks == null)
                throw new ArgumentNullException(nameof(inputBlocks));

            blocks = inputBlocks;
            blockSize = blocks[0, 0].GetBlockSize();

            ImageHandling handler = new ImageHandling();
            treeRoot = handler.BuildQuadTree(blocks);
            traversalOrder = handler.TraverseTreeDFS(treeRoot);
        }

        /// <summary>
        /// Extracts and decodes the watermark message from the image
        /// </summary>
        public abstract string ExtractWatermark();

        /// <summary>
        /// Returns extractor type (for display/logging)
        /// </summary>
        public abstract string GetExtractorName();

        /// <summary>
        /// Indicates whether this method works with full-color images
        /// </summary>
        public abstract bool SupportsColor();
    }
}
