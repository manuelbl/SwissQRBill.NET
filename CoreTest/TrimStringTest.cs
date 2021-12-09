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
    public class TrimStringTest
    {
        [Fact]
        public void NullString()
        {
            Assert.Null(((string)null).Trimmed());
        }

        [Fact]
        public void EmptyString()
        {
            Assert.Null("".Trimmed());
        }

        [Fact]
        public void OneSpace()
        {
            Assert.Null(" ".Trimmed());
        }

        [Fact]
        public void SeveralSpaces()
        {
            Assert.Null("   ".Trimmed());
        }

        [Fact]
        public void NoWhitespace()
        {
            Assert.Equal("ghj", "ghj".Trimmed());
        }

        [Fact]
        public void LeadingSpace()
        {
            Assert.Equal("klm", " klm".Trimmed());
        }

        [Fact]
        public void MultipleLeadingSpaces()
        {
            Assert.Equal("mnop", "   mnop".Trimmed());
        }

        [Fact]
        public void TrailingSpace()
        {
            Assert.Equal("pqrs", "pqrs ".Trimmed());
        }

        [Fact]
        public void MultipleTrailingSpaces()
        {
            Assert.Equal("rstu", "rstu    ".Trimmed());
        }

        [Fact]
        public void LeadingAndTrailingSpaces()
        {
            Assert.Equal("xyz", " xyz    ".Trimmed());
        }

        [Fact]
        public void InternalSpace()
        {
            Assert.Equal("cd ef", "cd ef".Trimmed());
        }

        [Fact]
        public void MultipleInternalSpaces()
        {
            Assert.Equal("fg  hi", "fg  hi".Trimmed());
        }

        [Fact]
        public void CopyAvoidanceWithoutSpaces()
        {
            string value = "cvbn";
            Assert.Same(value, value.Trimmed());
        }

        [Fact]
        public void CopyAvoidanceWithSpaces()
        {
            string value = "i o p";
            Assert.Same(value, value.Trimmed());
        }
    }
}
