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
using System.Threading.Tasks;
using Xunit;

namespace Codecrete.SwissQRBill.CoreTest
{
    public class PdfCanvasTest
    {
        [Fact]
        public void PdfWriteTo()
        {
            Bill bill = SampleData.CreateExample3();
            using var canvas = new PDFCanvas(QRBill.A4PortraitWidth, QRBill.A4PortraitHeight);
            QRBill.Draw(bill, canvas);
            var ms = new MemoryStream();
            canvas.WriteTo(ms);
            Assert.True(true);
        }

        [Fact]
        public void PdfSaveAs()
        {
            Bill bill = SampleData.CreateExample4();
            using var canvas = new PDFCanvas(QRBill.A4PortraitWidth, QRBill.A4PortraitHeight);
            QRBill.Draw(bill, canvas);
            canvas.SaveAs("qrbill.pdf");
            Assert.True(true);
        }

        [Fact]
        public Task LocaleIndependence()
        {
            CultureInfo savedCurrentCulture = CultureInfo.CurrentCulture;
            CultureInfo savedCurrentUiCulture = CultureInfo.CurrentUICulture;

            CultureInfo culture = CultureInfo.CreateSpecificCulture("de-DE");

            Bill bill = SampleData.CreateExample4();
            using var canvas = new PDFCanvas(QRBill.A4PortraitWidth, QRBill.A4PortraitHeight);
            try
            {
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;

                QRBill.Draw(bill, canvas);
                return VerifyImages.VerifyPdf(canvas.ToByteArray());
            }
            finally
            {
                CultureInfo.CurrentCulture = savedCurrentCulture;
                CultureInfo.CurrentUICulture = savedCurrentUiCulture;
            }
        }

        [Fact]
        public void DrawWithCharactersOutsideWinANSI_RaisesException()
        {
            var bill = SampleData.CreateExample8();
            bill.CharacterSet = SpsCharacterSet.ExtendedLatin;
            using var canvas = new PDFCanvas(QRBill.A4PortraitWidth, QRBill.A4PortraitHeight);
            Assert.Throws<QRBillGenerationException>(() => QRBill.Draw(bill, canvas));
        }

        [Fact]
        public void GenerateWithCharactersOutsideWinANSI_RaisesException()
        {
            var bill = SampleData.CreateExample8();
            bill.CharacterSet = SpsCharacterSet.ExtendedLatin;
            bill.Format.GraphicsFormat = GraphicsFormat.PDF;

            Assert.Throws<QRBillGenerationException>(() => QRBill.Generate(bill));
        }
    }
}
