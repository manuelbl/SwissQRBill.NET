//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using System;
using Xunit;

namespace Codecrete.SwissQRBill.GeneratorTest
{
    public class RemoveWhitespaceTest
    {
        [Fact]
        private void EmptyString()
        {
            Assert.Equal("", "".WhiteSpaceRemoved());
        }

        [Fact]
        private void OneSpace()
        {
            Assert.Equal("", " ".WhiteSpaceRemoved());
        }

        [Fact]
        private void SeveralSpaces()
        {
            Assert.Equal("", "   ".WhiteSpaceRemoved());
        }

        [Fact]
        private void NoWhitespace()
        {
            Assert.Equal("abcd", "abcd".WhiteSpaceRemoved());
        }

        [Fact]
        private void LeadingSpace()
        {
            Assert.Equal("fggh", " fggh".WhiteSpaceRemoved());
        }

        [Fact]
        private void MultipleLeadingSpaces()
        {
            Assert.Equal("jklm", "   jklm".WhiteSpaceRemoved());
        }

        [Fact]
        private void TrailingSpace()
        {
            Assert.Equal("nppo", "nppo ".WhiteSpaceRemoved());
        }

        [Fact]
        private void MultipleTrailingSpaces()
        {
            Assert.Equal("qrs", "qrs    ".WhiteSpaceRemoved());
        }

        [Fact]
        private void LeadingAndTrailingSpaces()
        {
            Assert.Equal("guj", " guj    ".WhiteSpaceRemoved());
        }

        [Fact]
        private void SingleSpace()
        {
            Assert.Equal("abde", "ab de".WhiteSpaceRemoved());
        }

        [Fact]
        private void MultipleSpaces()
        {
            Assert.Equal("cdef", "cd    ef".WhiteSpaceRemoved());
        }

        [Fact]
        private void MultipleGroupsOfSpaces()
        {
            Assert.Equal("ghijkl", "gh ij  kl".WhiteSpaceRemoved());
        }

        [Fact]
        private void LeadingAndMultipleGroupsOfSpaces()
        {
            Assert.Equal("opqrst", "  op   qr s t".WhiteSpaceRemoved());
        }

        [Fact]
        private void LeadingAndTrailingAndMultipleGroupsOfSpaces()
        {
            Assert.Equal("uvxyz", " uv x  y z  ".WhiteSpaceRemoved());
        }

        [Fact]
        private void CopyAvoidance()
        {
            string value = "qwerty";
            Assert.Same(value, value.WhiteSpaceRemoved());
        }

        [Fact]
        private void NullString()
        {
            Assert.Throws<NullReferenceException>(() => ((string)null).WhiteSpaceRemoved());
        }
    }
}
