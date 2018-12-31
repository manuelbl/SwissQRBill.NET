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
        private static readonly double PT_TO_MM = 25.4 / 72;

        private readonly ushort[] charWidthx20x7F;
        private readonly ushort[] charWidthxA0xFF;
        private readonly ushort charDefaultWidth;
        private FontMetrics boldMetrics;

        /// <summary>
        /// Initializes a new instance for the given list of font families.
        /// </summary>
        /// <param name="fontFamilyList">Font families, separated by comma (syntax as in CSS)</param>
        /// <remarks>
        /// If more than one family is specified, the first family is used for metrics.
        /// </remarks>
        public FontMetrics(string fontFamilyList)
        {
            FontFamilyList = fontFamilyList;
            FirstFontFamily = GetFirstFontFamily(fontFamilyList);
            string family = FirstFontFamily.ToLowerInvariant();

            ushort[] boldCharWidthx20x7F;
            ushort[] boldCharWidthxA0xFF;
            ushort boldCharDefaultWidth;

            if (family.Contains("arial"))
            {
                charWidthx20x7F = CharWidthData.ArialNormal_20_7F;
                charWidthxA0xFF = CharWidthData.ArialNormal_A0_FF;
                charDefaultWidth = CharWidthData.ArialNormalDefaultWidth;
                boldCharWidthx20x7F = CharWidthData.ArialBold_20_7F;
                boldCharWidthxA0xFF = CharWidthData.ArialBold_A0_FF;
                boldCharDefaultWidth = CharWidthData.ArialBoldDefaultWdith;
            }
            else if (family.Contains("liberation") && family.Contains("sans"))
            {
                charWidthx20x7F = CharWidthData.LiberationSansNormal_20_7F;
                charWidthxA0xFF = CharWidthData.LiberationSansNormal_A0_FF;
                charDefaultWidth = CharWidthData.LiberationSansNormalDefaultWidth;
                boldCharWidthx20x7F = CharWidthData.LiberationSansBold_20_7F;
                boldCharWidthxA0xFF = CharWidthData.LiberationSansBold_A0_FF;
                boldCharDefaultWidth = CharWidthData.LiberationSansBoldDefaultWidth;
            }
            else if (family.Contains("frutiger"))
            {
                charWidthx20x7F = CharWidthData.FrutigerNormal_20_7F;
                charWidthxA0xFF = CharWidthData.FrutigerNormal_A0_FF;
                charDefaultWidth = CharWidthData.FrutigerNormalDefaultWidth;
                boldCharWidthx20x7F = CharWidthData.FrutigerBold_20_7F;
                boldCharWidthxA0xFF = CharWidthData.FrutigerBold_A0_FF;
                boldCharDefaultWidth = CharWidthData.FrutigerBoldDefaultWidth;
            }
            else
            {
                charWidthx20x7F = CharWidthData.HelveticaNormal_20_7F;
                charWidthxA0xFF = CharWidthData.HelveticaNormal_A0_FF;
                charDefaultWidth = CharWidthData.HelveticaNormalDefaultWidth;
                boldCharWidthx20x7F = CharWidthData.HelveticaBold_20_7F;
                boldCharWidthxA0xFF = CharWidthData.HelveticaBold_A0_FF;
                boldCharDefaultWidth = CharWidthData.HelveticaBoldDefaultWidth;
            }

            boldMetrics = new FontMetrics(boldCharWidthx20x7F, boldCharWidthxA0xFF, boldCharDefaultWidth);
        }

        private FontMetrics(ushort[] charWidthx20x7F, ushort[] charWidthxA0xFF, ushort charDefaultWidth)
        {
            FontFamilyList = null;
            FirstFontFamily = null;
            this.charWidthx20x7F = charWidthx20x7F;
            this.charWidthxA0xFF = charWidthxA0xFF;
            this.charDefaultWidth = charDefaultWidth;
            boldMetrics = null;
        }

        /// <summary>
        /// Gets the font family list (comma separated, same synta as for CSS)
        /// </summary>
        public string FontFamilyList { get; private set; }

        /// <summary>
        /// Gets the first font family (from the font family list)
        /// </summary>
        public string FirstFontFamily { get; private set; }

        /// <summary>
        /// Distance between baseline and top of highest letter.
        /// </summary>
        /// <param name="fontSize">the font size (in pt)</param>
        /// <returns>the distance (in mm)</returns>
        public double Ascender(int fontSize)
        {
            return fontSize * 0.8 * PT_TO_MM;
        }

        /// <summary>
        /// Distance between baseline and bottom of letter extending the farest below the baseline.
        /// </summary>
        /// <param name="fontSize">the font size (in pt)</param>
        /// <returns>the distance (in mm)</returns>
        public double Descender(int fontSize)
        {
            return fontSize * 0.2 * PT_TO_MM;
        }

        /// <summary>
        /// Distance between the baselines of two consecutive text lines.
        /// </summary>
        /// <param name="fontSize">the font size (in pt)</param>
        /// <returns>the distance (in mm)</returns>
        public double LineHeight(int fontSize)
        {
            return fontSize * PT_TO_MM;
        }

        /// <summary>
        /// Splits the text into lines.
        /// </summary>
        /// <remarks>
        /// If a line would exceed the specified maximum length, line breaks are
        /// inserted.Newlines are treated as fixed line breaks.
        /// </remarks>
        /// <param name="text">the text</param>
        /// <param name="maxLength">the maximum line length (in mm)</param>
        /// <param name="fontSize">the font size (in pt)</param>
        /// <returns>an array of text lines</returns>
        public string[] SplitLines(string text, double maxLength, int fontSize)
        {
            List<string> lines = new List<string>();
            int max = (int)(maxLength * 1000 / fontSize);

            int len = text.Length; // length of line
            int pos = 0; // current position (0 ..< end)
            int lineStartPos = 0; // start position of current line
            int lineWidth = 0; // current line width (in AFM metric)
            bool addEmptyLine = true; // flag if an empty line should be added as the last line

            // iterate over all characters
            while (pos < len)
            {

                // get current character
                char ch = text[pos];

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
                        int spacePos = pos - 1;
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
        /// Add the specified text range to the resulting lines.
        /// </summary>
        /// <remarks>
        /// Trim trailing white space.
        /// </remarks>
        /// <param name="lines">resulting lines array</param>
        /// <param name="text">text</param>
        /// <param name="start">start of text range (including)</param>
        /// <param name="end">end of text range (excluding)</param>
        private static void AddResultLine(List<string> lines, string text, int start, int end)
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
        /// <param name="text">text</param>
        /// <param name="fontSize">font size (in pt)</param>
        /// <param name="isBold">indicates if the text is in bold or regular weight</param>
        /// <returns>width (in mm)</returns>
        public double GetTextWidth(string text, int fontSize, bool isBold)
        {
            if (isBold)
            {
                return boldMetrics.GetTextWidth(text, fontSize, false);
            }

            double width = 0;
            int len = text.Length;
            for (int i = 0; i < len; i++)
            {
                width += CharWidth(text[i]);
            }

            return width * fontSize / 1000 * PT_TO_MM;
        }

        /// <summary>
        /// Returns the width of the specified character.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The width is given in 0.0001 pt for a font size of 1 pt.So to get the
        /// effective width in pt(1/72 in), it must be multiplied with the font size and
        /// divided by 1000.
        /// </para>
        /// <para>
        /// The method only supports characters as defined in "Swiss Implementation
        /// Guidelines for Credit Transfer Initiation". For all other characters, a
        /// default width is returned.
        /// </para>
        /// </remarks>
        /// <param name="ch">the character</param>
        /// <returns>the width of the character</returns>
        private ushort CharWidth(char ch)
        {
            ushort width = 0;
            if (ch >= 0x20 && ch <= 0x7f)
            {
                width = charWidthx20x7F[ch - 0x20];
            }
            else if (ch >= 0xa0 && ch <= 0xff)
            {
                width = charWidthxA0xFF[ch - 0xa0];
            }
            if (width == 0)
            {
                width = charDefaultWidth;
            }

            return width;
        }

        private static string GetFirstFontFamily(string fontFamilyList)
        {
            int index = fontFamilyList.IndexOf(',');
            if (index < 0)
            {
                return fontFamilyList;
            }

            string fontFamily = fontFamilyList.Substring(0, index).Trim();
            if (fontFamily.StartsWith("\""))
            {
                fontFamily = fontFamily.Substring(1);
            }

            if (fontFamily.EndsWith(("\"")))
            {
                fontFamily = fontFamily.Substring(0, fontFamily.Length - 1);
            }

            return fontFamily;
        }
    }
}
