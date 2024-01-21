//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace Codecrete.SwissQRBill.CoreTest
{
    public class A4BillTest
    {

        [Fact]
        public Task CreateA4SvgBill1()
        {
            return GenerateAndCompareBill(SampleData.CreateExample1(), OutputSize.A4PortraitSheet, GraphicsFormat.SVG);
        }

        [Fact]
        public Task CreateA4PdfBill1()
        {
            return GenerateAndCompareBill(SampleData.CreateExample1(), OutputSize.A4PortraitSheet, GraphicsFormat.PDF);
        }

        [Fact]
        public Task CreateA4SvgBill2()
        {
            Bill bill = SampleData.CreateExample2();
            bill.Format.FontFamily = "Liberation Sans, Arial, Helvetica";
            return GenerateAndCompareBill(bill, OutputSize.A4PortraitSheet, GraphicsFormat.SVG);
        }

        [Fact]
        public Task CreateA4PdfBill2()
        {
            return GenerateAndCompareBill(SampleData.CreateExample2(), OutputSize.A4PortraitSheet, GraphicsFormat.PDF);
        }

        [Fact]
        public Task CreateA4SvgBill3()
        {
            return GenerateAndCompareBill(SampleData.CreateExample3(), OutputSize.A4PortraitSheet, GraphicsFormat.SVG);
        }

        [Fact]
        public Task CreateA4PdfBill3()
        {
            Bill bill = SampleData.CreateExample3();
            bill.Format.FontFamily = "Arial";
            return GenerateAndCompareBill(bill, OutputSize.A4PortraitSheet, GraphicsFormat.PDF);
        }

        [Fact]
        public Task CreateA4SvgBill4()
        {
            Bill bill = SampleData.CreateExample4();
            bill.Format.FontFamily = "Frutiger";
            return GenerateAndCompareBill(bill, OutputSize.A4PortraitSheet, GraphicsFormat.SVG);
        }

        [Fact]
        public Task CreateA4PdfBill4()
        {
            return GenerateAndCompareBill(SampleData.CreateExample4(), OutputSize.A4PortraitSheet, GraphicsFormat.PDF);
        }

        [Fact]
        public Task CreateA4SvgBill5()
        {
            return GenerateAndCompareBill(SampleData.CreateExample5(), OutputSize.A4PortraitSheet, GraphicsFormat.SVG);
        }

        [Fact]
        public Task CreateA4PdfBill5()
        {
            return GenerateAndCompareBill(SampleData.CreateExample5(), OutputSize.A4PortraitSheet, GraphicsFormat.PDF);
        }

        [Fact]
        public Task CreateA4SvgBill6()
        {
            return GenerateAndCompareBill(SampleData.CreateExample6(), OutputSize.A4PortraitSheet, GraphicsFormat.SVG);
        }

        [Fact]
        public Task CreateA4PdfBill6()
        {
            return GenerateAndCompareBill(SampleData.CreateExample6(), OutputSize.A4PortraitSheet, GraphicsFormat.PDF);
        }

        private static Task GenerateAndCompareBill(Bill bill, OutputSize outputSize, GraphicsFormat graphicsFormat)
        {
            bill.Format.OutputSize = outputSize;
            bill.Format.GraphicsFormat = graphicsFormat;
            byte[] imageData = QRBill.Generate(bill);
            return VerifyImages.Verify(imageData, graphicsFormat);
        }
    }
}
