//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator.Canvas;
using System;

namespace Codecrete.SwissQRBill.Generator
{
    /// <summary>
    /// Generates Swiss QR bill payment part.
    /// <para>
    /// Can also validate the bill data and encode and decode the text embedded in the QR code.
    /// </para>
    /// </summary>
    public static class QRBill
    {
        /// <summary>
        /// The width of an A4 sheet in portrait orientation.
        /// </summary>
        /// <value>The width, in mm.</value>
        public const double A4PortraitWidth = 210;

        /// <summary>
        /// The height of an A4 sheet in portrait orientation.
        /// </summary>
        /// <value>The height, in mm.</value>
        public const double A4PortraitHeight = 297;

        /// <summary>
        /// The width of a QR bill (payment part and receipt).
        /// </summary>
        /// <value>The width, in mm.</value>
        public const double QrBillWidth = 210;

        /// <summary>
        /// The height of a QR bill (payment part and receipt).
        /// </summary>
        /// <value>The height, in mm.</value>
        public const double QrBillHeight = 105;

        /// <summary>
        /// The width of a QR bill with horizontal separator line (payment part and receipt plus space for line and scissors).
        /// </summary>
        /// <value>The width, in mm.</value>
        public const double QrBillWithHoriLineWidth = 210;

        /// <summary>
        /// The height of a QR bill with horizontal separator line (payment part and receipt plus space for line and scissors).
        /// </summary>
        /// <value>The height, in mm.</value>
        public const double QrBillWithHoriLineHeight = 110;

        /// <summary>
        /// The width of the QR code.
        /// </summary>
        /// <value>The width, in mm.</value>
        public const double QrCodeWidth = 46;

        /// <summary>
        /// The height of the QR code.
        /// </summary>
        /// <value>The height, in mm.</value>
        public const double QrCodeHeight = 46;


        /// <summary>
        /// Validates and cleans the bill data.
        /// <para>
        /// The validation result contains the error and warning messages (if any) and the cleaned bill data.
        /// </para>
        /// <para>
        /// For details about the validation result, see <a href=
        /// "https://github.com/manuelbl/SwissQRBill/wiki/Bill-data-validation">Bill data validation</a>
        /// </para>
        /// </summary>
        /// <param name="bill">The bill data to validate.</param>
        /// <returns>The validation result.</returns>
        public static ValidationResult Validate(Bill bill)
        {
            return Validator.Validate(bill);
        }

        /// <summary>
        /// Generates a QR bill (payment part and receipt) or QR code as an SVG image or PDF document.
        /// <para>
        /// If the bill is not valid, a <see cref="QRBillValidationException"/> is
        /// thrown, containing the validation result. For details about the
        /// validation result, see <a href=
        /// "https://github.com/manuelbl/SwissQRBill/wiki/Bill-data-validation">Bill data
        /// validation</a>
        /// </para>
        /// <para>
        /// The graphics format is specified in <c>bill.Format.GraphisFormat</c>. This method
        /// only supports the generation of SVG images and PDF files. For other graphics
        /// formats (in particular PNG), use <see cref="Draw(Bill, ICanvas)"/>.
        /// </para>
        /// </summary>
        /// <param name="bill">The data for the bill.</param>
        /// <returns>The generated QR bill (as a byte array in the specified graphics format).</returns>
        /// <exception cref="QRBillValidationException">The bill data is invalid.</exception>
        /// <seealso cref="Draw(Bill, ICanvas)"/>
        public static byte[] Generate(Bill bill)
        {
            using (ICanvas canvas = CreateCanvas(bill.Format))
            {
                try
                {
                    ValidateAndGenerate(bill, canvas);
                    return (canvas as IToByteArray)?.ToByteArray();
                }
                catch (QRBillValidationException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw new QRBillGenerationException("Failed to generate QR bill", e);
                }
            }
        }

        /// <summary>
        /// Draws the QR bill (payment part and receipt) or QR code for the specified bill data onto the specified canvas.
        /// <para>
        /// The QR bill or code are drawn at position (0, 0) extending to the top and to the right.
        /// Typcially, the position (0, 0) is the bottom left corner of the canvas.
        /// </para>
        /// <para>
        /// This methods ignores the formatting properties <c>bill.Format.FontFamily</c> and <c>bill.Format.GraphicsFormat</c>.
        /// They can be set when the canvas instance passed to this method is created.
        /// </para>
        /// <para>
        /// If the bill data is not valid, a <see cref="QRBillValidationException"/> is thrown,
        /// containing the validation result. For details about the validation result, see
        /// <a href="https://github.com/manuelbl/SwissQRBill/wiki/Bill-data-validation">Bill data validation</a>.
        /// </para>
        /// </summary>
        /// <param name="bill">The data for the bill.</param>
        /// <param name="canvas">The canvas to draw to.</param>
        /// <exception cref="QRBillValidationException">The bill data is invalid.</exception>
        public static void Draw(Bill bill, ICanvas canvas)
        {
            try
            {
                ValidateAndGenerate(bill, canvas);
            }
            catch (QRBillValidationException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new QRBillGenerationException("Failed to generate QR bill", e);
            }
        }

        /// <summary>
        /// Draws the separator line(s) to the specified canvas.
        /// <para>
        /// The separator lines are drawn assuming that the QR bill starts at position (0, 0)
        /// and extends the top and right.So position (0, 0) should be in the bottom left corner.
        /// </para>
        /// <para>
        /// This method allows to add separator lines to an existing QR bill,
        /// e.g. on to an archived QR bill document.
        /// </para>
        /// </summary>
        /// <param name="separatorType">The separator type.</param>
        /// <param name="withHorizontalLine"><code>true</code> if both the horizontal and vertical line should be drawn,
        /// <code>false</code> for the vertical line only</param>
        /// <param name="canvas">The canvas to draw to.</param>
        public static void DrawSeparators(SeparatorType separatorType, bool withHorizontalLine, ICanvas canvas)
        {
            Bill bill = new Bill
            {
                Format = new BillFormat
                {
                    SeparatorType = separatorType,
                    OutputSize = withHorizontalLine ? OutputSize.QrBillWithHorizontalLine : OutputSize.QrBillOnly
                }
            };

            BillLayout layout = new BillLayout(bill, canvas);

            try
            {
                layout.DrawBorder();
            }
            catch (Exception e)
            {
                throw new QRBillGenerationException("Failed drawing separators", e);
            }
        }

        private static void ValidateAndGenerate(Bill bill, ICanvas canvas)
        {
            ValidationResult result = Validator.Validate(bill);
            Bill cleanedBill = result.CleanedBill;
            if (result.HasErrors)
            {
                throw new QRBillValidationException(result);
            }

            if (bill.Format.OutputSize == OutputSize.QrCodeOnly)
            {
                QRCode qrCode = new QRCode(cleanedBill);
                qrCode.Draw(canvas, 0, 0);
            }
            else
            {
                BillLayout layout = new BillLayout(cleanedBill, canvas);
                layout.Draw();
            }
        }

        /// <summary>
        /// Encodes the QR bill data as a text to be embedded in a QR code.
        /// <para>
        /// The specified bill data is first validated and cleaned.
        /// </para>
        /// <para>
        /// If the bill data is invalid, a <see cref="QRBillValidationException"/> is
        /// thrown, containing the validation result. For details about the
        /// validation result, see
        /// <a href="https://github.com/manuelbl/SwissQRBill/wiki/Bill-data-validation">Bill data
        /// validation</a>
        /// </para>
        /// </summary>
        /// <param name="bill">The bill data to encode.</param>
        /// <returns>The text to embed in a QR code.</returns>
        /// <exception cref="QRBillValidationException">The bill data is invalid.</exception>
        public static string EncodeQrCodeText(Bill bill)
        {
            ValidationResult result = Validator.Validate(bill);
            Bill cleanedBill = result.CleanedBill;
            if (result.HasErrors)
            {
                throw new QRBillValidationException(result);
            }

            return QRCodeText.Create(cleanedBill);
        }

        /// <summary>
        /// Decodes the text from a QR code and fills it into a <see cref="Bill"/> data structure
        /// <para>
        /// A subset of the validations related to embedded QR code text is run. It the
        /// validation fails, a <see cref="QRBillValidationException"/> is thrown containing
        /// the validation result. See the error messages marked with a dagger in
        /// <a href="https://github.com/manuelbl/SwissQRBill/wiki/Bill-data-validation">Bill data
        /// validation</a>.
        /// </para>
        /// </summary>
        /// <param name="text">The text to decode.</param>
        /// <returns>The decoded bill data.</returns>
        /// <exception cref="QRBillValidationException">If he text could not be decoded or the decoded bill data is invalid.</exception>
        public static Bill DecodeQrCodeText(string text)
        {
            return QRCodeText.Decode(text);
        }

        private static ICanvas CreateCanvas(BillFormat format)
        {
            double drawingWidth;
            double drawingHeight;

            // define page size
            switch (format.OutputSize)
            {
                case OutputSize.QrBillOnly:
                    drawingWidth = QrBillWidth;
                    drawingHeight = QrBillHeight;
                    break;
                case OutputSize.QrBillWithHorizontalLine:
                    drawingWidth = QrBillWithHoriLineWidth;
                    drawingHeight = QrBillWithHoriLineHeight;
                    break;
                case OutputSize.QrCodeOnly:
                    drawingWidth = QrCodeWidth;
                    drawingHeight = QrCodeHeight;
                    break;
                default:
                    drawingWidth = A4PortraitWidth;
                    drawingHeight = A4PortraitHeight;
                    break;
            }

            ICanvas canvas;
            switch (format.GraphicsFormat)
            {
                case GraphicsFormat.SVG:
                    canvas = new SVGCanvas(drawingWidth, drawingHeight, format.FontFamily);
                    break;
                case GraphicsFormat.PNG:
                    canvas = new PNGCanvas(drawingWidth, drawingHeight, format.PngResolution, format.FontFamily);
                    break;
                case GraphicsFormat.PDF:
                    canvas = new PDFCanvas(drawingWidth, drawingHeight);
                    break;
                default:
                    throw new QRBillGenerationException("Invalid graphics format specified");
            }
            return canvas;
        }
    }
}
