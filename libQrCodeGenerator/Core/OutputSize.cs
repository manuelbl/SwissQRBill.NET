//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System;

namespace libQrCodeGenerator.SwissQRBill.Generator
{
    /// <summary>
    /// The output size of the QR bill or QR code.
    /// </summary>
    /// <seealso href="https://github.com/manuelbl/SwissQRBill/wiki/Output-Sizes">Output Sizes (in Wiki)</seealso>
    public enum OutputSize
    {
        /// <summary>
        /// QR bill only (105 by 210 mm).
        /// <para>
        /// This size is suitable if the QR bill has no horizontal line.
        /// If the horizontal line is needed and the A4 sheet size is not
        /// suitable, use <see cref="QrBillExtraSpace"/> instead.
        /// </para>
        /// </summary>
        QrBillOnly,
        /// <summary>
        /// A4 sheet in portrait orientation. The QR bill is at the bottom.
        /// </summary>
        A4PortraitSheet,
        /// <summary>
        /// QR code only (46 by 46 mm).
        /// </summary>
        QrCodeOnly,
        /// <summary>
        /// QR bill only with additional space at the top for the horizontal line (about 110 by 210 mm).
        /// <para>
        /// The extra 5 mm at the top create space for the horizontal line and
        /// optionally for the scissors.
        /// </para>
        /// </summary>
        QrBillExtraSpace
    }
}
