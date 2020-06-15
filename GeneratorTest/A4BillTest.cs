//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using Xunit;

namespace Codecrete.SwissQRBill.GeneratorTest
{
    public class A4BillTest
    {

        [Fact]
        public void CreateA4SvgBill1()
        {
            GenerateAndCompareBill(SampleData.CreateExample1(), OutputSize.A4PortraitSheet, GraphicsFormat.SVG, "a4bill_ex1.svg");
        }

        [Fact]
        public void CreateA4PdfBill1()
        {
            GenerateAndCompareBill(SampleData.CreateExample1(), OutputSize.A4PortraitSheet, GraphicsFormat.PDF, "a4bill_ex1.pdf");
        }

        [Fact]
        public void CreateA4PngBill1()
        {
            GenerateAndCompareBill(SampleData.CreateExample1(), OutputSize.A4PortraitSheet, GraphicsFormat.PNG, "a4bill_ex1.png");
        }

        [Fact]
        public void CreateA4SvgBill2()
        {
            Bill bill = SampleData.CreateExample2();
            bill.Format.FontFamily = "Liberation Sans, Arial, Helvetica";
            GenerateAndCompareBill(bill, OutputSize.A4PortraitSheet, GraphicsFormat.SVG, "a4bill_ex2.svg");
        }

        [Fact]
        public void CreateA4PdfBill2()
        {
            GenerateAndCompareBill(SampleData.CreateExample2(), OutputSize.A4PortraitSheet, GraphicsFormat.PDF, "a4bill_ex2.pdf");
        }

        [Fact]
        public void CreateA4SvgBill3()
        {
            GenerateAndCompareBill(SampleData.CreateExample3(), OutputSize.A4PortraitSheet, GraphicsFormat.SVG, "a4bill_ex3.svg");
        }

        [Fact]
        public void CreateA4PdfBill3()
        {
            Bill bill = SampleData.CreateExample3();
            bill.Format.FontFamily = "Arial";
            GenerateAndCompareBill(bill, OutputSize.A4PortraitSheet, GraphicsFormat.PDF, "a4bill_ex3.pdf");
        }

        [Fact]
        public void CreateA4SvgBill4()
        {
            Bill bill = SampleData.CreateExample4();
            bill.Format.FontFamily = "Frutiger";
            GenerateAndCompareBill(bill, OutputSize.A4PortraitSheet, GraphicsFormat.SVG, "a4bill_ex4.svg");
        }

        [Fact]
        public void CreateA4PdfBill4()
        {
            GenerateAndCompareBill(SampleData.CreateExample4(), OutputSize.A4PortraitSheet, GraphicsFormat.PDF, "a4bill_ex4.pdf");
        }

        [Fact]
        public void CreateA4SvgBill5()
        {
            GenerateAndCompareBill(SampleData.CreateExample5(), OutputSize.A4PortraitSheet, GraphicsFormat.SVG, "a4bill_ex5.svg");
        }

        [Fact]
        public void CreateA4PdfBill5()
        {
            GenerateAndCompareBill(SampleData.CreateExample5(), OutputSize.A4PortraitSheet, GraphicsFormat.PDF, "a4bill_ex5.pdf");
        }

        [Fact]
        public void CreateA4SvgBill6()
        {
            GenerateAndCompareBill(SampleData.CreateExample6(), OutputSize.A4PortraitSheet, GraphicsFormat.SVG, "a4bill_ex6.svg");
        }

        [Fact]
        public void CreateA4PdfBill6()
        {
            GenerateAndCompareBill(SampleData.CreateExample6(), OutputSize.A4PortraitSheet, GraphicsFormat.PDF, "a4bill_ex6.pdf");
        }

        private void GenerateAndCompareBill(Bill bill, OutputSize outputSize, GraphicsFormat graphicsFormat,
                                            string expectedFileName)
        {
            bill.Format.OutputSize = outputSize;
            bill.Format.GraphicsFormat = graphicsFormat;
            byte[] imageData = QRBill.Generate(bill);
            FileComparison.AssertFileContentsEqual(imageData, expectedFileName);
        }
    }
}
