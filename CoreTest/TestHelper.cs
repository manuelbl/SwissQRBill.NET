//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using System.Collections.Generic;
using Xunit;
using static Codecrete.SwissQRBill.Generator.ValidationMessage;

namespace Codecrete.SwissQRBill.CoreTest
{
    public class TestHelper
    {
        public static void NormalizeSourceBill(Bill bill)
        {
            bill.Format.Language = Language.DE;
            bill.Account = bill.Account.Replace(" ", "");
            if (bill.Reference != null)
            {
                bill.Reference = bill.Reference.Replace(" ", "");
            }

            if (bill.Creditor != null)
            {
                if (bill.Creditor.Street == null)
                {
                    bill.Creditor.Street = ""; // replace null with empty string
                }

                if (bill.Creditor.HouseNo == null)
                {
                    bill.Creditor.HouseNo = ""; // replace null with empty string
                }
            }

            if (bill.Debtor?.Town != null)
            {
                bill.Debtor.Town = bill.Debtor.Town.Trim();
            }

            if (bill.Reference == null)
            {
                bill.Reference = ""; // replace null with empty string
            }

            if (bill.UnstructuredMessage == null)
            {
                bill.UnstructuredMessage = ""; // replace null with empty string
            }

            if (bill.BillInformation == null)
            {
                bill.BillInformation = ""; // replace null with empty string
            }

            if (bill.AlternativeSchemes != null)
            {
                foreach (AlternativeScheme scheme in bill.AlternativeSchemes)
                {
                    scheme.Name = null;
                }
            }
        }

        public static void NormalizeDecodedBill(Bill bill)
        {
            bill.Format.Language = Language.DE; // fix language (not contained in text)
        }

        public static void AssertSingleError(ValidationResult result, string messageKey, string field)
        {
            Assert.NotNull(result);
            List<ValidationMessage> messages = result.ValidationMessages;
            Assert.NotNull(messages);
            Assert.Single(messages);
            Assert.Equal(MessageType.Error, messages[0].Type);
            Assert.Equal(messageKey, messages[0].MessageKey);
            Assert.Equal(field, messages[0].Field);
        }
    }
}
