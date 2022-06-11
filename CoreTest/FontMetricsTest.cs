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

        [Theory]
        [InlineData("abc")]
        [InlineData("abcdefghij")]
        [InlineData("abcdef ghij")]
        [InlineData(" abcdefghij")]
        [InlineData("abcdefghij ")]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("                           ")]
        [InlineData("éà£$\uD83D\uDE03")]
        public void Input_IsSingleLine(string input)
        {
            string[] lines = _fontMetrics.SplitLines(input, 50, 10);
            Assert.Single(lines);
            Assert.Equal(input.Trim(), lines[0]);
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

        [Fact]
        public void NewLines_HaveWidth0()
        {
            Assert.Equal(0, _fontMetrics.TextWidth("\n", 10, false));
            Assert.Equal(0, _fontMetrics.TextWidth("\r", 10, false));
        }
    }
}
