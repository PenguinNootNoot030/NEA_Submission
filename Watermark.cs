using System;
namespace Testing_WPF
{
    public class Watermark
    {
        string wateramrk_message;
        const int max_length = 50;
        byte[] binary_rep;
        public Watermark(string message)
        {
            //set up the message to embed
            wateramrk_message = message;
        }

        int length;
        public int GetMessgeLength()
        { return length; }

        /// <summary>
        /// Ensure the format of the message is given correctly.
        /// </summary>
        /// <returns></returns>
        private bool ValidateMessage() => wateramrk_message != null && wateramrk_message.Length <= max_length;

        /// <summary>
        /// Converting the given message into a byte array
        /// </summary>
        private void ConvertMessageToBinary()
        { 
            binary_rep = new byte[wateramrk_message.Length];
            for (int i = 0; i < binary_rep.Length; i++)
            { binary_rep[i] = Convert.ToByte(wateramrk_message[i]); }
        }

        public byte[] GetBinaryRepresentation() => binary_rep;

        private string ConvertBinaryToString(byte[] byteArr)
        {
            string obj = "";
            foreach (byte b in byteArr)
            { obj += b.ToString(); }
            return obj;
        }
    }
}

