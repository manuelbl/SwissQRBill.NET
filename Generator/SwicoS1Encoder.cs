//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2020 Christian Bernasconi
// Copyright (c) 2020 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System;
using System.Collections.Generic;
using System.Text;

namespace Codecrete.SwissQRBill.Generator
{
    /// <summary>
    /// Encodes structured bill information according to Swico S1 syntax.
    /// <para>
    /// The encoded bill information can be used in a Swiss QR bill in the field <c>StrdBkgInf</c>.
    /// </para>
    /// <para>
    /// Also see http://swiss-qr-invoice.org/downloads/qr-bill-s1-syntax-de.pdf
    /// </para>
    /// </summary>
    internal static class SwicoS1Encoder
    {
        /// <summary>
        /// Encodes the specified bill information. 
        /// </summary>
        /// <param name="billInfo">The bill information.</param>
        /// <returns>The encoded bill information text.</returns>
        internal static string Encode(SwicoBillInformation billInfo)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("//S1");

            if (billInfo.InvoiceNumber != null)
            {
                sb.Append("/10/").Append(EscapedText(billInfo.InvoiceNumber));
            }
            if (billInfo.InvoiceDate != null)
            {
                sb.Append("/11/").Append(S1Date(billInfo.InvoiceDate.Value));
            }
            if (billInfo.CustomerReference != null)
            {
                sb.Append("/20/").Append(EscapedText(billInfo.CustomerReference));
            }
            if (billInfo.VatNumber != null)
            {
                sb.Append("/30/").Append(EscapedText(billInfo.VatNumber));
            }
            if (billInfo.VatDate != null)
            {
                sb.Append("/31/").Append(S1Date(billInfo.VatDate.Value));
            }
            else if (billInfo.VatStartDate != null && billInfo.VatEndDate != null)
            {
                sb.Append("/31/")
                    .Append(S1Date(billInfo.VatStartDate.Value))
                    .Append(S1Date(billInfo.VatEndDate.Value));
            }
            if (billInfo.VatRate != null)
            {
                sb.Append("/32/").Append(S1Number(billInfo.VatRate.Value));
            }
            else if (billInfo.VatRateDetails != null && billInfo.VatRateDetails.Count > 0)
            {
                sb.Append("/32/");
                AppendTupleList(sb, billInfo.VatRateDetails);
            }
            if (billInfo.VatImportTaxes != null && billInfo.VatImportTaxes.Count > 0)
            {
                sb.Append("/33/");
                AppendTupleList(sb, billInfo.VatImportTaxes);
            }
            if (billInfo.PaymentConditions != null && billInfo.PaymentConditions.Count > 0)
            {
                sb.Append("/40/");
                AppendTupleList(sb, billInfo.PaymentConditions);
            }

            return sb.Length > 4 ? sb.ToString() : null;
        }

        private static string EscapedText(string text)
        {
            return text.Replace(@"\", @"\\").Replace("/", @"\/");
        }

        private static string S1Date(DateTime date)
        {
            return date.ToString("yyMMdd");
        }

        private static string S1Number(decimal num)
        {
            return num.ToString("0.######");
        }

        private static void AppendTupleList(StringBuilder sb, List<(decimal, decimal)> list)
        {
            bool isFirst = true;
            foreach (var e in list)
            {
                if (!isFirst)
                {
                    sb.Append(";");
                }
                else
                {
                    isFirst = false;
                }
                sb.Append(S1Number(e.Item1)).Append(":").Append(S1Number(e.Item2));
            }
        }

        private static void AppendTupleList(StringBuilder sb, List<(decimal, int)> list)
        {
            bool isFirst = true;
            foreach (var e in list)
            {
                if (!isFirst)
                {
                    sb.Append(";");
                }
                else
                {
                    isFirst = false;
                }
                sb.Append(S1Number(e.Item1)).Append(":").Append(e.Item2);
            }
        }
    }
}
