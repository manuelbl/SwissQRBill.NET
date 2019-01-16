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
    public class QRCodeTest
    {
        [Fact]
        private void QrCodeAsSvg1()
        {
            Bill bill = SampleData.CreateExample1();
            bill.Format.OutputSize = OutputSize.QrCodeOnly;
            bill.Format.GraphicsFormat = GraphicsFormat.SVG;
            byte[] svg = QRBill.Generate(bill);
            FileComparison.AssertFileContentsEqual(svg, "qrcode_ex1.svg");
        }

        [Fact]
        private void QrCodeAsSvg2()
        {
            Bill bill = SampleData.CreateExample2();
            bill.Format.OutputSize = OutputSize.QrCodeOnly;
            bill.Format.GraphicsFormat = GraphicsFormat.SVG;
            byte[] svg = QRBill.Generate(bill);
            FileComparison.AssertFileContentsEqual(svg, "qrcode_ex2.svg");
        }

        [Fact]
        private void QrCodeAsSvg3()
        {
            Bill bill = SampleData.CreateExample3();
            bill.Format.OutputSize = OutputSize.QrCodeOnly;
            bill.Format.GraphicsFormat = GraphicsFormat.SVG;
            byte[] svg = QRBill.Generate(bill);
            FileComparison.AssertFileContentsEqual(svg, "qrcode_ex3.svg");
        }

        [Fact]
        private void QrCodeAsSvg4()
        {
            Bill bill = SampleData.CreateExample4();
            bill.Format.OutputSize = OutputSize.QrCodeOnly;
            bill.Format.GraphicsFormat = GraphicsFormat.SVG;
            byte[] svg = QRBill.Generate(bill);
            FileComparison.AssertFileContentsEqual(svg, "qrcode_ex4.svg");
        }
    }
}
