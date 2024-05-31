//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2024 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using System.Threading.Tasks;
using Xunit;

namespace Codecrete.SwissQRBill.CoreTest
{
    public class PaymentPartTest
    {
        [Fact]
        public Task CreateQrBill1()
        {
            Bill bill = SampleData.CreateExample1();
            bill.Format.OutputSize = OutputSize.PaymentPartOnly;
            bill.Format.GraphicsFormat = GraphicsFormat.SVG;
            byte[] svg = QRBill.Generate(bill);
            return VerifyImages.VerifySvg(svg);
        }

        [Fact]
        public Task CreateQrBill2()
        {
            Bill bill = SampleData.CreateExample2();
            bill.Format.OutputSize = OutputSize.PaymentPartOnly;
            bill.Format.GraphicsFormat = GraphicsFormat.PDF;
            byte[] pdf = QRBill.Generate(bill);
            return VerifyImages.VerifyPdf(pdf);
        }
    }
}
