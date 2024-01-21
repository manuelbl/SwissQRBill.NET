//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2024 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using Xunit;

namespace Codecrete.SwissQRBill.CoreTest
{
    public class PaymentCharacterSetTest : BillDataValidationBase
    {
        private static readonly string TEXT_WITHOUT_COMBINING_ACCENTS = "àáâäçèéêëìíîïñòóôöùúûüýßÀÁÂÄÇÈÉÊËÌÍÎÏÒÓÔÖÙÚÛÜÑ";
        private static readonly string TEXT_WITH_COMBINING_ACCENTS = "àáâäçèéêëìíîïñòóôöùúûüýßÀÁÂÄÇÈÉÊËÌÍÎÏÒÓÔÖÙÚÛÜÑ";

        [Theory]
        [InlineData('A')]
        [InlineData('b')]
        [InlineData('3')]
        [InlineData('%')]
        [InlineData('{')]
        [InlineData('®')]
        [InlineData('Ò')]
        [InlineData('æ')]
        [InlineData('Ă')]
        [InlineData('Ķ')]
        [InlineData('Ŕ')]
        [InlineData('ț')]
        [InlineData('€')]
        public void ValidCharacters_ReturnsTrue(char validChar)
        {
            Assert.True(Payments.IsValidCharacter(validChar));
        }

        [Theory]
        [InlineData('A')]
        [InlineData('b')]
        [InlineData('3')]
        [InlineData('%')]
        [InlineData('{')]
        [InlineData('®')]
        [InlineData('Ò')]
        [InlineData('æ')]
        [InlineData('Ă')]
        [InlineData('Ķ')]
        [InlineData('Ŕ')]
        [InlineData('ț')]
        [InlineData('€' )]
        public void ValidCodePoints_ReturnsTrue(char validChar)
        {
           Assert.True(Payments.IsValidCodePoint(validChar));
        }

        [Theory]
        [InlineData('\n')]
        [InlineData('\r')]
        [InlineData('\u007f')]
        [InlineData('\u0083')]
        [InlineData('Ɖ')]
        [InlineData('Ǒ')]
        [InlineData('Ȑ')]
        [InlineData('Ȟ' )]
        public void InvalidCharacters_ReturnsFalse(char invalidChar)
        {
            Assert.False(Payments.IsValidCharacter(invalidChar));
        }

        [Theory]
        [InlineData('\n')]
        [InlineData('\r')]
        [InlineData('\u007f')]
        [InlineData('\u0083')]
        [InlineData('Ɖ')]
        [InlineData('Ǒ')]
        [InlineData('Ȑ')]
        [InlineData('Ȟ' )]
        public void InvalidCodePoints_ReturnsFalse(char invalidChar)
        {
            Assert.False(Payments.IsValidCodePoint(invalidChar));
        }

        [Theory]
        [InlineData("abc")]
        [InlineData("ABC")]
        [InlineData("123")]
        [InlineData("äöüÄÖÜ")]
        [InlineData("àáâäçèéìíîïñôöùúûüýÿÀÁÂÄÇÊËÌÍÎÏÓÔÖÛÜÝ")]
        [InlineData("€")]
        [InlineData("£")]
        [InlineData("¥")]
        [InlineData(" ")]
        [InlineData("" )]
        public void ValidText_ReturnsTrue(string validText)
        {
            Assert.True(Payments.IsValidText(validText));
        }

        [Theory]
        [InlineData("a\nc")]
        [InlineData("ABǑC")]
        [InlineData("12\uD83D\uDE003")]
        [InlineData("äöü\uD83C\uDDEE\uD83C\uDDF9ÄÖÜ" )]
        public void InvalidText_ReturnsFalse(string invalidText)
        {
            Assert.False(Payments.IsValidText(invalidText));
        }

        [Theory]
        [InlineData("abc")]
        [InlineData(" a b c ")]
        [InlineData("àáâäçè" )]
        public void CleanText_IsNotChanged(string text)
        {
            Assert.Equal(text, Payments.CleanedText(text));
        }

        [Fact]
        public void DecomposedAccents_AreComposed()
        {
            Assert.Equal(46, TEXT_WITHOUT_COMBINING_ACCENTS.Length);
            Assert.Equal(59, TEXT_WITH_COMBINING_ACCENTS.Length);
            Assert.Equal(TEXT_WITHOUT_COMBINING_ACCENTS, Payments.CleanedText(TEXT_WITH_COMBINING_ACCENTS));
        }

        [Theory]
        [InlineData("ab\nc", "ab c")]
        [InlineData("ȆȇȈȉ", "EeIi")]
        [InlineData("Ǒ", "O")]
        [InlineData("ǉǌﬃ", "ljnjffi")]
        [InlineData("Ǽ", "Æ")]
        [InlineData("ʷ", "w")]
        [InlineData("⁵", "5")]
        [InlineData("℁", "a/s")]
        [InlineData("Ⅶ", "VII")]
        [InlineData("③", "3")]
        [InlineData("xƉx", "x.x")]
        [InlineData("x\uD83C\uDDE8\uD83C\uDDEDx", "x.x" )]
        public void InvalidCharacters_AreReplaced(string text, string expectedResult)
        {
            Assert.Equal(expectedResult, Payments.CleanedAndTrimmedText(text));
        }

        [Fact]
        public void CleanedNull_ReturnsNull()
        {
            Assert.Null(Payments.CleanedAndTrimmedText(null));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void BlankText_ReturnsNull(string text)
        {
            Assert.Null(Payments.CleanedAndTrimmedText(text));
        }

        [Theory]
        [InlineData("a   b  c", "a b c")]
        [InlineData(" a  b c", "a b c")]
        public void MultipleSpaces_BecomeSingleSpace(string text, string expectedText)
        {
            Assert.Equal(expectedText, Payments.CleanedAndTrimmedText(text));
        }

        [Theory]
        [InlineData(" a ")]
        [InlineData(" a  b")]
        [InlineData(" ")]
        public void Spaces_AreNotTrimmed(string text)
        {
            Assert.Equal(text, Payments.CleanedText(text));
        }

        /*
        [Fact]
        public void ValidLetters()
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
        public void ValidSpecialChars()
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
        public void ValidAccents()
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
        public void ValidCombiningAccents()
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
        public void NewlineReplacement()
        {
            SourceBill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Name = "abc\r\ndef";
            SourceBill.Creditor = address;
            Validate();
            AssertSingleWarningMessage(ValidationConstants.FieldCreditorName, ValidationConstants.KeyReplacedUnsupportedCharacters);
            Assert.Equal("abc def", ValidatedBill.Creditor.Name);
        }

        [Fact]
        public void InvalidCharacterReplacement()
        {
            SourceBill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Street = "abc€def©ghi^";
            SourceBill.Creditor = address;
            Validate();
            AssertSingleWarningMessage(ValidationConstants.FieldCreditorStreet, ValidationConstants.KeyReplacedUnsupportedCharacters);
            Assert.Equal("abc.def.ghi.", ValidatedBill.Creditor.Street);
        }

        [Fact]
        public void UnstructuredMessageReplacement()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.UnstructuredMessage = "Thanks 🙏 Lisa";
            Validate();
            AssertSingleWarningMessage(ValidationConstants.FieldUnstructuredMessage, ValidationConstants.KeyReplacedUnsupportedCharacters);
            Assert.Equal("Thanks . Lisa", ValidatedBill.UnstructuredMessage);
        }

        [Fact]
        public void BillInfoReplacement()
        {
            SourceBill = SampleData.CreateExample1();
            SourceBill.BillInformation = "//AZ/400€/123";
            Validate();
            AssertSingleWarningMessage(ValidationConstants.FieldBillInformation, ValidationConstants.KeyReplacedUnsupportedCharacters);
            Assert.Equal("//AZ/400./123", ValidatedBill.BillInformation);
        }

        [Fact]
        public void ReplacedSurrogatePair()
        {
            SourceBill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.PostalCode = "\uD83D\uDC80"; // surrogate pair (1 code point but 2 UTF-16 words)
            SourceBill.Creditor = address;
            Validate();
            AssertSingleWarningMessage(ValidationConstants.FieldCreditorPostalCode, ValidationConstants.KeyReplacedUnsupportedCharacters);
            Assert.Equal(".", ValidatedBill.Creditor.PostalCode);
        }

        [Fact]
        public void TwoReplacedConsecutiveSurrogatePairs()
        {
            SourceBill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Town = "\uD83C\uDDE8\uD83C\uDDED"; // two surrogate pairs
            SourceBill.Creditor = address;
            Validate();
            AssertSingleWarningMessage(ValidationConstants.FieldCreditorTown, ValidationConstants.KeyReplacedUnsupportedCharacters);
            Assert.Equal("..", ValidatedBill.Creditor.Town);
        }

        [Fact]
        public void TwoReplacedSurrogatePairsWithWhitespace()
        {
            SourceBill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Town = "-- \uD83D\uDC68\uD83C\uDFFB --"; // two surrogate pairs
            SourceBill.Creditor = address;
            Validate();
            AssertSingleWarningMessage(ValidationConstants.FieldCreditorTown, ValidationConstants.KeyReplacedUnsupportedCharacters);
            Assert.Equal("-- .. --", ValidatedBill.Creditor.Town);
        }

        [Theory]
        [MemberData(nameof(InvalidCharList))]
        public void InvalidChars(string invalidChar)
        {
            SourceBill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Street = "ABC" + invalidChar + "QRS";
            SourceBill.Creditor = address;
            Validate();
            AssertSingleWarningMessage(ValidationConstants.FieldCreditorStreet, ValidationConstants.KeyReplacedUnsupportedCharacters);
            Assert.Equal("ABC.QRS", ValidatedBill.Creditor.Street);
        }

        public static readonly TheoryData<string> InvalidCharList = new()
        {
            "^", "\u007f", "\u0080", "\u00a0", "\u00d0", "¡", "¤", "©", "±", "µ", "¼", "Å", "Æ",
            "×", "Ø", "Ý", "Þ", "å", "æ", "ø", "€", "¿", "Ą", "Ď", "ð", "õ", "ã", "Ã"
        };
        */
    }
}
