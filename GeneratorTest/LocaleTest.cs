//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using System.Globalization;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace Codecrete.SwissQRBill.GeneratorTest
{
    [UsesVerify]
    public class LocaleTest
    {
        [Fact]
        public Task DefaultIsDe()
        {
            return GenerateQRBill("de-DE");
        }

        [Fact]
        public Task DefaultIsUs()
        {
            return GenerateQRBill("en-US");
        }

        [Fact]
        public Task DefaultIsDech()
        {
            return GenerateQRBill("de-CH");
        }

        [Fact]
        public Task DefaultIsFrch()
        {
            return GenerateQRBill("fr-CH");
        }

        private Task GenerateQRBill(string locale)
        {
            CultureInfo savedCurrentCulture = CultureInfo.CurrentCulture;
            CultureInfo savedCurrentUiCulture = CultureInfo.CurrentUICulture;

            CultureInfo culture = CultureInfo.CreateSpecificCulture(locale);

            try
            {
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;

                Bill bill = SampleData.CreateExample3();
                bill.Format.OutputSize = OutputSize.QrBillOnly;
                bill.Format.GraphicsFormat = GraphicsFormat.SVG;
                byte[] svg = QRBill.Generate(bill);
                return VerifyImages.VerifySvg(svg);
            }
            finally
            {
                CultureInfo.CurrentCulture = savedCurrentCulture;
                CultureInfo.CurrentUICulture = savedCurrentUiCulture;
            }
        }
    }
}
