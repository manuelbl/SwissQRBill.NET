//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System;
using System.Collections.Generic;

namespace Codecrete.SwissQRBill.Generator
{
    /// <summary>
    /// Formatting properties for QR bill.
    /// </summary>
    public sealed class BillFormat : IEquatable<BillFormat>
    {
        /// Default width for left and right margin, in mm. 
        public static readonly double DefaultMarginWidth = 5.0;

        /// <summary>
        /// Initializes a new instance with default values.
        /// </summary>
        public BillFormat() { }

        /// <summary>
        /// Initializes a new instance with the same values as the specified instance.
        /// </summary>
        /// <param name="format">The instance with values to copy.</param>
        public BillFormat(BillFormat format)
        {
            OutputSize = format.OutputSize;
            Language = format.Language;
            SeparatorType = format.SeparatorType;
            FontFamily = format.FontFamily;
            GraphicsFormat = format.GraphicsFormat;
            MarginLeft = format.MarginLeft;
            MarginRight = format.MarginRight;
        }

        /// <summary>
        /// Gets or sets the output size for the generated QR bill.
        /// <para>
        /// Defaults to <see cref="OutputSize.QrBillOnly"/>, i.e. the QR bill only (105 by 210 mm).
        /// </para>
        /// </summary>
        /// <value>The output size.</value>
        /// <seealso href="https://github.com/manuelbl/SwissQRBill/wiki/Output-Sizes">Output Sizes (in Wiki)</seealso>
        public OutputSize OutputSize { get; set; } = OutputSize.QrBillOnly;

        /// <summary>
        /// Gets or sets the bill language.
        /// <para>
        /// Defaults to <see cref="Language.EN"/> (English).
        /// </para>
        /// </summary>
        /// <value>The bill language.</value>
        public Language Language { get; set; } = Language.EN;

        /// <summary>
        /// Gets or sets the type of separator drawn above and between the payment part and the receipt.
        /// <para>
        /// Defaults to <see cref="SeparatorType.DashedLineWithScissors"/>.
        /// </para>
        /// </summary>
        /// <value>The separator type.</value>
        public SeparatorType SeparatorType { get; set; } = SeparatorType.DashedLineWithScissors;

        /// <summary>
        /// Gets or sets the font family to be used for text.
        /// <para>
        /// According to the implementation guidelines Arial, Frutiger, Helvetica and Liberation Sans
        /// are the only permitted fonts.
        /// </para>
        /// <para>
        /// Two styles of the font are used: normal/regular and bold.
        /// </para>
        /// <para>
        /// Defaults to <c>Helvetica,Arial,"Liberation Sans"</c>.
        /// </para>
        /// </summary>
        /// <value>The font family.</value>
        public string FontFamily { get; set; } = "Helvetica,Arial,\"Liberation Sans\"";


        /// <summary>
        /// Gets or sets the resolution for pixel image graphics formats.
        /// <para>
        /// It is recommended to use at least 144 dpi for a readable result.
        /// </para>
        /// <para>
        /// Defaults to 144 dpi.
        /// </para>
        /// </summary>
        /// <value>The graphics resolution, in dpi (pixels per inch).</value>
        public int Resolution { get; set; } = 144;

        /// <summary>
        /// Gets or sets the graphics format to be generated.
        /// <para>
        /// Defaults to <see cref="GraphicsFormat.SVG"/>.
        /// </para>
        /// </summary>
        public GraphicsFormat GraphicsFormat { get; set; } = GraphicsFormat.SVG;

        /// <summary>
        /// Gets or sets the left margin width (from edge of paper to start of text).
        /// <para>
        /// Valid values are between 5mm and 12mm. The default is 5mm.
        /// </para>
        /// <para>
        /// Values other than 5mm are not fully standard compliant as the fields in the receipt on the left-hand side
        /// become narrower. It is especially obvious if <i>Payable by</i> or <i>Amount</i> are not pre-filled
        /// so that the black corners are printed instead of text. Yet values higher than 5mm are more compatible
        /// with typical office and home printers, which are not capable of printing up to the edge of the paper and
        /// require a margin wider than 5mm.
        /// </para>
        /// </summary>
        /// <value>margin width, in mm</value>
        public double MarginLeft { get; set; } = DefaultMarginWidth;

        /// <summary>
        /// Gets the the right margin width (from the end of the text to the edge of the paper).
        /// <para>
        /// Valid values are between 5mm and 12mm. The default is 5mm.
        /// </para>
        /// <para>
        /// Values other than 5mm are not fully standard compliant but are more compatible with typical office and home
        /// printers, which are not capable of printing up to the edge of the paper and require a margin wider than 5mm.
        /// </para>
        /// </summary>
        /// <value>width width, in mm</value>
        public double MarginRight { get; set; } = DefaultMarginWidth;


        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as BillFormat);
        }

        /// <summary>Determines whether the specified bill format is equal to the current bill format.</summary>
        /// <param name="other">The bill format to compare with the current bill format.</param>
        /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
        public bool Equals(BillFormat other)
        {
            return other != null &&
                   OutputSize == other.OutputSize &&
                   Language == other.Language &&
                   SeparatorType == other.SeparatorType &&
                   FontFamily == other.FontFamily &&
                   GraphicsFormat == other.GraphicsFormat &&
                   Resolution == other.Resolution &&
                   MarginLeft == other.MarginLeft &&
                   MarginRight == other.MarginRight;
        }

        /// <summary>Gets the hash code for this instance.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            int hashCode = 43802783;
            hashCode = hashCode * -1521134295 + OutputSize.GetHashCode();
            hashCode = hashCode * -1521134295 + Language.GetHashCode();
            hashCode = hashCode * -1521134295 + SeparatorType.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FontFamily);
            hashCode = hashCode * -1521134295 + GraphicsFormat.GetHashCode();
            hashCode = hashCode * -1521134295 + Resolution.GetHashCode();
            hashCode = hashCode * -1521134295 + MarginLeft.GetHashCode();
            hashCode = hashCode * -1521134295 + MarginRight.GetHashCode();
            return hashCode;
        }
    }
}
