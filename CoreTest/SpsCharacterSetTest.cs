//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2024 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using System;
using System.Linq;
using Xunit;

namespace Codecrete.SwissQRBill.CoreTest
{
    public class SpsCharacterSetTest
    {
        [Theory]
        [InlineData('\t')]
        [InlineData('^')]
        [InlineData('\u007F')]
        [InlineData('¡')]
        [InlineData('¶')]
        [InlineData('Õ')]
        [InlineData('æ')]
        [InlineData('ø')]
        [InlineData('€')]
        public void Latin1Subset_DoesNotContainInvalidCharacters(char validChar)
        {
            Assert.False(SpsCharacterSet.Latin1Subset.Contains(validChar));
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
        [InlineData('€')]
        public void ExtendedLatin_ContainsValidCharacters(char validChar)
        {
            Assert.True(SpsCharacterSet.ExtendedLatin.Contains(validChar));
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
        [InlineData('€')]
        public void ExtendedLatin_ContainsValidCodePoints(char validChar)
        {
            Assert.True(SpsCharacterSet.ExtendedLatin.Contains((int)validChar));
        }

        [Theory]
        [InlineData('\n')]
        [InlineData('\r')]
        [InlineData('\u007f')]
        [InlineData('\u0083')]
        [InlineData('Ɖ')]
        [InlineData('Ǒ')]
        [InlineData('Ȑ')]
        [InlineData('Ȟ')]
        public void ExtendedLatin_DoesNotContainInvalidCharacters(char invalidChar)
        {
            Assert.False(SpsCharacterSet.ExtendedLatin.Contains(invalidChar));
        }

        [Theory]
        [InlineData('\n')]
        [InlineData('\r')]
        [InlineData('\u007f')]
        [InlineData('\u0083')]
        [InlineData('Ɖ')]
        [InlineData('Ǒ')]
        [InlineData('Ȑ')]
        [InlineData('Ȟ')]
        public void ExtendedLatin_DoesNotContainInvalidCodePoints(char invalidChar)
        {
            Assert.False(SpsCharacterSet.ExtendedLatin.Contains((int)invalidChar));
        }
    }
}
