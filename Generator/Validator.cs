//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System;
using System.Collections.Generic;
using static Codecrete.SwissQRBill.Generator.Address;
using static Codecrete.SwissQRBill.Generator.ValidationMessage;

namespace Codecrete.SwissQRBill.Generator
{
    /// <summary>
    /// Internal class for validating and cleaning QR bill data.
    /// </summary>
    internal struct Validator
    {
        private readonly Bill _billIn;
        private readonly Bill _billOut;
        private readonly ValidationResult _validationResult;

        /// <summary>
        /// Validates the QR bill data and returns the validation messages (if any) and
        /// the cleaned bill data.
        /// </summary>
        /// <param name="bill">The bill data to validate.</param>
        /// <returns>The validation result.</returns>
        internal static ValidationResult Validate(Bill bill)
        {
            Validator validator = new Validator(bill);
            return validator.ValidateBill();
        }

        private Validator(Bill bill)
        {
            _billIn = bill;
            _billOut = new Bill();
            _validationResult = new ValidationResult();
        }

        private ValidationResult ValidateBill()
        {

            _billOut.Format = _billIn.Format != null ? new BillFormat(_billIn.Format) : null;
            _billOut.Version = _billIn.Version;

            ValidateAccountNumber();
            ValidateCreditor();
            ValidateCurrency();
            ValidateAmount();
            ValidateDebtor();
            ValidateReference();
            ValidateAdditionalInformation();
            ValidateAlternativeSchemes();

            _validationResult.CleanedBill = _billOut;
            return _validationResult;
        }

        private void ValidateCurrency()
        {
            string currency = _billIn.Currency.Trimmed();
            if (!ValidateMandatory(currency, ValidationConstants.FieldCurrency))
            {
                return;
            }

            currency = currency.ToUpperInvariant();
            if (currency != "CHF" && currency != "EUR")
            {
                _validationResult.AddMessage(MessageType.Error, ValidationConstants.FieldCurrency, ValidationConstants.KeyCurrencyNotChfOrEur);
            }
            else
            {
                _billOut.Currency = currency;
            }
        }

        private void ValidateAmount()
        {
            decimal? amount = _billIn.Amount;
            if (amount == null)
            {
                _billOut.Amount = null;
            }
            else
            {
                decimal amt = amount.Value;
                amt = Math.Round(amt, 2, MidpointRounding.AwayFromZero); // round to multiple of 0.01
                if (amt < 0.00m || amt > 999999999.99m)
                {
                    _validationResult.AddMessage(MessageType.Error, ValidationConstants.FieldAmount, ValidationConstants.KeyAmountOutsideValidRange);
                }
                else
                {
                    _billOut.Amount = amt;
                }
            }
        }

        private void ValidateAccountNumber()
        {
            string account = _billIn.Account.Trimmed();
            if (!ValidateMandatory(account, ValidationConstants.FieldAccount))
            {
                return;
            }

            account = account.WhiteSpaceRemoved().ToUpperInvariant();
            if (!ValidateIban(account))
            {
                return;
            }

            if (!account.StartsWith("CH") && !account.StartsWith("LI"))
            {
                _validationResult.AddMessage(MessageType.Error, ValidationConstants.FieldAccount, ValidationConstants.KeyAccountIbanNotFromChOrLi);
            }
            else if (account.Length != 21)
            {
                _validationResult.AddMessage(MessageType.Error, ValidationConstants.FieldAccount, ValidationConstants.KeyAccountIbanInvalid);
            }
            else
            {
                _billOut.Account = account;
            }
        }

        private void ValidateCreditor()
        {
            Address creditor = ValidateAddress(_billIn.Creditor, ValidationConstants.FieldRootCreditor, true);
            _billOut.Creditor = creditor;
        }

        private void ValidateReference()
        {
            string account = _billOut.Account;
            bool isValidAccount = account != null;
            bool isQrBillIban = account != null && Payments.IsQrIban(account);

            string reference = _billIn.Reference.Trimmed();

            bool hasReferenceError = false;
            if (reference != null)
            {
                reference = reference.WhiteSpaceRemoved();
                bool looksLikeQrRef = Payments.IsNumeric(reference);
                if (looksLikeQrRef)
                {
                    ValidateQrReference(reference);
                }
                else
                {
                    ValidateIsoReference(reference);
                }
                hasReferenceError = _billOut.Reference == null;
            }

            if (isQrBillIban)
            {
                if (Bill.ReferenceTypeNoRef == _billOut.ReferenceType && !hasReferenceError)
                {
                    _validationResult.AddMessage(MessageType.Error, ValidationConstants.FieldReference, ValidationConstants.KeyQrRefMissing);
                }
                else if (Bill.ReferenceTypeCredRef == _billOut.ReferenceType)
                {
                    _validationResult.AddMessage(MessageType.Error, ValidationConstants.FieldReference, ValidationConstants.KeyCredRefInvalidUseForQrIban);
                }
            }
            else if (isValidAccount)
            {
                if (Bill.ReferenceTypeQrRef == _billOut.ReferenceType)
                {
                    _validationResult.AddMessage(MessageType.Error, ValidationConstants.FieldReference, ValidationConstants.KeyQrRefInvalidUseForNonQrIban);
                }
            }
        }

        private void ValidateQrReference(string cleanedReference)
        {
            if (cleanedReference.Length < 27)
            {
                cleanedReference = "00000000000000000000000000".Substring(0, 27 - cleanedReference.Length) + cleanedReference;
            }

            if (!Payments.IsValidQrReference(cleanedReference))
            {
                _validationResult.AddMessage(MessageType.Error, ValidationConstants.FieldReference, ValidationConstants.KeyRefInvalid);
            }
            else
            {
                _billOut.Reference = cleanedReference;
                if (Bill.ReferenceTypeQrRef != _billIn.ReferenceType)
                    _validationResult.AddMessage(MessageType.Error, ValidationConstants.FieldReferenceType, ValidationConstants.KeyRefTypeInvalid);
            }
        }

        private void ValidateIsoReference(string cleanedReference)
        {
            if (!Payments.IsValidIso11649Reference(cleanedReference))
            {
                _validationResult.AddMessage(MessageType.Error, ValidationConstants.FieldReference, ValidationConstants.KeyRefInvalid);
            }
            else
            {
                _billOut.Reference = cleanedReference;
                if (Bill.ReferenceTypeCredRef != _billIn.ReferenceType)
                    _validationResult.AddMessage(MessageType.Error, ValidationConstants.FieldReferenceType, ValidationConstants.KeyRefTypeInvalid);
            }
        }

        private void ValidateAdditionalInformation()
        {
            string billInformation = _billIn.BillInformation.Trimmed();
            string unstructuredMessage = _billIn.UnstructuredMessage.Trimmed();

            if (billInformation != null && (!billInformation.StartsWith("//") || billInformation.Length < 4))
            {
                _validationResult.AddMessage(MessageType.Error, ValidationConstants.FieldBillInformation, ValidationConstants.KeyBillInfoInvalid);
                billInformation = null;
            }

            if (billInformation == null && unstructuredMessage == null)
            {
                return;
            }

            if (billInformation == null)
            {
                unstructuredMessage = CleanedValue(unstructuredMessage, ValidationConstants.FieldUnstructuredMessage);
                unstructuredMessage = ClippedValue(unstructuredMessage, 140, ValidationConstants.FieldUnstructuredMessage);
                _billOut.UnstructuredMessage = unstructuredMessage;
            }
            else if (unstructuredMessage == null)
            {
                billInformation = CleanedValue(billInformation, ValidationConstants.FieldBillInformation);
                if (ValidateLength(billInformation, 140, ValidationConstants.FieldBillInformation))
                {
                    _billOut.BillInformation = billInformation;
                }
            }
            else
            {
                billInformation = CleanedValue(billInformation, ValidationConstants.FieldBillInformation);
                unstructuredMessage = CleanedValue(unstructuredMessage, ValidationConstants.FieldUnstructuredMessage);

                int combinedLength = billInformation.Length + unstructuredMessage.Length;
                if (combinedLength > 140)
                {
                    _validationResult.AddMessage(MessageType.Error, ValidationConstants.FieldUnstructuredMessage, ValidationConstants.KeyAdditionalInfoTooLong);
                    _validationResult.AddMessage(MessageType.Error, ValidationConstants.FieldBillInformation, ValidationConstants.KeyAdditionalInfoTooLong);
                }
                else
                {
                    _billOut.UnstructuredMessage = unstructuredMessage;
                    _billOut.BillInformation = billInformation;
                }
            }
        }

        private void ValidateAlternativeSchemes()
        {
            List<AlternativeScheme> schemesOut = null;
            if (_billIn.AlternativeSchemes != null)
            {
                schemesOut = new List<AlternativeScheme>();

                foreach (AlternativeScheme schemeIn in _billIn.AlternativeSchemes)
                {
                    AlternativeScheme schemeOut = new AlternativeScheme
                    {
                        Name = schemeIn.Name.Trimmed(),
                        Instruction = schemeIn.Instruction.Trimmed()
                    };
                    if ((schemeOut.Name != null || schemeOut.Instruction != null)
                        && ValidateLength(schemeOut.Instruction, 100, ValidationConstants.FieldAlternativeSchemes))
                    {
                        schemesOut.Add(schemeOut);
                    }
                }

                if (schemesOut.Count > 0)
                {
                    if (schemesOut.Count > 2)
                    {
                        _validationResult.AddMessage(MessageType.Error, ValidationConstants.FieldAlternativeSchemes, ValidationConstants.KeyAltSchemeMaxExceeded);
                        schemesOut = schemesOut.GetRange(0, 2);
                    }
                }
                else
                {
                    schemesOut = null;
                }
            }
            _billOut.AlternativeSchemes = schemesOut;
        }

        private void ValidateDebtor()
        {
            Address debtor = ValidateAddress(_billIn.Debtor, ValidationConstants.FieldRootDebtor, false);
            _billOut.Debtor = debtor;
        }

        private Address ValidateAddress(Address addressIn, string fieldRoot, bool mandatory)
        {
            Address addressOut = CleanedPerson(addressIn, fieldRoot);
            if (addressOut == null)
            {
                ValidateEmptyAddress(fieldRoot, mandatory);
                return null;
            }

            if (addressOut.Type == AddressType.Conflicting)
            {
                EmitErrorsForConflictingType(addressOut, fieldRoot);
            }

            CheckMandatoryAddressFields(addressOut, fieldRoot);

            if (addressOut.CountryCode != null
                    && (addressOut.CountryCode.Length != 2 || !Payments.IsAlpha(addressOut.CountryCode)))
            {
                _validationResult.AddMessage(MessageType.Error, fieldRoot + ValidationConstants.SubFieldCountryCode,
                        ValidationConstants.KeyCountryCodeInvalid);
            }

            CleanAddressFields(addressOut, fieldRoot);

            return addressOut;
        }

        private void ValidateEmptyAddress(string fieldRoot, bool mandatory)
        {
            if (mandatory)
            {
                _validationResult.AddMessage(MessageType.Error, fieldRoot + ValidationConstants.SubFieldName, ValidationConstants.KeyFieldValueMissing);
                _validationResult.AddMessage(MessageType.Error, fieldRoot + ValidationConstants.SubFieldPostalCode, ValidationConstants.KeyFieldValueMissing);
                _validationResult.AddMessage(MessageType.Error, fieldRoot + ValidationConstants.SubFieldAddressLine2, ValidationConstants.KeyFieldValueMissing);
                _validationResult.AddMessage(MessageType.Error, fieldRoot + ValidationConstants.SubFieldTown, ValidationConstants.KeyFieldValueMissing);
                _validationResult.AddMessage(MessageType.Error, fieldRoot + ValidationConstants.SubFieldCountryCode, ValidationConstants.KeyFieldValueMissing);
            }
        }

        private void EmitErrorsForConflictingType(Address addressOut, string fieldRoot)
        {
            if (addressOut.AddressLine1 != null)
            {
                _validationResult.AddMessage(MessageType.Error, fieldRoot + ValidationConstants.SubFieldAddressLine1, ValidationConstants.KeyAddressTypeConflict);
            }

            if (addressOut.AddressLine2 != null)
            {
                _validationResult.AddMessage(MessageType.Error, fieldRoot + ValidationConstants.SubFieldAddressLine2, ValidationConstants.KeyAddressTypeConflict);
            }

            if (addressOut.Street != null)
            {
                _validationResult.AddMessage(MessageType.Error, fieldRoot + ValidationConstants.SubFieldStreet, ValidationConstants.KeyAddressTypeConflict);
            }

            if (addressOut.HouseNo != null)
            {
                _validationResult.AddMessage(MessageType.Error, fieldRoot + ValidationConstants.SubFieldHouseNo, ValidationConstants.KeyAddressTypeConflict);
            }

            if (addressOut.PostalCode != null)
            {
                _validationResult.AddMessage(MessageType.Error, fieldRoot + ValidationConstants.SubFieldPostalCode, ValidationConstants.KeyAddressTypeConflict);
            }

            if (addressOut.Town != null)
            {
                _validationResult.AddMessage(MessageType.Error, fieldRoot + ValidationConstants.SubFieldTown, ValidationConstants.KeyAddressTypeConflict);
            }
        }

        private void CheckMandatoryAddressFields(Address addressOut, string fieldRoot)
        {
            ValidateMandatory(addressOut.Name, fieldRoot, ValidationConstants.SubFieldName);
            if (addressOut.Type == AddressType.Structured || addressOut.Type == AddressType.Undetermined)
            {
                ValidateMandatory(addressOut.PostalCode, fieldRoot, ValidationConstants.SubFieldPostalCode);
                ValidateMandatory(addressOut.Town, fieldRoot, ValidationConstants.SubFieldTown);
            }
            if (addressOut.Type == AddressType.CombinedElements || addressOut.Type == AddressType.Undetermined)
            {
                ValidateMandatory(addressOut.AddressLine2, fieldRoot, ValidationConstants.SubFieldAddressLine2);
            }
            ValidateMandatory(addressOut.CountryCode, fieldRoot, ValidationConstants.SubFieldCountryCode);
        }

        private void CleanAddressFields(Address addressOut, string fieldRoot)
        {
            addressOut.Name = ClippedValue(addressOut.Name, 70, fieldRoot, ValidationConstants.SubFieldName);
            if (addressOut.Type == AddressType.Structured)
            {
                addressOut.Street = ClippedValue(addressOut.Street, 70, fieldRoot, ValidationConstants.SubFieldStreet);
                addressOut.HouseNo = ClippedValue(addressOut.HouseNo, 16, fieldRoot, ValidationConstants.SubFieldHouseNo);
                addressOut.PostalCode = ClippedValue(addressOut.PostalCode, 16, fieldRoot, ValidationConstants.SubFieldPostalCode);
                addressOut.Town = ClippedValue(addressOut.Town, 35, fieldRoot, ValidationConstants.SubFieldTown);
            }
            if (addressOut.Type == AddressType.CombinedElements)
            {
                addressOut.AddressLine1 = ClippedValue(addressOut.AddressLine1, 70, fieldRoot, ValidationConstants.SubFieldAddressLine1);
                addressOut.AddressLine2 = ClippedValue(addressOut.AddressLine2, 70, fieldRoot, ValidationConstants.SubFieldAddressLine2);
            }
            if (addressOut.CountryCode != null)
            {
                addressOut.CountryCode = addressOut.CountryCode.ToUpperInvariant();
            }
        }

        private bool ValidateIban(string iban)
        {
            if (Payments.IsValidIban(iban))
            {
                return true;
            }

            _validationResult.AddMessage(MessageType.Error, ValidationConstants.FieldAccount, ValidationConstants.KeyAccountIbanInvalid);
            return false;
        }

        private Address CleanedPerson(Address addressIn, string fieldRoot)
        {
            if (addressIn == null)
            {
                return null;
            }

            Address addressOut = new Address
            {
                Name = CleanedValue(addressIn.Name, fieldRoot, ValidationConstants.SubFieldName)
            };
            string value = CleanedValue(addressIn.AddressLine1, fieldRoot, ValidationConstants.SubFieldAddressLine1);
            if (value != null)
            {
                addressOut.AddressLine1 = value;
            }

            value = CleanedValue(addressIn.AddressLine2, fieldRoot, ValidationConstants.SubFieldAddressLine2);
            if (value != null)
            {
                addressOut.AddressLine2 = value;
            }

            value = CleanedValue(addressIn.Street, fieldRoot, ValidationConstants.SubFieldStreet);
            if (value != null)
            {
                addressOut.Street = value;
            }

            value = CleanedValue(addressIn.HouseNo, fieldRoot, ValidationConstants.SubFieldHouseNo);
            if (value != null)
            {
                addressOut.HouseNo = value;
            }

            value = CleanedValue(addressIn.PostalCode, fieldRoot, ValidationConstants.SubFieldPostalCode);
            if (value != null)
            {
                addressOut.PostalCode = value;
            }

            value = CleanedValue(addressIn.Town, fieldRoot, ValidationConstants.SubFieldTown);
            if (value != null)
            {
                addressOut.Town = value;
            }

            addressOut.CountryCode = addressIn.CountryCode.Trimmed();

            if (addressOut.Name == null && addressOut.CountryCode == null
                    && addressOut.Type == AddressType.Undetermined)
            {
                return null;
            }

            return addressOut;
        }

        private bool ValidateMandatory(string value, string field)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return true;
            }

            _validationResult.AddMessage(MessageType.Error, field, ValidationConstants.KeyFieldValueMissing);
            return false;

        }

        private void ValidateMandatory(string value, string fieldRoot, string subfield)
        {
            if (string.IsNullOrEmpty(value))
            {
                _validationResult.AddMessage(MessageType.Error, fieldRoot + subfield, ValidationConstants.KeyFieldValueMissing);
            }
        }

        private bool ValidateLength(string value, int maxLength, string field)
        {
            if (value == null || value.Length <= maxLength)
            {
                return true;
            }

            _validationResult.AddMessage(MessageType.Error, field, ValidationConstants.KeyFieldValueTooLong,
                new[] { maxLength.ToString() });
            return false;

        }

        private string ClippedValue(string value, int maxLength, string field)
        {
            if (value == null || value.Length <= maxLength)
            {
                return value;
            }

            _validationResult.AddMessage(MessageType.Warning, field, ValidationConstants.KeyFieldValueClipped,
                new[] { maxLength.ToString() });
            return value.Substring(0, maxLength);

        }

        private string ClippedValue(string value, int maxLength, string fieldRoot, string subfield)
        {
            if (value == null || value.Length <= maxLength)
            {
                return value;
            }

            _validationResult.AddMessage(MessageType.Warning, fieldRoot + subfield, ValidationConstants.KeyFieldValueClipped,
                new[] { maxLength.ToString() });
            return value.Substring(0, maxLength);

        }

        private string CleanedValue(string value, string field)
        {
            Payments.CleanValue(value, out Payments.CleaningResult result);
            if (result.ReplacedUnsupportedChars)
            {
                _validationResult.AddMessage(MessageType.Warning, field, ValidationConstants.KeyReplacedUnsupportedCharacters);
            }

            return result.CleanedString;
        }

        private string CleanedValue(string value, string fieldRoot, string subfield)
        {
            Payments.CleanValue(value, out Payments.CleaningResult result);
            if (result.ReplacedUnsupportedChars)
            {
                _validationResult.AddMessage(MessageType.Warning, fieldRoot + subfield, ValidationConstants.KeyReplacedUnsupportedCharacters);
            }

            return result.CleanedString;
        }
    }
}
