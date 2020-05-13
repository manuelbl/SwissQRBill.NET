using System;
using System.Globalization;

namespace Codecrete.SwissQRBill.Generator
{
    /// <summary>
    /// Internal class for decoding the billing information.
    /// </summary>
    internal class BillInformationText
    {
        private const string Separator = "//";

        private const char VatDetailsPairSeparator = ':';

        private const char VatDetailEntriesSeparator = ';';

        private const char ConditionPairSeparator = ':';

        private const char ConditionEntriesSeparator = ';';

        private const char TagSeparator = '/';

        private const string ValuesSeparator = "/";

        private const string PrefixTag = "S1";

        private const string InvoiceNumberTag = "10";

        private const string InvoiceDateTag = "11";

        private const string CustomerReferenceTag = "20";

        private const string VatNumberTag = "30";

        private const string VatDateTag = "31";

        private const string VatDetailsTag = "32";

        private const string VatImportTaxTag = "33";

        private const string ConditionsTag = "40";

        private const string CharactersToReplace = @"\/";

        private const string ReplacedCharacters = "||";

        /// <summary>
        /// Decodes the specified text and returns the billing information data.
        /// <para>
        /// The text is assumed to be in the Syntax definition for the Billing Information of Swico.
        /// Reference: https://www.paymentstandards.ch/dam/downloads/ig-qr-bill-en.pdf
        /// </para>
        /// </summary>
        /// <param name="text">The text containing the billing information.</param>
        /// <returns>The decoded bill information.</returns>
        /// <exception cref="QRBillInformationValidationException">The text is in an invalid format.</exception>
        public static BillInformation Decode(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            if (!text.StartsWith($"{Separator}{PrefixTag}",
                StringComparison.InvariantCultureIgnoreCase))
                throw new QRBillInformationValidationException($"Raw text has to start with {Separator}{PrefixTag}");

            text = SanitizeRawString(text);

            var billInformation = new BillInformation();

            var entries = text.Split(TagSeparator);

            for (var i = 0; i < entries.Length; i += 2)
            {
                if (entries.Length <= i + 1)
                    break;

                var key = entries[i];
                var value = entries[i + 1].Replace(ReplacedCharacters, "/");

                switch (key)
                {
                    case InvoiceNumberTag:
                        billInformation.InvoiceNumber = value;
                        break;
                    case InvoiceDateTag:
                        SetInvoiceDate(billInformation, value);
                        break;
                    case CustomerReferenceTag:
                        billInformation.CustomerReference = value;
                        break;
                    case VatNumberTag:
                        billInformation.VatNumber = value;
                        break;
                    case VatDateTag:
                        SetVatDates(billInformation, value);
                        break;
                    case VatDetailsTag:
                        SetVatDetails(billInformation, value);
                        break;
                    case VatImportTaxTag:
                        SetVatImportTax(billInformation, value);
                        break;
                    case ConditionsTag:
                        SetConditions(billInformation, value);
                        break;
                }
            }

            return billInformation;
        }

        private static void SetInvoiceDate(BillInformation billInformation, string value)
        {
            if (value.Length != 6)
                return;

            var invoiceDate = GetDateValue(value);

            if (invoiceDate.HasValue)
                billInformation.InvoiceDate = invoiceDate;
        }

        private static void SetVatDates(BillInformation billInformation, string value)
        {
            if (string.IsNullOrEmpty(value) || (value.Length != 6 && value.Length != 12))
                return;

            if (value.Length == 6)
            {
                var vatDate = GetDateValue(value);
                billInformation.VatDate = vatDate;
            }
            else
            {
                var vatStartDate = GetDateValue(value.Substring(0, 6));
                var vatSEndDate = GetDateValue(value.Substring(6, 6));
                billInformation.VatStartDate = vatStartDate;
                billInformation.VatEndDate = vatSEndDate;
            }
        }

        private static DateTime? GetDateValue(string dateText)
        {
            if (!string.IsNullOrEmpty(dateText) &&
                dateText.Length == 6 &&
                DateTime.TryParseExact(
                    dateText,
                    "yyMMdd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var date))
            {
                return date;
            }

            return null;
        }

        private static void SetVatDetails(BillInformation billInformation, string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            var vatEntries = value.Split(VatDetailEntriesSeparator);

            if (vatEntries.Length <= 0)
                return;

            foreach (var vatEntry in vatEntries)
            {
                var vatDetails = vatEntry.Split(VatDetailsPairSeparator);

                billInformation.VatDetails.Add(
                    Convert.ToDouble(vatDetails[0], CultureInfo.InvariantCulture),
                    vatDetails.Length == 2
                        ? Convert.ToDouble(vatDetails[1], CultureInfo.InvariantCulture)
                        : double.NaN);
            }
        }

        private static void SetVatImportTax(BillInformation billInformation, string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            var vatEntries = value.Split(VatDetailEntriesSeparator);

            if (vatEntries.Length <= 0)
                return;

            foreach (var vatEntry in vatEntries)
            {
                var vatDetails = vatEntry.Split(VatDetailsPairSeparator);

                billInformation.VatImportTaxes.Add(
                    Convert.ToDouble(vatDetails[0], CultureInfo.InvariantCulture),
                    vatDetails.Length == 2
                        ? Convert.ToDouble(vatDetails[1], CultureInfo.InvariantCulture)
                        : double.NaN);
            }
        }

        private static void SetConditions(BillInformation billInformation, string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            var conditionEntries = value.Split(ConditionEntriesSeparator);

            if (conditionEntries.Length <= 0)
                return;

            foreach (var conditionEntry in conditionEntries)
            {
                var conditionDetail = conditionEntry.Split(ConditionPairSeparator);

                if (conditionDetail.Length == 2)
                {
                    billInformation.Conditions.Add(
                        Convert.ToDouble(conditionDetail[0], CultureInfo.InvariantCulture),
                        Convert.ToInt16(conditionDetail[1], CultureInfo.InvariantCulture));
                }
            }
        }

        private static string SanitizeRawString(string rawBillingInformation)
        {
            rawBillingInformation = rawBillingInformation.Substring(
                rawBillingInformation.IndexOf(PrefixTag, StringComparison.InvariantCultureIgnoreCase) +
                PrefixTag.Length +
                ValuesSeparator.Length);
            rawBillingInformation = rawBillingInformation.Replace(CharactersToReplace, ReplacedCharacters);

            return rawBillingInformation;
        }
    }
}
