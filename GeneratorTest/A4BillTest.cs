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
        void CreateA4SVGBill1()
        {
            GenerateAndCompareBill(SampleData.CreateExample1(), OutputSize.A4PortraitSheet, GraphicsFormat.SVG, "a4bill_ex1.svg");
        }

        [Fact]
        void CreateA4PDFBill1()
        {
            GenerateAndCompareBill(SampleData.CreateExample1(), OutputSize.A4PortraitSheet, GraphicsFormat.PDF, "a4bill_ex1.pdf");
        }

        [Fact]
        void CreateA4SVGBill2()
        {
            Bill bill = SampleData.CreateExample2();
            bill.Format.FontFamily = "Liberation Sans, Arial, Helvetica";
            GenerateAndCompareBill(bill, OutputSize.A4PortraitSheet, GraphicsFormat.SVG, "a4bill_ex2.svg");
        }

        [Fact]
        void CreateA4PDFBill2()
        {
            GenerateAndCompareBill(SampleData.CreateExample2(), OutputSize.A4PortraitSheet, GraphicsFormat.PDF, "a4bill_ex2.pdf");
        }

        [Fact]
        void CreateA4SVGBill3()
        {
            GenerateAndCompareBill(SampleData.CreateExample3(), OutputSize.A4PortraitSheet, GraphicsFormat.SVG, "a4bill_ex3.svg");
        }

        [Fact]
        void CreateA4PDFBill3()
        {
            Bill bill = SampleData.CreateExample3();
            bill.Format.FontFamily = "Arial";
            GenerateAndCompareBill(bill, OutputSize.A4PortraitSheet, GraphicsFormat.PDF, "a4bill_ex3.pdf");
        }

        [Fact]
        void CreateA4SVGBill4()
        {
            Bill bill = SampleData.CreateExample4();
            bill.Format.FontFamily = "Frutiger";
            GenerateAndCompareBill(bill, OutputSize.A4PortraitSheet, GraphicsFormat.SVG, "a4bill_ex4.svg");
        }

        [Fact]
        void CreateA4PDFBill4()
        {
            GenerateAndCompareBill(SampleData.CreateExample4(), OutputSize.A4PortraitSheet, GraphicsFormat.PDF, "a4bill_ex4.pdf");
        }

        [Fact]
        void CreateA4SVGBill5()
        {
            GenerateAndCompareBill(SampleData.CreateExample5(), OutputSize.A4PortraitSheet, GraphicsFormat.SVG, "a4bill_ex5.svg");
        }

        [Fact]
        void CreateA4PDFBill5()
        {
            GenerateAndCompareBill(SampleData.CreateExample5(), OutputSize.A4PortraitSheet, GraphicsFormat.PDF, "a4bill_ex5.pdf");
        }

        [Fact]
        void CreateA4SVGBill6()
        {
            GenerateAndCompareBill(SampleData.CreateExample6(), OutputSize.A4PortraitSheet, GraphicsFormat.SVG, "a4bill_ex6.svg");
        }

        [Fact]
        void CreateA4PDFBill6()
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
