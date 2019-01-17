//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using Codecrete.SwissQRBill.Generator.Canvas;
using System.IO;
using Xunit;

namespace Codecrete.SwissQRBill.GeneratorTest
{
    public class PdfCanvasTest
    {
        [Fact]
        private void PdfWriteTo()
        {
            Bill bill = SampleData.CreateExample3();
            using (PDFCanvas canvas =
                new PDFCanvas(QRBill.A4PortraitWidth, QRBill.A4PortraitHeight))
            {
                QRBill.Draw(bill, canvas);
                MemoryStream ms = new MemoryStream();
                canvas.WriteTo(ms);
            }
        }

        [Fact]
        private void PdfSaveAs()
        {
            Bill bill = SampleData.CreateExample4();
            using (PDFCanvas canvas =
                new PDFCanvas(QRBill.A4PortraitWidth, QRBill.A4PortraitHeight))
            {
                QRBill.Draw(bill, canvas);
                canvas.SaveAs("qrbill.pdf");
            }
        }
    }
}
