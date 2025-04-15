using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing_WPF
{
    public static class WatermarkEvaluator
    {
        public static string Evaluate(string original, string extracted)
        {
            StringBuilder report = new StringBuilder();

            report.AppendLine("==== Watermark Evaluation ====");
            report.AppendLine($"Original:   \"{original}\"");
            report.AppendLine($"Extracted:  \"{extracted}\"");

            if (original == extracted)
            {
                report.AppendLine("✅ Watermark match: PERFECT");
                report.AppendLine("Bit Error Rate (BER): 0%");
                return report.ToString();
            }

            List<bool> originalBits = Watermark.GetBitStream(original);
            List<bool> extractedBits = Watermark.StringToBitstream(extracted);

            int minLen = Math.Min(originalBits.Count, extractedBits.Count);
            int bitErrors = 0;

            for (int i = 0; i < minLen; i++)
            {
                if (originalBits[i] != extractedBits[i])
                    bitErrors++;
            }

            bitErrors += Math.Abs(originalBits.Count - extractedBits.Count);
            int totalBits = Math.Max(originalBits.Count, extractedBits.Count);
            float ber = 100f * bitErrors / totalBits;

            report.AppendLine($"❌ Watermark mismatch");
            report.AppendLine($"Bit Errors: {bitErrors} / {totalBits}");
            report.AppendLine($"Bit Error Rate (BER): {ber:F2}%");

            if (ber < 10)
                report.AppendLine("🟡 Status: Still readable — watermark mostly survived");
            else if (ber < 30)
                report.AppendLine("🔴 Status: Heavily corrupted — watermark degraded");
            else
                report.AppendLine("⚫ Status: Extraction failed — watermark lost");

            return report.ToString();
        }
    }
}
