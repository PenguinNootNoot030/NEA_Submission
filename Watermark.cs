using System;
using System.Text;
namespace Testing_WPF
{
    //boxed off
    public class Watermark
    {
        private const int MaxLength = 50;

        private string Message;
        private static List<bool> BitStream;
        private int BitPointer;

        public Watermark(string message)
        {
            Message = message;
            if (!IsValid())
                         throw new ArgumentException("Invalid watermark message");

            GenerateBitStream();
        }

        private bool IsValid()
        {
            return !string.IsNullOrEmpty(Message) && Message.Length <= MaxLength;
        }

        private void GenerateBitStream()
        {
            BitStream = new List<bool>();

            // Optionally prefix length (e.g. 8 bits)
            byte lengthByte = (byte)Message.Length;
            for (int i = 7; i >= 0; i--)
                BitStream.Add((lengthByte & (1 << i)) != 0);

            // Message → bits (ASCII encoding)
            foreach (char c in Message)
            {
                byte chByte = (byte)c;
                for (int i = 7; i >= 0; i--)
                    BitStream.Add((chByte & (1 << i)) != 0);
            }

            BitPointer = 0;
        }

        // Return full bitstream
        public static List<bool> GetBitStream() => BitStream;

        public int GetLength() => BitStream.Count;

        // Return next bit for embedding
        public bool GetNextBit()
        {
            if (BitPointer >= BitStream.Count)
                throw new InvalidOperationException("All bits have been read.");
            return BitStream[BitPointer++];
        }

        public void ResetBitPointer()
        {
            BitPointer = 0;
        }

        // Convert bitstream back to string
        public static string ToMessage(List<bool> bitstream)
        {
            if (bitstream.Count < 8)
                return "<invalid bitstream>";

            // Extract length prefix
            int msgLength = 0;
            for (int i = 0; i < 8; i++)
                msgLength = (msgLength << 1) + (bitstream[i] ? 1 : 0);

            StringBuilder sb = new StringBuilder();
            int byteCount = 0;

            for (int i = 8; i + 7 < bitstream.Count && byteCount < msgLength; i += 8)
            {
                int charVal = 0;
                for (int j = 0; j < 8; j++)
                    charVal = (charVal << 1) + (bitstream[i + j] ? 1 : 0);
                sb.Append((char)charVal);
                byteCount++;
            }

            return sb.ToString();
        }

        public override string ToString()
        {
            return $"Watermark[{Message}], Bits={BitStream.Count}";
        }
    }
}

