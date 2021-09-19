//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using Xunit;

namespace Codecrete.SwissQRBill.GeneratorTest
{
    public class FieldLengthTest : BillDataValidationBase
    {

        [Fact]
        public void MaximumNameLength()
        {
            SourceBill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Name = "Name567890123456789012345678901234567890123456789012345678901234567890";
            SourceBill.Creditor = address;
            Validate();
            AssertNoMessages();
            Assert.Equal("Name567890123456789012345678901234567890123456789012345678901234567890",
                    ValidatedBill.Creditor.Name);
        }

        [Fact]
        public void ClippedName()
        {
            SourceBill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Name = "Name5678901234567890123456789012345678901234567890123456789012345678901";
            SourceBill.Creditor = address;
            Validate();
            AssertSingleWarningMessage(ValidationConstants.FieldCreditorName, ValidationConstants.KeyFieldValueClipped);
            Assert.Equal("Name567890123456789012345678901234567890123456789012345678901234567890",
                    ValidatedBill.Creditor.Name);
        }

        [Fact]
        public void MaximumStreetLength()
        {
            SourceBill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Street = "Street7890123456789012345678901234567890123456789012345678901234567890";
            SourceBill.Creditor = address;
            Validate();
            AssertNoMessages();
            Assert.Equal("Street7890123456789012345678901234567890123456789012345678901234567890",
                    ValidatedBill.Creditor.Street);
        }

        [Fact]
        public void ClippedStreet()
        {
            SourceBill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Street = "Street78901234567890123456789012345678901234567890123456789012345678901";
            SourceBill.Creditor = address;
            Validate();
            AssertSingleWarningMessage(ValidationConstants.FieldCreditorStreet, ValidationConstants.KeyFieldValueClipped);
            Assert.Equal("Street7890123456789012345678901234567890123456789012345678901234567890",
                    ValidatedBill.Creditor.Street);
        }

        [Fact]
        public void MaximumHouseNoLength()
        {
            SourceBill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.HouseNo = "HouseNo890123456";
            SourceBill.Creditor = address;
            Validate();
            AssertNoMessages();
            Assert.Equal("HouseNo890123456", ValidatedBill.Creditor.HouseNo);
        }

        [Fact]
        public void ClippedHouseNo()
        {
            SourceBill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.HouseNo = "HouseNo8901234567";
            SourceBill.Creditor = address;
            Validate();
            AssertSingleWarningMessage(ValidationConstants.FieldCreditorHouseNo, ValidationConstants.KeyFieldValueClipped);
            Assert.Equal("HouseNo890123456", ValidatedBill.Creditor.HouseNo);
        }

        [Fact]
        public void MaximumPostalCodeLength()
        {
            SourceBill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.PostalCode = "Postal7890123456";
            SourceBill.Creditor = address;
            Validate();
            AssertNoMessages();
            Assert.Equal("Postal7890123456", ValidatedBill.Creditor.PostalCode);
        }

        [Fact]
        public void ClippedPostalCode()
        {
            SourceBill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.PostalCode = "Postal78901234567";
            SourceBill.Creditor = address;
            Validate();
            AssertSingleWarningMessage(ValidationConstants.FieldCreditorPostalCode, ValidationConstants.KeyFieldValueClipped);
            Assert.Equal("Postal7890123456", ValidatedBill.Creditor.PostalCode);
        }

        [Fact]
        public void MaximumTownLength()
        {
            SourceBill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Town = "City5678901234567890123456789012345";
            SourceBill.Creditor = address;
            Validate();
            AssertNoMessages();
            Assert.Equal("City5678901234567890123456789012345", ValidatedBill.Creditor.Town);
        }

        [Fact]
        public void ClippedTown()
        {
            SourceBill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Town = "City56789012345678901234567890123456";
            SourceBill.Creditor = address;
            Validate();
            AssertSingleWarningMessage(ValidationConstants.FieldCreditorTown, ValidationConstants.KeyFieldValueClipped);
            Assert.Equal("City5678901234567890123456789012345", ValidatedBill.Creditor.Town);
        }
    }
}
