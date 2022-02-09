//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//


namespace libQrCodeGenerator.SwissQRBill.Generator
{
    /// <summary>
    /// Graphics format of generated QR bill.
    /// </summary>
    public enum GraphicsFormat
    {
        /// <summary>
        /// SVG graphics format.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        SVG,
        /// <summary>
        /// PNG graphics format.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        PNG,
        /// <summary>
        /// PDF graphics format.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        PDF
    }
}
