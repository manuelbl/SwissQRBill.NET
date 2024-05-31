//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System.Drawing;

namespace Codecrete.SwissQRBill.Generator
{
    /// <summary>
    /// The output size of the QR bill or QR code.
    /// </summary>
    /// <seealso href="https://github.com/manuelbl/SwissQRBill/wiki/Output-Sizes">Output Sizes (in Wiki)</seealso>
    public enum OutputSize
    {
        /// <summary>
        /// QR bill only (210 by 105 mm).
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
        /// QR bill only with additional space at the top for the horizontal line (about 210 by 110 mm).
        /// <para>
        /// The extra 5 mm at the top create space for the horizontal line and
        /// optionally for the scissors.
        /// </para>
        /// </summary>
        QrBillExtraSpace,
        /// <summary>
        /// QR code only with 5mm of white space on all sides (56 by 56 mm).
        /// <para>
        /// This format applies a white background (as opposed to a transparent one).
        /// </para>
        /// </summary>
        QrCodeWithQuietZone,
        /// <summary>
        /// Payment part only (about 148 by 105 mm).
        /// <para>
        /// This size does not include separator lines. It is suitable for displaying the QR bill in online channels.
        /// See <i>Implementation Guidelines QR Bill v2.3</i>, ch. <i>3.8 Layout rules for the online use of the QR-bill</i>
        /// for additional requirements when using this size.
        /// </para>
        /// </summary>
        PaymentPartOnly
    }
}
