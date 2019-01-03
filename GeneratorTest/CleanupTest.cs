//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator.Canvas;
using System;
using System.Reflection;
using Xunit;

namespace Codecrete.SwissQRBill.GeneratorTest
{
    public class CleanupTest
    {
        [Fact]
        void ClosePNGFreesResources()
        {
            Type type = typeof(PNGCanvas);
            FieldInfo bitmap = type.GetField("bitmap", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(bitmap);

            PNGCanvas pngCanvas;
            using (PNGCanvas canvas = new PNGCanvas(300))
            {
                canvas.SetupPage(200, 100, "Arial");
                pngCanvas = canvas;
                Assert.NotNull(bitmap.GetValue(pngCanvas));
            }

            Assert.Null(bitmap.GetValue(pngCanvas));
        }
    }
}
