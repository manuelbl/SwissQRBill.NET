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
        private readonly Bill billIn;
        private readonly Bill billOut;
        private readonly ValidationResult validationResult;

        /// <summary>
        /// Validates the QR bill data and returns the validation messages (if any) and
        /// the cleaned bill data.
        /// </summary>
        /// <param name="bill">bill data to validate</param>
        /// <returns>validation result</returns>
        internal static ValidationResult Validate(Bill bill)
        {
            Validator validator = new Validator(bill);
            return validator.ValidateBill();
        }

        private Validator(Bill bill)
        {
            billIn = bill;
            billOut = new Bill();
            validationResult = new ValidationResult();
        }

        private ValidationResult ValidateBill()
        {

            billOut.Format = billIn.Format != null ? new BillFormat(billIn.Format) : null;
            billOut.Version = billIn.Version;

            ValidateAccountNumber();
            ValidateCreditor();
            ValidateCurrency();
            ValidateAmount();
            ValidateDebtor();
            ValidateReference();
            ValidateUnstructuredMessage();
            ValidateBillInformation();
            ValidateAlternativeSchemes();

            validationResult.CleanedBill = billOut;
            return validationResult;
        }

        private void ValidateCurrency()
        {
            string currency = billIn.Currency.Trimmed();
            if (ValidateMandatory(currency, Bill.FieldCurrency))
            {
                currency = currency.ToUpperInvariant();
                if (currency != "CHF" && currency != "EUR")
                {
                    validationResult.AddMessage(MessageType.Error, Bill.FieldCurrency, QRBill.KeyCurrencyIsCHFOrEUR);
                }
                else
                {
                    billOut.Currency = currency;
                }
            }
        }

        private void ValidateAmount()
        {
            decimal? amount = billIn.Amount;
            if (amount == null)
            {
                billOut.Amount = null;
            }
            else
            {
                decimal amt = amount.Value;
                amt = Math.Round(amt, 2, MidpointRounding.AwayFromZero); // round to multiple of 0.01
                if (amt < 0.01m || amt > 999999999.99m)
                {
                    validationResult.AddMessage(MessageType.Error, Bill.FieldAmount, QRBill.KeyAmountIsInValidRange);
                }
                else
                {
                    billOut.Amount = amt;
                }
            }
        }

        private void ValidateAccountNumber()
        {
            string account = billIn.Account.Trimmed();
            if (ValidateMandatory(account, Bill.FieldAccount))
            {
                account = account.WhiteSpaceRemoved().ToUpperInvariant();
                if (ValidateIBAN(account))
                {
                    if (!account.StartsWith("CH") && !account.StartsWith("LI"))
                    {
                        validationResult.AddMessage(MessageType.Error, Bill.FieldAccount, QRBill.KeyAccountIsCH_LI_IBAN);
                    }
                    else if (account.Length != 21)
                    {
                        validationResult.AddMessage(MessageType.Error, Bill.FieldAccount, QRBill.KeyAccountIsValidIBAN);
                    }
                    else
                    {
                        billOut.Account = account;
                    }
                }
            }
        }

        private void ValidateCreditor()
        {
            Address creditor = ValidateAddress(billIn.Creditor, Bill.FieldRootCreditor, true);
            billOut.Creditor = creditor;
        }

        private void ValidateReference()
        {
            string account = billOut.Account;
            bool isValidAccount = account != null;
            bool isQRBillIBAN = account != null && account[4] == '3'
                    && (account[5] == '0' || account[5] == '1');

            string reference = billIn.Reference.Trimmed();
            if (reference != null)
            {
                reference = reference.WhiteSpaceRemoved();
            }

            if (isQRBillIBAN)
            {

                ValidateQRReference(reference);

            }
            else if (isValidAccount && reference != null)
            {

                ValidateISOReference(reference);

            }
            else
            {
                billOut.Reference = null;
            }
        }

        private void ValidateQRReference(string cleanedReference)
        {
            if (cleanedReference == null)
            {
                validationResult.AddMessage(MessageType.Error, Bill.FieldReference, QRBill.KeyMandatoryForQRIBAN);
                return;
            }

            if (cleanedReference.Length < 27)
            {
                cleanedReference = "00000000000000000000000000".Substring(0, 27 - cleanedReference.Length) + cleanedReference;
            }

            if (!Payments.IsValidQRReference(cleanedReference))
            {
                validationResult.AddMessage(MessageType.Error, Bill.FieldReference, QRBill.KeyValidQRRefNo);
            }
            else
            {
                billOut.Reference = cleanedReference;
            }
        }

        private void ValidateISOReference(string cleanedReference)
        {
            if (!Payments.IsValidISO11649Reference(cleanedReference))
            {
                validationResult.AddMessage(MessageType.Error, Bill.FieldReference, QRBill.KeyValidISO11649CreditorRef);
            }
            else
            {
                billOut.Reference = cleanedReference;
            }
        }

        private void ValidateUnstructuredMessage()
        {
            string unstructuredMessage = billIn.UnstructuredMessage.Trimmed();
            unstructuredMessage = ClippedValue(unstructuredMessage, 140, Bill.FieldUnstructuredMessage);
            billOut.UnstructuredMessage = unstructuredMessage;
        }

        private void ValidateBillInformation()
        {
            string billInformation = billIn.BillInformation.Trimmed();
            if (billInformation != null)
            {
                if (!ValidateLength(billInformation, 140, Bill.FieldBillInformation))
                {
                    return;
                }

                if (!billInformation.StartsWith("//") || billInformation.Length < 4)
                {
                    validationResult.AddMessage(MessageType.Error, Bill.FieldBillInformation, QRBill.KeyBillInfoInvalid);
                    return;
                }
            }
            billOut.BillInformation = billInformation;
        }

        private void ValidateAlternativeSchemes()
        {
            List<AlternativeScheme> schemesOut = null;
            if (billIn.AlternativeSchemes != null)
            {
                schemesOut = new List<AlternativeScheme>();

                foreach (AlternativeScheme schemeIn in billIn.AlternativeSchemes)
                {
                    AlternativeScheme schemeOut = new AlternativeScheme
                    {
                        Name = schemeIn.Name.Trimmed(),
                        Instruction = schemeIn.Instruction.Trimmed()
                    };
                    if ((schemeOut.Name != null || schemeOut.Instruction != null)
                        && ValidateLength(schemeOut.Instruction, 100, Bill.FieldAlternativeSchemes))
                    {
                        schemesOut.Add(schemeOut);
                    }
                }

                if (schemesOut.Count > 0)
                {
                    if (schemesOut.Count > 2)
                    {
                        validationResult.AddMessage(MessageType.Error, Bill.FieldAlternativeSchemes, QRBill.KeyAltSchemeExceeded);
                        schemesOut = schemesOut.GetRange(0, 2);
                    }
                }
                else
                {
                    schemesOut = null;
                }
            }
            billOut.AlternativeSchemes = schemesOut;
        }

        private void ValidateDebtor()
        {
            Address debtor = ValidateAddress(billIn.Debtor, Bill.FieldRootDebtor, false);
            billOut.Debtor = debtor;
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
                    && (addressOut.CountryCode.Length != 2 || !Payments.IsAlphaNumeric(addressOut.CountryCode)))
            {
                validationResult.AddMessage(MessageType.Error, fieldRoot + Bill.SubFieldCountryCode,
                        QRBill.KeyValidCountryCode);
            }

            CleanAddressFields(addressOut, fieldRoot);

            return addressOut;
        }

        private void ValidateEmptyAddress(string fieldRoot, bool mandatory)
        {
            if (mandatory)
            {
                validationResult.AddMessage(MessageType.Error, fieldRoot + Bill.SubFieldName, QRBill.KeyFieldIsMandatory);
                validationResult.AddMessage(MessageType.Error, fieldRoot + Bill.SubFieldPostalCode, QRBill.KeyFieldIsMandatory);
                validationResult.AddMessage(MessageType.Error, fieldRoot + Bill.SubFieldAddressLine2, QRBill.KeyFieldIsMandatory);
                validationResult.AddMessage(MessageType.Error, fieldRoot + Bill.SubFieldTown, QRBill.KeyFieldIsMandatory);
                validationResult.AddMessage(MessageType.Error, fieldRoot + Bill.SubFieldCountryCode, QRBill.KeyFieldIsMandatory);
            }
        }

        private void EmitErrorsForConflictingType(Address addressOut, string fieldRoot)
        {
            if (addressOut.AddressLine1 != null)
            {
                validationResult.AddMessage(MessageType.Error, fieldRoot + Bill.SubFieldAddressLine1, QRBill.KeyAddressTypeConflict);
            }

            if (addressOut.AddressLine2 != null)
            {
                validationResult.AddMessage(MessageType.Error, fieldRoot + Bill.SubFieldAddressLine2, QRBill.KeyAddressTypeConflict);
            }

            if (addressOut.Street != null)
            {
                validationResult.AddMessage(MessageType.Error, fieldRoot + Bill.SubFieldStreet, QRBill.KeyAddressTypeConflict);
            }

            if (addressOut.HouseNo != null)
            {
                validationResult.AddMessage(MessageType.Error, fieldRoot + Bill.SubFieldHouseNo, QRBill.KeyAddressTypeConflict);
            }

            if (addressOut.PostalCode != null)
            {
                validationResult.AddMessage(MessageType.Error, fieldRoot + Bill.SubFieldPostalCode, QRBill.KeyAddressTypeConflict);
            }

            if (addressOut.Town != null)
            {
                validationResult.AddMessage(MessageType.Error, fieldRoot + Bill.SubFieldTown, QRBill.KeyAddressTypeConflict);
            }
        }

        private void CheckMandatoryAddressFields(Address addressOut, string fieldRoot)
        {
            ValidateMandatory(addressOut.Name, fieldRoot, Bill.SubFieldName);
            if (addressOut.Type == AddressType.Structured || addressOut.Type == AddressType.Undetermined)
            {
                ValidateMandatory(addressOut.PostalCode, fieldRoot, Bill.SubFieldPostalCode);
                ValidateMandatory(addressOut.Town, fieldRoot, Bill.SubFieldTown);
            }
            if (addressOut.Type == AddressType.CombinedElements || addressOut.Type == AddressType.Undetermined)
            {
                ValidateMandatory(addressOut.AddressLine2, fieldRoot, Bill.SubFieldAddressLine2);
            }
            ValidateMandatory(addressOut.CountryCode, fieldRoot, Bill.SubFieldCountryCode);
        }

        private void CleanAddressFields(Address addressOut, string fieldRoot)
        {
            addressOut.Name = ClippedValue(addressOut.Name, 70, fieldRoot, Bill.SubFieldName);
            if (addressOut.Type == AddressType.Structured)
            {
                addressOut.Street = ClippedValue(addressOut.Street, 70, fieldRoot, Bill.SubFieldStreet);
                addressOut.HouseNo = ClippedValue(addressOut.HouseNo, 16, fieldRoot, Bill.SubFieldHouseNo);
                addressOut.PostalCode = ClippedValue(addressOut.PostalCode, 16, fieldRoot, Bill.SubFieldPostalCode);
                addressOut.Town = ClippedValue(addressOut.Town, 35, fieldRoot, Bill.SubFieldTown);
            }
            if (addressOut.Type == AddressType.CombinedElements)
            {
                addressOut.AddressLine1 = ClippedValue(addressOut.AddressLine1, 70, fieldRoot, Bill.SubFieldAddressLine1);
                addressOut.AddressLine2 = ClippedValue(addressOut.AddressLine2, 70, fieldRoot, Bill.SubFieldAddressLine2);
            }
            if (addressOut.CountryCode != null)
            {
                addressOut.CountryCode = addressOut.CountryCode.ToUpperInvariant();
            }
        }

        private bool ValidateIBAN(string iban)
        {
            if (!Payments.IsValidIBAN(iban))
            {
                validationResult.AddMessage(MessageType.Error, Bill.FieldAccount, QRBill.KeyAccountIsValidIBAN);
                return false;
            }
            return true;
        }

        private Address CleanedPerson(Address addressIn, string fieldRoot)
        {
            if (addressIn == null)
            {
                return null;
            }

            Address addressOut = new Address
            {
                Name = CleanedValue(addressIn.Name, fieldRoot, Bill.SubFieldName)
            };
            string value = CleanedValue(addressIn.AddressLine1, fieldRoot, Bill.SubFieldAddressLine1);
            if (value != null)
            {
                addressOut.AddressLine1 = value;
            }

            value = CleanedValue(addressIn.AddressLine2, fieldRoot, Bill.SubFieldAddressLine2);
            if (value != null)
            {
                addressOut.AddressLine2 = value;
            }

            value = CleanedValue(addressIn.Street, fieldRoot, Bill.SubFieldStreet);
            if (value != null)
            {
                addressOut.Street = value;
            }

            value = CleanedValue(addressIn.HouseNo, fieldRoot, Bill.SubFieldHouseNo);
            if (value != null)
            {
                addressOut.HouseNo = value;
            }

            value = CleanedValue(addressIn.PostalCode, fieldRoot, Bill.SubFieldPostalCode);
            if (value != null)
            {
                addressOut.PostalCode = value;
            }

            value = CleanedValue(addressIn.Town, fieldRoot, Bill.SubFieldTown);
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
            if (string.IsNullOrEmpty(value))
            {
                validationResult.AddMessage(MessageType.Error, field, QRBill.KeyFieldIsMandatory);
                return false;
            }

            return true;
        }

        private void ValidateMandatory(string value, string fieldRoot, string subfield)
        {
            if (string.IsNullOrEmpty(value))
            {
                validationResult.AddMessage(MessageType.Error, fieldRoot + subfield, QRBill.KeyFieldIsMandatory);
            }
        }

        private bool ValidateLength(string value, int maxLength, string field)
        {
            if (value != null && value.Length > maxLength)
            {
                validationResult.AddMessage(MessageType.Error, field, QRBill.KeyFieldTooLong,
                        new string[] { maxLength.ToString() });
                return false;
            }
            else
            {
                return true;
            }
        }

        private string ClippedValue(string value, int maxLength, string field)
        {
            if (value != null && value.Length > maxLength)
            {
                validationResult.AddMessage(MessageType.Warning, field, QRBill.KeyFieldClipped,
                        new string[] { maxLength.ToString() });
                return value.Substring(0, maxLength);
            }

            return value;
        }

        private string ClippedValue(string value, int maxLength, string fieldRoot, string subfield)
        {
            if (value != null && value.Length > maxLength)
            {
                validationResult.AddMessage(MessageType.Warning, fieldRoot + subfield, QRBill.KeyFieldClipped,
                        new string[] { maxLength.ToString() });
                return value.Substring(0, maxLength);
            }

            return value;
        }

        private string CleanedValue(string value, string fieldRoot, string subfield)
        {
            Payments.CleanValue(value, out Payments.CleaningResult result);
            if (result.ReplacedUnsupportedChars)
            {
                validationResult.AddMessage(MessageType.Warning, fieldRoot + subfield, QRBill.KeyReplacedUnsupportedCharacters);
            }

            return result.CleanedString;
        }
    }
}
