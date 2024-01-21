//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2020 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using Codecrete.SwissQRBill.PixelCanvas;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace Codecrete.SwissQRBill.PixelCanvasTest
{
    public class LineStyleTest
    {
        [Fact]
        public Task PngWithDashedLines()
        {
            Bill bill = SampleData.CreateExample1();
            return GenerateAndComparePngBill(bill, SeparatorType.DashedLine);
        }

        [Fact]
        public Task PngWithDottedLines()
        {
            Bill bill = SampleData.CreateExample2();
            return GenerateAndComparePngBill(bill, SeparatorType.DottedLineWithScissors);
        }

        private static Task GenerateAndComparePngBill(Bill bill, SeparatorType separatorType)
        {
            bill.Format.SeparatorType = separatorType;
            using var canvas = new PNGCanvas(QRBill.A4PortraitWidth, QRBill.A4PortraitHeight, 288, "Arial,Helvetica");
            QRBill.Draw(bill, canvas);
            return VerifyImages.VerifyPng(canvas.ToByteArray());
        }
    }
}
