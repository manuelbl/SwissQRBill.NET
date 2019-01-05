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
    /// </summary>
    /// <remarks>
    /// Can also validate the bill data and encode and decode the text embedded in the QR code.
    /// </remarks>
    public static class QRBill
    {
        /// <summary>
        /// Validation message key: currency must be "CHF" or "EUR"
        /// </summary>
        public static readonly string KeyCurrencyIsCHFOrEUR = "currency_is_chf_or_eur";
        /// <summary>
        /// Validation message key: amount must be between 0.01 and 999999999.99
        /// </summary>
        public static readonly string KeyAmountIsInValidRange = "amount_in_valid_range";
        /// <summary>
        /// Validation message key: IBAN must be from bank in Switzerland or
        /// Liechtenstein
        /// </summary>
        public static readonly string KeyAccountIsCH_LI_IBAN = "account_is_ch_li_iban";
        /// <summary>
        /// Validation message key: IBAN must have valid format and check digit
        /// </summary>
        public static readonly string KeyAccountIsValidIBAN = "account_is_valid_iban";
        /// <summary>
        /// Validation message key: Due to regular IBAN (outside QR-IID range) an ISO 11649 references is expected
        /// but it has invalid format or check digit
        /// </summary>
        public static readonly string KeyValidISO11649CreditorRef = "valid_iso11649_creditor_ref";
        /// <summary>
        /// Validation message key: Due to QR-IBAN (IBAN in QR-IID range) a QR reference number is expected
        /// but it has invalid format or check digit
        /// </summary>
        public static readonly string KeyValidQRRefNo = "valid_qr_ref_no";
        /// <summary>
        /// Validation message key: For QR-IBANs (IBAN in QR-IID range) a QR reference is mandatory
        /// </summary>
        public static readonly string KeyMandatoryForQRIBAN = "mandatory_for_qr_iban";
        /// <summary>
        /// Validation message key: Field is mandatory
        /// </summary>
        public static readonly string KeyFieldIsMandatory = "field_is_mandatory";
        /// <summary>
        /// Validation message key: Conflicting fields for both structured and combined elements address type have been used
        /// </summary>
        public static readonly string KeyAddressTypeConflict = "adress_type_conflict";
        /// <summary>
        /// Validation message key: Country code must consist of two letters
        /// </summary>
        public static readonly string KeyValidCountryCode = "valid_country_code";
        /// <summary>
        /// Validation message key: Field has been clipped to not exceed the maximum
        /// length
        /// </summary>
        public static readonly string KeyFieldClipped = "field_clipped";
        /// <summary>
        /// Validation message key: Field value exceed the maximum length
        /// </summary>
        public static readonly string KeyFieldTooLong = "field_value_too_long";
        /// <summary>
        /// Validation message key: Unsupported characters have been replaced
        /// </summary>
        public static readonly string KeyReplacedUnsupportedCharacters = "replaced_unsupported_characters";
        /// <summary>
        /// Validation message key: Valid data structure starts with "SPC" and consists
        /// of 32 to 34 lines of text
        /// </summary>
        public static readonly string KeyValidDataStructure = "valid_data_structure";
        /// <summary>
        /// Validation message key: Version 02.00 is supported only
        /// </summary>
        public static readonly string KeySupportedVersion = "supported_version";
        /// <summary>
        /// Validation message key: Coding type 1 is supported only
        /// </summary>
        public static readonly string KeySupportedCodingType = "supported_coding_type";
        /// <summary>
        /// Validation message key: Valid number required (nnnnn.nn)
        /// </summary>
        public static readonly string KeyValidNumber = "valid_number";
        /// <summary>
        /// Validation message key: The maximum of 2 alternative schemes has been exceeded
        /// </summary>
        public static readonly string KeyAltSchemeExceeded = "alt_scheme_max_exceed";
        /// <summary>
        /// Validation message key: The bill information is invalid (does not start with // or is too short)
        /// </summary>
        public static readonly string KeyBillInfoInvalid = "bill_info_invalid";


        /// <summary>
        /// Validates and cleans the bill data.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The validation result contains the error and warning messages (if any) and the cleaned bill data.
        /// </para>
        /// <para>
        /// For details about the validation result, see <a href=
        /// "https://github.com/manuelbl/SwissQRBill/wiki/Bill-data-validation">Bill data validation</a>
        /// </para>
        /// </remarks>
        /// <param name="bill">bill data</param>
        /// <returns>validation result</returns>
        public static ValidationResult Validate(Bill bill)
        {
            return Validator.Validate(bill);
        }

        /// <summary>
        /// Generates a QR bill payment part.
        /// </summary>
        /// <remarks>
        /// If the bill data does not validate, a <see cref="QRBillValidationException"/> is
        /// thrown, which contains the validation result. For details about the
        /// validation result, see <a href=
        /// "https://github.com/manuelbl/SwissQRBill/wiki/Bill-data-validation">Bill data
        /// validation</a>
        /// </remarks>
        /// <param name="bill">the bill data</param>
        /// <returns>the generated QR bill (as a byte array)</returns>
        /// <exception cref="QRBillValidationException">Thrown if the bill data does not validate</exception>
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
        /// Generates a QR bill payment part using the specified canvas.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the bill data does not validate, a <see cref="QRBillValidationException"/> is
        /// thrown, which contains the validation result. For details about the
        /// validation result, see <a href=
        /// "https://github.com/manuelbl/SwissQRBill/wiki/Bill-data-validation">Bill data
        /// validation</a>
        /// </para>
        /// <para>
        /// The canvas will be initialized with <see cref="ICanvas.SetupPage(double, double, string)"/> and it will be
        /// closed before returning the generated QR bill
        /// </para>
        /// </remarks>
        /// <param name="bill">the bill data</param>
        /// <param name="canvas">the canvas to draw to</param>
        /// <returns>the generated QR bill (as a byte array)</returns>
        /// <exception cref="QRBillValidationException">Thrown if the bill data does not validate</exception>
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

            if (bill.Format.OutputSize == OutputSize.QRCodeOnly)
            {
                return GenerateQRCode(cleanedBill, canvas);
            }
            else
            {
                return GeneratePaymentPart(cleanedBill, canvas);
            }
        }

        /// <summary>
        /// Encodes the text embedded in the QR code from the specified bill data.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The specified bill data is first validated and cleaned.
        /// </para>
        /// <para>
        /// If the bill data does not validate, a <see cref="QRBillValidationException"/> is
        /// thrown, which contains the validation result.For details about the
        /// validation result, see
        /// <a href="https://github.com/manuelbl/SwissQRBill/wiki/Bill-data-validation">Bill data
        /// validation</a>
        /// </para>
        /// </remarks>
        /// <param name="bill">bill data to encode</param>
        /// <returns>QR code text</returns>
        /// <exception cref="QRBillValidationException">Thrown if the bill data does not validate</exception>
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
        /// Decodes the text embedded in the QR code and fills it into a <see cref="Bill"/> data structure
        /// </summary>
        /// <remarks>
        /// A subset of the validations related to embedded QR code text is run.It the
        /// validation fails, a <see cref="QRBillValidationException"/> is thrown, which contains
        /// the validation result.See the error messages marked with a dagger in
        /// <a href="https://github.com/manuelbl/SwissQRBill/wiki/Bill-data-validation">Bill data
        /// validation</a>.
        /// </remarks>
        /// <param name="text">text to decode</param>
        /// <returns>decoded bill data</returns>
        /// <exception cref="QRBillValidationException">Thrown if the bill data does not validate</exception>
        public static Bill DecodeQrCodeText(string text)
        {
            return QRCodeText.Decode(text);
        }

        /// <summary>
        /// Generates the payment part as a byte array
        /// </summary>
        /// <param name="bill">the cleaned bill data</param>
        /// <param name="canvas">the canvas to draw to</param>
        /// <returns>byte array containing the binary data in the selected format</returns>
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
        /// Generate the QR code only
        /// </summary>
        /// <param name="bill">the bill data</param>
        /// <param name="canvas">the canvas to draw to</param>
        /// <returns>byte array containing the binary data in the selected format</returns>
        ////
        private static byte[] GenerateQRCode(Bill bill, ICanvas canvas)
        {

            canvas.SetupPage(QRCode.SIZE, QRCode.SIZE, bill.Format.FontFamily);
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
