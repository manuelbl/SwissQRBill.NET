//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2020 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using Codecrete.SwissQRBill.Generator.Canvas;
using Xunit;

namespace Codecrete.SwissQRBill.GeneratorTest
{
    public class LineStyleTest
    {
        [Fact]
        private void SvgWithDashedLines()
        {
            Bill bill = SampleData.CreateExample1();
            GenerateAndCompareBill(bill, GraphicsFormat.SVG, SeparatorType.DashedLine, "linestyle_1.svg");
        }

        [Fact]
        private void SvgWithDottedLines()
        {
            Bill bill = SampleData.CreateExample1();
            GenerateAndCompareBill(bill, GraphicsFormat.SVG, SeparatorType.DottedLineWithScissors, "linestyle_2.svg");
        }

        [Fact]
        private void PdfWithDashedLines()
        {
            Bill bill = SampleData.CreateExample1();
            GenerateAndCompareBill(bill, GraphicsFormat.PDF, SeparatorType.DashedLineWithScissors, "linestyle_1.pdf");
        }

        [Fact]
        private void PdfWithDottedLines()
        {
            Bill bill = SampleData.CreateExample1();
            GenerateAndCompareBill(bill, GraphicsFormat.PDF, SeparatorType.DottedLine, "linestyle_2.pdf");
        }

        [Fact]
        private void PngWithDashedLines()
        {
            Bill bill = SampleData.CreateExample1();
            GenerateAndComparePngBill(bill, SeparatorType.DashedLine, "linestyle_1.png");
        }

        [Fact]
        private void PngWithDottedLines()
        {
            Bill bill = SampleData.CreateExample2();
            GenerateAndComparePngBill(bill, SeparatorType.DottedLineWithScissors, "linestyle_2.png");
        }

        private void GenerateAndCompareBill(Bill bill, GraphicsFormat graphicsFormat, SeparatorType separatorType,
                                            string expectedFileName)
        {
            bill.Format.GraphicsFormat = graphicsFormat;
            bill.Format.SeparatorType = separatorType;
            byte[] imageData = QRBill.Generate(bill);
            FileComparison.AssertFileContentsEqual(imageData, expectedFileName);
        }

        private void GenerateAndComparePngBill(Bill bill, SeparatorType separatorType,
                                            string expectedFileName)
        {
            bill.Format.SeparatorType = separatorType;
            using (PNGCanvas canvas =
                new PNGCanvas(QRBill.A4PortraitWidth, QRBill.A4PortraitHeight, 288, "Arial,Helvetica"))
            {
                QRBill.Draw(bill, canvas);
                FileComparison.AssertGrayscaleImageContentsEqual(canvas.ToByteArray(), expectedFileName);
            }
        }
    }
}
