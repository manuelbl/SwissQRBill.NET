//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using Codecrete.SwissQRBill.PixelCanvas;
using System;
using System.Reflection;
using Xunit;

namespace Codecrete.SwissQRBill.PixelCanvasTest
{
    public class CleanupTest
    {
        [Fact]
        public void ClosePngFreesResources()
        {
            Type type = typeof(PNGCanvas);
            FieldInfo bitmapField = type.GetField("_bitmap", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(bitmapField);

            PNGCanvas pngCanvas;
            using (PNGCanvas canvas = new PNGCanvas(QRBill.QrBillWidth, QRBill.QrBillHeight, 300, "Arial, Helvatica, \"Liberation Sans\""))
            {
                pngCanvas = canvas;
                Assert.NotNull(bitmapField.GetValue(pngCanvas));
            }

            Assert.Null(bitmapField.GetValue(pngCanvas));
        }
    }
}
