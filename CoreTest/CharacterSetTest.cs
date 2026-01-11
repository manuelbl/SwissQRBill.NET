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
    public class CharacterSetTest : BillDataValidationBase
    {
        [Fact]
        public void InvalidCharacterReplacement()
        {
            SourceBill = SampleData.CreateExample1();
            var address = CreateValidPerson();
            address.Street = "abc\uD83C\uDDE8\uD83C\uDDEDdef";
            SourceBill.Creditor = address;
            Validate();
            Assert.Equal("abc.def", ValidatedBill.Creditor.Street);
            AssertSingleWarningMessage(ValidationConstants.FieldCreditorStreet, ValidationConstants.KeyReplacedUnsupportedCharacters);
        }

        [Fact]
        public void UnstructuredMessageReplacement()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.UnstructuredMessage = "Thanks 🙏 Lisa";
            Validate();
            Assert.Equal("Thanks . Lisa", ValidatedBill.UnstructuredMessage);
            AssertSingleWarningMessage(ValidationConstants.FieldUnstructuredMessage, ValidationConstants.KeyReplacedUnsupportedCharacters);
        }

        [Fact]
        public void UnstructuredMessageReplacementLatin1Subset()
        {
            SourceBill = SampleData.CreateExample8();
            SourceBill.CharacterSet = SpsCharacterSet.Latin1Subset;
            Validate();

            Assert.False(Result.HasErrors);
            Assert.True(Result.HasWarnings);
            Assert.True(Result.HasMessages);
            Assert.Equal(3, Result.ValidationMessages.Count);
            foreach (ValidationMessage message in Result.ValidationMessages)
            {
                Assert.Equal(ValidationConstants.KeyReplacedUnsupportedCharacters, message.MessageKey);
            }

            Assert.Equal("Facture 48390, E10 de réduction", ValidatedBill.UnstructuredMessage);
            Assert.Equal("Bugra Çavdarli", ValidatedBill.Creditor.Name);
            Assert.Equal("L'OEil de Boeuf", ValidatedBill.Debtor.Name);
        }

        [Fact]
        public void UnstructuredMessageReplacementExtendedLatin()
        {
            SourceBill = SampleData.CreateExample8();
            SourceBill.CharacterSet = SpsCharacterSet.ExtendedLatin;
            Validate();
            AssertNoMessages();
        }

        [Fact]
        public void BillInfoReplacement()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.BillInformation = "//abc \uD83D\uDE00 def";
            Validate();
            Assert.Equal("//abc . def", ValidatedBill.BillInformation);
            AssertSingleWarningMessage(ValidationConstants.FieldBillInformation, ValidationConstants.KeyReplacedUnsupportedCharacters);
        }
    }
}
