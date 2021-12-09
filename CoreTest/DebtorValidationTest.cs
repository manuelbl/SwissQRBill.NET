//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using Xunit;

namespace Codecrete.SwissQRBill.CoreTest
{
    public class DebtorValidationTest : BillDataValidationBase
    {
        [Fact]
        public void ValidDebtor()
        {
            SourceBill = SampleData.CreateExample1();

            Address address = CreateValidPerson();
            SourceBill.Debtor = address;
            Validate();
            AssertNoMessages();
            Assert.NotNull(ValidatedBill.Debtor);
            Assert.Equal("Zuppinger AG", ValidatedBill.Debtor.Name);
            Assert.Equal("Industriestrasse", ValidatedBill.Debtor.Street);
            Assert.Equal("34a", ValidatedBill.Debtor.HouseNo);
            Assert.Equal("9548", ValidatedBill.Debtor.PostalCode);
            Assert.Equal("Matzingen", ValidatedBill.Debtor.Town);
            Assert.Equal("CH", ValidatedBill.Debtor.CountryCode);
        }

        [Fact]
        public void EmptyDebtor()
        {
            SourceBill = SampleData.CreateExample1();
            Address emptyAddress = new Address();
            SourceBill.Debtor = emptyAddress;
            Validate();
            AssertNoMessages();
            Assert.Null(ValidatedBill.Debtor);
        }

        [Fact]
        public void EmptyDebtorWithSpaces()
        {
            SourceBill = SampleData.CreateExample1();
            Address emptyAddress = new Address
            {
                Name = "  "
            };
            SourceBill.Debtor = emptyAddress;
            Validate();
            AssertNoMessages();
            Assert.Null(ValidatedBill.Debtor);
        }

        [Fact]
        public void MissingName()
        {
            SourceBill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Name = "  ";
            SourceBill.Debtor = address;
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldDebtorName, ValidationConstants.KeyFieldValueMissing);
        }

        [Fact]
        public void MissingTown()
        {
            SourceBill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Town = null;
            SourceBill.Debtor = address;
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldDebtorTown, ValidationConstants.KeyFieldValueMissing);
        }

        [Fact]
        public void OpenDebtor()
        {
            SourceBill = SampleData.CreateExample1();

            SourceBill.Debtor = null;
            Validate();
            AssertNoMessages();
            Assert.Null(ValidatedBill.Debtor);
        }

        [Fact]
        public void DebtorWithInvalidCountryCode()
        {
            SourceBill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.CountryCode = "00";
            SourceBill.Debtor = address;
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldDebtorCountryCode, ValidationConstants.KeyCountryCodeInvalid);
        }

        [Fact]
        public void CreditorWithInvalidCountryCode2()
        {
            SourceBill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.CountryCode = "aà";
            SourceBill.Creditor = address;
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldCreditorCountryCode, ValidationConstants.KeyCountryCodeInvalid);
        }
    }
}
