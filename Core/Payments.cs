//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System;
using System.ComponentModel;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Codecrete.SwissQRBill.Generator
{
    /// <summary>
    /// Field validations related to Swiss Payment standards.
    /// </summary>
    public static class Payments
    {
        /// <summary>
        /// Cleans a string value to make it viable for the Swiss Payment Standards 2018.
        /// <para>
        /// Unsupported characters are replaced with supported characters, either with the same character without accent
        /// (e.g. A instead of Ă), with characters of similar meaning (e.g. TM instead of ™, ij instead of ĳ),
        /// with a space (for unsupported whitespace characters) or with a dot.
        /// </para>
        /// <para>
        /// Some valid letters can be represented either with a single Unicode code point or with two code points,
        /// e.g. the letter A with umlaut can be represented either with the single code point U+00C4 (latin capital
        /// letter A with diaeresis) or with the two code points U+0041 U+0308 (latin capital letter A,
        /// combining diaeresis). This will be detected and converted to the valid single code point form.
        /// </para>
        /// <para>
        /// If <paramref name="value"/> is <c>null</c> or the resulting string would be empty, <c>null</c> is returned.
        /// </para>
        /// </summary>
        /// 
        /// <param name="value">The string value to clean.</param>
        /// <param name="result">The result to be filled with cleaned string and flag.</param>
        public static void CleanValue(string value, out CleaningResult result)
        {
            result = new CleaningResult();
            if (value == null)
            {
                return;
            }
            StringCleanup.CleanText(value, SpsCharacterSet.Latin1Subset, false, out result);
        }

        /// <summary>
        /// Returns a cleaned text valid according to the specified character set.
        /// <para>
        /// Unsupported characters are replaced with supported characters, either with the same character without accent
        /// (e.g. A instead of Ă), with characters of similar meaning (e.g. TM instead of ™, ij instead of ĳ),
        /// with a space (for unsupported whitespace characters) or with a dot.
        /// </para>
        /// <para>
        /// Some valid letters can be represented either with a single Unicode code point or with two code points,
        /// e.g. the letter A with umlaut can be represented either with the single code point U+00C4 (latin capital
        /// letter A with diaeresis) or with the two code points U+0041 U+0308 (latin capital letter A,
        /// combining diaeresis). This will be detected and converted to the valid single code point form.
        /// </para>
        /// <para>
        /// If <paramref name="text"/> is <c>null</c> or the resulting string would be empty, <c>null</c> is returned.
        /// </para>
        ///
        /// <param name="text">The string value to clean.</param>
        /// <param name="characterSet">Character set specifying valid characters.</param>
        /// <returns>valid text for Swiss payments</returns>
        /// </summary>
        public static string CleanedText(string text, SpsCharacterSet characterSet)
        {
            return StringCleanup.CleanedText(text, characterSet);
        }

        /// <summary>
        /// Returns a cleaned and trimmed text valid according to the specified character set.
        /// <para>
        /// Unsupported characters are replaced with supported characters, either with the same character without accent
        /// (e.g. A instead of Ă), with characters of similar meaning (e.g. TM instead of ™, ij instead of ĳ),
        /// with a space (for unsupported whitespace characters) or with a dot.
        /// </para>
        /// <para>
        /// Leading and trailing whitespace is removed. Multiple consecutive spaces are replaced with a single whitespace.
        /// </para>
        /// <para>
        /// Some valid letters can be represented either with a single Unicode code point or with two code points,
        /// e.g. the letter A with umlaut can be represented either with the single code point U+00C4 (latin capital
        /// letter A with diaeresis) or with the two code points U+0041 U+0308 (latin capital letter A,
        /// combining diaeresis). This will be detected and converted to the valid single code point form.
        /// </para>
        /// <para>
        /// If <paramref name="text"/> is <c>null</c> or the resulting string would be empty, <c>null</c> is returned.
        /// </para>
        ///
        /// <param name="text">The string value to clean.</param>
        /// <param name="characterSet">Character set specifying valid characters.</param>
        /// <returns>valid text for Swiss payments</returns>
        /// </summary>
        public static string CleanedAndTrimmedText(string text, SpsCharacterSet characterSet)
        {
            return StringCleanup.CleanedAndTrimmedText(text, characterSet);
        }

        /// <summary>
        /// Indicates if the text consists only of characters allowed in the specified character set.
        /// <para>
        /// This method does not attempt to deal with accents and umlauts built from two code points. It will
        /// return <c>false</c> if the text contains such characters.
        /// </para>
        /// </summary>
        /// <param name="text">text to check, possibly <c>null</c></param>
        /// <param name="characterSet">Character set specifying valid characters.</param>
        /// <returns><c>true</c> if the text is valid, <c>false</c> otherwise</returns>
        public static bool IsValidText(string text, SpsCharacterSet characterSet)
        {
            return StringCleanup.IsValidText(text, characterSet);
        }

        /// <summary>
        /// Validates if the string is a valid IBAN number
        /// <para>
        /// The string is checked for valid characters, valid length and for a valid
        /// check digit. White space is ignored.
        /// </para>
        /// </summary>
        /// <param name="iban">The IBAN to validate.</param>
        /// <returns><c>true</c> if the IBAN is valid, <c>false</c> otherwise.</returns>
        public static bool IsValidIban(string iban)
        {
            iban = iban.WhiteSpaceRemoved();

            var len = iban.Length;
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

            var checkDigits = iban.Substring(2, 2);
            if ("00" == checkDigits || "01" == checkDigits || "99" == checkDigits)
            {
                return false;
            }

            return HasValidMod97CheckDigits(iban);
        }

        /// <summary>
        /// Indicates if the string is a valid QR-IBAN.
        /// <para>
        /// QR-IBANs are IBANs with an institution ID in the range 30000 to 31999
        /// and a country code for Switzerland or Liechtenstein.
        /// Thus, they must have the format "CH..30...", "CH..31...", "LI..30..." or "LI..31...".
        /// </para>
        /// </summary>
        /// <param name="iban">account number to check</param>
        /// <returns><c>true</c> for valid QR-IBANs, <c>false</c> otherwise</returns>
        public static bool IsQrIban(string iban)
        {
            iban = iban.WhiteSpaceRemoved().ToUpperInvariant();

            return IsValidIban(iban)
                    && (iban.StartsWith("CH") || iban.StartsWith("LI"))
                    && iban[4] == '3'
                    && (iban[5] == '0' || iban[5] == '1');
        }

        /// <summary>
        /// Formats an IBAN or creditor reference by inserting spaces
        /// <para>
        /// Spaces are inserted to form groups of 4 letters/digits. If a group of less
        /// than 4 letters/digits is needed, it appears at the end.
        /// </para>
        /// </summary>
        /// <param name="iban">The IBAN or creditor reference without spaces.</param>
        /// <returns>The formatted IBAN or creditor reference.</returns>
        public static string FormatIban(string iban)
        {
            var sb = new StringBuilder(25);
            var len = iban.Length;

            for (var pos = 0; pos < len; pos += 4)
            {
                var endPos = pos + 4;
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
        /// <para>
        /// The string is checked for valid characters, valid length and a valid check
        /// digit. White space is ignored.
        /// </para>
        /// </summary>
        /// <param name="reference">The ISO 11649 creditor reference to validate.</param>
        /// <returns><c>true</c> if the creditor reference is valid, <c>false</c> otherwise.</returns>
        public static bool IsValidIso11649Reference(string reference)
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
        /// <para>
        /// Whitespace is removed from the reference.
        /// </para>
        /// </summary>
        /// <param name="rawReference">The raw reference.</param>
        /// <returns>The created creditor reference.</returns>
        /// <exception cref="ArgumentException"><c>reference</c> contains invalid characters.</exception>
        public static string CreateIso11649Reference(string rawReference)
        {
            var whiteSpaceRemoved = rawReference.WhiteSpaceRemoved();
            var modulo = CalculateMod97("RF00" + whiteSpaceRemoved);
            return $"RF{98 - modulo:D2}{whiteSpaceRemoved}";
        }

        private static bool HasValidMod97CheckDigits(string number)
        {
            return CalculateMod97(number) == 1;
        }

        /// <summary>
        /// Calculates the reference's modulo 97 checksum according to ISO11649 and IBAN standard.
        /// <para>
        /// The string may only contains digits, letters ('A' to 'Z' and 'a' to 'z', no
        /// accents). It must not contain white space.
        /// </para>
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <returns>The calculated checksum (0 to 96).</returns>
        /// <exception cref="ArgumentException">The reference contains an invalid character.</exception>
        private static int CalculateMod97(string reference)
        {
            var len = reference.Length;
            if (len < 5)
            {
                throw new ArgumentException("Insufficient characters for checksum calculation");
            }

            var rearranged = reference.Substring(4) + reference.Substring(0, 4);
            var sum = 0;
            for (var i = 0; i < len; i++)
            {
                var ch = rearranged[i];
                if (ch >= '0' && ch <= '9')
                {
                    sum = sum * 10 + (ch - '0');
                }
                else if (ch >= 'A' && ch <= 'Z')
                {
                    sum = sum * 100 + ch - 'A' + 10;
                }
                else if (ch >= 'a' && ch <= 'z')
                {
                    sum = sum * 100 + ch - 'a' + 10;
                }
                else
                {
                    throw new ArgumentException("Invalid character in reference: " + ch);
                }
                if (sum > 9999999)
                {
                    sum %= 97;
                }
            }

            sum %= 97;
            return sum;
        }

        private static readonly int[] Mod10 = { 0, 9, 4, 6, 8, 2, 7, 1, 3, 5 };

        /// <summary>
        /// Validates if the string is a valid QR reference.
        /// <para>
        /// A valid QR reference is a valid ISR reference.
        /// </para>
        /// <para>
        /// The string is checked for valid characters, valid length and a valid check
        /// digit. White space is ignored.
        /// </para>
        /// </summary>
        /// <param name="reference">The QR reference number to validate.</param>
        /// <returns><c>true</c> if the reference number is valid, <c>false</c> otherwise.</returns>
        public static bool IsValidQrReference(string reference)
        {

            reference = reference.WhiteSpaceRemoved();

            if (!IsNumeric(reference))
            {
                return false;
            }

            var len = reference.Length;
            if (len != 27)
            {
                return false;
            }

            return CalculateMod10(reference) == 0;
        }

        /// <summary>
        /// Creates a QR reference from a raw string by appending the checksum digit
        /// and prepending zeros to make it the correct length.
        /// <para>
        /// Whitespace is removed from the reference.
        /// </para>
        /// </summary>
        /// <param name="rawReference">The raw string (digits and whitespace only).</param>
        /// <returns>The QR reference.</returns>
        /// <exception cref="ArgumentException">The reference contains non-numeric characters or is too long</exception>
        public static string CreateQRReference(string rawReference)
        {
            var rawRef = rawReference.WhiteSpaceRemoved();
            if (!IsNumeric(rawRef))
                throw new ArgumentException("Invalid character in reference (digits allowed only)");
            if (rawRef.Length > 26)
                throw new ArgumentException("Reference number is too long");
            var mod10 = CalculateMod10(rawRef);
            return "00000000000000000000000000".Substring(0, 26 - rawRef.Length) + rawRef + (char)('0' + mod10);
        }

        private static int CalculateMod10(string reference)
        {
            var len = reference.Length;
            var carry = 0;
            for (var i = 0; i < len; i++)
            {
                var digit = reference[i] - '0';
                carry = Mod10[(carry + digit) % 10];
            }

            return (10 - carry) % 10;
        }

        /// <summary>
        /// Formats a QR reference number by inserting spaces.
        /// <para>
        /// Spaces are inserted to create groups of 5 digits. If a group of less than 5
        /// digits is needed, it appears at the start of the formatted reference number.
        /// </para>
        /// </summary>
        /// <param name="refNo">The reference number without white space.</param>
        /// <returns>the formatted reference number.</returns>
        public static string FormatQrReferenceNumber(string refNo)
        {
            var len = refNo.Length;
            var sb = new StringBuilder();
            var t = 0;
            while (t < len)
            {
                var n = t + (len - t - 1) % 5 + 1;
                if (t != 0)
                {
                    sb.Append(" ");
                }

                sb.Append(refNo, t, n - t);
                t = n;
            }

            return sb.ToString();
        }

        internal static bool IsNumeric(string value)
        {
            var len = value.Length;
            for (var i = 0; i < len; i++)
            {
                var ch = value[i];
                if (ch < '0' || ch > '9')
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsAlphaNumeric(string value)
        {
            var len = value.Length;
            for (var i = 0; i < len; i++)
            {
                var ch = value[i];
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

        internal static bool IsAlpha(string value)
        {
            var len = value.Length;
            for (var i = 0; i < len; i++)
            {
                var ch = value[i];
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

        /// <summary>
        /// Result of cleaning a string
        /// </summary>
        public struct CleaningResult
        {
            /// <summary>
            /// Gets/sets the cleaned string.
            /// </summary>
            /// <value>The cleaned string.</value>
            public string CleanedString { get; set; }

            /// <summary>
            /// Gets/sets the flag indicating if unsupported characters have been replaced.
            /// </summary>
            /// <value>Flag indicating if unsupported characters have been replaced.</value>
            public bool ReplacedUnsupportedChars { get; set; }
        }
    }
}
