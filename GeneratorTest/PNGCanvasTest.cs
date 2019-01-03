//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using Codecrete.SwissQRBill.Generator.Canvas;
using Xunit;

namespace Codecrete.SwissQRBill.GeneratorTest
{
    public class PNGCanvasTest
    {
        [Fact]
        void PngBillQRBill()
        {
            Bill bill = SampleData.CreateExample1();
            bill.Format.FontFamily = "Arial";
            PNGCanvas canvas = new PNGCanvas(300);
            bill.Format.OutputSize = OutputSize.QRBillOnly;
            byte[] svg = QRBill.Generate(bill, canvas);
            FileComparison.AssertGrayscaleImageContentsEqual(svg, "qrbill_ex1.png");
        }

        [Fact]
        void PngBillA4()
        {
            Bill bill = SampleData.CreateExample3();
            bill.Format.FontFamily = "Arial,Helvetica";
            PNGCanvas canvas = new PNGCanvas(144);
            bill.Format.OutputSize = OutputSize.A4PortraitSheet;
            byte[] svg = QRBill.Generate(bill, canvas);
            FileComparison.AssertGrayscaleImageContentsEqual(svg, "a4bill_ex3.png");
        }
    }
}
