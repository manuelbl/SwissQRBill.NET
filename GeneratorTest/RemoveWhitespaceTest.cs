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
        void EmptyString()
        {
            Assert.Equal("", "".WhiteSpaceRemoved());
        }

        [Fact]
        void OneSpace()
        {
            Assert.Equal("", " ".WhiteSpaceRemoved());
        }

        [Fact]
        void SeveralSpaces()
        {
            Assert.Equal("", "   ".WhiteSpaceRemoved());
        }

        [Fact]
        void NoWhitespace()
        {
            Assert.Equal("abcd", "abcd".WhiteSpaceRemoved());
        }

        [Fact]
        void LeadingSpace()
        {
            Assert.Equal("fggh", " fggh".WhiteSpaceRemoved());
        }

        [Fact]
        void MultipleLeadingSpaces()
        {
            Assert.Equal("jklm", "   jklm".WhiteSpaceRemoved());
        }

        [Fact]
        void TrailingSpace()
        {
            Assert.Equal("nppo", "nppo ".WhiteSpaceRemoved());
        }

        [Fact]
        void MultipleTrailingSpaces()
        {
            Assert.Equal("qrs", "qrs    ".WhiteSpaceRemoved());
        }

        [Fact]
        void LeadingAndTrailingSpaces()
        {
            Assert.Equal("guj", " guj    ".WhiteSpaceRemoved());
        }

        [Fact]
        void SingleSpace()
        {
            Assert.Equal("abde", "ab de".WhiteSpaceRemoved());
        }

        [Fact]
        void MultipleSpaces()
        {
            Assert.Equal("cdef", "cd    ef".WhiteSpaceRemoved());
        }

        [Fact]
        void MultipleGroupsOfSpaces()
        {
            Assert.Equal("ghijkl", "gh ij  kl".WhiteSpaceRemoved());
        }

        [Fact]
        void LeadingAndMultipleGroupsOfSpaces()
        {
            Assert.Equal("opqrst", "  op   qr s t".WhiteSpaceRemoved());
        }

        [Fact]
        void LeadingAndTrailingAndMultipleGroupsOfSpaces()
        {
            Assert.Equal("uvxyz", " uv x  y z  ".WhiteSpaceRemoved());
        }

        [Fact]
        void CopyAvoidance()
        {
            string value = "qwerty";
            Assert.Same(value, value.WhiteSpaceRemoved());
        }

        [Fact]
        void NullString()
        {
            Assert.Throws<NullReferenceException>(() => ((string)null).WhiteSpaceRemoved());
        }
    }
}
