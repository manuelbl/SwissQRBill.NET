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
        public void CreateQrBill1()
        {
            Bill bill = SampleData.CreateExample1();
            bill.Format.OutputSize = OutputSize.QrBillOnly;
            bill.Format.GraphicsFormat = GraphicsFormat.SVG;
            byte[] svg = QRBill.Generate(bill);
            FileComparison.AssertFileContentsEqual(svg, "qrbill_ex1.svg");
        }

        [Fact]
        public void CreateQrBill2()
        {
            Bill bill = SampleData.CreateExample2();
            bill.Format.OutputSize = OutputSize.QrBillOnly;
            bill.Format.GraphicsFormat = GraphicsFormat.SVG;
            byte[] svg = QRBill.Generate(bill);
            FileComparison.AssertFileContentsEqual(svg, "qrbill_ex2.svg");
        }

        [Fact]
        public void CreateQrBill3()
        {
            Bill bill = SampleData.CreateExample3();
            bill.Format.OutputSize = OutputSize.QrBillOnly;
            bill.Format.GraphicsFormat = GraphicsFormat.SVG;
            byte[] svg = QRBill.Generate(bill);
            FileComparison.AssertFileContentsEqual(svg, "qrbill_ex3.svg");
        }

        [Fact]
        public void CreateQrBill4()
        {
            Bill bill = SampleData.CreateExample4();
            bill.Format.OutputSize = OutputSize.QrBillOnly;
            bill.Format.GraphicsFormat = GraphicsFormat.SVG;
            byte[] svg = QRBill.Generate(bill);
            FileComparison.AssertFileContentsEqual(svg, "qrbill_ex4.svg");
        }

        [Fact]
        public void CreateQrBill5()
        {
            Bill bill = SampleData.CreateExample3();
            bill.Format.OutputSize = OutputSize.QrBillWithHorizontalLine;
            bill.Format.GraphicsFormat = GraphicsFormat.SVG;
            byte[] svg = QRBill.Generate(bill);
            FileComparison.AssertFileContentsEqual(svg, "qrbill_ex5.svg");
        }

        [Fact]
        public void CreateQrBillFrench()
        {
            Bill bill = SampleData.CreateExample1();
            bill.Format.GraphicsFormat = GraphicsFormat.SVG;
            bill.Format.Language = Language.FR;
            byte[] svg = QRBill.Generate(bill);
            FileComparison.AssertFileContentsEqual(svg, "qrbill_fr.svg");
        }

        [Fact]
        public void GetLibraryVersion()
        {
            string version = QRBill.LibraryVersion;
            Assert.Matches(@"^[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+$", version);
        }
    }
}
