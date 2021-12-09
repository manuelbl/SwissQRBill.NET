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
    public class ReferenceNumberValidationTest : BillDataValidationBase
    {

        [Fact]
        public void ValidQrRef_Ok()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.Reference = "210000000003139471430009017";
            Validate();
            AssertNoMessages();
            Assert.Equal("210000000003139471430009017", ValidatedBill.Reference);
        }

        [Fact]
        public void ValidQrRefWithSpaces_Ok()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.Reference = "21 00000 00003 13947 14300 09017";
            Validate();
            AssertNoMessages();
            Assert.Equal("210000000003139471430009017", ValidatedBill.Reference);
        }

        [Fact]
        public void ValidCreditorRef_Ok()
        {
            SourceBill = SampleData.CreateExample3();
            SourceBill.Reference = "RF18539007547034";
            Validate();
            AssertNoMessages();
            Assert.Equal("RF18539007547034", ValidatedBill.Reference);
        }

        [Fact]
        public void NonQrIbanAndNoRef_Ok()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.Account = "CH3709000000304442225"; // non QR-IBAN
            SourceBill.Reference = null;
            Validate();
            AssertNoMessages();
            Assert.Null(ValidatedBill.Reference);
        }

        [Fact]
        public void NonQrIbanAndWhitespaceRef_Ok()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.Account = "CH3709000000304442225"; // non QR-IBAN
            SourceBill.Reference = "   ";
            Validate();
            AssertNoMessages();
            Assert.Null(ValidatedBill.Reference);
        }

        [Fact]
        public void InvalidRef_RefIsValidErr()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.Reference = "ABC";
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldReference, ValidationConstants.KeyRefInvalid);
        }

        [Fact]
        public void InvalidNumericRef_ReffIsValidErr()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.Reference = "1234567890";
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldReference, ValidationConstants.KeyRefInvalid);
        }

        [Fact]
        public void InvalidAlphaNumericRef_RefIsValidErr()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.Reference = "123ABC7890";
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldReference, ValidationConstants.KeyRefInvalid);
        }

        [Fact]
        public void InvalidCharInRef_RefIsValidErr()
        {
            SourceBill = SampleData.CreateExample3();
            SourceBill.Reference = "RF38302!!3393";
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldReference, ValidationConstants.KeyRefInvalid);
        }

        [Fact]
        public void InvalidCreditorRef_RefIsValidErr()
        {
            SourceBill = SampleData.CreateExample3();
            SourceBill.Reference = "RF00539007547034";
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldReference, ValidationConstants.KeyRefInvalid);
        }

        [Fact]
        public void MissingRefForQrIban_QrRefIsMandatoryErr()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.Account = "CH4431999123000889012";  // QR-IBAN
            SourceBill.Reference = null;
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldReference, ValidationConstants.KeyQrRefMissing);
        }

        [Fact]
        public void WhitespaceRefForQrIban_QrRefIsMandatoryErr()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.Reference = "   ";
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldReference, ValidationConstants.KeyQrRefMissing);
        }

        [Fact]
        public void CreditRefForQrIban_UseQrRefForQrIbanErr()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.Account = "CH4431999123000889012"; // QR-IBAN
            SourceBill.Reference = "RF18539007547034";
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldReference, ValidationConstants.KeyCredRefInvalidUseForQrIban);
        }

        [Fact]
        public void QrRefForNonQrIban_UseQrIbanForQrRefErr()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.Account = "CH3709000000304442225"; // non QR-IBAN
            SourceBill.Reference = "210000000003139471430009017";
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldReference, ValidationConstants.KeyQrRefInvalidUseForNonQrIban);
        }

        [Fact]
        public void InvalidRefType_RefTypeIsValidErr()
        {
            SourceBill = SampleData.CreateExample3();
            SourceBill.ReferenceType = "ABC";
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldReferenceType, ValidationConstants.KeyRefTypeInvalid);
        }

        [Fact]
        public void InvalidRefTypeForCredRef_RefTypeIsValidErr()
        {
            SourceBill = SampleData.CreateExample3();
            SourceBill.ReferenceType = "QRR";
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldReferenceType, ValidationConstants.KeyRefTypeInvalid);
        }

        [Fact]
        public void InvalidRefTypeForQrRef_RefTypeIsValidErr()
        {
            SourceBill = SampleData.CreateExample3();
            SourceBill.Account = "CH4431999123000889012"; // non QR-IBAN
            SourceBill.Reference = "210000000003139471430009017";
            SourceBill.ReferenceType = "SCOR";
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldReferenceType, ValidationConstants.KeyRefTypeInvalid);
        }
    }
}
