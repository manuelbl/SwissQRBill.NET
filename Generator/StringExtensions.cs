//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System.Text;

namespace Codecrete.SwissQRBill.Generator
{
    public static class StringExtensions
    {
        /// <summary>
        /// Returns a new string with leading and trailing whitespace removed
        /// </summary>
        /// <remarks>
        /// For empty strings or <c>null</c>, <c>null</c> is returned.
        /// </remarks>
        /// <param name="value">value string to trim or <c>null</c></param>
        /// <returns>trimmed string</returns>
        public static string Trimmed(this string value)
        {
            if (value == null)
            {
                return null;
            }

            value = value.Trim();
            if (value.Length == 0)
            {
                return null;
            }

            return value;
        }

        /// <summary>
        /// Returns a new string without white space
        /// </summary>
        /// <param name="value">string to process (non null)</param>
        /// <returns>resulting string with all whitespace removed</returns>
        public static string WhiteSpaceRemoved(this string value)
        {
            StringBuilder sb = null;
            int len = value.Length;
            int lastCopied = 0;
            for (int i = 0; i < len; i++)
            {
                char ch = value[i];
                if (ch <= ' ')
                {
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
            }

            if (sb == null)
            {
                if (lastCopied == 0)
                {
                    return value;
                }

                if (lastCopied == len)
                {
                    return "";
                }

                return value.Substring(lastCopied, len - lastCopied);
            }

            if (lastCopied < len)
            {
                sb.Append(value, lastCopied, len - lastCopied);
            }

            return sb.ToString();
        }
    }
}
