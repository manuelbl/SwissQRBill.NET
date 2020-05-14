//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2020 Christian Bernasconi
// Copyright (c) 2020 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Codecrete.SwissQRBill.Generator
{
    /// <summary>
    /// Decodes structured bill information according to Swico S1 syntax.
    /// <para>
    /// The encoded bill information can be found in a Swiss QR bill in th field <c>StrdBkgInf</c>.
    /// </para>
    /// <para>
    /// Also see see http://swiss-qr-invoice.org/downloads/qr-bill-s1-syntax-de.pdf
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
        /// Decodes the specified text 
        /// </summary>
        /// <param name="billInfoText">Encoded structured bill information.</param>
        /// <returns>The decoded bill information.</returns>
        /// <exception cref="SwicoDecodingException">The text is not valid Swico S1 syntax.</exception>
        internal static SwicoBillInformation Decode(string billInfoText)
        {
            if (billInfoText == null || !billInfoText.StartsWith("//S1/"))
            {
                throw new SwicoDecodingException("Bill information text does not start with \"//S1/\"");
            }

            string[] parts = Split(billInfoText.Substring(5));
            var elems = new List<(int Tag, string Value)>();

            int len = parts.Length;
            int lastTag = -1;
            for (int i = 0; i < len - 1; i += 2)
            {
                if (!ValidIntNumber.IsMatch(parts[i]) || !int.TryParse(parts[i], out int tag))
                {
                    throw new SwicoDecodingException($"Invalid tag /{parts[i]}/ in bill information");
                }

                if (tag <= lastTag)
                {
                    throw new SwicoDecodingException("Bill information: tags must appear in ascending order");
                }

                elems.Add((tag, parts[i + 1]));
                lastTag = tag;
            }

            // Odd number of parts is invalid
            if ((len & 1) != 0)
            {
                throw new SwicoDecodingException($"Bill information is truncated at tag /{parts[len - 1]}");
            }

            var billInformation = new SwicoBillInformation();
            foreach (var (Tag, Value) in elems)
            {
                DecodeElement(billInformation, Tag, Value);
            }

            return billInformation;
        }

        private static void DecodeElement(SwicoBillInformation billInformation, int tag, string value)
        {
            if (value.Length == 0)
                return;

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
                default:
                    throw new SwicoDecodingException($"Unknown tag /{tag}/ in bill information");
            }
        }

        private static void SetVatDates(SwicoBillInformation billInformation, string value)
        {
            if (value.Length != 6 && value.Length != 12)
            {
                throw new SwicoDecodingException($"Invalid VAT date(s) in bill information: {value}");
            }

            if (value.Length == 6)
            {
                billInformation.VatDate = GetDateValue(value);
            }
            else
            {
                billInformation.VatStartDate = GetDateValue(value.Substring(0, 6));
                billInformation.VatEndDate = GetDateValue(value.Substring(6, 6));
            }
        }

        private static void SetVatRateDetails(SwicoBillInformation billInformation, string value)
        {
            // Test for single VAT rate
            if (!value.Contains(":") && !value.Contains(";"))
            {
                billInformation.VatRate = GetDecimalValue(value);
                return;
            }

            billInformation.VatRateDetails = ParseDetailList(value);
        }

        private static void SetPaymentConditions(SwicoBillInformation billInformation, string value)
        {
            var entries = value.Split(';');

            var list = new List<(decimal, int)>();
            foreach (var listEntry in entries)
            {
                var detail = listEntry.Split(':');
                if (detail.Length != 2)
                {
                    throw new SwicoDecodingException($"Invalid discount / days tuple in bill information: {listEntry}");
                }

                decimal discount = GetDecimalValue(detail[0]);
                int days = GetIntValue(detail[1]);
                list.Add((discount, days));
            }

            billInformation.PaymentConditions = list;
        }

        private static List<(decimal, decimal)> ParseDetailList(string text)
        {
            var entries = text.Split(';');

            var list = new List<(decimal, decimal)>();
            foreach (var vatEntry in entries)
            {
                var vatDetails = vatEntry.Split(':');
                if (vatDetails.Length != 2)
                {
                    throw new SwicoDecodingException($"Invalid VAT rate / amount tuple in bill information: {vatEntry}");
                }

                decimal vatRate = GetDecimalValue(vatDetails[0]);
                decimal vatAmount = GetDecimalValue(vatDetails[1]);
                list.Add((vatRate, vatAmount));
            }
            return list;
        }

        private static DateTime GetDateValue(string dateText)
        {
            if (DateTime.TryParseExact(
                dateText,
                "yyMMdd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var date))
            {
                return date;
            }

            throw new SwicoDecodingException($"Invalid date value in bill information: {dateText}");
        }

        private static readonly Regex ValidIntNumber = new Regex(@"^[0-9]+$", RegexOptions.Compiled);

        private static int GetIntValue(string intText)
        {
            if (ValidIntNumber.IsMatch(intText) && int.TryParse(intText, out var num))
            {
                return num;
            }

            throw new SwicoDecodingException($"Invalid integer value in bill information: {intText}");
        }

        private static readonly Regex ValidDecimalNumber = new Regex(@"^[0-9]+(\.[0-9]+)?$", RegexOptions.Compiled);

        private static decimal GetDecimalValue(string decimalText)
        {
            if (ValidDecimalNumber.IsMatch(decimalText) && decimal.TryParse(decimalText, out var num))
            {
                return num;
            }

            throw new SwicoDecodingException($"Invalid numeric value in bill information: {decimalText}");
        }

        /// <summary>
        /// Splits the text at slash characters.
        /// </summary>
        /// <para>
        /// Additionally, the escaping with back slashes is undone.
        /// </para>
        /// <param name="text"></param>
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
