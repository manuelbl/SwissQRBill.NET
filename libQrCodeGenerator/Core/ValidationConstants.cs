//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//


namespace libQrCodeGenerator.SwissQRBill.Generator
{
    /// <summary>
    /// Constants for bill validation messages: message keys and field names.
    /// </summary>
    public static class ValidationConstants
    {
        /// <summary>
        /// Validation message key: currency must be "CHF" or "EUR"
        /// </summary>
        public const string KeyCurrencyNotChfOrEur = "currency_not_chf_or_eur";
        /// <summary>
        /// Validation message key: amount must be between 0.01 and 999999999.99
        /// </summary>
        public const string KeyAmountOutsideValidRange = "amount_outside_valid_range";
        /// <summary>
        /// Validation message key: account number should start with "CH" or "LI"
        /// </summary>
        public const string KeyAccountIbanNotFromChOrLi = "account_iban_not_from_ch_or_li";
        /// <summary>
        /// Validation message key: IBAN is not valid (incorrect format or check digit)
        /// </summary>
        public const string KeyAccountIbanInvalid = "account_iban_invalid";
        /// <summary>
        /// Validation message key: The reference is invalid. It is neither a valid QR reference nor a valid ISO 11649
        /// reference.
        /// </summary>
        public const string KeyRefInvalid = "ref_invalid";
        /// <summary>
        /// Validation message key: QR reference is missing; it is mandatory for payments to a QR-IBAN account.
        /// </summary>
        public const string KeyQrRefMissing = "qr_ref_missing";
        /// <summary>
        /// Validation message key: For payments to a QR-IBAN account, a QR reference is required. An ISO 11649 reference
        /// may not be used.
        /// </summary>
        public const string KeyCredRefInvalidUseForQrIban = "cred_ref_invalid_use_for_qr_iban";
        /// <summary>
        /// Validation message key: A QR reference is only allowed for payments to a QR-IBAN account.
        /// </summary>
        public const string KeyQrRefInvalidUseForNonQrIban = "qr_ref_invalid_use_for_non_qr_iban";
        /// <summary>
        /// Validation message key: Reference type should be one of "QRR", "SCOR" and "NON" and match the reference.
        /// </summary>
        public const string KeyRefTypeInvalid = "ref_type_invalid";
        /// <summary>
        /// Validation message key: Field must not be empty
        /// </summary>
        public const string KeyFieldValueMissing = "field_value_missing";
        /// <summary>
        /// Validation message key: Conflicting fields for both structured and combined elements address type have been used
        /// </summary>
        public const string KeyAddressTypeConflict = "address_type_conflict";
        /// <summary>
        /// Validation message key: Country code must consist of two letters
        /// </summary>
        public const string KeyCountryCodeInvalid = "country_code_invalid";
        /// <summary>
        /// Validation message key: Field has been clipped to not exceed the maximum length
        /// </summary>
        public const string KeyFieldValueClipped = "field_value_clipped";
        /// <summary>
        /// Validation message key: Field value exceed the maximum length
        /// </summary>
        public const string KeyFieldValueTooLong = "field_value_too_long";
        /// <summary>
        /// Validation message key: Unstructured message and bill information combined exceed the maximum length
        /// </summary>
        public const string KeyAdditionalInfoTooLong = "additional_info_too_long";
        /// <summary>
        /// Validation message key: Unsupported characters have been replaced
        /// </summary>
        public const string KeyReplacedUnsupportedCharacters = "replaced_unsupported_characters";
        /// <summary>
        /// Validation message key: Invalid data structure; it must start with "SPC" and consists
        /// of 32 to 34 lines of text (with exceptions)
        /// </summary>
        public const string KeyDataStructureInvalid = "data_structure_invalid";
        /// <summary>
        /// Validation message key: Version 02.00 is supported only
        /// </summary>
        public const string KeyVersionUnsupported = "version_unsupported";
        /// <summary>
        /// Validation message key: Coding type 1 is supported only
        /// </summary>
        public const string KeyCodingTypeUnsupported = "coding_type_unsupported";
        /// <summary>
        /// Validation message key: Valid number required (nnnnn.nn)
        /// </summary>
        public const string KeyNumberInvalid = "number_invalid";
        /// <summary>
        /// Validation message key: The maximum of 2 alternative schemes has been exceeded
        /// </summary>
        public const string KeyAltSchemeMaxExceeded = "alt_scheme_max_exceed";
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
