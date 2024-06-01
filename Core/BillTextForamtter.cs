//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2023 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System.Globalization;
using System.Text;
using static Codecrete.SwissQRBill.Generator.Address;

namespace Codecrete.SwissQRBill.Generator
{
    /// <summary>
    /// Formats text on a QR bill.
    /// <para>
    /// The resulting text often contains multiple lines, e.g. for addresses.These line breaks a represented
    /// by a line feed character(U+000A). Long lines might require additional line breaks to fit into the
    /// given text boxes.These additional line breaks are not included in the resulting text.
    /// </para>
    /// </summary>
    public class BillTextFormatter
    {
        private readonly Bill bill;

        /// <summary>
        /// Creates a new instance for the specified bill.
        /// </summary>
        /// <param name="bill">QR bill data</param>
        /// <exception cref="QRBillValidationException">thrown if the bill cannot be validated without errors</exception>
        public BillTextFormatter(Bill bill) : this(bill, false) { }

        /// <summary>
        /// Creates a new instance for the specified bill.
        /// </summary>
        /// <param name="bill">QR bill data</param>
        /// <param name="isValidated">indicates if the bill has already been validated and cleaned</param>
        internal BillTextFormatter(Bill bill, bool isValidated)
        {
            if (!isValidated)
            {
                var result = Validator.Validate(bill);
                this.bill = result.CleanedBill;
            }
            else
            {
                this.bill = bill;
            }
        }

        /// <summary>
        /// Gets the "payable to" text (account number and creditor address).
        /// </summary>
        /// <value>"payable to" text</value>
        public string PayableTo
        {
            get
            {
                return Account + "\n" + CreditorAddress;
            }
        }

        /// <summary>
        /// Gets the "payable to" text (account number and creditor address) with a reduced address.
        /// <para>
        /// If space is very tight, an address without street and house number can be used.
        /// </para>
        /// <value>"payable to" text</value>
        /// </summary>
        public string PayableToReduced
        {
            get
            {
                return Account + "\n" + CreditorAddressReduced;
            }
        }

        /// <summary>
        /// Gets the formatted account number.
        /// </summary>
        /// <value>account number</value>
        public string Account
        {
            get
            {
                return Payments.FormatIban(bill.Account);
            }
        }

        /// <summary>
        /// Gets the formatted creditor address.
        /// </summary>
        /// <value>creditor address</value>
        public string CreditorAddress
        {
            get
            {
                return FormatAddressForDisplay(bill.Creditor, IsCreditorWithCountryCode());
            }
        }

        /// <summary>
        /// Gets the reduced formatted creditor address.
        /// <para>
        /// If space is very tight, a reduced address without street and house number can be used.
        /// </para>
        /// </summary>
        /// <value>formatted address</value>
        public string CreditorAddressReduced
        {
            get
            {
                return FormatAddressForDisplay(CreateReducedAddress(bill.Creditor), IsCreditorWithCountryCode());
            }
        }

        /// <summary>
        /// Gets the formatted reference number.
        /// </summary>
        /// <value>reference number</value>
        public string Reference
        {
            get
            {
                return FormatReferenceNumber(bill.Reference);
            }
        }

        /// <summary>
        /// Gets the formatted amount.
        /// <para>
        /// Returns <c>null</c> if no amount has been set.
        /// </para>
        /// </summary>
        /// <value>formatted amount</value>
        public string Amount
        {
            get
            {
                if (bill.Amount == null)
                    return null;
                return FormatAmountForDisplay(bill.Amount.Value);
            }
        }

        /// <summary>
        /// Gets the "payable by" text (debtor address).
        /// <para>Returns <c>null</c> if no debtor has been set.</para>
        /// </summary>
        /// <value>formatted address</value>
        public string PayableBy
        {
            get
            {
                if (bill.Debtor == null)
                    return null;
                return FormatAddressForDisplay(bill.Debtor, IsDebtorWithCountryCode());
            }

        }

        /// <summary>
        /// Gets the "payable by" text with a reduced address.
        /// <para>
        /// If space is very tight, a reduced address without street and house number can be used.
        /// </para>
        /// <para>
        /// Returns <c>null</c> if no debtor has been set.
        /// </para>
        /// </summary>
        /// <value>formatted address</value>
        public string PayableByReduced
        {
            get
            {
                if (bill.Debtor == null)
                    return null;
                return FormatAddressForDisplay(CreateReducedAddress(bill.Debtor), IsDebtorWithCountryCode());
            }

        }

        /// <summary>
        /// Returns the additional information.
        /// <para>
        /// It consists of the unstructured message, the bill information, both or none,
        /// depending on what has been specified. If neither has been specified, <c>null</c> is returned.
        /// </para>
        /// </summary>
        /// <value>additional information</value>
        public string AdditionalInformation
        {
            get
            {
                var info = bill.UnstructuredMessage;
                if (bill.BillInformation != null)
                {
                    if (info == null)
                    {
                        info = bill.BillInformation;
                    }
                    else
                    {
                        info = info + "\n" + bill.BillInformation;
                    }
                }
                return info;
            }
        }

        private static string FormatAmountForDisplay(decimal amount)
        {

            return amount.ToString("N", AmountNumberInfo);
        }

        private static string FormatAddressForDisplay(Address address, bool withCountryCode)
        {
            var sb = new StringBuilder();
            sb.Append(address.Name);

            if (address.Type == AddressType.Structured)
            {
                var street = address.Street;
                if (street != null)
                {
                    sb.Append("\n");
                    sb.Append(street);
                }
                var houseNo = address.HouseNo;
                if (houseNo != null)
                {
                    sb.Append(street != null ? " " : "\n");
                    sb.Append(houseNo);
                }
                sb.Append("\n");
                if (withCountryCode)
                {
                    sb.Append(address.CountryCode);
                    sb.Append(" – ");
                }
                sb.Append(address.PostalCode);
                sb.Append(" ");
                sb.Append(address.Town);

            }
#pragma warning disable CS0618 // Type or member is obsolete
            else if (address.Type == AddressType.CombinedElements)
            {
                if (address.AddressLine1 != null)
                {
                    sb.Append("\n");
                    sb.Append(address.AddressLine1);
                }
                sb.Append("\n");
                if (withCountryCode)
                {
                    sb.Append(address.CountryCode);
                    sb.Append(" – ");
                }
                sb.Append(address.AddressLine2);
            }
#pragma warning restore CS0618 // Type or member is obsolete
            return sb.ToString();
        }

        private static Address CreateReducedAddress(Address address)
        {
            var reducedAddress = new Address
            {
                Name = address.Name,
                CountryCode = address.CountryCode
            };

            switch (address.Type)
            {
                case AddressType.Structured:
                    reducedAddress.PostalCode = address.PostalCode;
                    reducedAddress.Town = address.Town;
                    break;
#pragma warning disable CS0618 // Type or member is obsolete
                case AddressType.CombinedElements:
                    reducedAddress.AddressLine2 = address.AddressLine2;
                    break;
#pragma warning restore CS0618 // Type or member is obsolete
            }

            return reducedAddress;
        }

        private static string FormatReferenceNumber(string refNo)
        {
            if (refNo == null)
            {
                return null;
            }

            refNo = refNo.Trim();
            var len = refNo.Length;
            if (len == 0)
            {
                return null;
            }

            return refNo.StartsWith("RF") ? Payments.FormatIban(refNo) : Payments.FormatQrReferenceNumber(refNo);
        }

        private static readonly NumberFormatInfo AmountNumberInfo = CreateAmountNumberInfo();

        private static NumberFormatInfo CreateAmountNumberInfo()
        {
            var numberInfo = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            numberInfo.NumberDecimalDigits = 2;
            numberInfo.NumberDecimalSeparator = ".";
            numberInfo.NumberGroupSeparator = " ";
            return numberInfo;
        }

        private bool IsCreditorWithCountryCode()
        {
            // The creditor country code is even shown for a Swiss address if the debtor lives abroad
            return IsForeignAddress(bill.Creditor, bill.Format) || IsForeignAddress(bill.Debtor, bill.Format);
        }

        private bool IsDebtorWithCountryCode()
        {
            return IsForeignAddress(bill.Debtor, bill.Format);
        }

        private static bool IsForeignAddress(Address address, BillFormat format)
        {
            return address != null && format.LocalCountryCode != address.CountryCode;
        }
    }
}
