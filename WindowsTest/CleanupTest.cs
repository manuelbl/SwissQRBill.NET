//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using Codecrete.SwissQRBill.Windows;
using System;
using System.Reflection;
using Xunit;

namespace Codecrete.SwissQRBill.WindowsTest
{
    public class CleanupTest : TestBase
    {
        [WindowsFact]
        public void ClosePngFreesResources()
        {
            Type type = typeof(BitmapCanvas);
            FieldInfo bitmapField = type.GetField("_bitmap", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(bitmapField);

            PNGCanvas pngCanvas;
            using (var canvas = new PNGCanvas(QRBill.QrBillWidth, QRBill.QrBillHeight, 300, "Arial, Helvatica, \"Liberation Sans\""))
            {
                pngCanvas = canvas;
                Assert.NotNull(bitmapField.GetValue(pngCanvas));
            }

            Assert.Null(bitmapField.GetValue(pngCanvas));
        }
    }
}
