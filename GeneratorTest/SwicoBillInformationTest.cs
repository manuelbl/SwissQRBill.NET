//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2020 Christian Bernasconi
// Copyright (c) 2020 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using System;
using System.Collections.Generic;
using Xunit;

namespace Codecrete.SwissQRBill.GeneratorTest
{
    public class SwicoBillInformationTest
    {
        [Fact]
        private void SetInvoiceNumber()
        {
            SwicoBillInformation billInformation = new SwicoBillInformation
            {
                InvoiceNumber = "ABC"
            };
            Assert.Equal("ABC", billInformation.InvoiceNumber);
        }

        [Fact]
        private void SetInvoiceDate()
        {
            SwicoBillInformation billInformation = new SwicoBillInformation
            {
                InvoiceDate = new DateTime(2020, 6, 30)
            };
            Assert.Equal(new DateTime(2020, 6, 30), billInformation.InvoiceDate);
        }

        [Fact]
        private void SetCustomerReference()
        {
            SwicoBillInformation billInformation = new SwicoBillInformation
            {
                CustomerReference = "1234-ABC"
            };
            Assert.Equal("1234-ABC", billInformation.CustomerReference);
        }

        [Fact]
        private void SetVatNumber()
        {
            SwicoBillInformation billInformation = new SwicoBillInformation
            {
                VatNumber = "109030864"
            };
            Assert.Equal("109030864", billInformation.VatNumber);
        }

        [Fact]
        private void SetVatDate()
        {
            SwicoBillInformation billInformation = new SwicoBillInformation
            {
                VatDate = new DateTime(2020, 3, 1)
            };
            Assert.Equal(new DateTime(2020, 3, 1), billInformation.VatDate);
        }

        [Fact]
        private void SetVatStartDate()
        {
            SwicoBillInformation billInformation = new SwicoBillInformation
            {
                VatStartDate = new DateTime(2019, 3, 1)
            };
            Assert.Equal(new DateTime(2019, 3, 1), billInformation.VatStartDate);
        }

        [Fact]
        private void SetVatEndDate()
        {
            SwicoBillInformation billInformation = new SwicoBillInformation
            {
                VatEndDate = new DateTime(2020, 2, 29)
            };
            Assert.Equal(new DateTime(2020, 2, 29), billInformation.VatEndDate);
        }

        [Fact]
        private void SetVatRate()
        {
            SwicoBillInformation billInformation = new SwicoBillInformation
            {
                VatRate = 7.7m
            };
            Assert.Equal(7.7m, billInformation.VatRate);
        }

        [Fact]
        private void SetVatDetails()
        {
            SwicoBillInformation billInformation = new SwicoBillInformation
            {
                VatRateDetails = new List<(decimal, decimal)> { (8m, 1000m), (2.5m, 400m) }
            };
            Assert.Equal(2, billInformation.VatRateDetails.Count);
            Assert.Equal(8m, billInformation.VatRateDetails[0].Item1);
            Assert.Equal(400m, billInformation.VatRateDetails[1].Item2);
        }

        [Fact]
        private void SetVatImportTaxes()
        {
            SwicoBillInformation billInformation = new SwicoBillInformation
            {
                VatImportTaxes = new List<(decimal, decimal)> { (7.7m, 48.12m), (2.5m, 17.23m) }
            };
            Assert.Equal(2, billInformation.VatImportTaxes.Count);
            Assert.Equal(7.7m, billInformation.VatImportTaxes[0].Item1);
            Assert.Equal(17.23m, billInformation.VatImportTaxes[1].Item2);
        }

        [Fact]
        private void SetPaymentConditions()
        {
            SwicoBillInformation billInformation = new SwicoBillInformation
            {
                PaymentConditions = new List<(decimal, int)> { (2m, 10), (0m, 30) }
            };
            Assert.Equal(2, billInformation.PaymentConditions.Count);
            Assert.Equal(2m, billInformation.PaymentConditions[0].Item1);
            Assert.Equal(30, billInformation.PaymentConditions[1].Item2);
        }

        [Fact]
        private void GetDueDate()
        {
            SwicoBillInformation billInformation = new SwicoBillInformation
            {
                InvoiceDate = new DateTime(2020, 6, 30),
                PaymentConditions = new List<(decimal, int)> { (2m, 10), (0m, 30) }
            };
            Assert.Equal(new DateTime(2020, 7, 30), billInformation.DueDate);
        }

        [Fact]
        private void TestEqualsTrivial()
        {
            SwicoBillInformation info = new SwicoBillInformation();
            Assert.Equal(info, info);

            SwicoBillInformation nullBill = null;
            Assert.NotEqual(info, nullBill);
            Assert.NotEqual((object)"xxx", info);
        }

        [Fact]
        private void TestEquals()
        {
            SwicoBillInformation info1 = CreateBillInformation();
            SwicoBillInformation info2 = CreateBillInformation();
            Assert.Equal(info1, info2);
            Assert.Equal(info2, info1);

            info2.CustomerReference = "ABC";
            Assert.NotEqual(info1, info2);
        }

        [Fact]
        private void TestHashCode()
        {
            SwicoBillInformation info1 = CreateBillInformation();
            SwicoBillInformation info2 = CreateBillInformation();
            Assert.Equal(info1.GetHashCode(), info2.GetHashCode());
        }

        private SwicoBillInformation CreateBillInformation()
        {
            return new SwicoBillInformation
            {
                InvoiceNumber = "R0000700312",
                InvoiceDate = new DateTime(2020, 7, 10),
                CustomerReference = "Q.30007.100002",
                VatNumber = "105815317",
                VatStartDate = new DateTime(2019, 11, 1),
                VatEndDate = new DateTime(2020, 4, 30),
                VatRate = 8m,
                VatImportTaxes = new List<(decimal, decimal)> { (8m, 48.12m), (2.5m, 17.23m) },
                PaymentConditions = new List<(decimal, int)> { (0m, 30) }
            };
        }
    }
}
