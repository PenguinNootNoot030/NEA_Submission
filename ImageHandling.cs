using System;
using System.Drawing;

namespace Testing_WPF
{
    public class ImageHandling
    {
        const int max_size = 1024;
        public Block[,] blocks;
        
        public Block ConvertImageToMatrix(Bitmap source_bmp)
        {
            int width = source_bmp.Width;
            int height = source_bmp.Height;
            Matrix A_scale = new Matrix(height, width);
            Matrix R_scale = new Matrix(height, width);
            Matrix G_scale = new Matrix(height, width);
            Matrix B_scale = new Matrix(height, width);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Color pixel = source_bmp.GetPixel(j, i);
                    A_scale.SetElement(i, j, pixel.A / 255.0);
                    R_scale.SetElement(i, j, pixel.R / 255.0);
                    G_scale.SetElement(i, j, pixel.G / 255.0);
                    B_scale.SetElement(i, j, pixel.B / 255.0);
                }
            }
            Block imageBlock = new Block(A_scale, R_scale, G_scale, B_scale);
            return imageBlock;
        }

        private int ClampToByte(double value)
        { int v = (int)Math.Round(value * 255.0);
            return Math.Max(0, Math.Min(255, v));
        }


        //Check if the image is still following the origianl/ provided resolution
        private bool ValidateImage(Bitmap bmp) => bmp.Width < max_size && bmp.Height < max_size;

        // Trims the image into sizes that can perfectly divides into identical blocks of the given size
        public Bitmap TrimToBlockSize(Bitmap source, int blockSize)
        {
            int newWidth = (source.Width / blockSize) * blockSize;
            int newHeight = (source.Height / blockSize) * blockSize;

            Bitmap trimmed = new Bitmap(newWidth, newHeight);
            for (int y = 0; y < newHeight; y++)
            {
                for (int x = 0; x < newWidth; x++)
                {
                    trimmed.SetPixel(x, y, source.GetPixel(x, y));
                }
            }
            return trimmed;
        }

        public Block[,] SplitToBlocks(Matrix A, Matrix R, Matrix G, Matrix B, int blockSize)
        {
            int height = A.GetRow();
            int width = A.GetColumn();

            int rows = height / blockSize;
            int cols = width / blockSize;

            blocks = new Block[rows, cols];

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    Matrix blockA = A.GetSubMatrix(row * blockSize, col * blockSize, blockSize);
                    Matrix blockR = R.GetSubMatrix(row * blockSize, col * blockSize, blockSize);
                    Matrix blockG = G.GetSubMatrix(row * blockSize, col * blockSize, blockSize);
                    Matrix blockB = B.GetSubMatrix(row * blockSize, col * blockSize, blockSize);

                    blocks[row, col] = new Block(blockA, blockR, blockG, blockB);
                }
            }
            return blocks;
        }

        public Bitmap MergeBlocksToImage(Block[,] blocks)
        {
            int rows = blocks.GetLength(0);
            int cols = blocks.GetLength(1);
            int blockSize = blocks[0, 0].GetBlockSize();

            Bitmap result = new Bitmap(cols * blockSize, rows * blockSize);

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    Block block = blocks[row, col];
                    Matrix A = block.GetChannel("A");
                    Matrix R = block.GetChannel("R");
                    Matrix G = block.GetChannel("G");
                    Matrix B = block.GetChannel("B");

                    for (int i = 0; i < blockSize; i++)
                    {
                        for (int j = 0; j < blockSize; j++)
                        {
                            int x = col * blockSize + j;
                            int y = row * blockSize + i;

                            int alpha = ClampToByte(A.GetElement(i, j));
                            int red = ClampToByte(R.GetElement(i, j));
                            int green = ClampToByte(G.GetElement(i, j));
                            int blue = ClampToByte(B.GetElement(i, j));
                            
                            result.SetPixel(x, y, Color.FromArgb(alpha, red, green, blue));
                        }
                    }
                }
            }
            return result;
        }
        public class QuadTreeNode
        {
            public Block BlockData;
            public QuadTreeNode TopLeft, TopRight, BottomLeft, BottomRight;
            public int Row, Col;

            public bool IsLeaf => TopLeft == null && TopRight == null &&
                                  BottomLeft == null && BottomRight == null;

            public QuadTreeNode(Block block, int row, int col)
            {
                BlockData = block;
                Row = row;
                Col = col;
            }
        }

        public QuadTreeNode BuildQuadTree(Block[,] blocks)
        {
            int rows = blocks.GetLength(0);
            int cols = blocks.GetLength(1);
            //Console.WriteLine($"[QuadTree] Building tree for grid {rows}x{cols}");
            return BuildRecursive(blocks, 0, 0, rows, cols);
        }

        private QuadTreeNode BuildRecursive(Block[,] blocks, int startRow, int startCol, int height, int width)
        {
            int totalRows = blocks.GetLength(0);
            int totalCols = blocks.GetLength(1);

            if (height <= 1 && width <= 1)
            {
                if (startRow < totalRows && startCol < totalCols)
                {
                    return new QuadTreeNode(blocks[startRow, startCol], startRow, startCol);
                }
                else
                {
                    return null; // Safety net for any stray overflow
                }
            }

            int halfHeight = height / 2;
            int halfWidth = width / 2;

            // Ensure at least 1 for odd sizes
            if (halfHeight == 0 && height > 1) halfHeight = 1;
            if (halfWidth == 0 && width > 1) halfWidth = 1;

            QuadTreeNode node = new QuadTreeNode(null, startRow, startCol)
            {
                TopLeft = BuildRecursive(blocks, startRow, startCol, halfHeight, halfWidth),
                TopRight = BuildRecursive(blocks, startRow, startCol + halfWidth, halfHeight, width - halfWidth),
                BottomLeft = BuildRecursive(blocks, startRow + halfHeight, startCol, height - halfHeight, halfWidth),
                BottomRight = BuildRecursive(blocks, startRow + halfHeight, startCol + halfWidth, height - halfHeight, width - halfWidth)
            };

            return node;
        }

        public List<(int row, int col)> TraverseTreeDFS(QuadTreeNode node)
        {
            List<(int, int)> result = new List<(int, int)>();
            TraverseDFS_Helper(node, result);
            return result;
        }

        private void TraverseDFS_Helper(QuadTreeNode node, List<(int, int)> output)
        {
            if (node == null) return;

            if (node.IsLeaf)
            {
                output.Add((node.Row, node.Col));
                return;
            }

            TraverseDFS_Helper(node.TopLeft, output);
            TraverseDFS_Helper(node.TopRight, output);
            TraverseDFS_Helper(node.BottomLeft, output);
            TraverseDFS_Helper(node.BottomRight, output);
        }


    }
}
