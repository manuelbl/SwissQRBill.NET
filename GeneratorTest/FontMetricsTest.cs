//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator.Canvas;
using Xunit;

namespace Codecrete.SwissQRBill.GeneratorTest
{
    public class FontMetricsTest
    {
        private FontMetrics fontMetrics;

        public FontMetricsTest()
        {
            fontMetrics = new FontMetrics("Helvetica");
        }

        [Fact]
        void ShortOneLiner()
        {
            string[] lines = fontMetrics.SplitLines("abc", 50, 10);
            Assert.Single(lines);
            Assert.Equal("abc", lines[0]);
        }

        [Fact]
        void OneLiner()
        {
            string[] lines = fontMetrics.SplitLines("abcdefghij", 50, 10);
            Assert.Single(lines);
            Assert.Equal("abcdefghij", lines[0]);
        }

        [Fact]
        void OneLinerWithTwoWords()
        {
            string[] lines = fontMetrics.SplitLines("abcdef ghij", 50, 10);
            Assert.Single(lines);
            Assert.Equal("abcdef ghij", lines[0]);
        }

        [Fact]
        void LeadingSpaceOneLiner()
        {
            string[] lines = fontMetrics.SplitLines(" abcdefghij", 50, 10);
            Assert.Single(lines);
            Assert.Equal("abcdefghij", lines[0]);
        }

        [Fact]
        void TrailingSpaceOneLiner()
        {
            string[] lines = fontMetrics.SplitLines("abcdefghij ", 50, 10);
            Assert.Single(lines);
            Assert.Equal("abcdefghij", lines[0]);
        }

        [Fact]
        void EmptyLine()
        {
            string[] lines = fontMetrics.SplitLines("", 50, 10);
            Assert.Single(lines);
            Assert.Equal("", lines[0]);
        }

        [Fact]
        void SingleSpace()
        {
            string[] lines = fontMetrics.SplitLines(" ", 50, 10);
            Assert.Single(lines);
            Assert.Equal("", lines[0]);
        }

        [Fact]
        void ManySpaces()
        {
            string[] lines = fontMetrics.SplitLines("                           ", 50, 10);
            Assert.Single(lines);
            Assert.Equal("", lines[0]);
        }

        [Fact]
        void OutsideASCIIRange()
        {
            string[] lines = fontMetrics.SplitLines("éà£$\uD83D\uDE03", 50, 10);
            Assert.Single(lines);
            Assert.Equal("éà£$\uD83D\uDE03", lines[0]);
        }

        [Fact]
        void TwoLinesFromSpace()
        {
            string[] lines = fontMetrics.SplitLines("abcde fghijk", 50, 10);
            Assert.Equal(2, lines.Length);
            Assert.Equal("abcde", lines[0]);
            Assert.Equal("fghijk", lines[1]);
        }

        [Fact]
        void TwoLinesFromNewLine()
        {
            string[] lines = fontMetrics.SplitLines("abcde\nfghijk", 50, 10);
            Assert.Equal(2, lines.Length);
            Assert.Equal("abcde", lines[0]);
            Assert.Equal("fghijk", lines[1]);
        }

        [Fact]
        void TwoLinesWithTrailingNewline()
        {
            string[] lines = fontMetrics.SplitLines("abcde\n", 50, 10);
            Assert.Equal(2, lines.Length);
            Assert.Equal("abcde", lines[0]);
            Assert.Equal("", lines[1]);
        }

        [Fact]
        void SingleNewline()
        {
            string[] lines = fontMetrics.SplitLines("\n", 50, 10);
            Assert.Equal(2, lines.Length);
            Assert.Equal("", lines[0]);
            Assert.Equal("", lines[1]);
        }

        [Fact]
        void SpaceAndNewline()
        {
            string[] lines = fontMetrics.SplitLines("  \n ", 50, 10);
            Assert.Equal(2, lines.Length);
            Assert.Equal("", lines[0]);
            Assert.Equal("", lines[1]);
        }

        [Fact]
        void TrailingAndLeadingSpaceAndNewline()
        {
            string[] lines = fontMetrics.SplitLines(" abc \n", 50, 10);
            Assert.Equal(2, lines.Length);
            Assert.Equal("abc", lines[0]);
            Assert.Equal("", lines[1]);
        }

        [Fact]
        void ForcedWorkbreak()
        {
            string[] lines = fontMetrics.SplitLines("abcde", 2, 10);
            Assert.Equal(5, lines.Length);
            Assert.Equal("a", lines[0]);
            Assert.Equal("b", lines[1]);
            Assert.Equal("c", lines[2]);
            Assert.Equal("d", lines[3]);
            Assert.Equal("e", lines[4]);
        }

        [Fact]
        void ForcedWordbreakWithSpaces()
        {
            string[] lines = fontMetrics.SplitLines("  abcde  ", 2, 10);
            Assert.Equal(5, lines.Length);
            Assert.Equal("a", lines[0]);
            Assert.Equal("b", lines[1]);
            Assert.Equal("c", lines[2]);
            Assert.Equal("d", lines[3]);
            Assert.Equal("e", lines[4]);
        }
    }
}
