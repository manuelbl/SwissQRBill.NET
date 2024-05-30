//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2024 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

namespace Codecrete.SwissQRBill.Generator
{
    /// <summary>
    /// Swiss Payment Standard character set.
    /// <para>
    /// The character set defines the allowed characters in the various payment fields.
    /// </para>
    /// </summary>
    public enum SpsCharacterSet
    {
        /// <summary>
        /// Restrictive character set from the original Swiss Payment Standard and original QR bill specification.
        /// <para>
        /// Valid characters consist of a subset of the printable Latin-1 characters in the Unicode blocks <i>Basic Latin</i>
        /// and <i>Latin-1 Supplement</i>.
        /// </para>
        /// </summary>
        Latin1Subset,

        /// <summary>
        /// Extended Latin character set.
        /// <para>
        /// Valid characters are all printable characters from the Unicode blocks <i>Basic Latin</i> (Unicode codePoints
        /// U+0020 to U+007E), <i>Latin-1 Supplement</i> (Unicode codePoints U+00A0 to U+00FF) and <i>Latin Extended A</i>
        /// (Unicode codePoints U+0100 to U+017F) plus a few additional characters (such as the Euro sign).
        /// </para>
        /// <para>
        /// This character set has been introduced with SPS 2022 (November 18, 2022) but may not be used in QR bills
        /// until November 21, 2025 when all banks are ready to accept messages with this character set.
        /// </para>
        /// </summary>
        ExtendedLatin,

        /// <summary>
        /// Full Unicode character set.
        /// <para>
        /// This character set may be used when decoding the QR code text. It is not suitable for generating QR bills
        /// or payment messages in general, and it is not covered by the Swiss Payment Standard.
        /// </para>
        /// </summary>
        FullUnicode
    }
}
