//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2021 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System.IO;

namespace Codecrete.SwissQRBill.PixelCanvasTest
{
    public static class StreamExtensions
    {
        public static byte[] ToArray(this Stream stream)
        {
            if (stream is MemoryStream memoryStream)
            {
                return memoryStream.ToArray();
            }

            using var tempStream = new MemoryStream();
            stream.CopyTo(tempStream);
            return tempStream.ToArray();
        }
    }
}