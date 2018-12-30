//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using System.Collections.Generic;
using Xunit;

namespace Codecrete.SwissQRBill.GeneratorTest
{
    public class BillTest
    {
        [Fact]
        void SetVersion()
        {
            Bill bill = new Bill
            {
                Version = Bill.StandardVersion.V2_0
            };
            Assert.Equal(Bill.StandardVersion.V2_0, bill.Version);
        }

        [Fact]
        void SetAmount()
        {
            Bill bill = new Bill
            {
                Amount = 37.45m
            };
            Assert.Equal(37.45m, bill.Amount);
        }

        [Fact]
        void SetCurrency()
        {
            Bill bill = new Bill
            {
                Currency = "EUR"
            };
            Assert.Equal("EUR", bill.Currency);
        }

        [Fact]
        void SetAccount()
        {
            Bill bill = new Bill
            {
                Account = "BD93020293480234"
            };
            Assert.Equal("BD93020293480234", bill.Account);
        }

        [Fact]
        void SetCreditor()
        {
            Bill bill = new Bill();
            Address address = CreateAddress();
            bill.Creditor = address;
            Assert.Same(address, bill.Creditor);
            Assert.Equal(CreateAddress(), bill.Creditor);
        }

        [Fact]
        void SetReference()
        {
            Bill bill = new Bill
            {
                Reference = "RF839DF38202934"
            };
            Assert.Equal("RF839DF38202934", bill.Reference);
        }

        [Fact]
        void CreateCreditorReference()
        {
            Bill bill = new Bill();
            bill.CreateAndSetCreditorReference("ABCD3934803");
            Assert.Equal("RF93ABCD3934803", bill.Reference);
        }

        [Fact]
        void SetUnstructuredMessage()
        {
            Bill bill = new Bill
            {
                UnstructuredMessage = "Rechnung 3849-2001"
            };
            Assert.Equal("Rechnung 3849-2001", bill.UnstructuredMessage);
        }

        [Fact]
        void SetDebtor()
        {
            Bill bill = new Bill();
            Address address = CreateAddress();
            bill.Debtor = address;
            Assert.Same(address, bill.Debtor);
            Assert.Equal(CreateAddress(), bill.Debtor);
        }

        [Fact]
        void SetBillInformation()
        {
            Bill bill = new Bill
            {
                BillInformation = "S1/01/20170309/11/10201409/20/14000000/22/369 58/30/CH106017086/40/1020/41/3010"
            };
            Assert.Equal("S1/01/20170309/11/10201409/20/14000000/22/369 58/30/CH106017086/40/1020/41/3010",
                    bill.BillInformation);
        }

        [Fact]
        void SetAlternativeScheme()
        {
            Bill bill = new Bill
            {
                AlternativeSchemes = CreateAlternativeSchemes()
            };
            Assert.Equal(CreateAlternativeSchemes(), bill.AlternativeSchemes);
        }

        [Fact]
        void TestEqualsTrivial()
        {
            Bill bill = new Bill();
            Assert.Equal(bill, bill);
            Bill nullBill = null;
            Assert.NotEqual(bill, nullBill);
            Assert.NotEqual((object)"xxx", bill);
        }

        [Fact]
        void TestEquals()
        {
            Bill bill1 = CreateBill();
            Bill bill2 = CreateBill();
            Assert.Equal(bill1, bill2);
            Assert.Equal(bill2, bill1);

            bill2.UnstructuredMessage = "ABC";
            Assert.NotEqual(bill1, bill2);
        }

        [Fact]
        void TestHashCode()
        {
            Bill bill1 = CreateBill();
            Bill bill2 = CreateBill();
            Assert.Equal(bill1.GetHashCode(), bill2.GetHashCode());
        }

        private Address CreateAddress()
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

        private Bill CreateBill()
        {
            return new Bill
            {
                Account = "CH12343345345",
                Creditor = CreateAddress(),
                Amount = 100.3m,
                Debtor = CreateAddress()
            };
        }

        private List<AlternativeScheme> CreateAlternativeSchemes()
        {
            return new List<AlternativeScheme>(
             new AlternativeScheme[] {
                new AlternativeScheme { Name = "Ultraviolet", Instruction = "UV;UltraPay005;12345" },
                new AlternativeScheme { Name = "Xing Yong", Instruction = "XY;XYService;54321" }
        });
        }
    }
}
