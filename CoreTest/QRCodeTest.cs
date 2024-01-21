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
    public class QRCodeTest
    {
        [Fact]
        public Task QrCodeAsSvg1()
        {
            Bill bill = SampleData.CreateExample1();
            bill.Format.OutputSize = OutputSize.QrCodeOnly;
            bill.Format.GraphicsFormat = GraphicsFormat.SVG;
            byte[] svg = QRBill.Generate(bill);
            return VerifyImages.VerifySvg(svg);
        }

        [Fact]
        public Task QrCodeAsSvg2()
        {
            Bill bill = SampleData.CreateExample2();
            bill.Format.OutputSize = OutputSize.QrCodeOnly;
            bill.Format.GraphicsFormat = GraphicsFormat.SVG;
            byte[] svg = QRBill.Generate(bill);
            return VerifyImages.VerifySvg(svg);
        }

        [Fact]
        public Task QrCodeAsSvg3()
        {
            Bill bill = SampleData.CreateExample3();
            bill.Format.OutputSize = OutputSize.QrCodeOnly;
            bill.Format.GraphicsFormat = GraphicsFormat.SVG;
            byte[] svg = QRBill.Generate(bill);
            return VerifyImages.VerifySvg(svg);
        }

        [Fact]
        public Task QrCodeAsSvg4()
        {
            Bill bill = SampleData.CreateExample4();
            bill.Format.OutputSize = OutputSize.QrCodeOnly;
            bill.Format.GraphicsFormat = GraphicsFormat.SVG;
            byte[] svg = QRBill.Generate(bill);
            return VerifyImages.VerifySvg(svg);
        }

        [Fact]
        public Task QrCodeWithQuiteZone()
        {
            Bill bill = SampleData.CreateExample3();
            bill.Format.OutputSize = OutputSize.QrCodeWithQuietZone;
            bill.Format.GraphicsFormat = GraphicsFormat.SVG;
            byte[] svg = QRBill.Generate(bill);
            return VerifyImages.VerifySvg(svg);
        }

        [Fact]
        public Task QrCodeWithQuiteZonePDF()
        {
            Bill bill = SampleData.CreateExample3();
            bill.Format.OutputSize = OutputSize.QrCodeWithQuietZone;
            bill.Format.GraphicsFormat = GraphicsFormat.PDF;
            byte[] pdf = QRBill.Generate(bill);
            return VerifyImages.VerifyPdf(pdf);
        }
    }
}
