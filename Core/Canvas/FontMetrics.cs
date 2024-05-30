//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//


using System.Collections.Generic;

namespace Codecrete.SwissQRBill.Generator.Canvas
{
    /// <summary>
    /// Simple font metrics class, independent of graphics subsystems and installed fonts.
    /// </summary>
    public class FontMetrics
    {
        private const double PtToMm = 25.4 / 72;

        private readonly short[] _charWidthx20x7F;
        private readonly short[] _charWidthxA0xFF;
        private readonly short[] _charWidthx100x17F;
        private readonly short[] _charWidthx218x21B;
        private readonly short _charDefaultWidth;
        private readonly short _charNDashWidth;
        private readonly short _charEuroWidth;
        private readonly FontMetrics _boldMetrics;

        /// <summary>
        /// Initializes a new instance for the given list of font families.
        /// <para>
        /// If more than one family is specified, the first family is used for metrics.
        /// </para>
        /// </summary>
        /// <param name="fontFamilyList">The font families, separated by comma (syntax as in CSS).</param>
        public FontMetrics(string fontFamilyList)
        {
            FontFamilyList = fontFamilyList;
            FirstFontFamily = GetFirstFontFamily(fontFamilyList);
            var family = FirstFontFamily.ToLowerInvariant();

            short[] boldCharWidthx20x7F;
            short[] boldCharWidthxA0xFF;
            short[] boldCharWidthx100x17F;
            short[] boldCharWidthx218x21B;
            short boldCharDefaultWidth;
            short boldCharNDashWidth;
            short boldCharEuroWidth;

            if (family.Contains("arial"))
            {
                _charWidthx20x7F = CharWidthData.ArialNormal_20_7F;
                _charWidthxA0xFF = CharWidthData.ArialNormal_A0_FF;
                _charWidthx100x17F = CharWidthData.ArialNormal_100_17F;
                _charWidthx218x21B = CharWidthData.ArialNormal_218_21B;
                _charDefaultWidth = CharWidthData.ArialNormalDefaultWidth;
                _charNDashWidth = CharWidthData.ArialNormalNDashWidth;
                _charEuroWidth = CharWidthData.ArialNormalEuroWidth;
                boldCharWidthx20x7F = CharWidthData.ArialBold_20_7F;
                boldCharWidthxA0xFF = CharWidthData.ArialBold_A0_FF;
                boldCharWidthx100x17F = CharWidthData.ArialBold_100_17F;
                boldCharWidthx218x21B = CharWidthData.ArialBold_218_21B;
                boldCharDefaultWidth = CharWidthData.ArialBoldDefaultWidth;
                boldCharNDashWidth = CharWidthData.ArialBoldNDashWidth;
                boldCharEuroWidth = CharWidthData.ArialBoldEuroWidth;
            }
            else if (family.Contains("liberation") && family.Contains("sans"))
            {
                _charWidthx20x7F = CharWidthData.LiberationSansNormal_20_7F;
                _charWidthxA0xFF = CharWidthData.LiberationSansNormal_A0_FF;
                _charWidthx100x17F = CharWidthData.LiberationSansNormal_100_17F;
                _charWidthx218x21B = CharWidthData.LiberationSansNormal_218_21B;
                _charDefaultWidth = CharWidthData.LiberationSansNormalDefaultWidth;
                _charNDashWidth = CharWidthData.LiberationSansNormalNDashWidth;
                _charEuroWidth = CharWidthData.LiberationSansNormalEuroWidth;
                boldCharWidthx20x7F = CharWidthData.LiberationSansBold_20_7F;
                boldCharWidthxA0xFF = CharWidthData.LiberationSansBold_A0_FF;
                boldCharWidthx100x17F = CharWidthData.LiberationSansBold_100_17F;
                boldCharWidthx218x21B = CharWidthData.LiberationSansBold_218_21B;
                boldCharDefaultWidth = CharWidthData.LiberationSansBoldDefaultWidth;
                boldCharNDashWidth = CharWidthData.LiberationSansBoldNDashWidth;
                boldCharEuroWidth = CharWidthData.LiberationSansBoldEuroWidth;
            }
            else if (family.Contains("frutiger"))
            {
                _charWidthx20x7F = CharWidthData.FrutigerNormal_20_7F;
                _charWidthxA0xFF = CharWidthData.FrutigerNormal_A0_FF;
                _charWidthx100x17F = CharWidthData.FrutigerNormal_100_17F;
                _charWidthx218x21B = CharWidthData.FrutigerNormal_218_21B;
                _charDefaultWidth = CharWidthData.FrutigerNormalDefaultWidth;
                _charNDashWidth = CharWidthData.FrutigerNormalNDashWidth;
                _charEuroWidth = CharWidthData.FrutigerNormalEuroWidth;
                boldCharWidthx20x7F = CharWidthData.FrutigerBold_20_7F;
                boldCharWidthxA0xFF = CharWidthData.FrutigerBold_A0_FF;
                boldCharWidthx100x17F = CharWidthData.FrutigerBold_100_17F;
                boldCharWidthx218x21B = CharWidthData.FrutigerBold_218_21B;
                boldCharDefaultWidth = CharWidthData.FrutigerBoldDefaultWidth;
                boldCharNDashWidth = CharWidthData.FrutigerBoldNDashWidth;
                boldCharEuroWidth = CharWidthData.FrutigerBoldEuroWidth;
            }
            else
            {
                _charWidthx20x7F = CharWidthData.HelveticaNormal_20_7F;
                _charWidthxA0xFF = CharWidthData.HelveticaNormal_A0_FF;
                _charWidthx100x17F = CharWidthData.HelveticaNormal_100_17F;
                _charWidthx218x21B = CharWidthData.HelveticaNormal_218_21B;
                _charDefaultWidth = CharWidthData.HelveticaNormalDefaultWidth;
                _charNDashWidth = CharWidthData.HelveticaNormalNDashWidth;
                _charEuroWidth = CharWidthData.HelveticaNormalEuroWidth;
                boldCharWidthx20x7F = CharWidthData.HelveticaBold_20_7F;
                boldCharWidthxA0xFF = CharWidthData.HelveticaBold_A0_FF;
                boldCharWidthx100x17F = CharWidthData.HelveticaBold_100_17F;
                boldCharWidthx218x21B = CharWidthData.HelveticaBold_218_21B;
                boldCharDefaultWidth = CharWidthData.HelveticaBoldDefaultWidth;
                boldCharNDashWidth = CharWidthData.HelveticaBoldNDashWidth;
                boldCharEuroWidth = CharWidthData.HelveticaBoldEuroWidth;
            }

            _boldMetrics = new FontMetrics(boldCharWidthx20x7F, boldCharWidthxA0xFF, boldCharWidthx100x17F, boldCharWidthx218x21B, boldCharDefaultWidth, boldCharNDashWidth, boldCharEuroWidth);
        }

        private FontMetrics(short[] charWidthx20x7F, short[] charWidthxA0xFF, short[] charWidthx100x17F, short[] charWidthx218x21B, short charDefaultWidth, short charNDashWidth, short charEuroWidth)
        {
            FontFamilyList = null;
            FirstFontFamily = null;
            _charWidthx20x7F = charWidthx20x7F;
            _charWidthxA0xFF = charWidthxA0xFF;
            _charWidthx100x17F = charWidthx100x17F;
            _charWidthx218x21B = charWidthx218x21B;
            _charDefaultWidth = charDefaultWidth;
            _charNDashWidth = charNDashWidth;
            _charEuroWidth = charEuroWidth;
            _boldMetrics = null;
        }

        /// <summary>
        /// Gets the font family list (comma separated, same syntax as for CSS).
        /// </summary>
        /// <value>The font family list, comma separated.</value>
        public string FontFamilyList { get; }

        /// <summary>
        /// Gets the first font family (from the font family list).
        /// </summary>
        /// <value>The first font family name.</value>
        public string FirstFontFamily { get; }

        /// <summary>
        /// Gets the distance between the baseline and the top of tallest letter.
        /// </summary>
        /// <param name="fontSize">The font size (in pt).</param>
        /// <returns>The distance (in mm).</returns>
        public double Ascender(int fontSize)
        {
            return fontSize * 0.8 * PtToMm;
        }

        /// <summary>
        /// Gets the distance between the baseline and the bottom of letter extending the farthest below the baseline.
        /// </summary>
        /// <param name="fontSize">The font size (in pt).</param>
        /// <returns>The distance (in mm).</returns>
        public double Descender(int fontSize)
        {
            return fontSize * 0.2 * PtToMm;
        }

        /// <summary>
        /// Gets the distance between the baselines of two consecutive text lines.
        /// </summary>
        /// <param name="fontSize">The font size (in pt).</param>
        /// <returns>The distance (in mm).</returns>
        public double LineHeight(int fontSize)
        {
            return fontSize * PtToMm;
        }

        /// <summary>
        /// Splits the text into lines.
        /// <para>
        /// The text is split such that no line is wider the specified maximum width.
        /// If possible, the text is split at whitespace characters. If a word is wider than
        /// the specified maximum width, the word is split and put onto two or more lines.
        /// The text is always split at newlines.
        /// </para>
        /// </summary>
        /// <param name="text">The text to split into lines.</param>
        /// <param name="maxLength">The maximum line length (in pt).</param>
        /// <param name="fontSize">The font size (in pt).</param>
        /// <returns>The resulting array of text lines.</returns>
        public string[] SplitLines(string text, double maxLength, int fontSize)
        {
            var lines = new List<string>();
            var max = (int)(maxLength * 1000 / fontSize);

            var len = text.Length; // length of line
            var pos = 0; // current position (0 ..< end)
            var lineStartPos = 0; // start position of current line
            var lineWidth = 0; // current line width (in AFM metric)
            var addEmptyLine = true; // flag if an empty line should be added as the last line

            // iterate over all characters
            while (pos < len)
            {

                // get current character
                var ch = text[pos];

                // skip leading white space at start of current line
                if (ch == ' ' && pos == lineStartPos)
                {
                    lineStartPos++;
                    pos++;
                    continue;
                }

                // add width of character
                lineWidth += CharWidth(ch);
                addEmptyLine = false;

                // line break is need if the maximum width has been reached
                // or if an explicit line break has been encountered
                if (ch == '\n' || lineWidth > max)
                {

                    // find the position for the line break
                    int breakPos;
                    if (ch == '\n')
                    {
                        breakPos = pos;

                    }
                    else
                    {
                        // locate the previous space on the line
                        var spacePos = pos - 1;
                        while (spacePos > lineStartPos)
                        {
                            if (text[spacePos] == ' ')
                            {
                                break;
                            }

                            spacePos--;
                        }

                        // if space was found, it's the break position
                        if (spacePos > lineStartPos)
                        {
                            breakPos = spacePos;

                        }
                        else
                        {
                            // if no space was found, forcibly break word
                            if (pos > lineStartPos)
                            {
                                breakPos = pos;
                            }
                            else
                            {
                                breakPos = lineStartPos + 1; // at least one character
                            }
                        }
                    }

                    // add line to result
                    AddResultLine(lines, text, lineStartPos, breakPos);

                    // setup start of new line
                    lineStartPos = breakPos;
                    if (ch == '\n')
                    {
                        lineStartPos = breakPos + 1;
                        addEmptyLine = true;
                    }
                    pos = lineStartPos;
                    lineWidth = 0;

                }
                else
                {
                    // no line break needed; progress one character
                    pos++;
                }
            }

            // complete the last line
            if (pos > lineStartPos)
            {
                AddResultLine(lines, text, lineStartPos, pos);
            }
            else if (addEmptyLine)
            {
                lines.Add("");
            }

            return lines.ToArray();
        }

        /// <summary>
        /// Adds the specified text range to the resulting line array.
        /// <para>
        /// Trailing white space is trimmed.
        /// </para>
        /// </summary>
        /// <param name="lines">The line array to add to.</param>
        /// <param name="text">The text serving as a source for the line.</param>
        /// <param name="start">The start position of the line within the text.</param>
        /// <param name="end">The end position (excluding) of the line within the text.</param>
        private static void AddResultLine(ICollection<string> lines, string text, int start, int end)
        {
            while (end > start && text[end - 1] == ' ')
            {
                end--;
            }

            lines.Add(text.Substring(start, end - start));
        }

        /// <summary>
        /// Returns the width of the specified text for the specified font size.
        /// </summary>
        /// <param name="text">The text to measure.</param>
        /// <param name="fontSize">The font size (in pt).</param>
        /// <param name="isBold">The flag indicating if the text is in bold or regular weight.</param>
        /// <returns>The text's width (in mm).</returns>
        public double TextWidth(string text, int fontSize, bool isBold)
        {
            if (isBold)
            {
                return _boldMetrics.TextWidth(text, fontSize, false);
            }

            double width = 0;
            var len = text.Length;
            for (var i = 0; i < len; i++)
            {
                width += CharWidth(text[i]);
            }

            return width * fontSize / 1000 * PtToMm;
        }

        /// <summary>
        /// Returns the width of the specified character.
        /// <para>
        /// The width is given in 0.001 pt for a font size of 1 pt. So to get the
        /// effective width in pt(1/72 in), it must be multiplied with the font size and
        /// divided by 1000.
        /// </para>
        /// <para>
        /// The method only supports characters as defined in "Swiss Implementation
        /// Guidelines for Credit Transfer Initiation". For all other characters, a
        /// default width is returned.
        /// </para>
        /// </summary>
        /// <param name="ch">The character to measure.</param>
        /// <returns>The width of the character.</returns>
        private short CharWidth(char ch)
        {
            short width = 0;
            if (ch >= 0x20 && ch <= 0x7f)
            {
                width = _charWidthx20x7F[ch - 0x20];
            }
            else if (ch >= 0xa0 && ch <= 0xff)
            {
                width = _charWidthxA0xFF[ch - 0xa0];
            }
            else if (ch >= 0x100 && ch <= 0x17f)
            {
                width = _charWidthx100x17F[ch - 0x100];
            }
            else if (ch >= 0x0218 && ch <= 0x021b)
            {
                width = _charWidthx218x21B[ch - 0x0218];
            }
            else if (ch == 0x2013)
            {
                width = _charNDashWidth;
            }
            else if (ch == 0x20AC)
            {
                width = _charEuroWidth;
            }

            if (width == 0 && ch != '\n' && ch != '\r')
            {
                width = _charDefaultWidth;
            }

            return width;
        }

        private static string GetFirstFontFamily(string fontFamilyList)
        {
            var index = fontFamilyList.IndexOf(',');
            if (index < 0)
            {
                return fontFamilyList;
            }

            var fontFamily = fontFamilyList.Substring(0, index).Trim();
            if (fontFamily.StartsWith("\""))
            {
                fontFamily = fontFamily.Substring(1);
            }

            if (fontFamily.EndsWith("\""))
            {
                fontFamily = fontFamily.Substring(0, fontFamily.Length - 1);
            }

            return fontFamily;
        }
    }
}
