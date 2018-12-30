//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//


namespace Codecrete.SwissQRBill.Generator
{
    public class QRBill
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
        /// Encodes the text embedded in the QR code from the specified bill data.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The specified bill data is first validated and cleaned.
        /// </para>
        /// <para>
        /// If the bill data does not validate, a {@link QRBillValidationError} is
        /// thrown, which contains the validation result.For details about the
        /// validation result, see
        /// <a href="https://github.com/manuelbl/SwissQRBill/wiki/Bill-data-validation">Bill data
        /// validation</a>
        /// </para>
        /// </remarks>
        /// <param name="bill">bill data to encode</param>
        /// <returns>QR code text</returns>
        /// <exception cref="QRBillValidationError">Thrown if the bill data does not validate</exception>
        public static string EncodeQrCodeText(Bill bill)
        {
            ValidationResult result = Validator.Validate(bill);
            Bill cleanedBill = result.CleanedBill;
            if (result.HasErrors)
            {
                throw new QRBillValidationError(result);
            }

            return QRCodeText.Create(cleanedBill);
        }

        /// <summary>
        /// Decodes the text embedded in the QR code and fills it into a <see cref="Bill"/> data structure
        /// </summary>
        /// <remarks>
        /// A subset of the validations related to embedded QR code text is run.It the
        /// validation fails, a {@link QRBillValidationError} is thrown, which contains
        /// the validation result.See the error messages marked with a dagger in
        /// <a href="https://github.com/manuelbl/SwissQRBill/wiki/Bill-data-validation">Bill data
        /// validation</a>.
        /// </remarks>
        /// <param name="text">text to decode</param>
        /// <returns>decoded bill data</returns>
        /// <exception cref="QRBillValidationError">Thrown if the bill data does not validate</exception>
        public static Bill DecodeQrCodeText(string text)
        {
            return QRCodeText.Decode(text);
        }
    }
}
