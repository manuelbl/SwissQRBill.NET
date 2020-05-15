//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2020 Christian Bernasconi
// Copyright (c) 2020 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System;
using System.Collections.Generic;
using System.Linq;

namespace Codecrete.SwissQRBill.Generator
{
    /// <summary>
    /// Bill information (according to Swico S1) for automated processing of invoices.
    /// <para>
    /// Swico S1 (see http://swiss-qr-invoice.org/downloads/qr-bill-s1-syntax-de.pdf) is one of the
    /// supported standards for adding structured billing information to a QR bill
    /// (in the field StrdBkgInf).
    /// </para>
    /// </summary>
    public class SwicoBillInformation : IEquatable<SwicoBillInformation>
    {
        /// <summary>
        /// Gets or sets the invoice number.
        /// </summary>
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// Gets or sets the invoice date.
        /// </summary>
        public DateTime? InvoiceDate { get; set; }

        /// <summary>
        /// Gets or sets the customer reference.
        /// </summary>
        public string CustomerReference { get; set; }

        /// <summary>
        /// Gets or sets the UID number.
        /// <para>
        /// The number without any prefix, white space, separator or suffix, i.e. 106017086 instead of "CHE-106.017.086 MWST".
        /// </para>
        /// </summary>
        public string VatNumber { get; set; }

        /// <summary>
        /// Gets or sets the date when the goods or service were supplied.
        /// </summary>
        /// <remarks>
        /// If this property is non-null, <see cref="VatStartDate"/> and <see cref="VatEndDate"/>
        /// must not be used.
        /// </remarks>
        public DateTime? VatDate { get; set; }

        /// <summary>
        /// Gets or sets the start date of the period when the service was supplied (e.g. a subscription).
        /// </summary>
        /// <remarks>
        /// If this property is non-null, <see cref="VatEndDate"/> must be set as well,
        /// and <see cref="VatDate"/> must not be used.
        /// </remarks>
        public DateTime? VatStartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date of the period when the service was supplied (e.g. a subscription).
        /// </summary>
        /// <remarks>
        /// If this property is non-null, <see cref="VatStartDate"/> must be set as well,
        /// and <see cref="VatDate"/> must not be used.
        /// </remarks>
        public DateTime? VatEndDate { get; set; }

        /// <summary>
        /// Gets or sets the VAT rate (in percent) in case the same rate applies to the entire invoice.
        /// <para>
        /// If different rates apply to invoice line items, this property is <c>null</c> and
        /// <see cref="VatRateDetails"/> is used instead.
        /// </para>
        /// </summary>
        public decimal? VatRate { get; set; }

        /// <summary>
        /// Gets or sets a list of VAT rates.
        /// <para>
        /// Each element in the list is a tuple of VAT rate (in percent) and amount (in QR bill currency).
        /// It indicates that the specified VAT rate applies to the specified net amount (partial amount) of the invoice.
        /// </para>
        /// <para>
        /// If a single VAT rate applies to the entire invoice, this list is <c>null</c> and
        /// <see cref="VatRate"/> is used instead.
        /// </para>
        /// </summary>
        /// <example>
        /// If the list contained (8, 1000), (2.5, 51.8), (7.7, 250) for an invoice in CHF, a VAT rate of 8% would
        /// apply to CHF 1000.00, 2.5% for CHF 51.80 and 7.7% for CHF 250.00.
        /// </example>
        public List<(decimal, decimal)> VatRateDetails { get; set; }

        /// <summary>
        /// Gets or sets a list of VAT import taxes.
        /// <para>
        /// Each element in the list is a tuple of VAT rate (in percent) and VAT amount (in QR bill currency).
        /// It indicates that the specified VAT rate was applied and resulted in the specified tax amount.
        /// </para>
        /// </summary>
        /// <example>
        /// If the list contained (7.7, 48.37), (2.5, 12.4) for an invoice in CHF, a VAT rate of 7.7% has
        /// been applied to a part of the items resulting in CHF 48.37 in tax and a rate of 2.5% has been
        /// applied to another part of the items resulting in CHF 12.40 in tax.
        /// </example>
        public List<(decimal, decimal)> VatImportTaxes { get; set; }

        /// <summary>
        /// Gets or sets the payment conditions.
        /// <para>
        /// Each element in the list is a tuple of a payment discount (in percent) and a deadliine
        /// (in days from the invoice date).
        /// </para>
        /// </summary>
        /// <example>
        /// If the list contained (2, 10), (0, 60), a discount of 2% aplies if the payment is made
        /// by 10 days after invoice data. The payment is due 60 days after invoice date.
        /// </example>
        public List<(decimal, int)> PaymentConditions { get; set; }

        /// <summary>
        /// Gets the due date.
        /// <para>
        /// The due date is calculated from the payment condition with a discount of 0.
        /// </para>
        /// </summary>
        public DateTime? DueDate
        {
            get
            {
                // The invoice date has to be indicated and the condition with a percentage rate equal to zero defines the default payment date of the invoice.
                if (InvoiceDate == null || PaymentConditions == null)
                    return null;

                foreach (var tuple in PaymentConditions)
                {
                    if (tuple.Item1 == 0)
                    {
                        return InvoiceDate.Value.AddDays(tuple.Item2);
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Encodes this bill information as a single text string suitable
        /// to be added to a Swiss QR bill.
        /// </summary>
        /// <returns>Encoded text</returns>
        public string EncodeAsText()
        {
            return SwicoS1Encoder.Encode(this);
        }

        /// <summary>
        /// Decodes the text of structured billing information and creates a <see cref="SwicoBillInformation"/> instance.
        /// </summary>
        /// <param name="text">The structured billing information encoded according to Swico S1 syntax.</param>
        /// <returns>The decoded billing information.</returns>
        public static SwicoBillInformation DecodeText(string text)
        {
            return SwicoS1Decoder.Decode(text);
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as SwicoBillInformation);
        }

        /// <summary>Determines whether the specified bill information is equal to the current bill information.</summary>
        /// <param name="other">The bill information to compare with the current bill information.</param>
        /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
        public bool Equals(SwicoBillInformation other)
        {
            return other != null &&
                   InvoiceNumber == other.InvoiceNumber &&
                   InvoiceDate == other.InvoiceDate &&
                   CustomerReference == other.CustomerReference &&
                   VatNumber == other.VatNumber &&
                   VatDate == other.VatDate &&
                   VatStartDate == other.VatStartDate &&
                   VatEndDate == other.VatEndDate &&
                   VatRate == other.VatRate &&
                   SequenceEqual(VatRateDetails, other.VatRateDetails) &&
                   SequenceEqual(VatImportTaxes, other.VatImportTaxes) &&
                   SequenceEqual(PaymentConditions, other.PaymentConditions);
        }

        /// <summary>Gets the hash code for this instance.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            int hashCode = 1731431530;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(InvoiceNumber);
            hashCode = hashCode * -1521134295 + InvoiceDate.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CustomerReference);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(VatNumber);
            hashCode = hashCode * -1521134295 + VatDate.GetHashCode();
            hashCode = hashCode * -1521134295 + VatStartDate.GetHashCode();
            hashCode = hashCode * -1521134295 + VatEndDate.GetHashCode();
            hashCode = hashCode * -1521134295 + VatRate.GetHashCode();
            hashCode = hashCode * -1521134295 + SequenceHashCode(VatRateDetails);
            hashCode = hashCode * -1521134295 + SequenceHashCode(VatImportTaxes);
            hashCode = hashCode * -1521134295 + SequenceHashCode(PaymentConditions);
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

        private static int SequenceHashCode<T>(List<T> list)
        {
            if (list == null)
            {
                return 0;
            }

            int hashCode = 1731431530;
            foreach (T elem in list)
            {
                hashCode = hashCode * -1521134295 + elem.GetHashCode();
            }
            return hashCode;
        }
    }
}
