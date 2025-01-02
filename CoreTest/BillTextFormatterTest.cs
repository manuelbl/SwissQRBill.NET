using Codecrete.SwissQRBill.Generator;
using Xunit;

namespace Codecrete.SwissQRBill.CoreTest
{
    public class BillTextFormatterTest
    {
        private BillTextFormatter textFormatter = new BillTextFormatter(SampleData.CreateExample1());

        [Fact]
        public void PayableTo_IsCorrect()
        {
            Assert.Equal(
                    "CH44 3199 9123 0008 8901 2\nRobert Schneider AG\nRue du Lac 1268/2/22\n2501 Biel",
                    textFormatter.PayableTo
            );

        }

        [Fact]
        public void PayableToReduced_IsCorrect()
        {
            Assert.Equal(
                    "CH44 3199 9123 0008 8901 2\nRobert Schneider AG\n2501 Biel",
                    textFormatter.PayableToReduced
            );
        }


        [Fact]
        public void Account_IsCorrect()
        {
            Assert.Equal("CH44 3199 9123 0008 8901 2", textFormatter.Account);
        }

        [Fact]
        public void CreditorAdddress_IsCorrect()
        {
            Assert.Equal(
                    "Robert Schneider AG\nRue du Lac 1268/2/22\n2501 Biel",
                    textFormatter.CreditorAddress);
        }

        [Fact]
        public void CreditorAddressReduced_IsCorrect()
        {
            Assert.Equal(
                    "Robert Schneider AG\n2501 Biel",
                    textFormatter.CreditorAddressReduced);
        }

        [Fact]
        public void Reference_IsCorrect()
        {
            Assert.Equal("21 00000 00003 13947 14300 09017", textFormatter.Reference);
        }

        [Fact]
        public void EmptyReference_BecomesNull()
        {
            textFormatter = new BillTextFormatter(SampleData.CreateExample2());
            Assert.Null(textFormatter.Reference);
        }

        [Fact]
        public void Amount_IsCorrect()
        {
            Assert.Equal("123 949.75", textFormatter.Amount);
        }

        [Fact]
        public void MissingAmount_BecomesNull()
        {
            textFormatter = new BillTextFormatter(SampleData.CreateExample2());
            Assert.Null(textFormatter.Amount);
        }

        [Fact]
        public void PayableBy_IsCorrect()
        {
            Assert.Equal(
                    "Pia-Maria Rutschmann-Schnyder\nGrosse Marktgasse 28\n9400 Rorschach",
                    textFormatter.PayableBy
            );
        }

        [Fact]
        public void EmptyPayableBy_BecomesNull()
        {
            textFormatter = new BillTextFormatter(SampleData.CreateExample2());
            Assert.Null(textFormatter.PayableBy);
        }

        [Fact]
        public void PayableByReduced_IsCorrect()
        {
            Assert.Equal("Pia-Maria Rutschmann-Schnyder\n9400 Rorschach", textFormatter.PayableByReduced);
        }

        [Fact]
        public void EmptyPayableByReduced_BecomesNull()
        {
            textFormatter = new BillTextFormatter(SampleData.CreateExample2());
            Assert.Null(textFormatter.PayableByReduced);
        }

        [Fact]
        public void AdditionalInformationWithTwoParts_IsCorrect()
        {
            Assert.Equal(
                    "Instruction of 15.09.2019\n//S1/10/10201409/11/190512/20/1400.000-53/30/106017086/31/180508/32/7.7/40/2:10;0:30",
                    textFormatter.AdditionalInformation
            );
        }

        [Fact]
        public void AdditionalInformationWithUnstructuredMessage_IsCorrect()
        {
            textFormatter = new BillTextFormatter(SampleData.CreateExample2());
            Assert.Equal(
                    "Donation to the Winterfest Campaign",
                    textFormatter.AdditionalInformation
            );
        }

        [Fact]
        public void AdditionalInformationWithBillInformation_IsCorrect()
        {
            Bill bill = SampleData.CreateExample1();
            bill.UnstructuredMessage = "";
            textFormatter = new BillTextFormatter(bill);
            Assert.Equal(
                    "//S1/10/10201409/11/190512/20/1400.000-53/30/106017086/31/180508/32/7.7/40/2:10;0:30",
                    textFormatter.AdditionalInformation
            );
        }

        [Fact]
        public void NoAdditionalInformation_BecomesNull()
        {
            textFormatter = new BillTextFormatter(SampleData.CreateExample3());
            Assert.Null(textFormatter.AdditionalInformation);
        }

    }
}
