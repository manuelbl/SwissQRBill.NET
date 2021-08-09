//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2020 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using Codecrete.SwissQRBill.Generator.Canvas;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace Codecrete.SwissQRBill.GeneratorTest
{
    [UsesVerify]
    public class LineStyleTest
    {
        [Fact]
        public Task SvgWithDashedLines()
        {
            Bill bill = SampleData.CreateExample1();
            return GenerateAndCompareBill(bill, GraphicsFormat.SVG, SeparatorType.DashedLine);
        }

        [Fact]
        public Task SvgWithDottedLines()
        {
            Bill bill = SampleData.CreateExample1();
            return GenerateAndCompareBill(bill, GraphicsFormat.SVG, SeparatorType.DottedLineWithScissors);
        }

        [Fact]
        public Task PdfWithDashedLines()
        {
            Bill bill = SampleData.CreateExample1();
            return GenerateAndCompareBill(bill, GraphicsFormat.PDF, SeparatorType.DashedLineWithScissors);
        }

        [Fact]
        public Task PdfWithDottedLines()
        {
            Bill bill = SampleData.CreateExample1();
            return GenerateAndCompareBill(bill, GraphicsFormat.PDF, SeparatorType.DottedLine);
        }

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

        private Task GenerateAndCompareBill(Bill bill, GraphicsFormat graphicsFormat, SeparatorType separatorType)
        {
            bill.Format.GraphicsFormat = graphicsFormat;
            bill.Format.SeparatorType = separatorType;
            byte[] imageData = QRBill.Generate(bill);
            return VerifyImages.Verify(imageData, graphicsFormat);
        }

        private Task GenerateAndComparePngBill(Bill bill, SeparatorType separatorType)
        {
            bill.Format.SeparatorType = separatorType;
            using (PNGCanvas canvas =
                new PNGCanvas(QRBill.A4PortraitWidth, QRBill.A4PortraitHeight, 288, "Arial,Helvetica"))
            {
                QRBill.Draw(bill, canvas);
                return VerifyImages.VerifyPng(canvas.ToByteArray());
            }
        }
    }
}
