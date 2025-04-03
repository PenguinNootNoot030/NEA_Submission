using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Testing_WPF
{
    public class Block
    {
        Matrix A_Matrix;
        Matrix R_Matrix;
        Matrix G_Matrix;
        Matrix B_Matrix;
        Matrix Y_Matrix;
        Matrix Cr_Matrix;
        Matrix Cb_Matrix;
        int BlockSize;
        public Block()
        {  
        }
        private void ARGB_YCbCr_Converter()
        { }
        private void YCbCr_ARGB_Converter() 
        { }
        public void UpdateARBG() { }
        public void UpdateYCbCr() { }

    }
}
