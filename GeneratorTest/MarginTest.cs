//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2021 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System.Threading.Tasks;
using Codecrete.SwissQRBill.Generator;
using Xunit;

namespace Codecrete.SwissQRBill.GeneratorTest
{
    [VerifyXunit.UsesVerify]
    public class MarginTest : VerifyTest
    {

        [Fact]
        public Task CreateA4SvgBill1()
        {
            Bill bill = SampleData.CreateExample1();
            bill.Format.MarginLeft = 8.0;
            bill.Format.MarginRight = 8.0;
            byte[] svg = GenerateSvg(bill);
            return VerifySvg(svg);
        }

        [Fact]
        public Task CreateA4SvgBill2()
        {
            Bill bill = SampleData.CreateExample2();
            bill.Format.MarginLeft = 12.0;
            bill.Format.MarginRight = 12.0;
            byte[] svg = GenerateSvg(bill);
            return VerifySvg(svg);
        }

        [Fact]
        public Task CreateA4SvgBill6()
        {
            Bill bill = SampleData.CreateExample6();
            bill.Format.MarginLeft = 10.0;
            bill.Format.MarginRight = 9.0;
            byte[] svg = GenerateSvg(bill);
            return VerifySvg(svg);
        }

        private static byte[] GenerateSvg(Bill bill)
        {
            bill.Format.OutputSize = OutputSize.A4PortraitSheet;
            bill.Format.GraphicsFormat = GraphicsFormat.SVG;
            return QRBill.Generate(bill);
        }
    }
}
