//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using Codecrete.SwissQRBill.Generator.Canvas;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Codecrete.SwissQRBill.GeneratorTest
{
    [VerifyXunit.UsesVerify]
    public class SVGTest : VerifyTest
    {
        [Fact]
        public Task SvgWithChallengingCharacters()
        {
            Bill bill = SampleData.CreateExample1();
            bill.UnstructuredMessage = "<h1>&&\"ff\"'t'";
            bill.Format.OutputSize = OutputSize.QrBillOnly;
            bill.Format.GraphicsFormat = GraphicsFormat.SVG;
            byte[] svg = QRBill.Generate(bill);
            return VerifySvg(svg);
        }

        [Fact]
        public void SvgWriteTo()
        {
            Bill bill = SampleData.CreateExample1();
            using (SVGCanvas canvas =
                new SVGCanvas(QRBill.A4PortraitWidth, QRBill.A4PortraitHeight, "Helvetica, Arial, Sans"))
            {
                QRBill.Draw(bill, canvas);
                MemoryStream ms = new MemoryStream();
                canvas.WriteTo(ms);
            }
        }

        [Fact]
        public void SvgSaveAs()
        {
            Bill bill = SampleData.CreateExample2();
            using (SVGCanvas canvas =
                new SVGCanvas(QRBill.A4PortraitWidth, QRBill.A4PortraitHeight, "Helvetica, Arial, Sans"))
            {
                QRBill.Draw(bill, canvas);
                canvas.SaveAs("qrbill.svg");
            }
        }
    }
}
