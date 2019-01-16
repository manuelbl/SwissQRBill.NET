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
    public sealed class Bill : IEquatable<Bill>
    {
        /// <summary>
        /// QR bill standard version
        /// </summary>
        public enum QrBillStandardVersion
        {
            /// <summary>
            /// Version 2.0
            /// </summary>
            // ReSharper disable once InconsistentNaming
            V2_0
        }

        /// <summary>
        /// Gets or sets the version of the QR bill standard.
        /// </summary>
        /// <value>The QR bill standard version.</value>
        public QrBillStandardVersion Version { get; set; } = QrBillStandardVersion.V2_0;

        /// <summary>
        /// Gets or sets the payment amount.
        /// <para>
        /// Valid values are between 0.01 and 999,999,999.99.
        /// </para>
        /// </summary>
        /// <value>The payment amount.</value>
        public decimal? Amount { get; set; }

        /// <summary>
        /// Gets or sets the payment currency.
        /// <para>
        /// Valid values are "CHF" and "EUR".
        /// </para>
        /// </summary>
        /// <value>The payment currency.</value>
        public string Currency { get; set; } = "CHF";

        /// <summary>
        /// Gets or sets the creditor's account number.
        /// <para>
        /// Account numbers must be valid IBANs of a bank of Switzerland or
        /// Liechtenstein. Spaces are allowed in the account number.
        /// </para>
        /// </summary>
        /// <value>The creditor account number.</value>
        public string Account { get; set; }

        /// <summary>
        /// Gets or sets the creditor address.
        /// </summary>
        /// <value>The creditor address.</value>
        public Address Creditor { get; set; } = new Address();

        /// <summary>
        /// Gets or sets the creditor payment reference.
        /// <para>
        /// The reference is mandatory for QR IBANs, i.e.IBANs in the range
        /// CHxx30000xxxxxx through CHxx31999xxxxx.
        /// </para>
        /// <para>
        /// If specified, the reference must be either a valid QR reference
        /// (corresponding to ISR reference form) or a valid creditor reference
        /// according to ISO 11649 ("RFxxxx"). Both may contain spaces for formatting.
        /// </para>
        /// </summary>
        /// <value>The creditor payment reference.</value>
        public string Reference { get; set; }

        /// <summary>
        /// Creates and sets a ISO11649 creditor reference from a raw string by prefixing
        /// the String with "RF" and the modulo 97 checksum.
        /// <para>
        /// Whitespace is removed from the reference
        /// </para>
        /// </summary>
        /// <param name="rawReference">The raw reference.</param>
        /// <exception cref="ArgumentException"><c>rawReference</c> contains invalid characters.</exception>
        public void CreateAndSetCreditorReference(string rawReference)
        {
            Reference = Payments.CreateIso11649Reference(rawReference);
        }

        /// <summary>
        /// Gets or sets the debtor address.
        /// <para>
        /// The debtor is optional. If it is omitted, both setting this field to
        /// <c>null</c> or setting an address with all <c>null</c> or empty values is ok.
        /// </para>
        /// </summary>
        /// <value>The debtor address.</value>
        public Address Debtor { get; set; }

        /// <summary>
        /// Gets or sets the additional unstructured message.
        /// </summary>
        /// <value>The unstructured message.</value>
        public string UnstructuredMessage { get; set; }

        /// <summary>
        /// Gets or sets the additional structured bill information.
        /// </summary>
        /// <value>The structured bill information.</value>
        public string BillInformation { get; set; }

        /// <summary>
        /// Gets ors sets the alternative payment schemes.
        /// <para>
        /// A maximum of two schemes with parameters are allowed.
        /// </para>
        /// </summary>
        /// <value>The alternative payment schemes.</value>
        public List<AlternativeScheme> AlternativeSchemes { get; set; }

        /// <summary>
        /// Gets or sets the bill formatting information.
        /// </summary>
        /// <value>The bill formatting information.</value>
        public BillFormat Format { get; set; } = new BillFormat();

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Bill);
        }

        /// <summary>Determines whether the specified bill is equal to the current bill.</summary>
        /// <param name="other">The bill to compare with the current bill.</param>
        /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
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

        /// <summary>Gets the hash code for this instance.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            int hashCode = -765739998;
            hashCode = hashCode * -1521134295 + Version.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<decimal?>.Default.GetHashCode(Amount);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Currency);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Account);
            hashCode = hashCode * -1521134295 + EqualityComparer<Address>.Default.GetHashCode(Creditor);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Reference);
            hashCode = hashCode * -1521134295 + EqualityComparer<Address>.Default.GetHashCode(Debtor);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(UnstructuredMessage);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(BillInformation);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<AlternativeScheme>>.Default.GetHashCode(AlternativeSchemes);
            hashCode = hashCode * -1521134295 + EqualityComparer<BillFormat>.Default.GetHashCode(Format);
            return hashCode;
        }

        private static bool SequenceEqual<T>(List<T> list1, List<T> list2)
        {
            if (list1 == list2)
            {
                return true;
            }

            if (list1 == null || list2 == null)
            {
                return false;
            }

            if (list1.Count != list2.Count)
            {
                return false;
            }

            return list1.SequenceEqual(list2);
        }
    }
}
