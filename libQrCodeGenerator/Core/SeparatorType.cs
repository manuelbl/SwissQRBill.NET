//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//


namespace libQrCodeGenerator.SwissQRBill.Generator
{
    /// <summary>
    /// Separator type above and between payment part and receipt.
    /// </summary>
    public enum SeparatorType
    {
        /// <summary>
        /// No separators are drawn (for paper with perforation).
        /// </summary>
        None,
        /// <summary>
        /// Solid lines are drawn.
        /// </summary>
        SolidLine,
        /// <summary>
        /// Solid lines with a scissor symbol are drawn.
        /// </summary>
        SolidLineWithScissors,
        /// <summary>
        /// Dashed lines are drawn.
        /// </summary>
        DashedLine,
        /// <summary>
        /// Dashed lines with a scissor symbol are drawn.
        /// </summary>
        DashedLineWithScissors,
        /// <summary>
        /// Dotted lines are drawn.
        /// </summary>
        DottedLine,
        /// <summary>
        /// Dotted lines with a scissor symbol are drawn.
        /// </summary>
        DottedLineWithScissors
    }
}
