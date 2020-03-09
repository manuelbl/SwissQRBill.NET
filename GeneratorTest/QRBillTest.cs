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
    public class QRBillTest
    {
        [Fact]
        private void CreateQrBill1()
        {
            Bill bill = SampleData.CreateExample1();
            bill.Format.OutputSize = OutputSize.QrBillOnly;
            bill.Format.GraphicsFormat = GraphicsFormat.SVG;
            byte[] svg = QRBill.Generate(bill);
            FileComparison.AssertFileContentsEqual(svg, "qrbill_ex1.svg");
        }

        [Fact]
        private void CreateQrBill2()
        {
            Bill bill = SampleData.CreateExample2();
            bill.Format.OutputSize = OutputSize.QrBillOnly;
            bill.Format.GraphicsFormat = GraphicsFormat.SVG;
            byte[] svg = QRBill.Generate(bill);
            FileComparison.AssertFileContentsEqual(svg, "qrbill_ex2.svg");
        }

        [Fact]
        private void CreateQrBill3()
        {
            Bill bill = SampleData.CreateExample3();
            bill.Format.OutputSize = OutputSize.QrBillOnly;
            bill.Format.GraphicsFormat = GraphicsFormat.SVG;
            byte[] svg = QRBill.Generate(bill);
            FileComparison.AssertFileContentsEqual(svg, "qrbill_ex3.svg");
        }

        [Fact]
        private void CreateQrBill4()
        {
            Bill bill = SampleData.CreateExample4();
            bill.Format.OutputSize = OutputSize.QrBillOnly;
            bill.Format.GraphicsFormat = GraphicsFormat.SVG;
            byte[] svg = QRBill.Generate(bill);
            FileComparison.AssertFileContentsEqual(svg, "qrbill_ex4.svg");
        }

        [Fact]
        private void CreateQrBill5()
        {
            Bill bill = SampleData.CreateExample3();
            bill.Format.OutputSize = OutputSize.QrBillWithHorizontalLine;
            bill.Format.GraphicsFormat = GraphicsFormat.SVG;
            byte[] svg = QRBill.Generate(bill);
            FileComparison.AssertFileContentsEqual(svg, "qrbill_ex5.svg");
        }
    }
}
