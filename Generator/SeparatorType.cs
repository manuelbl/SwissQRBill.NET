//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//


namespace Codecrete.SwissQRBill.Generator
{
    /// <summary>
    /// Separator type above and betweeen payment part and receipt.
    /// </summary>
    public enum SeparatorType
    {
        /// <summary>
        /// No separators are drawn (for paper with perforation)
        /// </summary>
        None,
        /// <summary>
        /// Solid lines are drawn
        /// </summary>
        SolidLine,
        /// <summary>
        /// Solid lines with a scissor symbol are drawn
        /// </summary>
        SolidLineWithScissors
    }
}
