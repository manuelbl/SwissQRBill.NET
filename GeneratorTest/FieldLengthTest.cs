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
        void MaximumNameLength()
        {
            bill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Name = "Name567890123456789012345678901234567890123456789012345678901234567890";
            bill.Creditor = address;
            Validate();
            AssertNoMessages();
            Assert.Equal("Name567890123456789012345678901234567890123456789012345678901234567890",
                    validatedBill.Creditor.Name);
        }

        [Fact]
        void ClippedName()
        {
            bill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Name = "Name5678901234567890123456789012345678901234567890123456789012345678901";
            bill.Creditor = address;
            Validate();
            AssertSingleWarningMessage(Bill.FieldCreditorName, "field_clipped");
            Assert.Equal("Name567890123456789012345678901234567890123456789012345678901234567890",
                    validatedBill.Creditor.Name);
        }

        [Fact]
        void MaximumStreetLength()
        {
            bill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Street = "Street7890123456789012345678901234567890123456789012345678901234567890";
            bill.Creditor = address;
            Validate();
            AssertNoMessages();
            Assert.Equal("Street7890123456789012345678901234567890123456789012345678901234567890",
                    validatedBill.Creditor.Street);
        }

        [Fact]
        void ClippedStreet()
        {
            bill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Street = "Street78901234567890123456789012345678901234567890123456789012345678901";
            bill.Creditor = address;
            Validate();
            AssertSingleWarningMessage(Bill.FieldCreditorStreet, "field_clipped");
            Assert.Equal("Street7890123456789012345678901234567890123456789012345678901234567890",
                    validatedBill.Creditor.Street);
        }

        [Fact]
        void MaximumHouseNoLength()
        {
            bill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.HouseNo = "HouseNo890123456";
            bill.Creditor = address;
            Validate();
            AssertNoMessages();
            Assert.Equal("HouseNo890123456", validatedBill.Creditor.HouseNo);
        }

        [Fact]
        void ClippedHouseNo()
        {
            bill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.HouseNo = "HouseNo8901234567";
            bill.Creditor = address;
            Validate();
            AssertSingleWarningMessage(Bill.FieldCreditorHouseNo, "field_clipped");
            Assert.Equal("HouseNo890123456", validatedBill.Creditor.HouseNo);
        }

        [Fact]
        void MaximumPostalCodeLength()
        {
            bill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.PostalCode = "Postal7890123456";
            bill.Creditor = address;
            Validate();
            AssertNoMessages();
            Assert.Equal("Postal7890123456", validatedBill.Creditor.PostalCode);
        }

        [Fact]
        void ClippedPostalCode()
        {
            bill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.PostalCode = "Postal78901234567";
            bill.Creditor = address;
            Validate();
            AssertSingleWarningMessage(Bill.FieldCreditorPostalCode, "field_clipped");
            Assert.Equal("Postal7890123456", validatedBill.Creditor.PostalCode);
        }

        [Fact]
        void MaximumTownLength()
        {
            bill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Town = "City5678901234567890123456789012345";
            bill.Creditor = address;
            Validate();
            AssertNoMessages();
            Assert.Equal("City5678901234567890123456789012345", validatedBill.Creditor.Town);
        }

        [Fact]
        void ClippedTown()
        {
            bill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Town = "City56789012345678901234567890123456";
            bill.Creditor = address;
            Validate();
            AssertSingleWarningMessage(Bill.FieldCreditorTown, "field_clipped");
            Assert.Equal("City5678901234567890123456789012345", validatedBill.Creditor.Town);
        }
    }
}
