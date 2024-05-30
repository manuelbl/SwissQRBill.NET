//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.PixelCanvas;
using System;
using Xunit;

namespace Codecrete.SwissQRBill.PixelCanvasTest
{
    public class PngProcessorTest
    {
        [Fact]
        public void HeaderIncomplete_Fails()
        {
            var data = new byte[] { 0, 0, 0, 0, 0 };
            Assert.Throws<ArgumentException>(() =>
            {
                PngProcessor.InsertDpi(new System.IO.MemoryStream(data), new System.IO.MemoryStream(), 96);
            });
        }

        [Fact]
        public void NoPhysChunk_Fails()
        {
            var data = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            Assert.Throws<ArgumentException>(() =>
            {
                PngProcessor.InsertDpi(new System.IO.MemoryStream(data), new System.IO.MemoryStream(), 96);
            });
        }

        [Fact]
        public void FirstChunkIncomplete_Fails()
        {
            var data = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            Assert.Throws<ArgumentException>(() =>
            {
                PngProcessor.InsertDpi(new System.IO.MemoryStream(data), new System.IO.MemoryStream(), 96);
            });
        }

        [Fact]
        public void Incomplete_Fails()
        {
            var data = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            Assert.Throws<ArgumentException>(() =>
            {
                PngProcessor.InsertDpi(new System.IO.MemoryStream(data), new System.IO.MemoryStream(), 96);
            });
        }
    }
}
