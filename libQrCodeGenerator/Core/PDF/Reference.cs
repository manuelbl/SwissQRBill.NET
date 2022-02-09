//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System.IO;

namespace libQrCodeGenerator.SwissQRBill.Generator.PDF
{
    /// <summary>
    /// Reference to an object
    /// </summary>
    internal class Reference : IWritable
    {
        internal int Index { get; private set; }
        internal int Offset { get; set; }
        internal object Target { get; private set; }

        internal Reference(int index, object target)
        {
            Index = index;
            Target = target;
        }

        internal void WriteCrossReference(StreamWriter writer)
        {
            if (Index == 0)
            {
                writer.Write("0000000000 65535 f\r\n");
            }
            else
            {
                writer.Write($"{Offset:D10} 00000 n\r\n");
            }
        }

        internal void WriteDefinition(StreamWriter writer)
        {
            if (Index == 0)
            {
                return;
            }

            writer.Write($"{Index} 0 obj\n");
            WriterHelper.WriteObject(writer, Target);
            writer.Write("endobj\n");
        }

        void IWritable.Write(StreamWriter writer)
        {
            writer.Write($"{Index} 0 R");
        }
    }
}
