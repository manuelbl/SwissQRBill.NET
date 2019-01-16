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
    public class CharacterSetTest : BillDataValidationBase
    {
        private static readonly string TextWithoutCombiningAccents = "àáâäçèéêëìíîïñòóôöùúûüýßÀÁÂÄÇÈÉÊËÌÍÎÏÒÓÔÖÙÚÛÜÑ";
        private static readonly string TextWithCombiningAccents    = "àáâäçèéêëìíîïñòóôöùúûüýßÀÁÂÄÇÈÉÊËÌÍÎÏÒÓÔÖÙÚÛÜÑ";

        [Fact]
        private void VerifyTestData()
        {
            Assert.Equal(46, TextWithoutCombiningAccents.Length);
            Assert.Equal(59, TextWithCombiningAccents.Length);
        }

        [Fact]
        private void ValidLetters()
        {
            SourceBill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Name = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            SourceBill.Creditor = address;
            Validate();
            AssertNoMessages();
            Assert.Equal("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ", ValidatedBill.Creditor.Name);
        }

        [Fact]
        private void ValidSpecialChars()
        {
            SourceBill = SampleData.CreateExample1();

            Address address = CreateValidPerson();
            address.Name = "!\"#%&*;<>÷=@_$£[]{}\\`´";
            SourceBill.Creditor = address;
            Validate();
            AssertNoMessages();
            Assert.Equal("!\"#%&*;<>÷=@_$£[]{}\\`´", ValidatedBill.Creditor.Name);
        }

        [Fact]
        private void ValidAccents()
        {
            SourceBill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Name = TextWithoutCombiningAccents;
            SourceBill.Creditor = address;
            Validate();
            AssertNoMessages();
            Assert.Equal(TextWithoutCombiningAccents, ValidatedBill.Creditor.Name);
        }

        [Fact]
        private void ValidCombiningAccents()
        {
            SourceBill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Name = TextWithCombiningAccents;
            SourceBill.Creditor = address;
            Validate();
            AssertNoMessages(); // silently normalized
            Assert.Equal(TextWithoutCombiningAccents, ValidatedBill.Creditor.Name);
        }

        [Fact]
        private void NewlineReplacement()
        {
            SourceBill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Name = "abc\r\ndef";
            SourceBill.Creditor = address;
            Validate();
            AssertSingleWarningMessage(ValidationConstants.FieldCreditorName, "replaced_unsupported_characters");
            Assert.Equal("abc def", ValidatedBill.Creditor.Name);
        }

        [Fact]
        private void InvalidCharacterReplacement()
        {
            SourceBill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Street = "abc€def©ghi^";
            SourceBill.Creditor = address;
            Validate();
            AssertSingleWarningMessage(ValidationConstants.FieldCreditorStreet, "replaced_unsupported_characters");
            Assert.Equal("abc.def.ghi.", ValidatedBill.Creditor.Street);
        }

        [Fact]
        private void ReplacedSurrogatePair()
        {
            SourceBill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.PostalCode = "\uD83D\uDC80"; // surrogate pair (1 code point but 2 UTF-16 words)
            SourceBill.Creditor = address;
            Validate();
            AssertSingleWarningMessage(ValidationConstants.FieldCreditorPostalCode, "replaced_unsupported_characters");
            Assert.Equal(".", ValidatedBill.Creditor.PostalCode);
        }

        [Fact]
        private void TwoReplacedConsecutiveSurrogatePairs()
        {
            SourceBill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Town = "\uD83C\uDDE8\uD83C\uDDED"; // two surrogate pairs
            SourceBill.Creditor = address;
            Validate();
            AssertSingleWarningMessage(ValidationConstants.FieldCreditorTown, "replaced_unsupported_characters");
            Assert.Equal("..", ValidatedBill.Creditor.Town);
        }

        [Fact]
        private void TwoReplacedSuggoratePairsWithWhitespace()
        {
            SourceBill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Town = "-- \uD83D\uDC68\uD83C\uDFFB --"; // two surrogate pairs
            SourceBill.Creditor = address;
            Validate();
            AssertSingleWarningMessage(ValidationConstants.FieldCreditorTown, "replaced_unsupported_characters");
            Assert.Equal("-- .. --", ValidatedBill.Creditor.Town);
        }

        [Theory]
        [MemberData(nameof(InvalidCharList))]
        private void InvalidChars(string invalidChar)
        {
            SourceBill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Street = "ABC" + invalidChar + "QRS";
            SourceBill.Creditor = address;
            Validate();
            AssertSingleWarningMessage(ValidationConstants.FieldCreditorStreet, "replaced_unsupported_characters");
            Assert.Equal("ABC.QRS", ValidatedBill.Creditor.Street);
        }

        public static TheoryData<string> InvalidCharList = new TheoryData<string>
        {
            "^", "\u007f", "\u0080", "\u00a0", "\u00d0", "¡", "¤", "©", "±", "µ", "¼", "Å", "Æ",
            "×", "Ø", "Ý", "Þ", "å", "æ", "ø", "€", "¿", "Ą", "Ď", "ð", "õ", "ã", "Ã"
        };
    }
}
