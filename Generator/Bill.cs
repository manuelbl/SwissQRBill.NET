//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System;
using System.Collections.Generic;
using System.Linq;

namespace Codecrete.SwissQRBill.Generator
{
    /// <summary>
    /// QR bill data
    /// </summary>
    public class Bill : IEquatable<Bill>
    {
        /// <summary>
        /// QR bill version
        /// </summary>
        public enum StandardVersion
        {
            /// <summary>
            /// Version 2.0
            /// </summary>
            V2_0
        }

        /// <summary>
        /// Relative field name of an address' name
        /// </summary>
        public static readonly string SubFieldName = ".name";
        /// <summary>
        /// Relative field of an address' line 1
        /// </summary>
        public static readonly string SubFieldAddressLine1 = ".addressLine1";
        /// <summary>
        /// Relative field of an address' line 2
        /// </summary>
        public static readonly string SubFieldAddressLine2 = ".addressLine2";
        /// <summary>
        /// Relative field of an address' street
        /// </summary>
        public static readonly string SubFieldStreet = ".street";
        /// <summary>
        /// Relative field of an address' house number
        /// </summary>
        public static readonly string SubFieldHouseNo = ".houseNo";
        /// <summary>
        /// Relative field of an address' postal code
        /// </summary>
        public static readonly string SubFieldPostalCode = ".postalCode";
        /// <summary>
        /// Relative field of an address' town
        /// </summary>
        public static readonly string SubFieldTown = ".town";
        /// <summary>
        /// Relative field of an address' country code
        /// </summary>
        public static readonly string SubFieldCountryCode = ".countryCode";
        /// <summary>
        /// Field name of the QR code type
        /// </summary>
        public static readonly string FieldQRType = "qrText";
        /// <summary>
        /// Field name of the QR bill version
        /// </summary>
        public static readonly string FieldVersion = "version";
        /// <summary>
        /// Field name of the QR bill's coding type
        /// </summary>
        public static readonly string FieldCodingType = "codingType";
        /// <summary>
        /// Field name of the QR bill's trailer ("EPD")
        /// </summary>
        public static readonly string FieldTrailer = "trailer";
        /// <summary>
        /// Field name of the currency
        /// </summary>
        public static readonly string FieldCurrency = "currency";
        /// <summary>
        /// Field name of the amount
        /// </summary>
        public static readonly string FieldAmount = "amount";
        /// <summary>
        /// Field name of the account number
        /// </summary>
        public static readonly string FieldAccount = "account";
        /// <summary>
        /// Field name of the reference
        /// </summary>
        public static readonly string FieldReference = "reference";
        /// <summary>
        /// Start of field name of the creditor address
        /// </summary>
        public static readonly string FieldRootCreditor = "creditor";
        /// <summary>
        /// Field name of the creditor's name
        /// </summary>
        public static readonly string FieldCreditorName = "creditor.name";
        /// <summary>
        /// Field name of the creditor's street
        /// </summary>
        public static readonly string FieldCreditorStreet = "creditor.street";
        /// <summary>
        /// Field name of the creditor's house number
        /// </summary>
        public static readonly string FieldCreditorHouseNo = "creditor.houseNo";
        /// <summary>
        /// Field name of the creditor's postal codde
        /// </summary>
        public static readonly string FieldCreditorPostalCode = "creditor.postalCode";
        /// <summary>
        /// Field name of the creditor's town
        /// </summary>
        public static readonly string FieldCreditorTown = "creditor.town";
        /// <summary>
        /// Field name of the creditor's country code
        /// </summary>
        public static readonly string FieldCreditorCountryCode = "creditor.countryCode";
        /// <summary>
        /// Field name of the unstructured message
        /// </summary>
        public static readonly string FieldUnstructuredMessage = "unstructuredMessage";
        /// <summary>
        /// Field name of the bill information
        /// </summary>
        public static readonly string FieldBillInformation = "billInformation";
        /// <summary>
        /// Field name of the alternative schemes
        /// </summary>
        public static readonly string FieldAlternativeSchemes = "altSchemes";
        /// <summary>
        /// Start of field name of the debtor's address
        /// </summary>
        public static readonly string FieldRootDebtor = "debtor";
        /// <summary>
        /// Field name of the debtor's name
        /// </summary>
        public static readonly string FieldDebtorName = "debtor.name";
        /// <summary>
        /// Field name of the debtor's street
        /// </summary>
        public static readonly string FieldDebtorStreet = "debtor.street";
        /// <summary>
        /// Field name of the debtor's house number
        /// </summary>
        public static readonly string FieldDebtorHouseNo = "debtor.houseNo";
        /// <summary>
        /// Field name of the debtor's postal code
        /// </summary>
        public static readonly string FieldDebtorPostalCode = "debtor.postalCode";
        /// <summary>
        /// Field name of the debtor's town
        /// </summary>
        public static readonly string FieldDebtorTown = "debtor.town";
        /// <summary>
        /// Field name of the debtor's country code
        /// </summary>
        public static readonly string FieldDebtorCountryCode = "debtor.countryCode";

        /// <summary>
        /// Gets or sets the version of the QR bill standard.
        /// </summary>
        public StandardVersion Version { get; set; } = StandardVersion.V2_0;

        /// <summary>
        /// Gets or sets the payment amount.
        /// </summary>
        /// <remarks>
        /// Valid values are between 0.01 and 999,999,999.99.
        /// </remarks>
        public decimal? Amount { get; set; }

        /// <summary>
        /// Gets or sets the payment currency.
        /// </summary>
        /// <remarks>
        /// Valid values are "CHF" and "EUR".
        /// </remarks>
        public string Currency { get; set; } = "CHF";

        /// <summary>
        /// Gets or sets the creditor's account number.
        /// </summary>
        /// <remarks>
        /// Account numbers must be valid IBANs of a bank of Switzerland or
        /// Liechtenstein. Spaces are allowed in the account number.
        /// </remarks>
        public string Account { get; set; }

        /// <summary>
        /// Gets or sets the creditor address.
        /// </summary>
        public Address Creditor { get; set; } = new Address();

        /// <summary>
        /// Gets or sets the creditor payment reference.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The reference is mandatory for QR IBANs, i.e.IBANs in the range
        /// CHxx30000xxxxxx through CHxx31999xxxxx.
        /// </para>
        /// <para>
        /// If specified, the reference must be either a valid QR reference
        /// (corresponding to ISR reference form) or a valid creditor reference
        /// according to ISO 11649 ("RFxxxx"). Both may contain spaces for formatting.
        /// </para>
        /// </remarks>
        public string Reference { get; set; }

        /// <summary>
        /// Creates and sets a ISO11649 creditor reference from a raw string by prefixing
        /// the String with "RF" and the modulo 97 checksum.
        /// </summary>
        /// <remarks>
        /// Whitespace is removed from the reference
        /// </remarks>
        /// <param name="rawReference">raw string</param>
        /// <exception cref="ArgumentException">Thrown if <c>rawReference</c> contains invalid characters</exception>
        public void CreateAndSetCreditorReference(string rawReference)
        {
            Reference = Payments.CreateISO11649Reference(rawReference);
        }

        /// <summary>
        /// Gets or sets the debtor address.
        /// </summary>
        /// <remarks>
        /// The debtor is optional. If it is omitted, both setting this field to
        /// <c>null</c> or setting an address with all <c>null</c> or empty values is ok.
        /// </remarks>
        public Address Debtor { get; set; }

        /// <summary>
        /// Gets or sets the additional unstructured message.
        /// </summary>
        public string UnstructuredMessage { get; set; }

        /// <summary>
        /// Gets or sets the additional structured bill information.
        /// </summary>
        public string BillInformation { get; set; }

        /// <summary>
        /// Gets ors sets the alternative payment schemes.
        /// </summary>
        /// <remarks>
        /// A maximum of two schemes with parameters are allowed.
        /// </remarks>
        public List<AlternativeScheme> AlternativeSchemes;

        /// <summary>
        /// Gets or sets the bill format.
        /// </summary>
        public BillFormat Format { get; set; } = new BillFormat();

        public override bool Equals(object obj)
        {
            return Equals(obj as Bill);
        }

        public bool Equals(Bill other)
        {
            return other != null &&
                   Version == other.Version &&
                   EqualityComparer<decimal?>.Default.Equals(Amount, other.Amount) &&
                   Currency == other.Currency &&
                   Account == other.Account &&
                   EqualityComparer<Address>.Default.Equals(Creditor, other.Creditor) &&
                   Reference == other.Reference &&
                   EqualityComparer<Address>.Default.Equals(Debtor, other.Debtor) &&
                   UnstructuredMessage == other.UnstructuredMessage &&
                   BillInformation == other.BillInformation &&
                   SequenceEqual(AlternativeSchemes, other.AlternativeSchemes) &&
                   EqualityComparer<BillFormat>.Default.Equals(Format, other.Format);
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Version);
            hash.Add(Amount);
            hash.Add(Currency);
            hash.Add(Account);
            hash.Add(Creditor);
            hash.Add(Reference);
            hash.Add(Debtor);
            hash.Add(UnstructuredMessage);
            hash.Add(BillInformation);
            hash.Add(AlternativeSchemes);
            hash.Add(Format);
            return hash.ToHashCode();
        }

        private static bool SequenceEqual<T>(List<T> list1, List<T> list2)
        {
            if (list1 == list2)
                return true;
            if (list1 == null || list2 == null)
                return false;
            if (list1.Count != list2.Count)
                return false;

            return list1.SequenceEqual(list2);
        }
    }
}
