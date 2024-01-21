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
    }
}
