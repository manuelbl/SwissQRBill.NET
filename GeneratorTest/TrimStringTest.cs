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
    public class TrimStringTest
    {
        [Fact]
        void NullString()
        {
            Assert.Null(((string)null).Trimmed());
        }

        [Fact]
        void EmptyString()
        {
            Assert.Null("".Trimmed());
        }

        [Fact]
        void OneSpace()
        {
            Assert.Null(" ".Trimmed());
        }

        [Fact]
        void SeveralSpaces()
        {
            Assert.Null("   ".Trimmed());
        }

        [Fact]
        void NoWhitespace()
        {
            Assert.Equal("ghj", "ghj".Trimmed());
        }

        [Fact]
        void LeadingSpace()
        {
            Assert.Equal("klm", " klm".Trimmed());
        }

        [Fact]
        void MultipleLeadingSpaces()
        {
            Assert.Equal("mnop", "   mnop".Trimmed());
        }

        [Fact]
        void TrailingSpace()
        {
            Assert.Equal("pqrs", "pqrs ".Trimmed());
        }

        [Fact]
        void MultipleTrailingSpaces()
        {
            Assert.Equal("rstu", "rstu    ".Trimmed());
        }

        [Fact]
        void LeadingAndTrailingSpaces()
        {
            Assert.Equal("xyz", " xyz    ".Trimmed());
        }

        [Fact]
        void InternalSpace()
        {
            Assert.Equal("cd ef", "cd ef".Trimmed());
        }

        [Fact]
        void MultipleInternalSpaces()
        {
            Assert.Equal("fg  hi", "fg  hi".Trimmed());
        }

        [Fact]
        void CopyAvoidanceWithoutSpaces()
        {
            string value = "cvbn";
            Assert.Same(value, value.Trimmed());
        }

        [Fact]
        void CopyAvoidanceWithSpaces()
        {
            string value = "i o p";
            Assert.Same(value, value.Trimmed());
        }
    }
}
