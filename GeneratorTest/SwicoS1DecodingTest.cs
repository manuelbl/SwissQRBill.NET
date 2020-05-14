//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2020 Christian Bernasconi
// Copyright (c) 2020 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//
using Codecrete.SwissQRBill.Generator;
using Xunit;

namespace Codecrete.SwissQRBill.GeneratorTest
{
    public class SwicoS1DecodingTest
    {
        [Fact]
        private void DecodeExample1()
        {
            var billInformation = SwicoBillInformation.DecodeText(SwicoExamples.Example1Text);
            Assert.Equal(SwicoExamples.CreateExample1(), billInformation);
        }

        [Fact]
        private void DecodeExample2()
        {
            var billInformation = SwicoBillInformation.DecodeText(SwicoExamples.Example2Text);
            Assert.Equal(SwicoExamples.CreateExample2(), billInformation);
        }

        [Fact]
        private void DecodeExample3()
        {
            var billInformation = SwicoBillInformation.DecodeText(SwicoExamples.Example3Text);
            Assert.Equal(SwicoExamples.CreateExample3(), billInformation);
        }

        [Fact]
        private void DecodeExample4()
        {
            var billInformation = SwicoBillInformation.DecodeText(SwicoExamples.Example4Text);
            Assert.Equal(SwicoExamples.CreateExample4(), billInformation);
        }

        [Theory]
        [InlineData("//S1/10//11//20//30/")]
        private void DecodeEmptyValues(string rawBillInformation)
        {
            var billInformation = SwicoBillInformation.DecodeText(rawBillInformation);
            Assert.Equal(new SwicoBillInformation(), billInformation);
        }

        [Theory]
        [InlineData("/S1/10/X.66711")]
        [InlineData("///S1/10/X.66711")]
        [InlineData("S1/10/X.66711")]
        [InlineData("10/X.66711")]
        public void InvalidStart_ExceptionIsThrown(string rawBillInformation)
        {
            var exception = Record.Exception(() =>
                SwicoBillInformation.DecodeText(rawBillInformation));

            Assert.NotNull(exception);
            Assert.IsType<SwicoDecodingException>(exception);
            Assert.Equal("Bill information text does not start with \"//S1/\"", exception.Message);
        }

        [Theory]
        [InlineData("//S1/10/X.66711/XX/200")]
        [InlineData("//S1/10/X.66711/10 /200")]
        [InlineData("//S1/10/X.66711/ 10/200")]
        public void InvalidTag_ExceptionIsThrown(string rawBillInformation)
        {
            var exception = Record.Exception(() =>
                SwicoBillInformation.DecodeText(rawBillInformation));

            Assert.NotNull(exception);
            Assert.IsType<SwicoDecodingException>(exception);
            Assert.StartsWith("Invalid tag ", exception.Message);
        }

        [Theory]
        [InlineData("//S1/11/190520/10/X.66711")]
        [InlineData("//S1/10/X.66711/30/123456789/11/190520")]
        public void TagNotInAscendingOrder_ExceptionIsThrown(string rawBillInformation)
        {
            var exception = Record.Exception(() =>
                SwicoBillInformation.DecodeText(rawBillInformation));

            Assert.NotNull(exception);
            Assert.IsType<SwicoDecodingException>(exception);
            Assert.Equal("Bill information: tags must appear in ascending order", exception.Message);
        }

        [Theory]
        [InlineData("//S1/10/X.66711/11/190520/20")]
        [InlineData("//S1/10")]
        public void BillInformationTruncated_ExceptionIsThrown(string rawBillInformation)
        {
            var exception = Record.Exception(() =>
                SwicoBillInformation.DecodeText(rawBillInformation));

            Assert.NotNull(exception);
            Assert.IsType<SwicoDecodingException>(exception);
            Assert.StartsWith("Bill information is truncated", exception.Message);
        }

        [Theory]
        [InlineData("//S1/10/X.66711/11/190520/19/123")]
        [InlineData("//S1/10/10201409/12/ABC")]
        public void UnknownTag_ExceptionIsThrown(string rawBillInformation)
        {
            var exception = Record.Exception(() =>
                SwicoBillInformation.DecodeText(rawBillInformation));

            Assert.NotNull(exception);
            Assert.IsType<SwicoDecodingException>(exception);
            Assert.StartsWith("Unknown tag ", exception.Message);
        }

        [Theory]
        [InlineData("//S1/10/X.66711/11/190570/20/123")]
        [InlineData("//S1/10/10201409/11/19.05.20")]
        [InlineData("//S1/10/X.66711/11/1905213/20/123")]
        [InlineData("//S1/10/10201409/11/200301 ")]
        public void InvalidInvoiceDate_ExceptionIsThrown(string rawBillInformation)
        {
            var exception = Record.Exception(() =>
                SwicoBillInformation.DecodeText(rawBillInformation));

            Assert.NotNull(exception);
            Assert.IsType<SwicoDecodingException>(exception);
            Assert.StartsWith("Invalid date value ", exception.Message);
        }

        [Theory]
        [InlineData("//S1/10/X.66711/31/190570/32/7.7", "Invalid date value")]
        [InlineData("//S1/10/10201409/31/1905213", "Invalid VAT date(s)")]
        [InlineData("//S1/10/X.66711/31/1905211905232/32/7.7", "Invalid VAT date(s)")]
        [InlineData("//S1/10/10201409/31/19052119052 ", "Invalid date value")]
        public void InvalidVatDates_ExceptionIsThrown(string rawBillInformation, string exceptionMessage)
        {
            var exception = Record.Exception(() =>
                SwicoBillInformation.DecodeText(rawBillInformation));

            Assert.NotNull(exception);
            Assert.IsType<SwicoDecodingException>(exception);
            Assert.StartsWith(exceptionMessage, exception.Message);
        }

        [Theory]
        [InlineData("//S1/10/X.66711/11/190530/32/.5/40/0:30")]
        [InlineData("//S1/10/10201409/11/200604/32/A/40/0:30")]
        [InlineData("//S1/10/329348709/11/200823/32/ 7.7/40/0:10")]
        public void InvalidVatRate_ExceptionIsThrown(string rawBillInformation)
        {
            var exception = Record.Exception(() =>
                SwicoBillInformation.DecodeText(rawBillInformation));

            Assert.NotNull(exception);
            Assert.IsType<SwicoDecodingException>(exception);
            Assert.StartsWith("Invalid numeric value ", exception.Message);
        }

        [Theory]
        [InlineData("//S1/10/X.66711/11/190530/32/1;2/40/0:30", "Invalid VAT rate / amount tuple")]
        [InlineData("//S1/10/10201409/11/200604/32/8:300;5/40/0:30", "Invalid VAT rate / amount tuple")]
        [InlineData("//S1/10/329348709/11/200823/32/;8:500/40/0:10", "Invalid VAT rate / amount tuple")]
        [InlineData("//S1/10/329348709/11/200823/32/8:500;/40/0:10", "Invalid VAT rate / amount tuple")]
        [InlineData("//S1/10/X.66711/11/190530/32/1:.5/40/0:30", "Invalid numeric value")]
        [InlineData("//S1/10/10201409/11/200604/32/8:B/40/0:30", "Invalid numeric value")]
        [InlineData("//S1/10/329348709/11/200823/32/8:/40/0:10", "Invalid numeric value")]
        [InlineData("//S1/10/329348709/11/200823/32/8:500;2.5:200;x:200/40/0:10", "Invalid numeric value")]
        public void InvalidVatRateDetails_ExceptionIsThrown(string rawBillInformation, string exceptionMessage)
        {
            var exception = Record.Exception(() =>
                SwicoBillInformation.DecodeText(rawBillInformation));

            Assert.NotNull(exception);
            Assert.IsType<SwicoDecodingException>(exception);
            Assert.StartsWith(exceptionMessage, exception.Message);
        }

        [Theory]
        [InlineData("//S1/10/X.66711/11/190530/33/1;2/40/0:30", "Invalid VAT rate / amount tuple")]
        [InlineData("//S1/10/10201409/11/200604/33/8:300;5/40/0:30", "Invalid VAT rate / amount tuple")]
        [InlineData("//S1/10/329348709/11/200823/33/;8:500/40/0:10", "Invalid VAT rate / amount tuple")]
        [InlineData("//S1/10/329348709/11/200823/33/8:500;/40/0:10", "Invalid VAT rate / amount tuple")]
        [InlineData("//S1/10/X.66711/11/190530/33/1:.5/40/0:30", "Invalid numeric value")]
        [InlineData("//S1/10/10201409/11/200604/33/8:B/40/0:30", "Invalid numeric value")]
        [InlineData("//S1/10/329348709/11/200823/33/8:/40/0:10", "Invalid numeric value")]
        [InlineData("//S1/10/329348709/11/200823/33/8:500;2.5:200;x:200/40/0:10", "Invalid numeric value")]
        public void InvalidVatImportTaxes_ExceptionIsThrown(string rawBillInformation, string exceptionMessage)
        {
            var exception = Record.Exception(() =>
                SwicoBillInformation.DecodeText(rawBillInformation));

            Assert.NotNull(exception);
            Assert.IsType<SwicoDecodingException>(exception);
            Assert.StartsWith(exceptionMessage, exception.Message);
        }

        [Theory]
        [InlineData("//S1/10/X.66711/11/190530/40/1;60", "Invalid discount / days tuple")]
        [InlineData("//S1/10/10201409/11/200604/40/2:10;5", "Invalid discount / days tuple")]
        [InlineData("//S1/10/329348709/11/200823/40/;2:30", "Invalid discount / days tuple")]
        [InlineData("//S1/10/329348709/11/200823/40/0:30;", "Invalid discount / days tuple")]
        [InlineData("//S1/10/X.66711/11/190530/40/1: 5", "Invalid integer value")]
        [InlineData("//S1/10/10201409/11/200604/40/3:B", "Invalid integer value")]
        [InlineData("//S1/10/329348709/11/200823/40/8.:20", "Invalid numeric value")]
        [InlineData("//S1/10/329348709/11/200823/40/3:10;1:20;x:200", "Invalid numeric value")]
        public void InvalidPaymentConditions_ExceptionIsThrown(string rawBillInformation, string exceptionMessage)
        {
            var exception = Record.Exception(() =>
                SwicoBillInformation.DecodeText(rawBillInformation));

            Assert.NotNull(exception);
            Assert.IsType<SwicoDecodingException>(exception);
            Assert.StartsWith(exceptionMessage, exception.Message);
        }

    }
}
