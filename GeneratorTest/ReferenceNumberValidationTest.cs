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
        private void ValidQrReference()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.Reference = "210000000003139471430009017";
            Validate();
            AssertNoMessages();
            Assert.Equal("210000000003139471430009017", ValidatedBill.Reference);
        }

        [Fact]
        private void ValidQrReferenceWithSpaces()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.Reference = "21 00000 00003 13947 14300 09017";
            Validate();
            AssertNoMessages();
            Assert.Equal("210000000003139471430009017", ValidatedBill.Reference);
        }

        [Fact]
        private void ValidCreditorReference()
        {
            SourceBill = SampleData.CreateExample3();
            SourceBill.Reference = "RF18539007547034";
            Validate();
            AssertNoMessages();
            Assert.Equal("RF18539007547034", ValidatedBill.Reference);
        }

        [Fact]
        private void QrIbanNoAndQrReference()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.Account = "CH3709000000304442225"; // non QR-IBAN
            SourceBill.Reference = null;
            Validate();
            AssertNoMessages();
            Assert.Null(ValidatedBill.Reference);
        }

        [Fact]
        private void WhitespaceReference()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.Account = "CH3709000000304442225"; // non QR-IBAN
            SourceBill.Reference = "   ";
            Validate();
            AssertNoMessages();
            Assert.Null(ValidatedBill.Reference);
        }

        [Fact]
        private void MissingReferenceForQriban()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.Account = "CH4431999123000889012"; // QR-IBAN
            SourceBill.Reference = null;
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldReference, "mandatory_for_qr_iban");
        }

        [Fact]
        private void WhitespaceReferenceForQriban()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.Reference = "   ";
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldReference, "mandatory_for_qr_iban");
        }

        [Fact]
        private void InvalidReference()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.Reference = "ABC";
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldReference, "valid_qr_ref_no");
        }

        [Fact]
        private void InvalidNumericReference()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.Reference = "1234567890";
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldReference, "valid_qr_ref_no");
        }

        [Fact]
        private void InvalidNonNumericReference()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.Reference = "123ABC7890";
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldReference, "valid_qr_ref_no");
        }

        [Fact]
        private void InvalidCharsInCreditorReference()
        {
            SourceBill = SampleData.CreateExample3();
            SourceBill.Reference = "RF38302!!3393";
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldReference, "valid_iso11649_creditor_ref");
        }

        [Fact]
        private void InvalidCreditorReference()
        {
            SourceBill = SampleData.CreateExample3();
            SourceBill.Reference = "RF00539007547034";
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldReference, "valid_iso11649_creditor_ref");
        }

        [Fact]
        private void InvalidReferenceType()
        {
            SourceBill = SampleData.CreateExample3();
            SourceBill.ReferenceType = "ABC";
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldReferenceType, "valid_ref_type");
        }
    }
}
