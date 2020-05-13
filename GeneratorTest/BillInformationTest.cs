using System;
using System.Globalization;
using Codecrete.SwissQRBill.Generator;
using Xunit;

namespace Codecrete.SwissQRBill.GeneratorTest
{
    public class BillInformationTest
    {
        [Fact]
        public void DecodeBillInformationText_EmptyRawString_ReturnsNull()
        {
            var billInformation = BillInformation.DecodeBillInformationText(string.Empty);

            Assert.Null(billInformation);
        }

        [Theory]
        [InlineData("/S1/10/X.66711")]
        [InlineData("///S1/10/X.66711")]
        [InlineData("S1/10/X.66711")]
        [InlineData("10/X.66711")]
        public void DecodeBillInformationText_InvalidBillInformation_ExceptionIsThrown(string rawBillInformation)
        {
            var exception = Record.Exception(() =>
                BillInformation.DecodeBillInformationText(rawBillInformation));

            Assert.NotNull(exception);
            Assert.IsType<QRBillInformationValidationException>(exception);
        }

        [Theory]
        [InlineData(@"//S1/10/X.66711\/8824/11/200712/20/MW-2020-04", "X.66711/8824")]
        [InlineData("//S1/10/10201409/11/190512/20/1400.000-53/30/106017086/31/180508/32/7.7/40/2:10;0:30", "10201409")]
        public void DecodeBillInformationText_InvoiceNumber_IsMapped(string rawBillInformation,
            string expectedInvoiceNumber)
        {
            var billInformation = BillInformation.DecodeBillInformationText(rawBillInformation);

            Assert.Equal(expectedInvoiceNumber, billInformation.InvoiceNumber);
        }

        [Fact]
        public void DecodeBillInformationText_InvoiceDate_IsMapped()
        {
            const string rawBillInformation =
                "//S1/10/10201409/11/190512/20/1400.000-53/30/106017086/31/180508/32/7.7/40/2:10;0:30";
            var expectedInvoiceDate = new DateTime(2019, 5, 12);

            var billInformation = BillInformation.DecodeBillInformationText(rawBillInformation);

            Assert.Equal(expectedInvoiceDate, billInformation.InvoiceDate);
        }

        [Fact]
        public void DecodeBillInformationText_InvalidInvoiceDate_IsNull()
        {
            const string rawBillInformation = "//S1/10/10201409/11/192012";

            var billInformation = BillInformation.DecodeBillInformationText(rawBillInformation);

            Assert.Null(billInformation.InvoiceDate);

        }

        [Theory]
        [InlineData("//S1/10/10201409/11/190512/20/1400.000-53/30/106017086/31/180508/32/7.7/40/2:10;0:30",
            "1400.000-53")]
        [InlineData(
            "//S1/10/90015218/11/200113/20/Ihre Lieferung vom 10.12.2019/30/106017086/31/200113/32/7.7:120/40/0:0",
            "Ihre Lieferung vom 10.12.2019")]
        public void DecodeBillInformationText_CustomerReference_IsMapped(string rawBillInformation,
            string expectedCustomerReference)
        {
            var billInformation = BillInformation.DecodeBillInformationText(rawBillInformation);

            Assert.Equal(expectedCustomerReference, billInformation.CustomerReference);
        }

        [Fact]
        public void DecodeBillInformationText_VatNumber_IsMapped()
        {
            const string rawBillInformation = "//S1/10/10201409/11/190512/20/1400.000-53/30/106017086";
            const string expectedVatNumber = "106017086";

            var billInformation = BillInformation.DecodeBillInformationText(rawBillInformation);

            Assert.Equal(expectedVatNumber, billInformation.VatNumber);
        }

        [Fact]
        public void DecodeBillInformationText_NoVatNumber_IsNull()
        {
            const string rawBillInformation = "//S1/10/10201409/11/190512/20/1400.000-53/31/180508";

            var billInformation = BillInformation.DecodeBillInformationText(rawBillInformation);

            Assert.Null(billInformation.VatNumber);

        }

        [Fact]
        public void DecodeBillInformationText_VatDate_IsMapped()
        {
            const string rawBillInformation =
                "//S1/10/10201409/11/190512/20/1400.000-53/30/106017086/31/180508/32/7.7/40/2:10;0:30";
            var expectedVatDate = new DateTime(2018, 5, 8);

            var billInformation = BillInformation.DecodeBillInformationText(rawBillInformation);

            Assert.Equal(expectedVatDate, billInformation.VatDate);
            Assert.Null(billInformation.VatStartDate);
            Assert.Null(billInformation.VatEndDate);
        }

        [Fact]
        public void DecodeBillInformationText_NoVatDate_AllVatDatesAreMNull()
        {
            const string rawBillInformation = "//S1/10/10201409/11/190512";

            var billInformation = BillInformation.DecodeBillInformationText(rawBillInformation);

            Assert.Null(billInformation.VatDate);
            Assert.Null(billInformation.VatStartDate);
            Assert.Null(billInformation.VatEndDate);
        }

        [Theory]
        [InlineData("//S1/10/10201409/11/190512/20/1400.000-53/30/106017086/31/182008/32/7.7/40/2:10;0:30")]
        [InlineData("//S1/10/10201409/11/190512/20/1400.000-53/30/106017086/31/18010800")]
        public void DecodeBillInformationText_InvalidVatDate_AllVatDatesAreMNull(string rawBillInformation)
        {
            var billInformation = BillInformation.DecodeBillInformationText(rawBillInformation);

            Assert.Null(billInformation.VatDate);
            Assert.Null(billInformation.VatStartDate);
            Assert.Null(billInformation.VatEndDate);
        }

        [Fact]
        public void DecodeBillInformationText_VatStartEndDate_AreMapped()
        {
            const string rawBillInformation =
                "//S1/10/10104/11/180228/30/395856455/31/180226180227/32/3.7:400.19;7.7:553.39;0:14/40/0:30";
            var expectedVatStartDate = new DateTime(2018, 2, 26);
            var expectedVatEndDate = new DateTime(2018, 2, 27);

            var billInformation = BillInformation.DecodeBillInformationText(rawBillInformation);

            Assert.Null(billInformation.VatDate);
            Assert.Equal(expectedVatStartDate, billInformation.VatStartDate);
            Assert.Equal(expectedVatEndDate, billInformation.VatEndDate);
        }

        [Theory]
        [InlineData("//S1/10/10104/11/180228/30/395856455/31/183226180227")]
        public void DecodeBillInformationText_InvalidVatStartDate_VatStartDateIsNull(string rawBillInformation)
        {
            var billInformation = BillInformation.DecodeBillInformationText(rawBillInformation);

            Assert.Null(billInformation.VatStartDate);
        }

        [Theory]
        [InlineData("//S1/10/10201409/11/190512/20/1400.000-53/30/106017086/31/180108000000")]
        [InlineData("//S1/10/10104/11/180228/30/395856455/31/180226182227")]
        [InlineData("//S1/10/10104/11/180228/30/395856455/31/18022618022700")]
        public void DecodeBillInformationText_InvalidVatEndDate_VatEndDateIsNull(string rawBillInformation)
        {
            var billInformation = BillInformation.DecodeBillInformationText(rawBillInformation);

            Assert.Null(billInformation.VatEndDate);
        }

        [Fact]
        public void DecodeBillInformationText_VatDetailsSingleVatRate_IsMapped()
        {
            const string rawBillInformation =
                "//S1/10/10201409/11/190512/20/1400.000-53/30/106017086/31/180508/32/7.7/40/2:10;0:30";
            const double expectedVatRate = 7.7;

            var billInformation = BillInformation.DecodeBillInformationText(rawBillInformation);

            Assert.Collection(billInformation.VatDetails,
                item =>
                {
                    Assert.Equal(expectedVatRate, item.Key);
                    Assert.Equal(double.NaN, item.Value);
                });
        }

        [Fact]
        public void DecodeBillInformationText_NoVatDetails_AreEmpty()
        {
            const string rawBillInformation =
                "//S1/10/10201409/11/190512/20/1400.000-53/30/106017086/31/180508/40/2:10;0:30";

            var billInformation = BillInformation.DecodeBillInformationText(rawBillInformation);

            Assert.Empty(billInformation.VatDetails);
        }

        [Fact]
        public void DecodeBillInformationText_VatDetails_AreMapped()
        {
            const string rawBillInformation =
                "//S1/10/10104/11/180228/30/395856455/31/180226180227/32/3.7:400.19;7.7:553.39;0:14/40/0:30";
            const double expectedFirstVatRate = 3.7;
            const double expectedFirstAmount = 400.19;
            const double expectedSecondVatRate = 7.7;
            const double expectedSecondAmount = 553.39;
            const int expectedThirdVatRate = 0;
            const int expectedThirdAmount = 14;

            var billInformation = BillInformation.DecodeBillInformationText(rawBillInformation);

            Assert.Collection(billInformation.VatDetails,
                item =>
                {
                    Assert.Equal(expectedFirstVatRate, item.Key);
                    Assert.Equal(expectedFirstAmount, item.Value);
                },
                item =>
                {
                    Assert.Equal(expectedSecondVatRate, item.Key);
                    Assert.Equal(expectedSecondAmount, item.Value);
                },
                item =>
                {
                    Assert.Equal(expectedThirdVatRate, item.Key);
                    Assert.Equal(expectedThirdAmount, item.Value);
                });
        }

        [Fact]
        public void DecodeBillInformationText_VatPureImportTax_IsMapped()
        {
            const string rawBillInformation =
                "//S1/10/4031202511/11/180107/20/61257233.4/30/105493567/32/8:49.82/33/2.5:14.85/40/0:30";
            const double expectedRate = 2.5;
            const double expectedAmount = 14.85;

            var billInformation = BillInformation.DecodeBillInformationText(rawBillInformation);

            Assert.Collection(billInformation.VatImportTaxes,
                item =>
                {
                    Assert.Equal(expectedRate, item.Key);
                    Assert.Equal(expectedAmount, item.Value);
                });
        }

        [Fact]
        public void DecodeBillInformationText_NoImportTax_AreEmpty()
        {
            const string rawBillInformation = "//S1/10/4031202511/11/180107/20/61257233.4/30/105493567/32/8:49.82";

            var billInformation = BillInformation.DecodeBillInformationText(rawBillInformation);

            Assert.Empty(billInformation.VatImportTaxes);
        }

        [Fact]
        public void DecodeBillInformationText_SingleCondition_IsMapped()
        {
            const string rawBillInformation =
                "//S1/10/4031202511/11/180107/20/61257233.4/30/105493567/32/8:49.82/33/2.5:14.85/40/0:30";
            const double expectedDiscount = 0;
            const int expectedDeadline = 30;

            var billInformation = BillInformation.DecodeBillInformationText(rawBillInformation);

            Assert.Collection(billInformation.Conditions,
                item =>
                {
                    Assert.Equal(expectedDiscount, item.Key);
                    Assert.Equal(expectedDeadline, item.Value);
                });
        }

        [Fact]
        public void DecodeBillInformationText_MultipleConditions_AreMapped()
        {
            const string rawBillInformation =
                "//S1/10/X.66711-8824/11/200712/20/MW-2020-04/30/107978798/32/2.5:117.22/40/3:5;1.5:20;1:40;0:60";
            const double expectedFirstDiscount = 3;
            const int expectedFirstDeadline = 5;
            const double expectedSecondDiscount = 1.5;
            const int expectedSecondDeadline = 20;
            const double expectedThirdDiscount = 1;
            const int expectedThirdDeadline = 40;
            const double expectedLastDiscount = 0;
            const int expectedLastDeadline = 60;

            var billInformation = BillInformation.DecodeBillInformationText(rawBillInformation);

            Assert.Collection(billInformation.Conditions,
                item =>
                {
                    Assert.Equal(expectedFirstDiscount, item.Key);
                    Assert.Equal(expectedFirstDeadline, item.Value);
                },
                item =>
                {
                    Assert.Equal(expectedSecondDiscount, item.Key);
                    Assert.Equal(expectedSecondDeadline, item.Value);
                },
                item =>
                {
                    Assert.Equal(expectedThirdDiscount, item.Key);
                    Assert.Equal(expectedThirdDeadline, item.Value);
                },
                item =>
                {
                    Assert.Equal(expectedLastDiscount, item.Key);
                    Assert.Equal(expectedLastDeadline, item.Value);
                });
        }

        [Fact]
        public void DecodeBillInformationText_NoCondition_AreEmpty()
        {
            const string rawBillInformation = "//S1/10/4031202511/11/180107/20/61257233.4/30/105493567/32/8:49.82/33/2.5:14.85";

            var billInformation = BillInformation.DecodeBillInformationText(rawBillInformation);

            Assert.Empty(billInformation.Conditions);
        }

        [Theory]
        [InlineData(
            "//S1/10/100000/11/200114/40/5.00:10;2.00:20;0.00:30", "200213")]
        [InlineData(
            "//S1/10/4031202511/11/180107/20/61257233.4/30/105493567/32/8:49.82/33/2.5:14.85/40/0:30", "180206")]
        [InlineData(
            "//S1/10/4031202511/11/180107/20/61257233.4/30/105493567/32/8:49.82/33/2.5:14.85/40", null)]
        [InlineData(
            "//S1/10/4031202511/20/61257233.4/30/105493567/32/8:49.82/33/2.5:14.85/40/0:30", null)]
        [InlineData(
            "//S1/10/4031202511/20/61257233.4/30/105493567/32/8:49.82/33/2.5:14.85/0:30", null)]
        [InlineData("//S1/10/X.66711-8824/11/200712/20/MW-2020-04/30/107978798/32/2.5:117.22/40/3:5;1.5:20;1:40;0:60",
            "200910")]
        public void DecodeBillInformationText_DueDate_IsMapped(string rawBillInformation, string expectedDueDate)
        {
            var dueDate = string.IsNullOrEmpty(expectedDueDate)
                ? (DateTime?) null
                : DateTime.ParseExact(
                    expectedDueDate,
                    "yyMMdd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None);

            var billInformation = BillInformation.DecodeBillInformationText(rawBillInformation);

            if (string.IsNullOrEmpty(expectedDueDate))
            {
                Assert.Null(billInformation.DueDate);
            }
            else
            {
                Assert.NotNull(billInformation.DueDate);
                Assert.Equal(dueDate, billInformation.DueDate);
            }
        }
    }
}
