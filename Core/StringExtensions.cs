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

#pragma warning disable S3776

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

#pragma warning restore S3776

    }
}
