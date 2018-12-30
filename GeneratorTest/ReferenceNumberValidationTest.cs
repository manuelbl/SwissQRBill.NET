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
    public class ReferenceNumberValidationTest : BillDataValidationBase
    {

        [Fact]
        void ValidQRReference()
        {
            bill = SampleData.CreateExample1();
            bill.Reference = "210000000003139471430009017";
            Validate();
            AssertNoMessages();
            Assert.Equal("210000000003139471430009017", validatedBill.Reference);
        }

        [Fact]
        void ValidQRReferenceWithSpaces()
        {
            bill = SampleData.CreateExample1();
            bill.Reference = "21 00000 00003 13947 14300 09017";
            Validate();
            AssertNoMessages();
            Assert.Equal("210000000003139471430009017", validatedBill.Reference);
        }

        [Fact]
        void ValidCreditorReference()
        {
            bill = SampleData.CreateExample3();
            bill.Reference = "RF18539007547034";
            Validate();
            AssertNoMessages();
            Assert.Equal("RF18539007547034", validatedBill.Reference);
        }

        [Fact]
        void QrIBANNoAndQRReference()
        {
            bill = SampleData.CreateExample1();
            bill.Account = "CH3709000000304442225"; // non QR-IBAN
            bill.Reference = null;
            Validate();
            AssertNoMessages();
            Assert.Null(validatedBill.Reference);
        }

        [Fact]
        void WhitespaceReference()
        {
            bill = SampleData.CreateExample1();
            bill.Account = "CH3709000000304442225"; // non QR-IBAN
            bill.Reference = "   ";
            Validate();
            AssertNoMessages();
            Assert.Null(validatedBill.Reference);
        }

        [Fact]
        void MissingReferenceForQRIBAN()
        {
            bill = SampleData.CreateExample1();
            bill.Account = "CH4431999123000889012"; // QR-IBAN
            bill.Reference = null;
            Validate();
            AssertSingleErrorMessage(Bill.FieldReference, "mandatory_for_qr_iban");
        }

        [Fact]
        void WhitespaceReferenceForQRIBAN()
        {
            bill = SampleData.CreateExample1();
            bill.Reference = "   ";
            Validate();
            AssertSingleErrorMessage(Bill.FieldReference, "mandatory_for_qr_iban");
        }

        [Fact]
        void InvalidReference()
        {
            bill = SampleData.CreateExample1();
            bill.Reference = "ABC";
            Validate();
            AssertSingleErrorMessage(Bill.FieldReference, "valid_qr_ref_no");
        }

        [Fact]
        void InvalidNumericReference()
        {
            bill = SampleData.CreateExample1();
            bill.Reference = "1234567890";
            Validate();
            AssertSingleErrorMessage(Bill.FieldReference, "valid_qr_ref_no");
        }

        [Fact]
        void InvalidNonNumericReference()
        {
            bill = SampleData.CreateExample1();
            bill.Reference = "123ABC7890";
            Validate();
            AssertSingleErrorMessage(Bill.FieldReference, "valid_qr_ref_no");
        }

        [Fact]
        void InvalidCharsInCreditorReference()
        {
            bill = SampleData.CreateExample3();
            bill.Reference = "RF38302!!3393";
            Validate();
            AssertSingleErrorMessage(Bill.FieldReference, "valid_iso11649_creditor_ref");
        }

        [Fact]
        void InvalidCreditorReference()
        {
            bill = SampleData.CreateExample3();
            bill.Reference = "RF00539007547034";
            Validate();
            AssertSingleErrorMessage(Bill.FieldReference, "valid_iso11649_creditor_ref");
        }
    }
}
