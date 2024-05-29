//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using static Codecrete.SwissQRBill.Generator.Address;
using static Codecrete.SwissQRBill.Generator.ValidationMessage;

namespace Codecrete.SwissQRBill.Generator
{
    /// <summary>
    /// Internal class for encoding and decoding the text embedded in the QR code.
    /// </summary>
    internal class QRCodeText
    {
        private readonly Bill _bill;
        private StringWriter _textBuilder;

        private QRCodeText(Bill bill)
        {
            _bill = bill;
        }

        /// <summary>
        /// Gets the text embedded in the QR code (according to the data structure defined by SIX).
        /// </summary>
        /// <param name="bill">The bill data.</param>
        /// <returns>The QR code text.</returns>
        public static string Create(Bill bill)
        {
            var qrCodeText = new QRCodeText(bill);
            return qrCodeText.CreateText();
        }

        private string CreateText()
        {
            _textBuilder = new StringWriter { NewLine = _bill.Separator == Bill.QrDataSeparator.Lf ? "\n" : "\r\n" };

            // Header
            _textBuilder.WriteLine("SPC"); // QRType
            _textBuilder.WriteLine("0200"); // Version
            _textBuilder.Write("1"); // Coding

            // CdtrInf
            AppendDataField(_bill.Account); // IBAN
            AppendPerson(_bill.Creditor); // Cdtr
            AppendPerson(null); // UltmtCdtr

            // CcyAmt
            AppendDataField(_bill.Amount == null ? "" : FormatAmountForCode(_bill.Amount.Value)); // Amt
            AppendDataField(_bill.Currency); // Ccy

            // UltmtDbtr
            AppendPerson(_bill.Debtor);

            // RmtInf
            AppendDataField(_bill.ReferenceType); // Tp
            AppendDataField(_bill.Reference); // Ref

            // AddInf
            AppendDataField(_bill.UnstructuredMessage); // Unstrd
            AppendDataField("EPD"); // Trailer
            var hasAlternativeSchemes = _bill.AlternativeSchemes != null && _bill.AlternativeSchemes.Count > 0;
            if (hasAlternativeSchemes || _bill.BillInformation != null)
            {
                AppendDataField(_bill.BillInformation); // StrdBkgInf
            }

            // AltPmtInf
            if (hasAlternativeSchemes)
            {
                AppendDataField(_bill.AlternativeSchemes[0].Instruction); // AltPmt
                if (_bill.AlternativeSchemes.Count > 1)
                {
                    AppendDataField(_bill.AlternativeSchemes[1].Instruction); // AltPmt
                }
            }

            return _textBuilder.ToString();
        }

        private void AppendPerson(Address address)
        {
            if (address != null)
            {
                AppendDataField(address.Type == AddressType.Structured ? "S" : "K"); // AdrTp
                AppendDataField(address.Name); // Name
                AppendDataField(address.Type == AddressType.Structured
                        ? address.Street : address.AddressLine1); // StrtNmOrAdrLine1
                AppendDataField(address.Type == AddressType.Structured
                        ? address.HouseNo : address.AddressLine2); // StrtNmOrAdrLine2
                AppendDataField(address.PostalCode); // PstCd
                AppendDataField(address.Town); // TwnNm
                AppendDataField(address.CountryCode); // Ctry
            }
            else
            {
                AppendDataField(null);
                AppendDataField(null);
                AppendDataField(null);
                AppendDataField(null);
                AppendDataField(null);
                AppendDataField(null);
                AppendDataField(null);
            }
        }

        private void AppendDataField(string value)
        {
            _textBuilder.WriteLine();
            _textBuilder.Write(value);
        }


        private static string FormatAmountForCode(decimal amount)
        {
            return string.Format(AmountNumberInfo, "{0:0.00}", amount);
        }

        private static readonly Regex ValidVersion = new Regex(@"^02\d\d$", RegexOptions.Compiled);

        /// <summary>
        /// Decodes the specified text and returns the bill data.
        /// <para>
        /// The text is assumed to be in the data structured for the QR code defined by SIX.
        /// </para>
        /// <para>
        /// The returned data is only minimally validated. The format and the header are
        /// checked. Amount and date must be parsable.
        /// </para>
        /// </summary>
        /// <param name="text">The text to decode.</param>
        /// <returns>The decoded bill data.</returns>
        /// <exception cref="QRBillValidationException">The text is in an invalid format.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1066:Mergeable \"if\" statements should be combined", Justification = "Easier to read the way it is")]
        public static Bill Decode(string text)
        {
            var lines = SplitLines(text);
            if (lines.Count < 31 || lines.Count > 34)
            {
                // A line feed at the end is illegal (cf 4.2.3) but found in practice. Don't be too strict.
                if (!(lines.Count == 35 && lines[34].Length == 0))
                {
                    ThrowSingleValidationError(ValidationConstants.FieldQrType, ValidationConstants.KeyDataStructureInvalid);
                }
            }

            if ("SPC" != lines[0])
            {
                ThrowSingleValidationError(ValidationConstants.FieldQrType, ValidationConstants.KeyDataStructureInvalid);
            }

            if (!ValidVersion.IsMatch(lines[1]))
            {
                ThrowSingleValidationError(ValidationConstants.FieldVersion, ValidationConstants.KeyVersionUnsupported);
            }

            if ("1" != lines[2])
            {
                ThrowSingleValidationError(ValidationConstants.FieldCodingType, ValidationConstants.KeyCodingTypeUnsupported);
            }

            var billData = new Bill
            {
                Version = Bill.QrBillStandardVersion.V2_0,

                Separator = text.Contains("\r\n") ? Bill.QrDataSeparator.CrLf : Bill.QrDataSeparator.Lf,

                Account = lines[3],

                Creditor = DecodeAddress(lines, 4, false)
            };

            if (lines[18].Length > 0)
            {
                if (decimal.TryParse(lines[18], NumberStyles.Number, AmountNumberInfo, out var amount))
                {
                    billData.Amount = amount;
                }
                else
                {
                    ThrowSingleValidationError(ValidationConstants.FieldAmount, ValidationConstants.KeyNumberInvalid);
                }
            }
            else
            {
                billData.Amount = null;
            }

            billData.Currency = lines[19];

            billData.Debtor = DecodeAddress(lines, 20, true);

            // Set reference type and reference in reverse order
            // to retain reference type (as it is updated by setting 'Reference')
            billData.Reference = lines[28];
            billData.ReferenceType = lines[27];
            billData.UnstructuredMessage = lines[29];
            if ("EPD" != lines[30])
            {
                ThrowSingleValidationError(ValidationConstants.FieldTrailer, ValidationConstants.KeyDataStructureInvalid);
            }

            billData.BillInformation = lines.Count > 31 ? lines[31] : "";

            List<AlternativeScheme> alternativeSchemes = null;
            var numSchemes = lines.Count - 32;
            // skip empty schemes at end (due to invalid trailing line feed)
            if (numSchemes > 0 && lines[32 + numSchemes - 1].Length == 0)
            {
                numSchemes--;
            }
            if (numSchemes > 0)
            {
                alternativeSchemes = new List<AlternativeScheme>();
                for (var i = 0; i < numSchemes; i++)
                {
                    var scheme = new AlternativeScheme
                    {
                        Instruction = lines[32 + i]
                    };
                    alternativeSchemes.Add(scheme);
                }
            }
            billData.AlternativeSchemes = alternativeSchemes;

            return billData;
        }

        /// <summary>
        /// Process seven lines and extract and address.
        /// </summary>
        /// <param name="lines">The line array.</param>
        /// <param name="startLine">The index of first line to process.</param>
        /// <param name="isOptional">The flag indicating if the address is optional.</param>
        /// <returns>The decoded address or <c>null</c> if the address is optional and empty.</returns>
        private static Address DecodeAddress(IReadOnlyList<string> lines, int startLine, bool isOptional)
        {

            var isEmpty = lines[startLine].Length == 0 && lines[startLine + 1].Length == 0
                    && lines[startLine + 2].Length == 0 && lines[startLine + 3].Length == 0
                    && lines[startLine + 4].Length == 0 && lines[startLine + 5].Length == 0
                    && lines[startLine + 6].Length == 0;

            if (isEmpty && isOptional)
            {
                return null;
            }

            var address = new Address();
            var isStructuredAddress = "S" == lines[startLine];
            address.Name = lines[startLine + 1];
            if (isStructuredAddress)
            {
                address.Street = lines[startLine + 2];
                address.HouseNo = lines[startLine + 3];
            }
            else
            {
                address.AddressLine1 = lines[startLine + 2];
                address.AddressLine2 = lines[startLine + 3];
            }
            if (lines[startLine + 4].Length > 0)
            {
                address.PostalCode = lines[startLine + 4];
            }

            if (lines[startLine + 5].Length > 0)
            {
                address.Town = lines[startLine + 5];
            }

            address.CountryCode = lines[startLine + 6];
            return address;
        }

        private static IReadOnlyList<string> SplitLines(string text)
        {
            var lines = new List<string>(32);
            using (var reader = new StringReader(text))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }

            // If the last line ends with a newline character, it is consumed by the last line. That's why an empty string is manually added in this case.
            // See https://github.com/dotnet/runtime/issues/27715 and https://docs.microsoft.com/en-us/dotnet/api/system.io.stringreader.readline#remarks
            if (text.EndsWith("\n", StringComparison.Ordinal) || text.EndsWith("\r", StringComparison.Ordinal))
            {
                lines.Add("");
            }

            return lines;
        }

        private static void ThrowSingleValidationError(string field, string messageKey)
        {
            var result = new ValidationResult();
            result.AddMessage(MessageType.Error, field, messageKey);
            throw new QRBillValidationException(result);
        }


        private static readonly NumberFormatInfo AmountNumberInfo = CreateAmountNumberInfo();

        private static NumberFormatInfo CreateAmountNumberInfo()
        {
            var numberInfo = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            numberInfo.NumberDecimalSeparator = ".";
            numberInfo.NumberGroupSeparator = "";
            return numberInfo;
        }
    }
}
