//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System.Text;

namespace Codecrete.SwissQRBill.Generator
{
    /// <summary>
    /// Extension methods for <see cref="string"/>.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Returns a new string with leading and trailing whitespace removed.
        /// <para>
        /// For empty strings or <c>null</c>, <c>null</c> is returned.
        /// </para>
        /// </summary>
        /// <param name="value">The string instance that this method extends or <c>null</c>.</param>
        /// <returns>The trimmed string or <c>null</c>.</returns>
        public static string Trimmed(this string value)
        {
            if (value == null)
            {
                return null;
            }

            value = value.Trim();
            return value.Length == 0 ? null : value;
        }

        /// <summary>
        /// Returns a new string without white space.
        /// </summary>
        /// <param name="value">The string instance that this method extends</param>
        /// <returns>The resulting string with all whitespace removed.</returns>
        public static string WhiteSpaceRemoved(this string value)
        {
            StringBuilder sb = null;
            var len = value.Length;
            var lastCopied = 0;
            for (var i = 0; i < len; i++)
            {
                var ch = value[i];
                if (ch > ' ')
                {
                    continue;
                }

                if (i > lastCopied)
                {
                    if (sb == null)
                    {
                        sb = new StringBuilder();
                    }

                    sb.Append(value, lastCopied, i - lastCopied);
                }
                lastCopied = i + 1;
            }

            if (sb == null)
            {
                if (lastCopied == 0)
                {
                    return value;
                }

                return lastCopied == len ? "" : value.Substring(lastCopied, len - lastCopied);
            }

            if (lastCopied < len)
            {
                sb.Append(value, lastCopied, len - lastCopied);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns a new string without leading and trailing spaces and with all
        /// consecutive space characters replaced by a single space.
        /// <para>
        /// Whitespace other than space characters are not cleaned.
        /// </para>
        /// </summary>
        /// <param name="value">The string instance that this method extends</param>
        /// <returns>The resulting string with all whitespace removed.</returns>
        public static string SpacesCleaned(this string value)
        {
            value = value.Trim();

            int len = value.Length;
            bool inWhitespace = false;
            StringBuilder sb = null;

            for (int i = 0; i < len; i += 1)
            {
                char ch = value[i];
                if (ch == ' ' && inWhitespace && sb == null)
                {
                    sb = new StringBuilder(value.Length);
                    sb.Append(value, 0, i);
                }
                else if (ch != ' ' || !inWhitespace)
                {
                    inWhitespace = ch == ' ';
                    sb?.Append(ch);
                }
            }

            return sb != null ? sb.ToString() : value;
        }

        /// <summary>
        /// Appends the given Unicode code point to this instance. 
        /// </summary>
        /// <param name="sb">this instance</param>
        /// <param name="codePoint">the Unicode code point</param>
        public static void AppendCodePoint(this StringBuilder sb, int codePoint)
        {
            int len = GetUtf16SequenceLength((uint)codePoint);
            if (len == 1)
            {
                sb.Append((char)codePoint);
            }
            else
            {
                sb.Append(char.ConvertFromUtf32(codePoint));
            }
        }

        /// <summary>
        /// Given a Unicode scalar value, gets the number of UTF-16 code units required to represent this value.
        /// </summary>
        internal static int GetUtf16SequenceLength(uint value)
        {
            value -= 0x10000;   // if value < 0x10000, high byte = 0xFF; else high byte = 0x00
            value += 2 << 24;   // if value < 0x10000, high byte = 0x01; else high byte = 0x02
            value >>= 24;       // shift high byte down
            return (int)value;  // and return it
        }
    }
}
