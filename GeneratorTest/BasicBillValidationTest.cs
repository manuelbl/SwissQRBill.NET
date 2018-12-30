//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using System.Collections.Generic;
using Xunit;

namespace Codecrete.SwissQRBill.GeneratorTest
{
    public class BasicBillValidationTest : BillDataValidationBase
    {
        [Fact]
        void ValidCurrency()
        {
            bill = SampleData.CreateExample1();
            bill.Currency = "CHF";
            Validate();
            AssertNoMessages();
            Assert.Equal("CHF", validatedBill.Currency);
        }

        [Fact]
        void MissingCurrency()
        {
            bill = SampleData.CreateExample1();
            bill.Currency = null;
            Validate();
            AssertSingleErrorMessage(Bill.FieldCurrency, "field_is_mandatory");
        }

        [Fact]
        void InvalidCurrency()
        {
            bill = SampleData.CreateExample1();
            bill.Currency = "USD";
            Validate();
            AssertSingleErrorMessage(Bill.FieldCurrency, "currency_is_chf_or_eur");
        }

        [Fact]
        void OpenAmount()
        {
            bill = SampleData.CreateExample1();
            bill.Amount = null;
            Validate();
            AssertNoMessages();
            Assert.Null(validatedBill.Amount);
        }

        [Fact]
        void ValidAmount()
        {
            bill = SampleData.CreateExample1();
            bill.Amount = 100.15m;
            Validate();
            AssertNoMessages();
            Assert.Equal(100.15m, validatedBill.Amount);
        }

        [Fact]
        void AmountTooLow()
        {
            bill = SampleData.CreateExample1();
            bill.Amount = 0m;
            Validate();
            AssertSingleErrorMessage(Bill.FieldAmount, "amount_in_valid_range");
        }

        [Fact]
        void AmountTooHigh2()
        {
            bill = SampleData.CreateExample1();
            bill.Amount = 1000000000.0m;
            Validate();
            AssertSingleErrorMessage(Bill.FieldAmount, "amount_in_valid_range");
        }

        [Fact]
        void ValidCHAccount()
        {
            bill = SampleData.CreateExample1();
            bill.Account = "CH4431999123000889012";
            Validate();
            AssertNoMessages();
            Assert.Equal("CH4431999123000889012", validatedBill.Account);
        }

        [Fact]
        void ValidLIAccount()
        {
            bill = SampleData.CreateExample3();
            bill.Account = "LI56 0880 0000 0209 4080 8";
            Validate();
            AssertNoMessages();
            Assert.Equal("LI5608800000020940808", validatedBill.Account);
        }

        [Fact]
        void ValidAccountWithSpaces()
        {
            bill = SampleData.CreateExample1();
            bill.Account = " CH44 3199 9123 0008 89012";
            Validate();
            AssertNoMessages();
            Assert.Equal("CH4431999123000889012", validatedBill.Account);
        }

        [Fact]
        void MissingAccount()
        {
            bill = SampleData.CreateExample1();
            bill.Account = null;
            Validate();
            AssertSingleErrorMessage(Bill.FieldAccount, "field_is_mandatory");
        }

        [Fact]
        void ForeignAccount()
        {
            bill = SampleData.CreateExample1();
            bill.Account = "DE68 2012 0700 3100 7555 55";
            Validate();
            AssertSingleErrorMessage(Bill.FieldAccount, "account_is_ch_li_iban");
        }

        [Fact]
        void InvalidIBAN1()
        {
            bill = SampleData.CreateExample1();
            bill.Account = "CH0031999123000889012";
            Validate();
            AssertSingleErrorMessage(Bill.FieldAccount, "account_is_valid_iban");
        }

        [Fact]
        void InvalidIBAN2()
        {
            bill = SampleData.CreateExample1();
            bill.Account = "CH503199912300088333339012";
            Validate();
            AssertSingleErrorMessage(Bill.FieldAccount, "account_is_valid_iban");
        }

        [Fact]
        void ValidUnstructuredMessage()
        {
            bill = SampleData.CreateExample1();

            bill.UnstructuredMessage = "Bill no 39133";
            Validate();
            AssertNoMessages();
            Assert.Equal("Bill no 39133", validatedBill.UnstructuredMessage);
        }

        [Fact]
        void EmptyUnstructureMessage()
        {
            bill = SampleData.CreateExample1();
            bill.UnstructuredMessage = "   ";
            Validate();
            AssertNoMessages();
            Assert.Null(validatedBill.UnstructuredMessage);
        }

        [Fact]
        void UnstructuredMessageWithLeadingAndTrailingWhitespace()
        {
            bill = SampleData.CreateExample1();
            bill.UnstructuredMessage = "  Bill no 39133 ";
            Validate();
            AssertNoMessages();
            Assert.Equal("Bill no 39133", validatedBill.UnstructuredMessage);
        }

        [Fact]
        void TooLongBillInformation()
        {
            bill = SampleData.CreateExample1();
            bill.BillInformation = "//AA4567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789x";
            Validate();
            AssertSingleErrorMessage(Bill.FieldBillInformation, "field_value_too_long");
        }

        [Fact]
        void InvalidBillInformation1()
        {
            bill = SampleData.CreateExample1();
            bill.BillInformation = "ABCD";
            Validate();
            AssertSingleErrorMessage(Bill.FieldBillInformation, "bill_info_invalid");
        }

        [Fact]
        void InvalidBillInformation2()
        {
            bill = SampleData.CreateExample1();
            bill.BillInformation = "//A";
            Validate();
            AssertSingleErrorMessage(Bill.FieldBillInformation, "bill_info_invalid");
        }

        [Fact]
        void TooManyAltSchemes()
        {
            bill = SampleData.CreateExample1();
            bill.AlternativeSchemes = new List<AlternativeScheme> {
                new AlternativeScheme{ Name = "Ultraviolet", Instruction = "UV;UltraPay005;12345" },
                new AlternativeScheme{ Name = "Xing Yong", Instruction = "XY;XYService;54321" },
                new AlternativeScheme{ Name = "Too Much", Instruction = "TM/asdfa/asdfa/" }
            };
            Validate();
            AssertSingleErrorMessage(Bill.FieldAlternativeSchemes, "alt_scheme_max_exceed");
        }

        [Fact]
        void TooLongAltSchemeInstructions()
        {
            bill = SampleData.CreateExample1();
            bill.AlternativeSchemes = new List<AlternativeScheme> {
                new AlternativeScheme{ Name = "Ultraviolet", Instruction =
                        "UV;UltraPay005;12345;xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx" },
                new AlternativeScheme{ Name = "Xing Yong", Instruction = "XY;XYService;54321" }
            };
            Validate();
            AssertSingleErrorMessage(Bill.FieldAlternativeSchemes, "field_value_too_long");
        }
    }
}
