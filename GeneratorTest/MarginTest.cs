//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2021 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using Xunit;

namespace Codecrete.SwissQRBill.GeneratorTest
{
    public class MarginTest
    {

        [Fact]
        public void CreateA4SvgBill1()
        {
            Bill bill = SampleData.CreateExample1();
            bill.Format.MarginLeft = 8.0;
            bill.Format.MarginRight = 8.0;
            GenerateAndCompareBill(bill, "a4bill_ma1.svg");
        }

        [Fact]
        public void CreateA4SvgBill2()
        {
            Bill bill = SampleData.CreateExample2();
            bill.Format.MarginLeft = 12.0;
            bill.Format.MarginRight = 12.0;
            GenerateAndCompareBill(bill, "a4bill_ma2.svg");
        }

        [Fact]
        public void CreateA4SvgBill6()
        {
            Bill bill = SampleData.CreateExample6();
            bill.Format.MarginLeft = 10.0;
            bill.Format.MarginRight = 9.0;
            GenerateAndCompareBill(bill, "a4bill_ma6.svg");
        }

        private void GenerateAndCompareBill(Bill bill, string expectedFileName)
        {
            bill.Format.OutputSize = OutputSize.A4PortraitSheet;
            bill.Format.GraphicsFormat = GraphicsFormat.SVG;
            byte[] imageData = QRBill.Generate(bill);
            FileComparison.AssertFileContentsEqual(imageData, expectedFileName);
        }
    }
}
