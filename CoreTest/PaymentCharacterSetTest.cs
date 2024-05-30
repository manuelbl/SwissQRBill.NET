//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2024 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace Codecrete.SwissQRBill.CoreTest
{
    public class PaymentCharacterSetTest : BillDataValidationBase
    {
        private static readonly string TextWithoutCombiningAccents = "àáâäçèéêëìíîïñòóôöùúûüýßÀÁÂÄÇÈÉÊËÌÍÎÏÒÓÔÖÙÚÛÜÑ";
        private static readonly string TextWithCombiningAccents = "àáâäçèéêëìíîïñòóôöùúûüýßÀÁÂÄÇÈÉÊËÌÍÎÏÒÓÔÖÙÚÛÜÑ";

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
        [InlineData("")]
        public void ValidText_ReturnsTrue(string validText)
        {
            Assert.True(Payments.IsValidText(validText, SpsCharacterSet.ExtendedLatin));
        }

        [Theory]
        [InlineData("a\nc")]
        [InlineData("ABǑC")]
        [InlineData("12\uD83D\uDE003")]
        [InlineData("äöü\uD83C\uDDEE\uD83C\uDDF9ÄÖÜ")]
        public void InvalidText_ReturnsFalse(string invalidText)
        {
            Assert.False(Payments.IsValidText(invalidText, SpsCharacterSet.ExtendedLatin));
        }

        [Fact]
        public void NullText_IsValid()
        {
            Assert.True(Payments.IsValidText(null, SpsCharacterSet.Latin1Subset));
            Assert.True(Payments.IsValidText(null, SpsCharacterSet.ExtendedLatin));
        }

        [Theory]
        [InlineData("abc")]
        [InlineData(" a b c ")]
        [InlineData("àáâäçè")]
        public void CleanedText_IsNotChanged(string text)
        {
            Assert.Equal(text, Payments.CleanedText(text, SpsCharacterSet.ExtendedLatin));
        }

        [Fact]
        public void DecomposedAccents_AreComposed()
        {
            Assert.Equal(46, TextWithoutCombiningAccents.Length);
            Assert.Equal(59, TextWithCombiningAccents.Length);
            Assert.Equal(TextWithoutCombiningAccents, Payments.CleanedText(TextWithCombiningAccents, SpsCharacterSet.ExtendedLatin));
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
        [InlineData("x\uD83C\uDDE8\uD83C\uDDEDx", "x.x")]
        [InlineData("Ǆ", "DZ")]
        public void InvalidCharacters_AreReplaced(string text, string expectedResult)
        {
            Assert.Equal(expectedResult, Payments.CleanedAndTrimmedText(text, SpsCharacterSet.ExtendedLatin));
        }

        [Fact]
        public void CleanedNull_ReturnsNull()
        {
            Assert.Null(Payments.CleanedAndTrimmedText(null, SpsCharacterSet.ExtendedLatin));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void BlankText_ReturnsNull(string text)
        {
            Assert.Null(Payments.CleanedAndTrimmedText(text, SpsCharacterSet.ExtendedLatin));
        }

        [Theory]
        [InlineData("a   b  c", "a b c")]
        [InlineData(" a  b c", "a b c")]
        public void MultipleSpaces_BecomeSingleSpace(string text, string expectedResult)
        {
            Assert.Equal(expectedResult, Payments.CleanedAndTrimmedText(text, SpsCharacterSet.ExtendedLatin));
        }

        [Theory]
        [InlineData(" a ")]
        [InlineData(" a  b")]
        [InlineData(" ")]
        public void Spaces_AreNotTrimmed(string text)
        {
            Assert.Equal(text, Payments.CleanedText(text, SpsCharacterSet.ExtendedLatin));
        }

        [Theory]
        [ClassData(typeof(ExtendedLatinCharsProvider))]
        public void AllChars_HaveGoodReplacement(char ch)
        {
            string cleaned = Payments.CleanedText(char.ToString(ch), SpsCharacterSet.Latin1Subset);
            Assert.NotEqual(".", cleaned);
            Assert.True(Payments.IsValidText(cleaned, SpsCharacterSet.Latin1Subset));


            cleaned = Payments.CleanedText(char.ToString(ch), SpsCharacterSet.ExtendedLatin);
            Assert.NotEqual(".", cleaned);
            Assert.True(Payments.IsValidText(cleaned, SpsCharacterSet.ExtendedLatin));
        }

        public class ExtendedLatinCharsProvider : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                for (char ch = '\u0020'; ch <= '\u007e'; ch++)
                {
                    if (ch == '.' || ch == '^')
                        continue;
                    yield return new object[] { ch };
                }
                for (char ch = '\u00a0'; ch <= '\u017f'; ch++)
                {
                    yield return new object[] { ch };
                }

                yield return new object[] { '\u0218' };
                yield return new object[] { '\u0219' };
                yield return new object[] { '\u021a' };
                yield return new object[] { '\u021b' };
                yield return new object[] { '\u20ac' };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
