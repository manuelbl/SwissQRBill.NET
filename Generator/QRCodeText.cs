//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System.Collections.Generic;
using System.Globalization;
using System.Text;
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
        private StringBuilder _textBuilder;

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
            QRCodeText qrCodeText = new QRCodeText(bill);
            return qrCodeText.CreateText();
        }

        private string CreateText()
        {
            _textBuilder = new StringBuilder();

            // Header
            _textBuilder.Append("SPC\n"); // QRType
            _textBuilder.Append("0200\n"); // Version
            _textBuilder.Append("1"); // Coding

            // CdtrInf
            AppendDataField(_bill.Account); // IBAN
            AppendPerson(_bill.Creditor); // Cdtr
            _textBuilder.Append("\n\n\n\n\n\n\n"); // UltmtCdtr

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
            AppendDataField(_bill.BillInformation); // StrdBkgInf

            // AltPmtInf
            if (_bill.AlternativeSchemes != null && _bill.AlternativeSchemes.Count > 0)
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
                _textBuilder.Append("\n\n\n\n\n\n\n");
            }
        }

        private void AppendDataField(string value)
        {
            if (value == null)
            {
                value = "";
            }

            _textBuilder.Append('\n').Append(value);
        }


        private static string FormatAmountForCode(decimal amount)
        {
            return string.Format(AmountNumberInfo, "{0:#.00}", amount);
        }

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
        public static Bill Decode(string text)
        {
            string[] lines = SplitLines(text);
            if (lines.Length < 31 || lines.Length > 34)
            {
                // A line feed at the end is illegal (cf 4.2.3) but found in practice. Don't be too strict.
                if (!(lines.Length == 35 && lines[34].Length == 0))
                {
                    ThrowSingleValidationError(ValidationConstants.FieldQrType, ValidationConstants.KeyValidDataStructure);
                }
            }

            if ("SPC" != lines[0])
            {
                ThrowSingleValidationError(ValidationConstants.FieldQrType, ValidationConstants.KeyValidDataStructure);
            }

            if ("0200" != lines[1])
            {
                ThrowSingleValidationError(ValidationConstants.FieldVersion, ValidationConstants.KeySupportedVersion);
            }

            if ("1" != lines[2])
            {
                ThrowSingleValidationError(ValidationConstants.FieldCodingType, ValidationConstants.KeySupportedCodingType);
            }

            Bill billData = new Bill
            {
                Version = Bill.QrBillStandardVersion.V2_0,

                Account = lines[3],

                Creditor = DecodeAddress(lines, 4, false)
            };

            if (lines[18].Length > 0)
            {
                if (decimal.TryParse(lines[18], NumberStyles.Number, AmountNumberInfo, out decimal amount))
                {
                    billData.Amount = amount;
                }
                else
                {
                    ThrowSingleValidationError(ValidationConstants.FieldAmount, ValidationConstants.KeyValidNumber);
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
                ThrowSingleValidationError(ValidationConstants.FieldTrailer, ValidationConstants.KeyValidDataStructure);
            }

            billData.BillInformation = lines.Length > 31 ? lines[31] : "";

            List<AlternativeScheme> alternativeSchemes = null;
            int numSchemes = lines.Length - 32;
            // skip empty schemes at end (due to invalid trailing line feed)
            if (numSchemes > 0 && lines[32 + numSchemes - 1].Length == 0)
            {
                numSchemes--;
            }
            if (numSchemes > 0)
            {
                alternativeSchemes = new List<AlternativeScheme>();
                for (int i = 0; i < numSchemes; i++)
                {
                    AlternativeScheme scheme = new AlternativeScheme
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
        private static Address DecodeAddress(string[] lines, int startLine, bool isOptional)
        {

            bool isEmpty = lines[startLine].Length == 0 && lines[startLine + 1].Length == 0
                    && lines[startLine + 2].Length == 0 && lines[startLine + 3].Length == 0
                    && lines[startLine + 4].Length == 0 && lines[startLine + 5].Length == 0
                    && lines[startLine + 6].Length == 0;

            if (isEmpty && isOptional)
            {
                return null;
            }

            Address address = new Address();
            bool isStructuredAddress = "S" == lines[startLine];
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

        private static string[] SplitLines(string text)
        {
            List<string> lines = new List<string>(32);
            int lastPos = 0;
            while (true)
            {
                int pos = text.IndexOf('\n', lastPos);
                if (pos < 0)
                {
                    break;
                }

                int pos2 = pos;
                if (pos2 > lastPos && text[pos2 - 1] == '\r')
                {
                    pos2--;
                }

                lines.Add(text.Substring(lastPos, pos2 - lastPos));
                lastPos = pos + 1;
            }

            // add last line
            lines.Add(text.Substring(lastPos, text.Length - lastPos));
            return lines.ToArray();
        }

        private static void ThrowSingleValidationError(string field, string messageKey)
        {
            ValidationResult result = new ValidationResult();
            result.AddMessage(MessageType.Error, field, messageKey);
            throw new QRBillValidationException(result);
        }


        private static readonly NumberFormatInfo AmountNumberInfo = CreateAmountNumberInfo();

        private static NumberFormatInfo CreateAmountNumberInfo()
        {
            NumberFormatInfo numberInfo = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            numberInfo.NumberDecimalSeparator = ".";
            numberInfo.NumberGroupSeparator = "";
            return numberInfo;
        }
    }
}
