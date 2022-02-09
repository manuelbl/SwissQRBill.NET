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
    /// Name object
    /// </summary>
    internal class Name : IWritable
    {
        internal string Value { get; }

        internal Name(string value)
        {
            Value = value;
        }

        void IWritable.Write(StreamWriter writer)
        {
            writer.Write($"/{Value}");
        }
    }
}
