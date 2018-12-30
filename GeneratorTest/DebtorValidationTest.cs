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
    public class DebtorValidationTest : BillDataValidationBase
    {
        [Fact]
        void ValidDebtor()
        {
            bill = SampleData.CreateExample1();

            Address address = CreateValidPerson();
            bill.Debtor = address;
            Validate();
            AssertNoMessages();
            Assert.NotNull(validatedBill.Debtor);
            Assert.Equal("Zuppinger AG", validatedBill.Debtor.Name);
            Assert.Equal("Industriestrasse", validatedBill.Debtor.Street);
            Assert.Equal("34a", validatedBill.Debtor.HouseNo);
            Assert.Equal("9548", validatedBill.Debtor.PostalCode);
            Assert.Equal("Matzingen", validatedBill.Debtor.Town);
            Assert.Equal("CH", validatedBill.Debtor.CountryCode);
        }

        [Fact]
        void EmptyDebtor()
        {
            bill = SampleData.CreateExample1();
            Address emptyAddress = new Address();
            bill.Debtor = emptyAddress;
            Validate();
            AssertNoMessages();
            Assert.Null(validatedBill.Debtor);
        }

        [Fact]
        void EmptyDebtorWithSpaces()
        {
            bill = SampleData.CreateExample1();
            Address emptyAddress = new Address
            {
                Name = "  "
            };
            bill.Debtor = emptyAddress;
            Validate();
            AssertNoMessages();
            Assert.Null(validatedBill.Debtor);
        }

        [Fact]
        void MissingName()
        {
            bill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Name = "  ";
            bill.Debtor = address;
            Validate();
            AssertSingleErrorMessage(Bill.FieldDebtorName, "field_is_mandatory");
        }

        [Fact]
        void MissingTown()
        {
            bill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Town = null;
            bill.Debtor = address;
            Validate();
            AssertSingleErrorMessage(Bill.FieldDebtorTown, "field_is_mandatory");
        }

        [Fact]
        void OpenDebtor()
        {
            bill = SampleData.CreateExample1();

            bill.Debtor = null;
            Validate();
            AssertNoMessages();
            Assert.Null(validatedBill.Debtor);
        }
    }
}
