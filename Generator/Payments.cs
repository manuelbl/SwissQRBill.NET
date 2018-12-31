//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System;
using System.Globalization;
using System.Text;

namespace Codecrete.SwissQRBill.Generator
{
    /// <summary>
    /// Field validations related to Swiss Payment standards.
    /// </summary>
    public static class Payments
    {
        /// <summary>
        /// Cleans a string value to make it viable for the Swiss Payment Standards 2018.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Unsupported characters(according to Swiss Payment Standards 2018, ch. 2.4.1
        /// and appendix D) are replaced with spaces(unsupported whitespace) or dots
        /// (all other unsupported characters). Leading and trailing whitespace is
        /// removed.
        /// </para>
        /// <para>
        /// If characters beyond 0xff are detected, the string is first normalized such
        /// that letters with umlauts or accents expressed with two code points are
        /// merged into a single code point(if possible), some of which might become
        /// valid.
        /// </para>
        /// <para>
        /// If the resulting strings is all white space, <c>null</c> is returned.
        /// </para>
        /// </remarks>
        /// <param name="value">string value to clean</param>
        /// <param name="result">result to be filled with cleaned string and flag</param>
        public static void CleanValue(string value, out CleaningResult result)
        {
            CleanValue(value, out result, false);
            if (result.CleanedString != null && result.CleanedString.Length == 0)
            {
                result.CleanedString = null;
            }
        }

        private static void CleanValue(string value, out CleaningResult result, bool isNormalized)
        {
            result = new CleaningResult();
            if (value == null)
            {
                return;
            }

            int len = value.Length; // length of value
            bool justProcessedSpace = false; // flag indicating whether we've just processed a space character
            StringBuilder sb = null; // String builder for result
            int lastCopiedPos = 0; // last position (excluding) copied to the result

            // String processing pattern: Iterate all characters and focus on runs of valid
            // characters that can simply be copied. If all characters are valid, no memory
            // is allocated.
            int pos = 0;
            while (pos < len)
            {
                char ch = value[pos]; // current character

                if (IsValidQRBillCharacter(ch))
                {
                    justProcessedSpace = ch == ' ';
                    pos++;
                    continue;
                }

                // Check for normalization
                if (ch > 0xff && !isNormalized)
                {
                    isNormalized = value.IsNormalized(NormalizationForm.FormC);
                    if (!isNormalized)
                    {
                        // Normalize string and start over
                        value = value.Normalize(NormalizationForm.FormC);
                        CleanValue(value, out result, true);
                        return;
                    }
                }

                if (sb == null)
                {
                    sb = new StringBuilder(value.Length);
                }

                // copy processed characters to result before taking care of the invalid
                // character
                if (pos > lastCopiedPos)
                {
                    sb.Append(value, lastCopiedPos, pos - lastCopiedPos);
                }

                if (char.IsHighSurrogate(ch))
                {
                    // Proper Unicode handling to prevent surrogates and combining characters
                    // from being replaced with multiples periods.
                    UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(value, pos);
                    if (category != UnicodeCategory.SpacingCombiningMark)
                    {
                        sb.Append('.');
                    }

                    justProcessedSpace = false;
                    pos++;
                }
                else
                {
                    if (ch <= ' ')
                    {
                        if (!justProcessedSpace)
                        {
                            sb.Append(' ');
                        }

                        justProcessedSpace = true;
                    }
                    else
                    {
                        sb.Append('.');
                        justProcessedSpace = false;
                    }
                }
                pos++;
                lastCopiedPos = pos;
            }

            if (sb == null)
            {
                result.CleanedString = value.Trim();
                return;
            }

            if (lastCopiedPos < len)
            {
                sb.Append(value, lastCopiedPos, len - lastCopiedPos);
            }

            result.CleanedString = sb.ToString().Trim();
            result.ReplacedUnsupportedChars = true;
        }

        /// <summary>
        /// Validates if the string is a valid IBAN number
        /// </summary>
        /// <remarks>
        /// The string is checked for valid characters, valid length and for a valid
        /// check digit. White space is ignored.
        /// </remarks>
        /// <param name="iban">IBAN to validate</param>
        /// <returns><c>true</c> if the IBAN is valid, <c>false</c> otherwise</returns>
        public static bool IsValidIBAN(string iban)
        {

            iban = iban.WhiteSpaceRemoved();

            int len = iban.Length;
            if (len < 5)
            {
                return false;
            }

            if (!IsAlphaNumeric(iban))
            {
                return false;
            }

            // Check for country code
            if (!char.IsLetter(iban[0]) || !char.IsLetter(iban[1]))
            {
                return false;
            }

            // Check for check digits
            if (!char.IsDigit(iban[2]) || !char.IsDigit(iban[3]))
            {
                return false;
            }

            string checkDigits = iban.Substring(2, 2);
            if ("00" == checkDigits || "01" == checkDigits || "99" == checkDigits)
            {
                return false;
            }

            return HasValidMod97CheckDigits(iban);
        }

        /// <summary>
        /// Formats an IBAN or creditor reference by inserting spaces
        /// </summary>
        /// <remarks>
        /// Spaces are inserted to form groups of 4 letters/digits. If a group of less
        /// than 4 letters/digits is needed, it appears at the end.
        /// </remarks>
        /// <param name="iban">IBAN or creditor reference without spaces</param>
        /// <returns>formatted IBAN or creditor reference</returns>
        public static string FormatIBAN(string iban)
        {
            StringBuilder sb = new StringBuilder(25);
            int len = iban.Length;

            for (int pos = 0; pos < len; pos += 4)
            {
                int endPos = pos + 4;
                if (endPos > len)
                {
                    endPos = len;
                }

                sb.Append(iban, pos, endPos - pos);
                if (endPos != len)
                {
                    sb.Append(' ');
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Validates if the string is a valid ISO 11649 reference number
        /// </summary>
        /// <remarks>
        /// The string is checked for valid characters, valid length and a valid check
        /// digit. White space is ignored.
        /// </remarks>
        /// <param name="reference">ISO 11649 creditor reference to validate</param>
        /// <returns><c>true</c> if the creditor reference is valid, <c>false</c></returns>
        public static bool IsValidISO11649Reference(string reference)
        {

            reference = reference.WhiteSpaceRemoved();

            if (reference.Length < 5 || reference.Length > 25)
            {
                return false;
            }

            if (!IsAlphaNumeric(reference))
            {
                return false;
            }

            if (reference[0] != 'R' || reference[1] != 'F')
            {
                return false;
            }

            if (!char.IsDigit(reference[2]) || !char.IsDigit(reference[3]))
            {
                return false;
            }

            return HasValidMod97CheckDigits(reference);
        }

        /// <summary>
        /// Creates a ISO11649 creditor reference from a raw string by prefixing the
        /// string with "RF" and the modulo 97 checksum
        /// </summary>
        /// <remarks>
        /// Whitespace is removed from the reference
        /// </remarks>
        /// <param name="rawReference">The raw string</param>
        /// <returns>creditor reference</returns>
        /// <exception cref="ArgumentException">Thrown if <c>reference</c> contains invalid characters</exception>
        public static string CreateISO11649Reference(string rawReference)
        {
            string whiteSpaceRemoved = rawReference.WhiteSpaceRemoved();
            int modulo = CalculateMod97("RF00" + whiteSpaceRemoved);
            return $"RF{98 - modulo:D2}{whiteSpaceRemoved}";
        }

        private static bool HasValidMod97CheckDigits(string number)
        {
            return CalculateMod97(number) == 1;
        }

        /// <summary>
        /// Calculate the reference's modulo 97 checksum according to ISO11649 and IBAN standard.
        /// </summary>
        /// <remarks>
        /// The string may only contains digits, letters ('A' to 'Z' and 'a' to 'z', no
        /// accents). It must not contain white space.
        /// </remarks>
        /// <param name="reference">the reference</param>
        /// <returns>the checksum (0 to 96)</returns>
        /// <exception cref="ArgumentException">Thrown if the reference contains an invalid character</exception>
        private static int CalculateMod97(string reference)
        {
            int len = reference.Length;
            if (len < 5)
            {
                throw new ArgumentException("Insufficient characters for checksum calculation");
            }

            string rearranged = reference.Substring(4) + reference.Substring(0, 4);
            int sum = 0;
            for (int i = 0; i < len; i++)
            {
                char ch = rearranged[i];
                if (ch >= '0' && ch <= '9')
                {
                    sum = sum * 10 + (ch - '0');
                }
                else if (ch >= 'A' && ch <= 'Z')
                {
                    sum = sum * 100 + (ch - 'A' + 10);
                }
                else if (ch >= 'a' && ch <= 'z')
                {
                    sum = sum * 100 + (ch - 'a' + 10);
                }
                else
                {
                    throw new ArgumentException("Invalid character in reference: " + ch);
                }
                if (sum > 9999999)
                {
                    sum = sum % 97;
                }
            }

            sum = sum % 97;
            return sum;
        }

        private static readonly int[] MOD_10 = { 0, 9, 4, 6, 8, 2, 7, 1, 3, 5 };

        /// <summary>
        /// Validates if the string is a valid QR reference.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A valid QR reference is a valid ISR reference.
        /// </para>
        /// <para>
        /// The string is checked for valid characters, valid length and a valid check
        /// digit. White space is ignored.
        /// </para>
        /// </remarks>
        /// <param name="reference">QR reference number to validate</param>
        /// <returns><c>true</c> if the reference number is valid, <c>false</c> otherwise</returns>
        public static bool IsValidQRReference(string reference)
        {

            reference = reference.WhiteSpaceRemoved();

            if (!IsNumeric(reference))
            {
                return false;
            }

            int carry = 0;
            int len = reference.Length;
            if (len != 27)
            {
                return false;
            }

            for (int i = 0; i < len; i++)
            {
                int digit = reference[i] - '0';
                carry = MOD_10[(carry + digit) % 10];
            }

            return carry == 0;
        }

        /// <summary>
        /// Formats a QR reference number by inserting spaces.
        /// </summary>
        /// <remarks>
        /// Spaces are inserted to create groups of 5 digits. If a group of less than 5
        /// digits is needed, it appears at the start of the formatted reference number.
        /// </remarks>
        /// <param name="refNo">reference number without white space</param>
        /// <returns>reference number</returns>
        public static string FormatQRReferenceNumber(string refNo)
        {
            int len = refNo.Length;
            StringBuilder sb = new StringBuilder();
            int t = 0;
            while (t < len)
            {
                int n = t + (len - t - 1) % 5 + 1;
                if (t != 0)
                {
                    sb.Append(" ");
                }

                sb.Append(refNo, t, n - t);
                t = n;
            }

            return sb.ToString();
        }

        private static bool IsNumeric(string value)
        {
            int len = value.Length;
            for (int i = 0; i < len; i++)
            {
                char ch = value[i];
                if (ch < '0' || ch > '9')
                {
                    return false;
                }
            }
            return true;
        }

        internal static bool IsAlphaNumeric(string value)
        {
            int len = value.Length;
            for (int i = 0; i < len; i++)
            {
                char ch = value[i];
                if (ch >= '0' && ch <= '9')
                {
                    continue;
                }

                if (ch >= 'A' && ch <= 'Z')
                {
                    continue;
                }

                if (ch >= 'a' && ch <= 'z')
                {
                    continue;
                }

                return false;
            }
            return true;
        }

        private static bool IsValidQRBillCharacter(char ch)
        {
            if (ch < 0x20)
            {
                return false;
            }

            if (ch == 0x5e)
            {
                return false;
            }

            if (ch <= 0x7e)
            {
                return true;
            }

            if (ch == 0xa3 || ch == 0xb4)
            {
                return true;
            }

            if (ch < 0xc0 || ch > 0xfd)
            {
                return false;
            }

            if (ch == 0xc3 || ch == 0xc5 || ch == 0xc6)
            {
                return false;
            }

            if (ch == 0xd0 || ch == 0xd5 || ch == 0xd7 || ch == 0xd8)
            {
                return false;
            }

            if (ch == 0xdd || ch == 0xde)
            {
                return false;
            }

            if (ch == 0xe3 || ch == 0xe5 || ch == 0xe6)
            {
                return false;
            }

            if (ch == 0xf0 || ch == 0xf5 || ch == 0xf8)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Result of cleaning a string
        /// </summary>
        public struct CleaningResult
        {
            /// <summary>
            /// Cleaned string
            /// </summary>
            public string CleanedString { get; set; }

            /// <summary>
            /// Flag indicating that unsupported characters have been replaced
            /// </summary>
            public bool ReplacedUnsupportedChars { get; set; }
        }

    }
}
