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
        private static readonly string TEXT_WITHOUT_COMBINING_ACCENTS = "àáâäçèéêëìíîïñòóôöùúûüýßÀÁÂÄÇÈÉÊËÌÍÎÏÒÓÔÖÙÚÛÜÑ";
        private static readonly string TEXT_WITH_COMBINING_ACCENTS = "àáâäçèéêëìíîïñòóôöùúûüýßÀÁÂÄÇÈÉÊËÌÍÎÏÒÓÔÖÙÚÛÜÑ";

        [Fact]
        void VerifyTestData()
        {
            Assert.Equal(46, TEXT_WITHOUT_COMBINING_ACCENTS.Length);
            Assert.Equal(59, TEXT_WITH_COMBINING_ACCENTS.Length);
        }

        [Fact]
        void ValidLetters()
        {
            bill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Name = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            bill.Creditor = address;
            Validate();
            AssertNoMessages();
            Assert.Equal("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ", validatedBill.Creditor.Name);
        }

        [Fact]
        void ValidSpecialChars()
        {
            bill = SampleData.CreateExample1();

            Address address = CreateValidPerson();
            address.Name = "!\"#%&*;<>÷=@_$£[]{}\\`´";
            bill.Creditor = address;
            Validate();
            AssertNoMessages();
            Assert.Equal("!\"#%&*;<>÷=@_$£[]{}\\`´", validatedBill.Creditor.Name);
        }

        [Fact]
        void ValidAccents()
        {
            bill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Name = TEXT_WITHOUT_COMBINING_ACCENTS;
            bill.Creditor = address;
            Validate();
            AssertNoMessages();
            Assert.Equal(TEXT_WITHOUT_COMBINING_ACCENTS, validatedBill.Creditor.Name);
        }

        [Fact]
        void ValidCombiningAccents()
        {
            bill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Name = TEXT_WITH_COMBINING_ACCENTS;
            bill.Creditor = address;
            Validate();
            AssertNoMessages(); // silently normalized
            Assert.Equal(TEXT_WITHOUT_COMBINING_ACCENTS, validatedBill.Creditor.Name);
        }

        [Fact]
        void NewlineReplacement()
        {
            bill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Name = "abc\r\ndef";
            bill.Creditor = address;
            Validate();
            AssertSingleWarningMessage(Bill.FieldCreditorName, "replaced_unsupported_characters");
            Assert.Equal("abc def", validatedBill.Creditor.Name);
        }

        [Fact]
        void InvalidCharacterReplacement()
        {
            bill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Street = "abc€def©ghi^";
            bill.Creditor = address;
            Validate();
            AssertSingleWarningMessage(Bill.FieldCreditorStreet, "replaced_unsupported_characters");
            Assert.Equal("abc.def.ghi.", validatedBill.Creditor.Street);
        }

        [Fact]
        void ReplacedSurrogatePair()
        {
            bill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.PostalCode = "\uD83D\uDC80"; // surrogate pair (1 code point but 2 UTF-16 words)
            bill.Creditor = address;
            Validate();
            AssertSingleWarningMessage(Bill.FieldCreditorPostalCode, "replaced_unsupported_characters");
            Assert.Equal(".", validatedBill.Creditor.PostalCode);
        }

        [Fact]
        void TwoReplacedConsecutiveSurrogatePairs()
        {
            bill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Town = "\uD83C\uDDE8\uD83C\uDDED"; // two surrogate pairs
            bill.Creditor = address;
            Validate();
            AssertSingleWarningMessage(Bill.FieldCreditorTown, "replaced_unsupported_characters");
            Assert.Equal("..", validatedBill.Creditor.Town);
        }

        [Fact]
        void TwoReplacedSuggoratePairsWithWhitespace()
        {
            bill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Town = "-- \uD83D\uDC68\uD83C\uDFFB --"; // two surrogate pairs
            bill.Creditor = address;
            Validate();
            AssertSingleWarningMessage(Bill.FieldCreditorTown, "replaced_unsupported_characters");
            Assert.Equal("-- .. --", validatedBill.Creditor.Town);
        }

        [Theory]
        [MemberData(nameof(InvalidCharList))]
        void InvalidChars(string invalidChar)
        {
            bill = SampleData.CreateExample1();
            Address address = CreateValidPerson();
            address.Street = "ABC" + invalidChar + "QRS";
            bill.Creditor = address;
            Validate();
            AssertSingleWarningMessage(Bill.FieldCreditorStreet, "replaced_unsupported_characters");
            Assert.Equal("ABC.QRS", validatedBill.Creditor.Street);
        }

        public static TheoryData<string> InvalidCharList = new TheoryData<string>
        {
            "^", "\u007f", "\u0080", "\u00a0", "\u00A0", "¡", "¤", "©", "±", "µ", "¼", "Å", "Æ", "Ð",
            "×", "Ø", "Ý", "Þ", "å", "æ", "ø", "€", "¿", "Ý", "Ą", "Ď", "ð", "õ", "ã", "Ã"
        };
    }
}
