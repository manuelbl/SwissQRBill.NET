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
        private void NullString()
        {
            Assert.Null(((string)null).Trimmed());
        }

        [Fact]
        private void EmptyString()
        {
            Assert.Null("".Trimmed());
        }

        [Fact]
        private void OneSpace()
        {
            Assert.Null(" ".Trimmed());
        }

        [Fact]
        private void SeveralSpaces()
        {
            Assert.Null("   ".Trimmed());
        }

        [Fact]
        private void NoWhitespace()
        {
            Assert.Equal("ghj", "ghj".Trimmed());
        }

        [Fact]
        private void LeadingSpace()
        {
            Assert.Equal("klm", " klm".Trimmed());
        }

        [Fact]
        private void MultipleLeadingSpaces()
        {
            Assert.Equal("mnop", "   mnop".Trimmed());
        }

        [Fact]
        private void TrailingSpace()
        {
            Assert.Equal("pqrs", "pqrs ".Trimmed());
        }

        [Fact]
        private void MultipleTrailingSpaces()
        {
            Assert.Equal("rstu", "rstu    ".Trimmed());
        }

        [Fact]
        private void LeadingAndTrailingSpaces()
        {
            Assert.Equal("xyz", " xyz    ".Trimmed());
        }

        [Fact]
        private void InternalSpace()
        {
            Assert.Equal("cd ef", "cd ef".Trimmed());
        }

        [Fact]
        private void MultipleInternalSpaces()
        {
            Assert.Equal("fg  hi", "fg  hi".Trimmed());
        }

        [Fact]
        private void CopyAvoidanceWithoutSpaces()
        {
            string value = "cvbn";
            Assert.Same(value, value.Trimmed());
        }

        [Fact]
        private void CopyAvoidanceWithSpaces()
        {
            string value = "i o p";
            Assert.Same(value, value.Trimmed());
        }
    }
}
