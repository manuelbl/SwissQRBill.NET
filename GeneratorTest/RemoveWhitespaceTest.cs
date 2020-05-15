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
        public void EmptyString()
        {
            Assert.Equal("", "".WhiteSpaceRemoved());
        }

        [Fact]
        public void OneSpace()
        {
            Assert.Equal("", " ".WhiteSpaceRemoved());
        }

        [Fact]
        public void SeveralSpaces()
        {
            Assert.Equal("", "   ".WhiteSpaceRemoved());
        }

        [Fact]
        public void NoWhitespace()
        {
            Assert.Equal("abcd", "abcd".WhiteSpaceRemoved());
        }

        [Fact]
        public void LeadingSpace()
        {
            Assert.Equal("fggh", " fggh".WhiteSpaceRemoved());
        }

        [Fact]
        public void MultipleLeadingSpaces()
        {
            Assert.Equal("jklm", "   jklm".WhiteSpaceRemoved());
        }

        [Fact]
        public void TrailingSpace()
        {
            Assert.Equal("nppo", "nppo ".WhiteSpaceRemoved());
        }

        [Fact]
        public void MultipleTrailingSpaces()
        {
            Assert.Equal("qrs", "qrs    ".WhiteSpaceRemoved());
        }

        [Fact]
        public void LeadingAndTrailingSpaces()
        {
            Assert.Equal("guj", " guj    ".WhiteSpaceRemoved());
        }

        [Fact]
        public void SingleSpace()
        {
            Assert.Equal("abde", "ab de".WhiteSpaceRemoved());
        }

        [Fact]
        public void MultipleSpaces()
        {
            Assert.Equal("cdef", "cd    ef".WhiteSpaceRemoved());
        }

        [Fact]
        public void MultipleGroupsOfSpaces()
        {
            Assert.Equal("ghijkl", "gh ij  kl".WhiteSpaceRemoved());
        }

        [Fact]
        public void LeadingAndMultipleGroupsOfSpaces()
        {
            Assert.Equal("opqrst", "  op   qr s t".WhiteSpaceRemoved());
        }

        [Fact]
        public void LeadingAndTrailingAndMultipleGroupsOfSpaces()
        {
            Assert.Equal("uvxyz", " uv x  y z  ".WhiteSpaceRemoved());
        }

        [Fact]
        public void CopyAvoidance()
        {
            string value = "qwerty";
            Assert.Same(value, value.WhiteSpaceRemoved());
        }

        [Fact]
        public void NullString()
        {
            Assert.Throws<NullReferenceException>(() => ((string)null).WhiteSpaceRemoved());
        }
    }
}
