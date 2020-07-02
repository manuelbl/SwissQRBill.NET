//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//


namespace Codecrete.SwissQRBill.Generator
{
    /// <summary>
    /// Constants for bill validation messages: message keys and field names.
    /// </summary>
    public static class ValidationConstants
    {
        /// <summary>
        /// Validation message key: currency must be "CHF" or "EUR"
        /// </summary>
        public const string KeyCurrencyIsCHFOrEUR = "currency_is_chf_or_eur";
        /// <summary>
        /// Validation message key: amount must be between 0.01 and 999999999.99
        /// </summary>
        public const string KeyAmountIsInValidRange = "amount_in_valid_range";
        /// <summary>
        /// Validation message key: IBAN must be from bank in Switzerland or
        /// Liechtenstein
        /// </summary>
        public const string KeyAccountIsCH_LI_IBAN = "account_is_ch_li_iban";
        /// <summary>
        /// Validation message key: IBAN must have valid format and check digit
        /// </summary>
        public const string KeyAccountIsValidIBAN = "account_is_valid_iban";
        /// <summary>
        /// Validation message key: Due to regular IBAN (outside QR-IID range) an ISO 11649 references is expected
        /// but it has invalid format or check digit
        /// </summary>
        public const string KeyValidISO11649CreditorRef = "valid_iso11649_creditor_ref";
        /// <summary>
        /// Validation message key: Due to QR-IBAN (IBAN in QR-IID range) a QR reference number is expected
        /// but it has invalid format or check digit
        /// </summary>
        public const string KeyValidQRRefNo = "valid_qr_ref_no";
        /// <summary>
        /// Validation message key: For QR-IBANs (IBAN in QR-IID range) a QR reference is mandatory
        /// </summary>
        public const string KeyMandatoryForQRIBAN = "mandatory_for_qr_iban";
        /// <summary>
        /// Validation message key: Reference type must be one of "QRR", "SCOR" and "NON" and match the reference
        /// </summary>
        public const string KeyValidRefType = "valid_ref_type";
        /// <summary>
        /// Validation message key: Field is mandatory
        /// </summary>
        public const string KeyFieldIsMandatory = "field_is_mandatory";
        /// <summary>
        /// Validation message key: Conflicting fields for both structured and combined elements address type have been used
        /// </summary>
        public const string KeyAddressTypeConflict = "address_type_conflict";
        /// <summary>
        /// Validation message key: Country code must consist of two letters
        /// </summary>
        public const string KeyValidCountryCode = "valid_country_code";
        /// <summary>
        /// Validation message key: Field has been clipped to not exceed the maximum
        /// length
        /// </summary>
        public const string KeyFieldClipped = "field_clipped";
        /// <summary>
        /// Validation message key: Field value exceed the maximum length
        /// </summary>
        public const string KeyFieldTooLong = "field_value_too_long";
        /// <summary>
        /// Validation message key: The unstructured message and the bill information combined exceed 140 characters
        /// </summary>
        public const string KeyAdditionalInfoTooLong = "additional_info_too_long";
        /// <summary>
        /// Validation message key: Unsupported characters have been replaced
        /// </summary>
        public const string KeyReplacedUnsupportedCharacters = "replaced_unsupported_characters";
        /// <summary>
        /// Validation message key: Valid data structure starts with "SPC" and consists
        /// of 32 to 34 lines of text (with exceptions)
        /// </summary>
        public const string KeyValidDataStructure = "valid_data_structure";
        /// <summary>
        /// Validation message key: Version 02.00 is supported only
        /// </summary>
        public const string KeySupportedVersion = "supported_version";
        /// <summary>
        /// Validation message key: Coding type 1 is supported only
        /// </summary>
        public const string KeySupportedCodingType = "supported_coding_type";
        /// <summary>
        /// Validation message key: Valid number required (nnnnn.nn)
        /// </summary>
        public const string KeyValidNumber = "valid_number";
        /// <summary>
        /// Validation message key: The maximum of 2 alternative schemes has been exceeded
        /// </summary>
        public const string KeyAltSchemeExceeded = "alt_scheme_max_exceed";
        /// <summary>
        /// Validation message key: The bill information is invalid (does not start with // or is too short)
        /// </summary>
        public const string KeyBillInfoInvalid = "bill_info_invalid";

        /// <summary>
        /// Relative field name of an address' name.
        /// </summary>
        public const string SubFieldName = ".name";
        /// <summary>
        /// Relative field of an address' line 1.
        /// </summary>
        public const string SubFieldAddressLine1 = ".addressLine1";
        /// <summary>
        /// Relative field of an address' line 2.
        /// </summary>
        public const string SubFieldAddressLine2 = ".addressLine2";
        /// <summary>
        /// Relative field of an address' street.
        /// </summary>
        public const string SubFieldStreet = ".street";
        /// <summary>
        /// Relative field of an address' house number.
        /// </summary>
        public const string SubFieldHouseNo = ".houseNo";
        /// <summary>
        /// Relative field of an address' postal code.
        /// </summary>
        public const string SubFieldPostalCode = ".postalCode";
        /// <summary>
        /// Relative field of an address' town.
        /// </summary>
        public const string SubFieldTown = ".town";
        /// <summary>
        /// Relative field of an address' country code.
        /// </summary>
        public const string SubFieldCountryCode = ".countryCode";
        /// <summary>
        /// Field name of the QR code type.
        /// </summary>
        public const string FieldQrType = "qrText";
        /// <summary>
        /// Field name of the QR bill version.
        /// </summary>
        public const string FieldVersion = "version";
        /// <summary>
        /// Field name of the QR bill's coding type.
        /// </summary>
        public const string FieldCodingType = "codingType";
        /// <summary>
        /// Field name of the QR bill's trailer ("EPD").
        /// </summary>
        public const string FieldTrailer = "trailer";
        /// <summary>
        /// Field name of the currency.
        /// </summary>
        public const string FieldCurrency = "currency";
        /// <summary>
        /// Field name of the amount.
        /// </summary>
        public const string FieldAmount = "amount";
        /// <summary>
        /// Field name of the account number.
        /// </summary>
        public const string FieldAccount = "account";
        /// <summary>
        /// Field name of the reference type.
        /// </summary>
        public const string FieldReferenceType = "referenceType";
        /// <summary>
        /// Field name of the reference.
        /// </summary>
        public const string FieldReference = "reference";
        /// <summary>
        /// Start of field name of the creditor address.
        /// </summary>
        public const string FieldRootCreditor = "creditor";
        /// <summary>
        /// Field name of the creditor's name.
        /// </summary>
        public const string FieldCreditorName = "creditor.name";
        /// <summary>
        /// Field name of the creditor's street.
        /// </summary>
        public const string FieldCreditorStreet = "creditor.street";
        /// <summary>
        /// Field name of the creditor's house number.
        /// </summary>
        public const string FieldCreditorHouseNo = "creditor.houseNo";
        /// <summary>
        /// Field name of the creditor's postal code.
        /// </summary>
        public const string FieldCreditorPostalCode = "creditor.postalCode";
        /// <summary>
        /// Field name of the creditor's town.
        /// </summary>
        public const string FieldCreditorTown = "creditor.town";
        /// <summary>
        /// Field name of the creditor's country code.
        /// </summary>
        public const string FieldCreditorCountryCode = "creditor.countryCode";
        /// <summary>
        /// Field name of the unstructured message.
        /// </summary>
        public const string FieldUnstructuredMessage = "unstructuredMessage";
        /// <summary>
        /// Field name of the bill information.
        /// </summary>
        public const string FieldBillInformation = "billInformation";
        /// <summary>
        /// Field name of the alternative schemes.
        /// </summary>
        public const string FieldAlternativeSchemes = "altSchemes";
        /// <summary>
        /// Start of field name of the debtor's address.
        /// </summary>
        public const string FieldRootDebtor = "debtor";
        /// <summary>
        /// Field name of the debtor's name.
        /// </summary>
        public const string FieldDebtorName = "debtor.name";
        /// <summary>
        /// Field name of the debtor's street.
        /// </summary>
        public const string FieldDebtorStreet = "debtor.street";
        /// <summary>
        /// Field name of the debtor's house number.
        /// </summary>
        public const string FieldDebtorHouseNo = "debtor.houseNo";
        /// <summary>
        /// Field name of the debtor's postal code.
        /// </summary>
        public const string FieldDebtorPostalCode = "debtor.postalCode";
        /// <summary>
        /// Field name of the debtor's town.
        /// </summary>
        public const string FieldDebtorTown = "debtor.town";
        /// <summary>
        /// Field name of the debtor's country code.
        /// </summary>
        public const string FieldDebtorCountryCode = "debtor.countryCode";
    }
}
