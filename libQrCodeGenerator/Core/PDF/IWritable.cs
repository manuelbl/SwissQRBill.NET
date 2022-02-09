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
    /// Object that can write itself to output
    /// </summary>
    internal interface IWritable
    {
        void Write(StreamWriter writer);
    }
}
