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
        /// Generates a QR bill (payment part and receipt).
        /// <para>
        /// If the bill is not valid, a <see cref="QRBillValidationException"/> is
        /// thrown, containing the validation result. For details about the
        /// validation result, see <a href=
        /// "https://github.com/manuelbl/SwissQRBill/wiki/Bill-data-validation">Bill data
        /// validation</a>
        /// </para>
        /// <para>
        /// This method only supports the generation of SVG and PDF bills. For other graphics
        /// formats (in particular PNG), use <see cref="Generate(Bill, ICanvas)"/>.
        /// </para>
        /// </summary>
        /// <param name="bill">The data for the bill.</param>
        /// <returns>The generated QR bill (as a byte array in the specified graphics format).</returns>
        /// <exception cref="QRBillValidationException">The bill data is invalid.</exception>
        /// <seealso cref="Generate(Bill, ICanvas)"/>
        public static byte[] Generate(Bill bill)
        {
            using (ICanvas canvas = CreateCanvas(bill.Format.GraphicsFormat))
            {
                try
                {
                    return ValidateAndGenerate(bill, canvas);
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
        /// Generates a QR bill (payment part and receipt) using the specified canvas.
        /// <para>
        /// If the bill data is not valid, a <see cref="QRBillValidationException"/> is
        /// thrown, containing the validation result. For details about the
        /// validation result, see <a href=
        /// "https://github.com/manuelbl/SwissQRBill/wiki/Bill-data-validation">Bill data
        /// validation</a>
        /// </para>
        /// <para>
        /// The bill is drawn on the specified canvas. It will be initialized with
        /// <see cref="ICanvas.SetupPage(double, double, string)"/> and it will be
        /// closed before returning the result.
        /// </para>
        /// </summary>
        /// <param name="bill">The data for the bill.</param>
        /// <param name="canvas">The canvas to draw to.</param>
        /// <returns>The generated QR bill (as a byte array).</returns>
        /// <exception cref="QRBillValidationException">The bill data is invalid.</exception>
        public static byte[] Generate(Bill bill, ICanvas canvas)
        {
            using (ICanvas c = canvas)
            {
                try
                {
                    return ValidateAndGenerate(bill, c);
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

        private static byte[] ValidateAndGenerate(Bill bill, ICanvas canvas)
        {
            ValidationResult result = Validator.Validate(bill);
            Bill cleanedBill = result.CleanedBill;
            if (result.HasErrors)
            {
                throw new QRBillValidationException(result);
            }

            if (bill.Format.OutputSize == OutputSize.QrCodeOnly)
            {
                return GenerateQrCode(cleanedBill, canvas);
            }
            else
            {
                return GeneratePaymentPart(cleanedBill, canvas);
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

        /// <summary>
        /// Generates the bill (payment part and receipt) as a byte array.
        /// </summary>
        /// <param name="bill">The cleaned bill data.</param>
        /// <param name="canvas">The canvas to draw to.</param>
        /// <returns>The byte array containing the binary data in the specified graphics format.</returns>
        private static byte[] GeneratePaymentPart(Bill bill, ICanvas canvas)
        {

            double drawingWidth;
            double drawingHeight;

            // define page size
            switch (bill.Format.OutputSize)
            {
                case OutputSize.A4PortraitSheet:
                    drawingWidth = 210;
                    drawingHeight = 297;
                    break;
                default:
                    drawingWidth = 210;
                    drawingHeight = 105;
                    break;
            }

            canvas.SetupPage(drawingWidth, drawingHeight, bill.Format.FontFamily);
            BillLayout layout = new BillLayout(bill, canvas);
            layout.Draw();
            return canvas.GetResult();
        }

        /// <summary>
        /// Generates the QR code for the specified clean bill data.
        /// </summary>
        /// <param name="bill">The bill data.</param>
        /// <param name="canvas">The canvas to draw to.</param>
        /// <returns>The byte array containing the binary data in the specified graphics format.</returns>
        ////
        private static byte[] GenerateQrCode(Bill bill, ICanvas canvas)
        {

            canvas.SetupPage(QRCode.Size, QRCode.Size, bill.Format.FontFamily);
            QRCode qrCode = new QRCode(bill);
            qrCode.Draw(canvas, 0, 0);
            return canvas.GetResult();
        }

        private static ICanvas CreateCanvas(GraphicsFormat graphicsFormat)
        {
            ICanvas canvas;
            switch (graphicsFormat)
            {
                case GraphicsFormat.SVG:
                    canvas = new SVGCanvas();
                    break;
                case GraphicsFormat.PDF:
                    canvas = new PDFCanvas();
                    break;
                default:
                    throw new QRBillGenerationException("Invalid graphics format specified");
            }
            return canvas;
        }
    }
}
