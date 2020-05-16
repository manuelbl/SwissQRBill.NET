//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2020 Christian Bernasconi
// Copyright (c) 2020 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace Codecrete.SwissQRBill.GeneratorTest
{
    public class SwicoS1EncodingTest
    {
        [Fact]
        public void EncodeExample1()
        {
            SwicoBillInformation billInfo = SwicoExamples.CreateExample1();
            string text = billInfo.EncodeAsText();
            Assert.Equal(SwicoExamples.Example1Text, text);
        }

        [Fact]
        public void EncodeExample2()
        {
            SwicoBillInformation billInfo = SwicoExamples.CreateExample2();
            string text = billInfo.EncodeAsText();
            Assert.Equal(SwicoExamples.Example2Text, text);
        }

        [Fact]
        public void EncodeExample3()
        {
            SwicoBillInformation billInfo = SwicoExamples.CreateExample3();
            string text = billInfo.EncodeAsText();
            Assert.Equal(SwicoExamples.Example3Text, text);
        }

        [Fact]
        public void EncodeExample4()
        {
            SwicoBillInformation billInfo = SwicoExamples.CreateExample3();
            string text = billInfo.EncodeAsText();
            Assert.Equal(SwicoExamples.Example3Text, text);
        }

        [Fact]
        public void EncodeTextWithBackslash()
        {
            SwicoBillInformation billInfo = new SwicoBillInformation
            {
                InvoiceNumber = "X.66711/8824",
                InvoiceDate = new DateTime(2020, 7, 12),
                CustomerReference = @"MW-2020\04",
                VatNumber = "107978798",
                VatRateDetails = new List<(decimal, decimal)> { (2.5m, 117.22m) },
                PaymentConditions = new List<(decimal, int)> { (3m, 5), (1.5m, 20), (1m, 40), (0m, 60) }
            };

            string text = billInfo.EncodeAsText();
            Assert.Equal(
                @"//S1/10/X.66711\/8824/11/200712/20/MW-2020\\04/30/107978798/32/2.5:117.22/40/3:5;1.5:20;1:40;0:60",
                text);
        }

        [Fact]
        public void EncodeEmptyList()
        {
            SwicoBillInformation billInfo = new SwicoBillInformation
            {
                InvoiceNumber = "10201409",
                InvoiceDate = new DateTime(2019, 5, 12),
                CustomerReference = "1400.000-53",
                VatNumber = "106017086",
                VatDate = new DateTime(2018, 5, 8),
                VatRate = 7.7m,
                VatImportTaxes = new List<(decimal, decimal)>(),
                PaymentConditions = new List<(decimal, int)> { (2m, 10), (0m, 30) }
            };

            string text = billInfo.EncodeAsText();
            Assert.Equal(
                "//S1/10/10201409/11/190512/20/1400.000-53/30/106017086/31/180508/32/7.7/40/2:10;0:30",
                text);
        }

        [Fact]
        public void NoValidData_ReturnsNull()
        {
            var info = new SwicoBillInformation();
            Assert.Null(info.EncodeAsText());

            info.VatStartDate = new DateTime(2020, 8, 12);
            Assert.Null(info.EncodeAsText());

            info.VatStartDate = null;
            info.VatEndDate = new DateTime(2020, 8, 12);
            Assert.Null(info.EncodeAsText());

            info.VatRateDetails = new List<(decimal, decimal)>();
            info.VatImportTaxes = new List<(decimal, decimal)>();
            info.PaymentConditions = new List<(decimal, int)>();
            Assert.Null(info.EncodeAsText());
        }

        [Theory]
        [InlineData("de-DE")]
        [InlineData("en-US")]
        [InlineData("de-CH")]
        [InlineData("fr-CH")]
        public void DifferentLocales_HaveNoEffect(string locale)
        {
            CultureInfo savedCurrentCulture = CultureInfo.CurrentCulture;
            CultureInfo savedCurrentUiCulture = CultureInfo.CurrentUICulture;

            CultureInfo culture = CultureInfo.CreateSpecificCulture(locale);

            try
            {
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;

                SwicoBillInformation billInfo = SwicoExamples.CreateExample3();
                string text = billInfo.EncodeAsText();
                Assert.Equal(SwicoExamples.Example3Text, text);
            }
            finally
            {
                CultureInfo.CurrentCulture = savedCurrentCulture;
                CultureInfo.CurrentUICulture = savedCurrentUiCulture;
            }
        }
    }
}
