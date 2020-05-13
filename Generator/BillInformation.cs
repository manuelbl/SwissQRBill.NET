using System;
using System.Collections.Generic;

namespace Codecrete.SwissQRBill.Generator
{
    /// <summary>
    /// Coded information for automated booking of the payment.
    /// </summary>
    public class BillInformation
    {
        public BillInformation()
        {
            VatDetails = new Dictionary<double, double>();
            VatImportTaxes = new Dictionary<double, double>();
            Conditions = new Dictionary<double, int>();
        }

        /// <summary>
        /// The invoice number.
        /// </summary>
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// The invoice date.
        /// </summary>
        public DateTime? InvoiceDate { get; set; }

        /// <summary>
        /// The customer reference.
        /// </summary>
        public string CustomerReference { get; set; }

        /// <summary>
        /// The UID number.
        /// <para>
        /// UID CHE-106.017.086 without the CHE prefix, separator and without MWST/TVA/IVA/VAT suffix.
        /// </para>
        /// </summary>
        public string VatNumber { get; set; }

        /// <summary>
        /// Date on which the service was provided.
        /// </summary>
        public DateTime? VatDate { get; set; }

        /// <summary>
        /// Start date of the service.
        /// <para>
        /// e.g. for a subscription.
        /// </para>
        /// </summary>
        public DateTime? VatStartDate { get; set; }

        /// <summary>
        /// End date of the service.
        /// <para>
        /// e.g. for a subscription.
        /// </para>
        /// </summary>
        public DateTime? VatEndDate { get; set; }

        /// <summary>
        /// Refers to the invoiced amount, excluding any discount.
        /// <para>
        /// Contains either:
        /// – a single percentage that is to be applied to the whole invoiced amount or
        /// – a list of the VAT amounts, defined by a percentage rate and a net amount
        /// </para>
        /// </summary>
        public Dictionary<double, double> VatDetails { get; }

        /// <summary>
        /// Where goods are imported, the import tax can be entered in this field.
        /// </summary>
        public Dictionary<double, double> VatImportTaxes { get; }

        /// <summary>
        /// The payment conditions.
        /// <para>
        /// Contains:
        /// – discount percentage
        /// – discount deadline (in days)
        /// </para>
        /// </summary>
        public Dictionary<double, int> Conditions { get; }

        /// <summary>
        /// The due date.
        /// <para>
        /// This field is calculated taking in consideration the invoice date and the information contained in Conditions field.
        /// </para>
        /// </summary>
        public DateTime? DueDate
        {
            get
            {
                // The invoice date has to be indicated and the condition with a percentage rate equal to zero defines the default payment date of the invoice.
                if (InvoiceDate == null || Conditions == null || !Conditions.ContainsKey(0))
                    return null;

                return InvoiceDate.Value.AddDays(Conditions[0]);
            }
        }

        /// <summary>
        /// Decodes the raw text of billing information and fills it into a <see cref="BillInformation"/> data structure.
        /// </summary>
        /// <param name="text">The raw text of billing information to be decoded.</param>
        /// <returns>The decoded billing information.</returns>
        /// <exception cref="QRBillInformationValidationException">If the text could not be decoded or is invalid.</exception>
        public static BillInformation DecodeBillInformationText(string text)
        {
            return BillInformationText.Decode(text);
        }

    }
}
