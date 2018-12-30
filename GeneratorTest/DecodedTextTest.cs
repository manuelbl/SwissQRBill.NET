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

namespace Codecrete.SwissQRBill.GeneratorTest
{
    public class DecodedTextTest
    {
        [Fact]
        void DecodeText1()
        {
            Bill bill = SampleData.CreateExample1();
            NormalizeSourceBill(bill);
            Bill bill2 = QRBill.DecodeQrCodeText(QRBill.EncodeQrCodeText(bill));
            NormalizeDecodedBill(bill2);
            Assert.Equal(bill, bill2);
        }

        [Fact]
        void DecodeText2()
        {
            Bill bill = SampleData.CreateExample2();
            NormalizeSourceBill(bill);
            Bill bill2 = QRBill.DecodeQrCodeText(QRBill.EncodeQrCodeText(bill));
            NormalizeDecodedBill(bill2);
            Assert.Equal(bill, bill2);
        }

        [Fact]
        void DecodeText3()
        {
            Bill bill = SampleData.CreateExample3();
            NormalizeSourceBill(bill);
            Bill bill2 = QRBill.DecodeQrCodeText(QRBill.EncodeQrCodeText(bill));
            NormalizeDecodedBill(bill2);
            Assert.Equal(bill, bill2);
        }

        [Fact]
        void DecodeText4()
        {
            Bill bill = SampleData.CreateExample4();
            bill.Format = new BillFormat(); // set default values
            NormalizeSourceBill(bill);
            Bill bill2 = QRBill.DecodeQrCodeText(QRBill.EncodeQrCodeText(bill));
            NormalizeDecodedBill(bill2);
            Assert.Equal(bill, bill2);
        }

        [Fact]
        void DecodeTextB1a()
        {
            Bill bill = SampleQRCodeText.CreateBillData1();
            NormalizeSourceBill(bill);
            Bill bill2 = QRBill.DecodeQrCodeText(SampleQRCodeText.CreateQrCodeText1(false));
            NormalizeDecodedBill(bill2);
            Assert.Equal(bill, bill2);
        }

        [Fact]
        void DecodeTextB1b()
        {
            Bill bill = SampleQRCodeText.CreateBillData1();
            NormalizeSourceBill(bill);
            Bill bill2 = QRBill.DecodeQrCodeText(SampleQRCodeText.CreateQrCodeText1(true));
            NormalizeDecodedBill(bill2);
            Assert.Equal(bill, bill2);
        }

        [Fact]
        void DecodeTextB2()
        {
            Bill bill = SampleQRCodeText.CreateBillData2();
            NormalizeSourceBill(bill);
            Bill bill2 = QRBill.DecodeQrCodeText(SampleQRCodeText.CreateQrCodeText2(false));
            NormalizeDecodedBill(bill2);
            Assert.Equal(bill, bill2);
        }

        [Fact]
        void DecodeTextB3()
        {
            Bill bill = SampleQRCodeText.CreateBillData3();
            NormalizeSourceBill(bill);
            Bill bill2 = QRBill.DecodeQrCodeText(SampleQRCodeText.CreateQrCodeText3(false));
            NormalizeDecodedBill(bill2);
            Assert.Equal(bill, bill2);
        }

        [Fact]
        void DecodeTextB4()
        {
            Bill bill = SampleQRCodeText.CreateBillData4();
            NormalizeSourceBill(bill);
            Bill bill2 = QRBill.DecodeQrCodeText(SampleQRCodeText.CreateQrCodeText4(false));
            NormalizeDecodedBill(bill2);
            Assert.Equal(bill, bill2);
        }

        private void NormalizeSourceBill(Bill bill)
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
            if (bill.Debtor != null)
            {
                if (bill.Debtor.Town != null)
                {
                    bill.Debtor.Town = bill.Debtor.Town.Trim();
                }
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

        private void NormalizeDecodedBill(Bill bill)
        {
            bill.Format.Language = Language.DE; // fix language (not contained in text)
        }

        [Fact]
        void DecodeInvalidFormat1()
        {
            QRBillValidationError err = Assert.Throws<QRBillValidationError>(() => QRBill.DecodeQrCodeText("garbage"));
            AssertSingleError(err.Result, QRBill.KeyValidDataStructure, Bill.FieldQRType);
        }

        [Fact]
        void DecodeInvalidFormat2a()
        {
            QRBillValidationError err = Assert.Throws<QRBillValidationError>(
                    () => QRBill.DecodeQrCodeText("SPC\r\n0100\r\n\r\n\r\n"));
            AssertSingleError(err.Result, QRBill.KeyValidDataStructure, Bill.FieldQRType);
        }

        [Fact]
        void DecodeInvalidFormat2b()
        {
            QRBillValidationError err = Assert.Throws<QRBillValidationError>(() => QRBill.DecodeQrCodeText(
                       "SPC1\r\n0200\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n"));
            AssertSingleError(err.Result, QRBill.KeyValidDataStructure, Bill.FieldQRType);
        }

        [Fact]
        void DecodeInvalidFormat3()
        {
            QRBillValidationError err = Assert.Throws<QRBillValidationError>(() => QRBill.DecodeQrCodeText(
                       "SPC1\r\n0200\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n"));
            AssertSingleError(err.Result, QRBill.KeyValidDataStructure, Bill.FieldQRType);
        }

        [Fact]
        void DecodeInvalidVersion()
        {
            QRBillValidationError err = Assert.Throws<QRBillValidationError>(() => QRBill.DecodeQrCodeText(
                       "SPC\r\n0101\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n"));
            AssertSingleError(err.Result, QRBill.KeySupportedVersion, Bill.FieldVersion);
        }

        [Fact]
        void DecodeInvalidCodingType()
        {
            QRBillValidationError err = Assert.Throws<QRBillValidationError>(() => QRBill.DecodeQrCodeText(
                       "SPC\r\n0200\r\n0\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n"));
            AssertSingleError(err.Result, QRBill.KeySupportedCodingType, Bill.FieldCodingType);
        }

        [Fact]
        void DecodeInvalidNumber()
        {
            string invalidText = SampleQRCodeText.CreateQrCodeText1(false).Replace("3949.75", "1239d49.75");
            QRBillValidationError err = Assert.Throws<QRBillValidationError>(
                        () => QRBill.DecodeQrCodeText(invalidText));
            AssertSingleError(err.Result, QRBill.KeyValidNumber, Bill.FieldAmount);
        }

        [Fact]
        void DecodeMissingEPD()
        {
            string invalidText = SampleQRCodeText.CreateQrCodeText1(false).Replace("EPD", "E_P");
            QRBillValidationError err = Assert.Throws<QRBillValidationError>(
                        () => QRBill.DecodeQrCodeText(invalidText));
            AssertSingleError(err.Result, QRBill.KeyValidDataStructure, Bill.FieldTrailer);
        }

        private void AssertSingleError(ValidationResult result, string messageKey, string field)
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
