//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using System.Collections.Generic;


namespace Codecrete.SwissQRBill.GeneratorTest
{
    public class SampleData
    {
        public static Bill CreateExample1()
        {
            Address creditor = new Address
            {
                Name = "Robert Schneider AG",
                Street = "Rue du Lac",
                HouseNo = "1268/2/22",
                PostalCode = "2501",
                Town = "Biel",
                CountryCode = "CH"
            };
            Address debtor = new Address
            {
                Name = "Pia-Maria Rutschmann-Schnyder",
                Street = "Grosse Marktgasse",
                HouseNo = "28",
                PostalCode = "9400",
                Town = " Rorschach",
                CountryCode = "CH"
            };
            Bill bill = new Bill
            {
                Account = "CH44 3199 9123 0008  89012",
                Creditor = creditor,
                Amount = 123949.75m,
                Currency = "CHF",
                Debtor = debtor,
                Reference = "210000 000 00313 9471430009017",
                UnstructuredMessage = "Instruction of 15.09.2019",
                BillInformationText =
                    "//S1/10/10201409/11/190512/20/1400.000-53/30/106017086/31/180508/32/7.7/40/2:10;0:30",
                AlternativeSchemes = new List<AlternativeScheme>
                {
                    new AlternativeScheme {Name = "Ultraviolet", Instruction = "UV;UltraPay005;12345"},
                    new AlternativeScheme {Name = "Xing Yong", Instruction = "XY;XYService;54321"}
                },
                Format = { Language = Language.EN }
            };
            return bill;
        }

        public static Bill CreateExample2()
        {
            Address creditor = new Address
            {
                Name = "Salvation Army Foundation Switzerland",
                Street = null,
                HouseNo = null,
                PostalCode = "3000",
                Town = "Berne",
                CountryCode = "CH"
            };
            Bill bill = new Bill
            {
                Account = "CH3709000000304442225",
                Creditor = creditor,
                Amount = null,
                Currency = "CHF",
                Debtor = null,
                Reference = "",
                UnstructuredMessage = "Donation to the Winterfest Campaign",
                Format = { Language = Language.DE }
            };
            return bill;
        }

        public static Bill CreateExample3()
        {
            Address creditor = new Address
            {
                Name = "Robert Schneider AG",
                Street = "Rue du Lac",
                HouseNo = "1268/2/22",
                PostalCode = "2501",
                Town = "Biel",
                CountryCode = "CH"
            };
            Address debtor = new Address
            {
                Name = "Pia-Maria Rutschmann-Schnyder",
                Street = "Grosse Marktgasse",
                HouseNo = "28",
                PostalCode = "9400",
                Town = "Rorschach",
                CountryCode = "CH"
            };
            Bill bill = new Bill
            {
                Account = "CH74 0070 0110 0061 1600 2",
                Creditor = creditor,
                Amount = 199.95m,
                Currency = "CHF",
                Debtor = debtor,
                Reference = "RF18539007547034",
                UnstructuredMessage = null,
                Format = { Language = Language.FR }
            };
            return bill;
        }

        public static Bill CreateExample4()
        {
            Address creditor = new Address
            {
                Name = "ABC AG",
                Street = null,
                HouseNo = null,
                PostalCode = "3000",
                Town = "Bern",
                CountryCode = "CH"
            };
            Bill bill = new Bill
            {
                Account = "CH3709000000304442225",
                Creditor = creditor,
                Amount = null,
                Currency = "CHF",
                Debtor = null,
                Reference = "",
                UnstructuredMessage = "",
                Format = { Language = Language.IT, SeparatorType = SeparatorType.SolidLine }
            };
            return bill;
        }

        public static Bill CreateExample5()
        {
            Address creditor = new Address
            {
                Name = "Herrn und Frau Ambikaipagan & Deepshikha Thirugnanasampanthamoorthy",
                AddressLine1 = "c/o Pereira De Carvalho, Conrad-Ferdinand-Meyer-Strasse 317 Wohnung 7B",
                AddressLine2 = "9527 Niederhelfenschwil bei Schönholzerswilen im Kanton St. Gallen",
                CountryCode = "CH"
            };
            Address debtor = new Address
            {
                Name = "Annegret Karin & Hansruedi Frischknecht-Bernhardsgrütter",
                AddressLine1 = "1503 South New Hampshire Avenue, Lower East-side Bellvue",
                AddressLine2 = "Poughkeepsie NY 12601-1233",
                CountryCode = "US"
            };
            Bill bill = new Bill
            {
                Account = "CH44 3199 9123 0008  89012",
                Creditor = creditor,
                Amount = 987654321.5m,
                Currency = "CHF",
                Debtor = debtor,
                Reference = "210000 000 00313 9471430009017",
                UnstructuredMessage = "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed",
                BillInformationText = "//S1/10/10201409/11/190512/20/1400.000-53/30/106017086/31/180508/32/7.7/40/2:10;0:30",
                Format = { Language = Language.EN }
            };
            return bill;
        }

        public static Bill CreateExample6()
        {
            Address creditor = new Address
            {
                Name = "Herrn und Frau Ambikaipagan & Deepshikha Thirugnanasampanthamoorthy",
                AddressLine1 = "c/o Pereira De Carvalho, Conrad-Ferdinand-Meyer-Strasse 317 Wohnung 7B",
                AddressLine2 = "9527 Niederhelfenschwil bei Schönholzerswilen im Kanton St. Gallen",
                CountryCode = "CH"
            };
            Bill bill = new Bill
            {
                Account = "CH44 3199 9123 0008  89012",
                Creditor = creditor,
                Currency = "EUR",
                Reference = "210000 000 00313 9471430009017",
                UnstructuredMessage = "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed",
                BillInformationText = "//S1/10/10201409/11/190512/20/1400.000-53/30/106017086/31/180508/32/7.7/40/2:10;0:30",
                Format = { Language = Language.EN }
            };
            return bill;
        }
    }
}
