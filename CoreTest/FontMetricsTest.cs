//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator.Canvas;
using Xunit;

namespace Codecrete.SwissQRBill.CoreTest
{
    public class FontMetricsTest
    {
        private readonly FontMetrics _fontMetrics;

        public FontMetricsTest()
        {
            _fontMetrics = new FontMetrics("Helvetica");
        }

        [Fact]
        public void ShortOneLiner()
        {
            string[] lines = _fontMetrics.SplitLines("abc", 50, 10);
            Assert.Single(lines);
            Assert.Equal("abc", lines[0]);
        }

        [Fact]
        public void OneLiner()
        {
            string[] lines = _fontMetrics.SplitLines("abcdefghij", 50, 10);
            Assert.Single(lines);
            Assert.Equal("abcdefghij", lines[0]);
        }

        [Fact]
        public void OneLinerWithTwoWords()
        {
            string[] lines = _fontMetrics.SplitLines("abcdef ghij", 50, 10);
            Assert.Single(lines);
            Assert.Equal("abcdef ghij", lines[0]);
        }

        [Fact]
        public void LeadingSpaceOneLiner()
        {
            string[] lines = _fontMetrics.SplitLines(" abcdefghij", 50, 10);
            Assert.Single(lines);
            Assert.Equal("abcdefghij", lines[0]);
        }

        [Fact]
        public void TrailingSpaceOneLiner()
        {
            string[] lines = _fontMetrics.SplitLines("abcdefghij ", 50, 10);
            Assert.Single(lines);
            Assert.Equal("abcdefghij", lines[0]);
        }

        [Fact]
        public void EmptyLine()
        {
            string[] lines = _fontMetrics.SplitLines("", 50, 10);
            Assert.Single(lines);
            Assert.Equal("", lines[0]);
        }

        [Fact]
        public void SingleSpace()
        {
            string[] lines = _fontMetrics.SplitLines(" ", 50, 10);
            Assert.Single(lines);
            Assert.Equal("", lines[0]);
        }

        [Fact]
        public void ManySpaces()
        {
            string[] lines = _fontMetrics.SplitLines("                           ", 50, 10);
            Assert.Single(lines);
            Assert.Equal("", lines[0]);
        }

        [Fact]
        public void OutsideAsciiRange()
        {
            string[] lines = _fontMetrics.SplitLines("éà£$\uD83D\uDE03", 50, 10);
            Assert.Single(lines);
            Assert.Equal("éà£$\uD83D\uDE03", lines[0]);
        }

        [Fact]
        public void TwoLinesFromSpace()
        {
            string[] lines = _fontMetrics.SplitLines("abcde fghijk", 50, 10);
            Assert.Equal(2, lines.Length);
            Assert.Equal("abcde", lines[0]);
            Assert.Equal("fghijk", lines[1]);
        }

        [Fact]
        public void TwoLinesFromNewLine()
        {
            string[] lines = _fontMetrics.SplitLines("abcde\nfghijk", 50, 10);
            Assert.Equal(2, lines.Length);
            Assert.Equal("abcde", lines[0]);
            Assert.Equal("fghijk", lines[1]);
        }

        [Fact]
        public void TwoLinesWithTrailingNewline()
        {
            string[] lines = _fontMetrics.SplitLines("abcde\n", 50, 10);
            Assert.Equal(2, lines.Length);
            Assert.Equal("abcde", lines[0]);
            Assert.Equal("", lines[1]);
        }

        [Fact]
        public void SingleNewline()
        {
            string[] lines = _fontMetrics.SplitLines("\n", 50, 10);
            Assert.Equal(2, lines.Length);
            Assert.Equal("", lines[0]);
            Assert.Equal("", lines[1]);
        }

        [Fact]
        public void SpaceAndNewline()
        {
            string[] lines = _fontMetrics.SplitLines("  \n ", 50, 10);
            Assert.Equal(2, lines.Length);
            Assert.Equal("", lines[0]);
            Assert.Equal("", lines[1]);
        }

        [Fact]
        public void TrailingAndLeadingSpaceAndNewline()
        {
            string[] lines = _fontMetrics.SplitLines(" abc \n", 50, 10);
            Assert.Equal(2, lines.Length);
            Assert.Equal("abc", lines[0]);
            Assert.Equal("", lines[1]);
        }

        [Fact]
        public void ForcedWordBreak()
        {
            string[] lines = _fontMetrics.SplitLines("abcde", 2, 10);
            Assert.Equal(5, lines.Length);
            Assert.Equal("a", lines[0]);
            Assert.Equal("b", lines[1]);
            Assert.Equal("c", lines[2]);
            Assert.Equal("d", lines[3]);
            Assert.Equal("e", lines[4]);
        }

        [Fact]
        public void ForcedWordBreakWithSpaces()
        {
            string[] lines = _fontMetrics.SplitLines("  abcde  ", 2, 10);
            Assert.Equal(5, lines.Length);
            Assert.Equal("a", lines[0]);
            Assert.Equal("b", lines[1]);
            Assert.Equal("c", lines[2]);
            Assert.Equal("d", lines[3]);
            Assert.Equal("e", lines[4]);
        }
    }
}
