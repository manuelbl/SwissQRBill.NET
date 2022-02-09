﻿//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2020 Christian Bernasconi
// Copyright (c) 2020 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System;
using System.Collections.Generic;
using System.Globalization;

namespace libQrCodeGenerator.SwissQRBill.Generator
{
    /// <summary>
    /// Decodes structured bill information according to Swico S1 syntax.
    /// <para>
    /// The encoded bill information can be found in a Swiss QR bill in th field <c>StrdBkgInf</c>.
    /// </para>
    /// <para>
    /// Also see http://swiss-qr-invoice.org/downloads/qr-bill-s1-syntax-de.pdf
    /// </para>
    /// </summary>
    internal static class SwicoS1Decoder
    {
        private const int InvoiceNumberTag = 10;

        private const int InvoiceDateTag = 11;

        private const int CustomerReferenceTag = 20;

        private const int VatNumberTag = 30;

        private const int VatDateTag = 31;

        private const int VatRateDetailsTag = 32;

        private const int VatImportTaxesTag = 33;

        private const int PaymentConditionsTag = 40;

        /// <summary>
        /// Decodes the specified text.
        /// <para>
        /// As much data as possible is decoded. Invalid data is silently ignored.
        /// </para>
        /// </summary>
        /// <param name="billInfoText">Encoded structured bill information.</param>
        /// <returns>The decoded bill information (or <c>null</c> if no valid Swico bill information is found).</returns>
        internal static SwicoBillInformation Decode(string billInfoText)
        {
            if (billInfoText == null || !billInfoText.StartsWith("//S1/"))
            {
                return null;
            }

            // Split text as slashes
            string[] parts = Split(billInfoText.Substring(5));

            // Create a list of tuples (tag, value)
            var tuples = new List<(int Tag, string Value)>();
            int len = parts.Length;
            for (int i = 0; i < len - 1; i += 2)
            {
                if (int.TryParse(parts[i], out int tag))
                {
                    tuples.Add((tag, parts[i + 1]));
                }
            }

            // Process the tuples and assign them to bill information
            var billInformation = new SwicoBillInformation();
            foreach (var (Tag, Value) in tuples)
            {
                DecodeElement(billInformation, Tag, Value);
            }

            return billInformation;
        }

        private static void DecodeElement(SwicoBillInformation billInformation, int tag, string value)
        {
            if (value.Length == 0)
            {
                return;
            }

            switch (tag)
            {
                case InvoiceNumberTag:
                    billInformation.InvoiceNumber = value;
                    break;
                case InvoiceDateTag:
                    billInformation.InvoiceDate = GetDateValue(value);
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
                case VatRateDetailsTag:
                    SetVatRateDetails(billInformation, value);
                    break;
                case VatImportTaxesTag:
                    billInformation.VatImportTaxes = ParseDetailList(value);
                    break;
                case PaymentConditionsTag:
                    SetPaymentConditions(billInformation, value);
                    break;
            }
        }

        private static void SetVatDates(SwicoBillInformation billInformation, string value)
        {
            if (value.Length != 6 && value.Length != 12)
            {
                return;
            }

            if (value.Length == 6)
            {
                // Single VAT date
                DateTime? date = GetDateValue(value);
                if (date != null)
                {
                    billInformation.VatDate = date;
                    billInformation.VatStartDate = null;
                    billInformation.VatEndDate = null;
                }
            }
            else
            {
                // VAT date range
                DateTime? startDate = GetDateValue(value.Substring(0, 6));
                DateTime? endDate = GetDateValue(value.Substring(6, 6));
                if (startDate != null && endDate != null)
                {
                    billInformation.VatStartDate = startDate;
                    billInformation.VatEndDate = endDate;
                    billInformation.VatDate = null;
                }
            }
        }

        private static void SetVatRateDetails(SwicoBillInformation billInformation, string value)
        {
            // Test for single VAT rate vs list of tuples
            if (!value.Contains(":") && !value.Contains(";"))
            {
                billInformation.VatRate = GetDecimalValue(value);
                billInformation.VatRateDetails = null;
            }
            else
            {
                billInformation.VatRateDetails = ParseDetailList(value);
                billInformation.VatRate = null;
            }
        }

        private static void SetPaymentConditions(SwicoBillInformation billInformation, string value)
        {
            // Split into tuples
            var tuples = value.Split(';');

            var list = new List<(decimal, int)>();
            foreach (var listEntry in tuples)
            {
                // Split into tuple (discount, days)
                var detail = listEntry.Split(':');
                if (detail.Length != 2)
                {
                    continue;
                }

                decimal? discount = GetDecimalValue(detail[0]);
                int? days = GetIntValue(detail[1]);
                if (discount != null && days != null)
                {
                    list.Add((discount.Value, days.Value));
                }
            }

            if (list.Count > 0)
            {
                billInformation.PaymentConditions = list;
            }
        }

        private static List<(decimal, decimal)> ParseDetailList(string text)
        {
            // Split into tuples
            var tuples = text.Split(';');

            var list = new List<(decimal, decimal)>();
            foreach (var vatEntry in tuples)
            {
                // Split into tuple (rate, amount)
                var vatDetails = vatEntry.Split(':');
                if (vatDetails.Length != 2)
                {
                    continue;
                }

                decimal? vatRate = GetDecimalValue(vatDetails[0]);
                decimal? vatAmount = GetDecimalValue(vatDetails[1]);
                if (vatRate != null && vatAmount != null)
                {
                    list.Add((vatRate.Value, vatAmount.Value));
                }
            }
            return list.Count > 0 ? list : null;
        }

        private static DateTime? GetDateValue(string dateText)
        {
            if (string.IsNullOrWhiteSpace(dateText))
            {
                return null;
            }

            if (dateText.Length == 6) // Consistent with specifications
            {
                // Validation with 
                if (DateTime.TryParseExact(
                    dateText,
                    "yyMMdd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var date))
                {
                    return date;
                }
            }
            else if (dateText.Length == 12) // Not consistent with specifications but seen in production (year, month, day, hour, minute, second)
            {
                // Validation with 
                if (DateTime.TryParseExact(
                    dateText,
                    "yyMMddHHmmss",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var date))
                {
                    return date.Date;
                }
            }
            else if (dateText.Length == 10) // Not consistent with specifications but seen in production (year, month, day, hour, minute)
            {
                // Validation with 
                if (DateTime.TryParseExact(
                    dateText,
                    "yyMMddHHmm",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var date))
                {
                    return date.Date;
                }
            }

            return null;
        }

        private static int? GetIntValue(string intText)
        {
            if (int.TryParse(intText, out var num))
            {
                return num;
            }
            return null;
        }

        private static decimal? GetDecimalValue(string decimalText)
        {
            if (decimal.TryParse(decimalText, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var num))
            {
                return num;
            }
            return null;
        }

        /// <summary>
        /// Splits the text at slash characters.
        /// </summary>
        /// <para>
        /// Additionally, the escaping with back slashes is undone.
        /// </para>
        /// <param name="text">The text to split.</param>
        /// <returns>An array of substrings</returns>
        private static string[] Split(string text)
        {
            // Use placeholders for escaped characters (outside of valid QR bill character set)
            // and undo back slash escaping.
            text = text.Replace(@"\\", "☁").Replace(@"\/", "★");

            // Split
            var parts = text.Split('/');

            // Fix placeholders
            for (int i = 0; i < parts.Length; i++)
            {
                parts[i] = parts[i].Replace('★', '/').Replace('☁', '\\');
            }

            return parts;
        }
    }
}
