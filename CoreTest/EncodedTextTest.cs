//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using System.Collections;
using System.Collections.Generic;
using Xunit;
using static Codecrete.SwissQRBill.Generator.Bill;

namespace Codecrete.SwissQRBill.CoreTest
{
    public class EncodedTextTest
    {
        [Theory]
        [ClassData(typeof(SampleAndNewlineProvider))]
        public void CreateText(int sample, QrDataSeparator separator, string newline)
        {
            Bill bill = SampleQRCodeText.CreateBillData(sample, separator);
            Assert.Equal(
                    SampleQRCodeText.CreateQrCodeText(sample, newline),
                    QRBill.EncodeQrCodeText(bill)
            );
        }

        [Fact]
        public void CreateTextError1()
        {
            Assert.Throws<QRBillValidationException>(() =>
            {
                Bill bill = SampleData.CreateExample4();
                bill.Amount = -0.01m;
                QRBill.EncodeQrCodeText(bill);
            });
        }

        [Theory]
        [ClassData(typeof(NewLineTheoryData))]
        public void CreateTextEmptyReference(QrDataSeparator separator, string newLine)
        {
            var bill = SampleQRCodeText.CreateBillData3(separator);
            var result = QRBill.Validate(bill);
            Assert.False(result.HasErrors);
            bill = result.CleanedBill;
            bill.Reference = "";
            Assert.Equal(SampleQRCodeText.CreateQrCodeText3(newLine), QRBill.EncodeQrCodeText(bill));
        }

        private class NewLineTheoryData : TheoryData<QrDataSeparator, string>
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1144:Unused private types or members should be removed", Justification = "false positive")]
            public NewLineTheoryData()
            {
                Add(QrDataSeparator.Lf, "\n");
                Add(QrDataSeparator.CrLf, "\r\n");
            }
        }

        public class SampleAndNewlineProvider : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                for (var i = 1; i <= 5; i += 1)
                {
                    yield return new object[] { i, QrDataSeparator.Lf, "\n" };
                    yield return new object[] { i, QrDataSeparator.CrLf, "\r\n" };
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

    }
}
