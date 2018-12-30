//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using Xunit;
using static Codecrete.SwissQRBill.Generator.ValidationMessage;

namespace Codecrete.SwissQRBill.GeneratorTest
{
    public class CreditorValidationTest : BillDataValidationBase
    {
        [Fact]
        void ValidCreditor()
        {
            bill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            bill.Creditor = address;
            Validate();
            AssertNoMessages();
            Assert.NotNull(validatedBill.Creditor);
            Assert.Equal("Zuppinger AG", validatedBill.Creditor.Name);
            Assert.Equal("Industriestrasse", validatedBill.Creditor.Street);
            Assert.Equal("34a", validatedBill.Creditor.HouseNo);
            Assert.Equal("9548", validatedBill.Creditor.PostalCode);
            Assert.Equal("Matzingen", validatedBill.Creditor.Town);
            Assert.Equal("CH", validatedBill.Creditor.CountryCode);
        }

        [Fact]
        void MissingCreditor()
        {
            bill = SampleData.CreateExample1();
            bill.Creditor = null;
            Validate();
            AssertMandatoryPersonMessages();
        }

        [Fact]
        void EmptyCreditor()
        {
            bill = SampleData.CreateExample1();
            Address emptyAddress = new Address();
            bill.Creditor = emptyAddress;
            Validate();
            AssertMandatoryPersonMessages();
        }

        [Fact]
        void EmptyCreditorWithSpaces()
        {
            bill = SampleData.CreateExample1();
            Address emptyAddress = new Address
            {
                Name = "  "
            };
            bill.Creditor = emptyAddress;
            Validate();
            AssertMandatoryPersonMessages();
        }

        [Fact]
        void MissingCreditorName()
        {
            bill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Name = "  ";
            bill.Creditor = address;
            Validate();
            AssertSingleErrorMessage(Bill.FieldCreditorName, "field_is_mandatory");
        }

        [Fact]
        void CreditorWithoutStreet()
        {
            bill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Street = null;
            bill.Creditor = address;
            Validate();
            AssertNoMessages();
        }

        [Fact]
        void CreditorWithoutHouseNo()
        {
            bill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.HouseNo = null;
            bill.Creditor = address;
            Validate();
            AssertNoMessages();
        }

        [Fact]
        void CreditorWithMissingPostalCode()
        {
            bill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.PostalCode = "";
            bill.Creditor = address;
            Validate();
            AssertSingleErrorMessage(Bill.FieldCreditorPostalCode, "field_is_mandatory");
        }

        [Fact]
        void CreditorWithMissingTown()
        {
            bill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Town = null;
            bill.Creditor = address;
            Validate();
            AssertSingleErrorMessage(Bill.FieldCreditorTown, "field_is_mandatory");
        }

        [Fact]
        void CreditorWithMissingCountryCode()
        {
            bill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.CountryCode = "  ";
            bill.Creditor = address;
            Validate();
            AssertSingleErrorMessage(Bill.FieldCreditorCountryCode, "field_is_mandatory");
        }

        [Fact]
        void CreditorWithInvalidCountryCode()
        {
            bill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.CountryCode = "Schweiz";
            bill.Creditor = address;
            Validate();
            AssertSingleErrorMessage(Bill.FieldCreditorCountryCode, "valid_country_code");
        }

        [Fact]
        void CreditorWithInvalidCounturyCode2()
        {
            bill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.CountryCode = "R!";
            bill.Creditor = address;
            Validate();
            AssertSingleErrorMessage(Bill.FieldCreditorCountryCode, "valid_country_code");
        }

        [Fact]
        void CreditorWithConflictingAddress()
        {
            bill = SampleData.CreateExample1();
            bill.Creditor.AddressLine1 = "Conflict";
            Validate();
            Assert.True(result.HasErrors);
            Assert.False(result.HasWarnings);
            Assert.True(result.HasMessages);
            Assert.Equal(5, result.ValidationMessages.Count);
            foreach (ValidationMessage msg in result.ValidationMessages)
            {
                Assert.Equal(MessageType.Error, msg.Type);
                Assert.Equal("adress_type_conflict", msg.MessageKey);
                Assert.StartsWith(Bill.FieldRootCreditor, msg.Field);
            }
        }

        private void AssertMandatoryPersonMessages()
        {
            Assert.True(result.HasErrors);
            Assert.False(result.HasWarnings);
            Assert.True(result.HasMessages);
            Assert.Equal(5, result.ValidationMessages.Count);
            foreach (ValidationMessage msg in result.ValidationMessages)
            {
                Assert.Equal(MessageType.Error, msg.Type);
                Assert.Equal("field_is_mandatory", msg.MessageKey);
                Assert.StartsWith(Bill.FieldRootCreditor, msg.Field);
            }
        }
    }
}
