//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using System.Globalization;
using System.Threading.Tasks;
using VerifyTests;
using Xunit;

namespace Codecrete.SwissQRBill.GeneratorTest
{
    [VerifyXunit.UsesVerify]
    public class LocaleTest : VerifyTest
    {
        [Theory]
        [InlineData("de-DE")]
        [InlineData("en-US")]
        [InlineData("de-CH")]
        [InlineData("fr-CH")]
        public Task CurrentCulture(string locale)
        {
            SvgSettings.UseParameters(locale);

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
                return VerifySvg(svg);
            }
            finally
            {
                CultureInfo.CurrentCulture = savedCurrentCulture;
                CultureInfo.CurrentUICulture = savedCurrentUiCulture;
            }
        }
    }
}
