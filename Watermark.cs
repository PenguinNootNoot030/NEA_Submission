using System;
namespace Testing_WPF
{
    public class Watermark
    {
        string watermark_message;
        const int max_length = 50;
        byte[] binary_rep;
        int length;
        public Watermark(string message)
        { 
            watermark_message = message;
            length = watermark_message.Length;
        } 
        
        private bool IfValidMessage() => watermark_message != null && watermark_message.Length <= max_length;

        private void ConvertMessageToBinary()
        {
            if (IfValidMessage())
            {
                binary_rep = new byte[watermark_message.Length];
                for (int i = 0; i < binary_rep.Length; i++)
                { binary_rep[i] = Convert.ToByte(watermark_message[i]); }
            }
            else { throw new Exception("Messgae not in correct format."); }
        }

        public byte[] GetBinaryRepresentation()
        {
            ConvertMessageToBinary();
            return binary_rep;
        } 

        public string ConvertBinaryToString(byte[] byteArr)
        {
            string obj = "";
            foreach (byte b in byteArr)
            { obj += b.ToString(); }
            return obj;
        }
    }
}

