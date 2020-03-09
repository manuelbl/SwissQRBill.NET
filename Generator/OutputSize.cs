//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

namespace Codecrete.SwissQRBill.Generator
{
    /// <summary>
    /// The output size of the QR bill.
    /// </summary>
    public enum OutputSize
    {
        /// <summary>
        /// QR bill only (105 by 210 mm).
        /// </summary>
        QrBillOnly,
        /// <summary>
        /// QR bill with horizontal separator line (110 by 210 mm).
        /// </summary>
        QrBillWithHorizontalLine,
        /// <summary>
        /// A4 sheet in portrait orientation. The QR bill is at the bottom.
        /// </summary>
        A4PortraitSheet,
        /// <summary>
        /// QR code only (46 by 46 mm).
        /// </summary>
        QrCodeOnly
    }
}
