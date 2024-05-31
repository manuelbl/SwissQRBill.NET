//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using System.Collections.Generic;
using Xunit;

namespace Codecrete.SwissQRBill.CoreTest
{
    public class BillTest
    {
        [Fact]
        public void SetVersion()
        {
            var bill = new Bill
            {
                Version = Bill.QrBillStandardVersion.V2_0
            };
            Assert.Equal(Bill.QrBillStandardVersion.V2_0, bill.Version);
        }

        [Fact]
        public void SetAmount()
        {
            var bill = new Bill
            {
                Amount = 37.45m
            };
            Assert.Equal(37.45m, bill.Amount);
        }

        [Fact]
        public void SetCurrency()
        {
            var bill = new Bill
            {
                Currency = "EUR"
            };
            Assert.Equal("EUR", bill.Currency);
        }

        [Fact]
        public void SetAccount()
        {
            var bill = new Bill
            {
                Account = "BD93020293480234"
            };
            Assert.Equal("BD93020293480234", bill.Account);
        }

        [Fact]
        public void SetCreditor()
        {
            var bill = new Bill();
            Address address = CreateAddress();
            bill.Creditor = address;
            Assert.Same(address, bill.Creditor);
            Assert.Equal(CreateAddress(), bill.Creditor);
        }

        [Fact]
        public void SetReference()
        {
            var bill = new Bill
            {
                Reference = "RF839DF38202934"
            };
            Assert.Equal("RF839DF38202934", bill.Reference);
            Assert.Equal(Bill.ReferenceTypeCredRef, bill.ReferenceType);
        }

        [Fact]
        public void SetReferenceToEmpty()
        {
            Bill bill = new Bill
            {
                Reference = ""
            };
            Assert.Equal("", bill.Reference);
            Assert.Equal(Bill.ReferenceTypeNoRef, bill.ReferenceType);
        }

        [Fact]
        public void CreateCreditorReference()
        {
            var bill = new Bill();
            bill.CreateAndSetCreditorReference("ABCD3934803");
            Assert.Equal("RF93ABCD3934803", bill.Reference);
            Assert.Equal(Bill.ReferenceTypeCredRef, bill.ReferenceType);
        }

        [Fact]
        public void CreateQRReference()
        {
            var bill = new Bill();
            bill.CreateAndSetQRReference("20187383000000000000721928");
            Assert.Equal("201873830000000000007219287", bill.Reference);
            Assert.Equal(Bill.ReferenceTypeQrRef, bill.ReferenceType);
        }

        [Fact]
        public void SetReferenceType()
        {
            var bill = new Bill
            {
                ReferenceType = Bill.ReferenceTypeCredRef
            };
            Assert.Equal(Bill.ReferenceTypeCredRef, bill.ReferenceType);
        }

        [Fact]
        public void SetUnstructuredMessage()
        {
            var bill = new Bill
            {
                UnstructuredMessage = "Rechnung 3849-2001"
            };
            Assert.Equal("Rechnung 3849-2001", bill.UnstructuredMessage);
        }

        [Fact]
        public void SetDebtor()
        {
            var bill = new Bill();
            Address address = CreateAddress();
            bill.Debtor = address;
            Assert.Same(address, bill.Debtor);
            Assert.Equal(CreateAddress(), bill.Debtor);
        }

        [Fact]
        public void SetBillInformation()
        {
            var bill = new Bill
            {
                BillInformation = "S1/01/20170309/11/10201409/20/14000000/22/369 58/30/CH106017086/40/1020/41/3010"
            };
            Assert.Equal("S1/01/20170309/11/10201409/20/14000000/22/369 58/30/CH106017086/40/1020/41/3010",
                    bill.BillInformation);
        }

        [Fact]
        public void SetSwicoBillInfo()
        {
            var bill = CreateBill();
            bill.SetSwicoBillInformation(new SwicoBillInformation
            {
                InvoiceNumber = "ABC-293234",
                CustomerReference = "234.2343-094",
                VatRate = 8m
            });
            Assert.Equal("//S1/10/ABC-293234/20/234.2343-094/32/8", bill.BillInformation);
        }

        [Fact]
        public void RetrieveSwicoBillInfo()
        {
            var bill = CreateBill();
            bill.BillInformation = "//S1/10/ABC-293234/20/234.2343-094/32/8";
            var billInfo = bill.RetrieveSwicoBillInformation();
            Assert.Equal(new SwicoBillInformation
            {
                InvoiceNumber = "ABC-293234",
                CustomerReference = "234.2343-094",
                VatRate = 8m
            },
                billInfo);
        }

        [Fact]
        public void RetrieveInvalidSwicoBillInfo()
        {
            var bill = CreateBill();
            bill.BillInformation = "//S2/10234234234";
            Assert.Null(bill.RetrieveSwicoBillInformation());
        }

        [Fact]
        public void SetAlternativeScheme()
        {
            var bill = new Bill
            {
                AlternativeSchemes = CreateAlternativeSchemes()
            };
            Assert.Equal(CreateAlternativeSchemes(), bill.AlternativeSchemes);
        }

        [Fact]
        public void TestEqualsTrivial()
        {
            var bill = new Bill();
            Assert.Equal(bill, bill);
            Bill nullBill = null;
            Assert.NotEqual(bill, nullBill);
            Assert.NotEqual((object)"xxx", bill);
        }

        [Fact]
        public void TestEquals()
        {
            var bill1 = CreateBill();
            var bill2 = CreateBill();
            Assert.Equal(bill1, bill2);
            Assert.Equal(bill2, bill1);

            bill2.UnstructuredMessage = "ABC";
            Assert.NotEqual(bill1, bill2);
        }

        [Fact]
        public void TestHashCode()
        {
            Bill bill1 = CreateBill();
            Bill bill2 = CreateBill();
            Assert.Equal(bill1.GetHashCode(), bill2.GetHashCode());
        }

        private static Address CreateAddress()
        {
            return new Address
            {
                Name = "Vision Consult GmbH",
                Street = "Hintergasse",
                HouseNo = "7b",
                PostalCode = "8400",
                Town = "Winterthur",
                CountryCode = "CH"
            };
        }

        private static Bill CreateBill()
        {
            return new Bill
            {
                Account = "CH12343345345",
                Creditor = CreateAddress(),
                Amount = 100.3m,
                Debtor = CreateAddress()
            };
        }

        private static List<AlternativeScheme> CreateAlternativeSchemes()
        {
            return new List<AlternativeScheme>(
             new[] {
                new AlternativeScheme { Name = "Ultraviolet", Instruction = "UV;UltraPay005;12345" },
                new AlternativeScheme { Name = "Xing Yong", Instruction = "XY;XYService;54321" }
        });
        }
    }
}
