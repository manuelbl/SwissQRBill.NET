using System;
using System.Collections.Generic;

namespace Codecrete.SwissQRBill.Generator
{
    /// <summary>
    /// Formatting properties for QR bill
    /// </summary>
    public sealed class BillFormat : IEquatable<BillFormat>
    {
        /// <summary>
        /// Initializes a new instance with default values.
        /// </summary>
        public BillFormat() { }

        /// <summary>
        /// Initializes a new instance with the same values as the specified instance.
        /// </summary>
        /// <param name="format">instance with values to copy</param>
        public BillFormat(BillFormat format)
        {
            OutputSize = format.OutputSize;
            Language = format.Language;
            SeparatorType = format.SeparatorType;
            FontFamily = format.FontFamily;
            GraphicsFormat = format.GraphicsFormat;
        }

        /// <summary>
        /// Gets or sets the output size for the generated QR bill.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="OutputSize.QRBillOnly"/>, i.e. the QR bill only (105 by 210 mm)
        /// </remarks>
        public OutputSize OutputSize { get; set; } = OutputSize.QRBillOnly;

        /// <summary>
        /// Gets or sets the bill language.
        /// </summary>
        /// <remarks>
        /// Defaults to EN (English).
        /// </remarks>
        public Language Language { get; set; } = Language.EN;

        /// <summary>
        /// Gets or sets the type of separator drawn above and between the payment part and the receipt.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="SeparatorType.SolidLineWithScissors"/>.
        /// </remarks>
        public SeparatorType SeparatorType { get; set; } = SeparatorType.SolidLineWithScissors;

        /// <summary>
        /// Gets or sets the font family to be used for text.
        /// </summary>
        /// <remarks>
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
        /// </remarks>
        public string FontFamily { get; set; } = "Helvetica,Arial,\"Liberation Sans\"";

        public GraphicsFormat GraphicsFormat { get; set; } = GraphicsFormat.SVG;

        public override bool Equals(object obj)
        {
            return Equals(obj as BillFormat);
        }

        public bool Equals(BillFormat other)
        {
            return other != null &&
                   OutputSize == other.OutputSize &&
                   Language == other.Language &&
                   SeparatorType == other.SeparatorType &&
                   FontFamily == other.FontFamily &&
                   GraphicsFormat == other.GraphicsFormat;
        }

        public override int GetHashCode()
        {
            int hashCode = 43802783;
            hashCode = hashCode * -1521134295 + OutputSize.GetHashCode();
            hashCode = hashCode * -1521134295 + Language.GetHashCode();
            hashCode = hashCode * -1521134295 + SeparatorType.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FontFamily);
            hashCode = hashCode * -1521134295 + GraphicsFormat.GetHashCode();
            return hashCode;
        }
    }
}
