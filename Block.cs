using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Testing_WPF
{
    internal class Block
    {
        Matrix A;
        Matrix R;
        Matrix G;
        Matrix B;
        public Block(Matrix A, Matrix R, Matrix G, Matrix B)
        { 
            this.A = A;
            this.G = G;
            this.B = B; 
            this.R = R;
        }
    }
}
