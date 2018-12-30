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
    public class QRCodeText
    {
        private Bill bill;
        private StringBuilder textBuilder;

        private QRCodeText(Bill bill)
        {
            this.bill = bill;
        }

        /// <summary>
        /// Gets the text embedded in the QR code (according to the data structure defined by SIX)
        /// </summary>
        /// <param name="bill">bill data</param>
        /// <returns>QR code text</returns>
        public static string Create(Bill bill)
        {
            QRCodeText qrCodeText = new QRCodeText(bill);
            return qrCodeText.CreateText();
        }

        private string CreateText()
        {
            textBuilder = new StringBuilder();

            // Header
            textBuilder.Append("SPC\n"); // QRType
            textBuilder.Append("0200\n"); // Version
            textBuilder.Append("1"); // Coding

            // CdtrInf
            AppendDataField(bill.Account); // IBAN
            AppendPerson(bill.Creditor); // Cdtr
            textBuilder.Append("\n\n\n\n\n\n\n"); // UltmtCdtr

            // CcyAmt
            AppendDataField(bill.Amount == null ? "" : FormatAmountForCode(bill.Amount.Value)); // Amt
            AppendDataField(bill.Currency); // Ccy

            // UltmtDbtr
            AppendPerson(bill.Debtor);

            // RmtInf
            string referenceType = "NON";
            if (bill.Reference != null)
            {
                if (bill.Reference.StartsWith("RF"))
                {
                    referenceType = "SCOR";
                }
                else if (bill.Reference.Length > 0)
                {
                    referenceType = "QRR";
                }
            }
            AppendDataField(referenceType); // Tp
            AppendDataField(bill.Reference); // Ref

            // AddInf
            AppendDataField(bill.UnstructuredMessage); // Unstrd
            AppendDataField("EPD"); // Trailer
            AppendDataField(bill.BillInformation); // StrdBkgInf

            // AltPmtInf
            if (bill.AlternativeSchemes != null && bill.AlternativeSchemes.Count > 0)
            {
                AppendDataField(bill.AlternativeSchemes[0].Instruction); // AltPmt
                if (bill.AlternativeSchemes.Count > 1)
                {
                    AppendDataField(bill.AlternativeSchemes[1].Instruction); // AltPmt
                }
            }

            return textBuilder.ToString();
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
                textBuilder.Append("\n\n\n\n\n\n\n");
            }
        }

        private void AppendDataField(string value)
        {
            if (value == null)
            {
                value = "";
            }

            textBuilder.Append('\n').Append(value);
        }


        private static string FormatAmountForCode(decimal amount)
        {
            return amount.ToString("n", AmountNumberInfo); ;
        }

        /// <summary>
        /// Decodes the specified text and returns the bill data.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The Text is assumed to be in the data structured for the QR code defined by SIX.
        /// </para>
        /// <para>
        /// The returned data is only minimally validated. The format and the header are
        /// checked. Amount and date must be parsable.
        /// </para>
        /// </remarks>
        /// <param name="text">the text to decode</param>
        /// <returns>bill data</returns>
        /// <exception cref="QRBillValidationError">Trhown if a validationn error occurs</exception>
        public static Bill Decode(string text)
        {
            string[] lines = SplitLines(text);
            if (lines.Length < 32 || lines.Length > 34)
            {
                ThrowSingleValidationError(Bill.FieldQRType, QRBill.KeyValidDataStructure);
            }

            if ("SPC" != lines[0])
            {
                ThrowSingleValidationError(Bill.FieldQRType, QRBill.KeyValidDataStructure);
            }

            if ("0200" != lines[1])
            {
                ThrowSingleValidationError(Bill.FieldVersion, QRBill.KeySupportedVersion);
            }

            if ("1" != lines[2])
            {
                ThrowSingleValidationError(Bill.FieldCodingType, QRBill.KeySupportedCodingType);
            }

            Bill bill = new Bill
            {
                Version = Bill.StandardVersion.V2_0,

                Account = lines[3],

                Creditor = DecodeAddress(lines, 4, false)
            };

            if (lines[18].Length > 0)
            {
                if (decimal.TryParse(lines[18], NumberStyles.Number, AmountNumberInfo, out decimal amount))
                {
                    bill.Amount = amount;
                }
                else
                {
                    ThrowSingleValidationError(Bill.FieldAmount, QRBill.KeyValidNumber);
                }
            }
            else
            {
                bill.Amount = null;
            }

            bill.Currency = lines[19];

            bill.Debtor = DecodeAddress(lines, 20, true);

            // reference type is ignored (line 27)
            bill.Reference = lines[28];
            bill.UnstructuredMessage = lines[29];
            if ("EPD" != lines[30])
            {
                ThrowSingleValidationError(Bill.FieldTrailer, QRBill.KeyValidDataStructure);
            }

            bill.BillInformation = lines[31];

            List<AlternativeScheme> alternativeSchemes = null;
            int numSchemes = lines.Length - 32;
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
            bill.AlternativeSchemes = alternativeSchemes;

            return bill;
        }

        /// <summary>
        /// Process seven lines and extract and address.
        /// </summary>
        /// <param name="lines">line array</param>
        /// <param name="startLine">index of first line to process</param>
        /// <param name="isOptional">indicates if address is optional</param>
        /// <returns>decoded address or <c>null</c> if address is optional and empty</returns>
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
            throw new QRBillValidationError(result);
        }


        private static NumberFormatInfo AmountNumberInfo;

        static QRCodeText()
        {
            AmountNumberInfo = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            AmountNumberInfo.NumberDecimalDigits = 2;
            AmountNumberInfo.NumberDecimalSeparator = ".";
            AmountNumberInfo.NumberGroupSeparator = "";
        }
    }
}
