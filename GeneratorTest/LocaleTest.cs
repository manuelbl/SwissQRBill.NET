//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using System.Globalization;
using Xunit;

namespace Codecrete.SwissQRBill.GeneratorTest
{
    public class LocaleTest
    {
        [Fact]
        public void DefaultIsDe()
        {
            GenerateQRBill("de-DE", "qrbill-locale-1.svg");
        }

        [Fact]
        public void DefaultIsUs()
        {
            GenerateQRBill("en-US", "qrbill-locale-2.svg");
        }

        [Fact]
        public void DefaultIsDech()
        {
            GenerateQRBill("de-CH", "qrbill-locale-3.svg");
        }

        [Fact]
        public void DefaultIsFrch()
        {
            GenerateQRBill("fr-CH", "qrbill-locale-4.svg");
        }

        private void GenerateQRBill(string locale, string expectedFileName)
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
                FileComparison.AssertFileContentsEqual(svg, expectedFileName);
            }
            finally
            {
                CultureInfo.CurrentCulture = savedCurrentCulture;
                CultureInfo.CurrentUICulture = savedCurrentUiCulture;
            }
        }
    }
}
