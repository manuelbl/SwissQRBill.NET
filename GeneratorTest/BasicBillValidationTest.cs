//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using System.Collections.Generic;
using Xunit;
using static Codecrete.SwissQRBill.Generator.ValidationMessage;

namespace Codecrete.SwissQRBill.GeneratorTest
{
    public class BasicBillValidationTest : BillDataValidationBase
    {
        [Fact]
        private void ValidCurrency()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.Currency = "CHF";
            Validate();
            AssertNoMessages();
            Assert.Equal("CHF", ValidatedBill.Currency);
        }

        [Fact]
        private void MissingCurrency()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.Currency = null;
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldCurrency, "field_is_mandatory");
        }

        [Fact]
        private void InvalidCurrency()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.Currency = "USD";
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldCurrency, "currency_is_chf_or_eur");
        }

        [Fact]
        private void OpenAmount()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.Amount = null;
            Validate();
            AssertNoMessages();
            Assert.Null(ValidatedBill.Amount);
        }

        [Fact]
        private void ValidAmount()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.Amount = 100.15m;
            Validate();
            AssertNoMessages();
            Assert.Equal(100.15m, ValidatedBill.Amount);
        }

        [Fact]
        private void AmountTooLow()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.Amount = -0.01m;
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldAmount, "amount_in_valid_range");
        }

        [Fact]
        private void AmountTooHigh2()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.Amount = 1000000000.0m;
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldAmount, "amount_in_valid_range");
        }

        [Fact]
        private void ValidChAccount()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.Account = "CH4431999123000889012";
            Validate();
            AssertNoMessages();
            Assert.Equal("CH4431999123000889012", ValidatedBill.Account);
        }

        [Fact]
        private void ValidLiAccount()
        {
            SourceBill = SampleData.CreateExample3();
            SourceBill.Account = "LI56 0880 0000 0209 4080 8";
            Validate();
            AssertNoMessages();
            Assert.Equal("LI5608800000020940808", ValidatedBill.Account);
        }

        [Fact]
        private void ValidAccountWithSpaces()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.Account = " CH44 3199 9123 0008 89012";
            Validate();
            AssertNoMessages();
            Assert.Equal("CH4431999123000889012", ValidatedBill.Account);
        }

        [Fact]
        private void MissingAccount()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.Account = null;
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldAccount, "field_is_mandatory");
        }

        [Fact]
        private void ForeignAccount()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.Account = "DE68 2012 0700 3100 7555 55";
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldAccount, "account_is_ch_li_iban");
        }

        [Fact]
        private void InvalidIban1()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.Account = "CH0031999123000889012";
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldAccount, "account_is_valid_iban");
        }

        [Fact]
        private void InvalidIban2()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.Account = "CH503199912300088333339012";
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldAccount, "account_is_valid_iban");
        }

        [Fact]
        private void ValidUnstructuredMessage()
        {
            SourceBill = SampleData.CreateExample1();

            SourceBill.UnstructuredMessage = "Bill no 39133";
            Validate();
            AssertNoMessages();
            Assert.Equal("Bill no 39133", ValidatedBill.UnstructuredMessage);
        }

        [Fact]
        private void EmptyUnstructureMessage()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.UnstructuredMessage = "   ";
            Validate();
            AssertNoMessages();
            Assert.Null(ValidatedBill.UnstructuredMessage);
        }

        [Fact]
        private void UnstructuredMessageWithLeadingAndTrailingWhitespace()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.UnstructuredMessage = "  Bill no 39133 ";
            Validate();
            AssertNoMessages();
            Assert.Equal("Bill no 39133", ValidatedBill.UnstructuredMessage);
        }

        [Fact]
        private void TooLongBillInformation()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.UnstructuredMessage = null;
            SourceBill.BillInformation = "//AA4567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789x";
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldBillInformation, "field_value_too_long");
        }

        [Fact]
        private void InvalidBillInformation1()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.BillInformation = "ABCD";
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldBillInformation, "bill_info_invalid");
        }

        [Fact]
        private void InvalidBillInformation2()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.BillInformation = "//A";
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldBillInformation, "bill_info_invalid");
        }

        [Fact]
        private void TooLongAdditionalInfo()
        {
            SourceBill = SampleData.CreateExample6();
            Assert.Equal(140, SourceBill.UnstructuredMessage.Length + SourceBill.BillInformation.Length);
            SourceBill.UnstructuredMessage = SourceBill.UnstructuredMessage + "A";
            Validate();

            Assert.True(Result.HasErrors);
            Assert.False(Result.HasWarnings);
            Assert.True(Result.HasMessages);
            Assert.Equal(2, Result.ValidationMessages.Count);

            ValidationMessage msg = Result.ValidationMessages[0];
            Assert.Equal(MessageType.Error, msg.Type);
            Assert.Equal(ValidationConstants.FieldUnstructuredMessage, msg.Field);
            Assert.Equal(ValidationConstants.KeyAdditionalInfoTooLong, msg.MessageKey);

            msg = Result.ValidationMessages[1];
            Assert.Equal(MessageType.Error, msg.Type);
            Assert.Equal(ValidationConstants.FieldBillInformation, msg.Field);
            Assert.Equal(ValidationConstants.KeyAdditionalInfoTooLong, msg.MessageKey);
        }

        [Fact]
        private void TooManyAltSchemes()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.AlternativeSchemes = new List<AlternativeScheme> {
                new AlternativeScheme{ Name = "Ultraviolet", Instruction = "UV;UltraPay005;12345" },
                new AlternativeScheme{ Name = "Xing Yong", Instruction = "XY;XYService;54321" },
                new AlternativeScheme{ Name = "Too Much", Instruction = "TM/asdfa/asdfa/" }
            };
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldAlternativeSchemes, "alt_scheme_max_exceed");
        }

        [Fact]
        private void TooLongAltSchemeInstructions()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.AlternativeSchemes = new List<AlternativeScheme> {
                new AlternativeScheme{ Name = "Ultraviolet", Instruction =
                        "UV;UltraPay005;12345;xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx" },
                new AlternativeScheme{ Name = "Xing Yong", Instruction = "XY;XYService;54321" }
            };
            Validate();
            AssertSingleErrorMessage(ValidationConstants.FieldAlternativeSchemes, "field_value_too_long");
        }
    }
}
