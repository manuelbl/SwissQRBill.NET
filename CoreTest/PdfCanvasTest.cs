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
            using (var canvas = new PDFCanvas(QRBill.A4PortraitWidth, QRBill.A4PortraitHeight))
            {
                QRBill.Draw(bill, canvas);
                var ms = new MemoryStream();
                canvas.WriteTo(ms);
            }

            Assert.True(true);
        }

        [Fact]
        public void PdfSaveAs()
        {
            Bill bill = SampleData.CreateExample4();
            using (var canvas = new PDFCanvas(QRBill.A4PortraitWidth, QRBill.A4PortraitHeight))
            {
                QRBill.Draw(bill, canvas);
                canvas.SaveAs("qrbill.pdf");
            }

            Assert.True(true);
        }

        [Fact]
        public Task LocaleIndependence()
        {
            CultureInfo savedCurrentCulture = CultureInfo.CurrentCulture;
            CultureInfo savedCurrentUiCulture = CultureInfo.CurrentUICulture;

            CultureInfo culture = CultureInfo.CreateSpecificCulture("de-DE");

            Bill bill = SampleData.CreateExample4();
            using (var canvas = new PDFCanvas(QRBill.A4PortraitWidth, QRBill.A4PortraitHeight))
            {
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
        }

        [Fact]
        public void DrawWithCharactersOutsideWinANSI_RaisesException()
        {
            var bill = SampleData.CreateExample8();
            bill.CharacterSet = SpsCharacterSet.ExtendedLatin;
            using (var canvas = new PDFCanvas(QRBill.A4PortraitWidth, QRBill.A4PortraitHeight))
            {
                Assert.Throws<QRBillGenerationException>(() => QRBill.Draw(bill, canvas));
            }
        }

        [Fact]
        public void PDFWithEmbeddedFonts()
        {
            using (var canvas = new PDFCanvas(210, 297, PDFFontSettings.EmbeddedLiberationSans()))
            {
                DrawTextLine(canvas, 270, false, 0x0020, 0x003F);
                DrawTextLine(canvas, 260, false, 0x0040, 0x005F);
                DrawTextLine(canvas, 250, false, 0x0060, 0x007E);
                DrawTextLine(canvas, 240, false, 0x00A0, 0x00BF);
                DrawTextLine(canvas, 230, false, 0x00C0, 0x00DF);
                DrawTextLine(canvas, 220, false, 0x00E0, 0x00FE);
                DrawTextLine(canvas, 210, false, 0x0100, 0x011F);
                DrawTextLine(canvas, 200, false, 0x0120, 0x013F);
                DrawTextLine(canvas, 190, false, 0x0140, 0x015F);
                DrawTextLine(canvas, 180, false, 0x0160, 0x017F);
                DrawTextLine(canvas, 170, false, 0x0218, 0x021C);
                DrawTextLine(canvas, 160, false, 0x20AC, 0x20AC);

                DrawTextLine(canvas, 140, true, 'A', 'Z');
                DrawTextLine(canvas, 130, true, 'a', 'z');
                DrawTextLine(canvas, 120, true, "äöüàçéèô");

                canvas.SaveAs("fonts.pdf");
            }
            Assert.True(true);
        }

        private void DrawTextLine(PDFCanvas canvas, float yOffset, bool isBold, int firstCodePoint, int lastCodePoint)
        {
            var chars = new char[lastCodePoint - firstCodePoint + 1];
            for (var codePoint = firstCodePoint; codePoint <= lastCodePoint; codePoint += 1)
            {
                chars[codePoint - firstCodePoint] = (char)codePoint;
            }
            var text = new string(chars);
            DrawTextLine(canvas, yOffset, isBold, text);
        }

        private void DrawTextLine(PDFCanvas canvas, float yOffset, bool isBold, string text)
        {
            canvas.PutText(text, 20, yOffset, 12, isBold);
        }
    }
}
