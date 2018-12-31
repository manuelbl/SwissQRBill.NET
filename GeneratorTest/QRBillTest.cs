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
        void CreateQRBill1()
        {
            Bill bill = SampleData.CreateExample1();
            bill.Format.OutputSize = OutputSize.QRBillOnly;
            bill.Format.GraphicsFormat = GraphicsFormat.SVG;
            byte[] svg = QRBill.Generate(bill);
            FileComparison.AssertFileContentsEqual(svg, "qrbill_ex1.svg");
        }

        [Fact]
        void CreateQRBill2()
        {
            Bill bill = SampleData.CreateExample2();
            bill.Format.OutputSize = OutputSize.QRBillOnly;
            bill.Format.GraphicsFormat = GraphicsFormat.SVG;
            byte[] svg = QRBill.Generate(bill);
            FileComparison.AssertFileContentsEqual(svg, "qrbill_ex2.svg");
        }

        [Fact]
        void CreateQRBill3()
        {
            Bill bill = SampleData.CreateExample3();
            bill.Format.OutputSize = OutputSize.QRBillOnly;
            bill.Format.GraphicsFormat = GraphicsFormat.SVG;
            byte[] svg = QRBill.Generate(bill);
            FileComparison.AssertFileContentsEqual(svg, "qrbill_ex3.svg");
        }

        [Fact]
        void CreateQRBill4()
        {
            Bill bill = SampleData.CreateExample4();
            bill.Format.OutputSize = OutputSize.QRBillOnly;
            bill.Format.GraphicsFormat = GraphicsFormat.SVG;
            byte[] svg = QRBill.Generate(bill);
            FileComparison.AssertFileContentsEqual(svg, "qrbill_ex4.svg");
        }
    }
}
