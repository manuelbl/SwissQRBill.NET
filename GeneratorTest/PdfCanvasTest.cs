//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using Codecrete.SwissQRBill.Generator.Canvas;
using System.Globalization;
using System.IO;
using Xunit;

namespace Codecrete.SwissQRBill.GeneratorTest
{
    public class PdfCanvasTest
    {
        [Fact]
        public void PdfWriteTo()
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
        public void PdfSaveAs()
        {
            Bill bill = SampleData.CreateExample4();
            using (PDFCanvas canvas =
                new PDFCanvas(QRBill.A4PortraitWidth, QRBill.A4PortraitHeight))
            {
                QRBill.Draw(bill, canvas);
                canvas.SaveAs("qrbill.pdf");
            }
        }

        [Fact]
        public void LocaleIndependence()
        {
            CultureInfo savedCurrentCulture = CultureInfo.CurrentCulture;
            CultureInfo savedCurrentUiCulture = CultureInfo.CurrentUICulture;

            CultureInfo culture = CultureInfo.CreateSpecificCulture("de-DE");

            Bill bill = SampleData.CreateExample4();
            using (PDFCanvas canvas =
                new PDFCanvas(QRBill.A4PortraitWidth, QRBill.A4PortraitHeight))
            {
                try
                {
                    CultureInfo.CurrentCulture = culture;
                    CultureInfo.CurrentUICulture = culture;

                    QRBill.Draw(bill, canvas);
                    FileComparison.AssertFileContentsEqual(canvas.ToByteArray(), "pdfcanvas-locale-1.pdf");
                }
                finally
                {
                    CultureInfo.CurrentCulture = savedCurrentCulture;
                    CultureInfo.CurrentUICulture = savedCurrentUiCulture;
                }
            }
        }
    }
}
