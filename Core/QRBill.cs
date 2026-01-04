//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator.Canvas;
using System;
using System.Diagnostics;
using System.Reflection;

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
        /// <seealso cref="OutputSize.A4PortraitSheet"/>
        /// </summary>
        /// <value>The width, in mm.</value>
        public const double A4PortraitWidth = 210;

        /// <summary>
        /// The height of an A4 sheet in portrait orientation.
        /// <seealso cref="OutputSize.A4PortraitSheet"/>
        /// </summary>
        /// <value>The height, in mm.</value>
        public const double A4PortraitHeight = 297;

        /// <summary>
        /// The width of a QR bill (payment part and receipt).
        /// <seealso cref="OutputSize.QrBillOnly"/>
        /// </summary>
        /// <value>The width, in mm.</value>
        public const double QrBillWidth = 210;

        /// <summary>
        /// The height of a QR bill (payment part and receipt).
        /// <seealso cref="OutputSize.QrBillOnly"/>
        /// </summary>
        /// <value>The height, in mm.</value>
        public const double QrBillHeight = 105;

        /// <summary>
        /// The width of the output format with extra space for horizontal separator line (payment part and receipt plus space for line and scissors).
        /// <seealso cref="OutputSize.QrBillExtraSpace"/>
        /// </summary>
        /// <value>The width, in mm.</value>
        public const double QrBillWithHoriLineWidth = 210;

        /// <summary>
        /// The height of a the output format with extra space for horizontal separator line (payment part and receipt plus space for line and scissors).
        /// <seealso cref="OutputSize.QrBillExtraSpace"/>
        /// </summary>
        /// <value>The height, in mm.</value>
        public const double QrBillWithHoriLineHeight = 110;

        /// <summary>
        /// The width of the QR code.
        /// <seealso cref="OutputSize.QrCodeOnly"/>
        /// </summary>
        /// <value>The width, in mm.</value>
        public const double QrCodeWidth = 46;

        /// <summary>
        /// The height of the QR code.
        /// <seealso cref="OutputSize.QrCodeOnly"/>
        /// </summary>
        /// <value>The height, in mm.</value>
        public const double QrCodeHeight = 46;

        /// <summary>
        /// The width of the QR code with quiet zone, in mm.
        /// <seealso cref="OutputSize.QrCodeWithQuietZone"/>
        /// </summary>
        /// <value>The width, in mm.</value>
        public const double QrCodeWithQuietZoneWidth = 56;

        /// <summary>
        /// The height of the QR code with quiet zone, in mm.
        /// <seealso cref="OutputSize.QrCodeWithQuietZone"/>
        /// </summary>
        /// <value>The height, in mm.</value>
        public const double QrCodeWithQuietZoneHeight = 56;

        /// <summary>
        /// The width of the payment part, in mm.
        /// <seealso cref="OutputSize.PaymentPartOnly"/>
        /// </summary>
        /// <value>The width, in mm.</value>
        public const double PaymentPartWidth = 148;

        /// <summary>
        /// The height of the payment part, in mm.
        /// <seealso cref="OutputSize.PaymentPartOnly"/>
        /// </summary>
        /// <value>The height, in mm.</value>
        public const double PaymentPartHeight = 105;


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
        /// The graphics format is specified in <c>bill.Format.GraphicsFormat</c>.
        /// </para>
        /// </summary>
        /// <param name="bill">The data for the bill.</param>
        /// <returns>The generated QR bill (as a byte array in the specified graphics format).</returns>
        /// <exception cref="QRBillValidationException">The bill data is invalid.</exception>
        /// <seealso cref="Draw(Bill, ICanvas)"/>
        public static byte[] Generate(Bill bill)
        {
            using (var canvas = CreateCanvas(bill))
            {
                try
                {
                    ValidateAndGenerate(bill, canvas);
                    return canvas.ToByteArray();
                }
                catch (QRBillValidationException)
                {
                    throw;
                }
                catch (QRBillGenerationException)
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
        /// Typically, the position (0, 0) is the bottom left corner of the canvas.
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
            catch (QRBillGenerationException)
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
            var bill = new Bill
            {
                Format = new BillFormat
                {
                    SeparatorType = separatorType,
                    OutputSize = withHorizontalLine ? OutputSize.QrBillExtraSpace : OutputSize.QrBillOnly
                }
            };

            var layout = new BillLayout(bill, canvas);

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
            var result = Validator.Validate(bill);
            var cleanedBill = result.CleanedBill;
            if (result.HasErrors)
            {
                throw new QRBillValidationException(result);
            }

            if (bill.Format.OutputSize == OutputSize.QrCodeOnly)
            {
                var qrCode = new QRCode(cleanedBill);
                qrCode.Draw(canvas, 0, 0);
            }
            else if (bill.Format.OutputSize == OutputSize.QrCodeWithQuietZone)
            {
                var qrCode = new QRCode(cleanedBill);
                canvas.StartPath();
                canvas.AddRectangle(0, 0, QrCodeWithQuietZoneWidth, QrCodeWithQuietZoneHeight);
                canvas.FillPath(0xffffff, false);
                qrCode.Draw(canvas, 5, 5);
            }
            else
            {
                var layout = new BillLayout(cleanedBill, canvas);
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
            var result = Validator.Validate(bill);
            var cleanedBill = result.CleanedBill;
            if (result.HasErrors)
            {
                throw new QRBillValidationException(result);
            }

            return QRCodeText.Create(cleanedBill);
        }

        /// <summary>
        /// Decodes the text from a QR code and fills it into a <see cref="Bill"/> data structure
        /// <para>
        /// A subset of the validations related to embedded QR code text is run. If the
        /// validation fails, a <see cref="QRBillValidationException"/> is thrown containing
        /// the validation result. See the error messages marked with a dagger in
        /// <a href="https://github.com/manuelbl/SwissQRBill/wiki/Bill-data-validation">Bill data
        /// validation</a>.
        /// </para>
        /// </summary>
        /// <param name="text">The text to decode.</param>
        /// <returns>The decoded bill data.</returns>
        /// <exception cref="QRBillValidationException">If the text could not be decoded or the decoded bill data is invalid.</exception>
        public static Bill DecodeQrCodeText(string text)
        {
            return QRCodeText.Decode(text, false);
        }

        /// <summary>
        /// Decodes the text from a QR code and fills it into a <see cref="Bill"/> data structure
        /// <para>
        /// A subset of the validations related to embedded QR code text is run. If the
        /// validation fails, a <see cref="QRBillValidationException"/> is thrown containing
        /// the validation result. See the error messages marked with a dagger in
        /// <a href="https://github.com/manuelbl/SwissQRBill/wiki/Bill-data-validation">Bill data
        /// validation</a>.
        /// </para>
        /// </summary>
        /// <param name="text">The text to decode.</param>
        /// <param name="allowInvalidAmount">If <c>true</c>, invalid values for the amount are also accepted (currently "NULL" / "null"). In this case, the amount is set to <c>null</c>.</param>
        /// <returns>The decoded bill data.</returns>
        /// <exception cref="QRBillValidationException">If the text could not be decoded or the decoded bill data is invalid.</exception>
        public static Bill DecodeQrCodeText(string text, bool allowInvalidAmount)
        {
            return QRCodeText.Decode(text, allowInvalidAmount);
        }

        private static ICanvas CreateCanvas(Bill bill)
        {
            double drawingWidth;
            double drawingHeight;
            var format = bill.Format;

            // define page size
            switch (format.OutputSize)
            {
                case OutputSize.QrBillOnly:
                    drawingWidth = QrBillWidth;
                    drawingHeight = QrBillHeight;
                    break;
                case OutputSize.QrBillExtraSpace:
                    drawingWidth = QrBillWithHoriLineWidth;
                    drawingHeight = QrBillWithHoriLineHeight;
                    break;
                case OutputSize.PaymentPartOnly:
                    drawingWidth = PaymentPartWidth;
                    drawingHeight = PaymentPartHeight;
                    break;
                case OutputSize.QrCodeOnly:
                    drawingWidth = QrCodeWidth;
                    drawingHeight = QrCodeHeight;
                    break;
                case OutputSize.QrCodeWithQuietZone:
                    drawingWidth = QrCodeWithQuietZoneWidth;
                    drawingHeight = QrCodeWithQuietZoneHeight;
                    break;
                default:
                    drawingWidth = A4PortraitWidth;
                    drawingHeight = A4PortraitHeight;
                    break;
            }

            var canvas = CanvasCreator.Create(format, bill.CharacterSet, drawingWidth, drawingHeight);
            if (canvas != null)
            {
                return canvas;
            }
            
            if (format.GraphicsFormat == GraphicsFormat.PNG)
            {
                // The PNG canvas factory is provided by a separate assembly / NuGet package.
                // Try to load the factory dynamically, and if it still fails, print a message with specific hint.
                CanvasCreator.RegisterPixelCanvasFactory();
                canvas = CanvasCreator.Create(format, drawingWidth, drawingHeight);
                if (canvas == null)
                    throw new QRBillGenerationException("Graphics format PNG not available (are you missing the NuGet package Codecrete.SwissQRBill.Generator?)");
            }
            else
            {
                throw new QRBillGenerationException("Invalid graphics format specified");
            }

            return canvas;
        }

        /// <summary>
        /// Gets the library's version number.
        /// </summary>
        /// <value>version number in semantic versioning format (major.minor.patch)</value>
        public static string LibraryVersion => FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
    }
}
