//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Principal;
using System.Text;


namespace Codecrete.SwissQRBill.CoreTest
{
    public static class SampleData
    {
        public static Bill CreateExample1()
        {
            var creditor = new Address
            {
                Name = "Robert Schneider AG",
                Street = "Rue du Lac",
                HouseNo = "1268/2/22",
                PostalCode = "2501",
                Town = "Biel",
                CountryCode = "CH"
            };
            var debtor = new Address
            {
                Name = "Pia-Maria Rutschmann-Schnyder",
                Street = "Grosse Marktgasse",
                HouseNo = "28",
                PostalCode = "9400",
                Town = " Rorschach",
                CountryCode = "CH"
            };
            var bill = new Bill
            {
                Account = "CH44 3199 9123 0008  89012",
                Creditor = creditor,
                Amount = 123949.75m,
                Currency = "CHF",
                Debtor = debtor,
                Reference = "210000 000 00313 9471430009017",
                UnstructuredMessage = "Instruction of 15.09.2019",
                BillInformation =
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
            var creditor = new Address
            {
                Name = "Salvation Army Foundation Switzerland",
                Street = null,
                HouseNo = null,
                PostalCode = "3000",
                Town = "Berne",
                CountryCode = "CH"
            };
            var bill = new Bill
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
            var creditor = new Address
            {
                Name = "Robert Schneider AG",
                Street = "Rue du Lac",
                HouseNo = "1268/2/22",
                PostalCode = "2501",
                Town = "Biel",
                CountryCode = "CH"
            };
            var debtor = new Address
            {
                Name = "Pia-Maria Rutschmann-Schnyder",
                Street = "Grosse Marktgasse",
                HouseNo = "28",
                PostalCode = "9400",
                Town = "Rorschach",
                CountryCode = "CH"
            };
            var bill = new Bill
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
            var creditor = new Address
            {
                Name = "ABC AG",
                Street = null,
                HouseNo = null,
                PostalCode = "3000",
                Town = "Bern",
                CountryCode = "CH"
            };
            var bill = new Bill
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

#pragma warning disable CS0618 // Type or member is obsolete
        public static Bill CreateExample5()
        {
            var creditor = new Address
            {
                Name = "Herrn und Frau Ambikaipagan & Deepshikha Thirugnanasampanthamoorthy",
                AddressLine1 = "c/o Pereira De Carvalho, Conrad-Ferdinand-Meyer-Strasse 317 Wohnung 7B",
                AddressLine2 = "9527 Niederhelfenschwil bei Schönholzerswilen im Kanton St. Gallen",
                CountryCode = "CH"
            };
            var debtor = new Address
            {
                Name = "Annegret Karin & Hansruedi Frischknecht-Bernhardsgrütter",
                AddressLine1 = "1503 South New Hampshire Avenue, Lower East-side Bellvue",
                AddressLine2 = "Poughkeepsie NY 12601-1233",
                CountryCode = "US"
            };
            var bill = new Bill
            {
                Account = "CH44 3199 9123 0008  89012",
                Creditor = creditor,
                Amount = 987654321.5m,
                Currency = "CHF",
                Debtor = debtor,
                Reference = "210000 000 00313 9471430009017",
                UnstructuredMessage = "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed",
                BillInformation = "//S1/10/10201409/11/190512/20/1400.0001-53/30/106017086/31/180508/32/7.7/40/0:30",
                Format = { Language = Language.RM }
            };
            return bill;
        }

        public static Bill CreateExample6()
        {
            var creditor = new Address
            {
                Name = "Herrn und Frau Ambikaipagan & Deepshikha Thirugnanasampanthamoorthy",
                AddressLine1 = "c/o Pereira De Carvalho, Conrad-Ferdinand-Meyer-Strasse 317 Wohnung 7B",
                AddressLine2 = "9527 Niederhelfenschwil bei Schönholzerswilen im Kanton St. Gallen",
                CountryCode = "CH"
            };
            var bill = new Bill
            {
                Account = "CH44 3199 9123 0008  89012",
                Creditor = creditor,
                Currency = "EUR",
                Reference = "210000 000 00313 9471430009017",
                UnstructuredMessage = "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed",
                BillInformation = "//S1/10/102015409/11/190512/20/1400.000-53/30/106017086/31/180508/32/7.7/40/0:30",
                Format = { Language = Language.EN }
            };
            return bill;
        }


        public static Bill CreateExample7()
        {
            var creditor = new Address()
            {
                Name = "Omnia Trading AG",
                AddressLine1 = "Allmendweg 30",
                AddressLine2 = "4528 Zuchwil",
                CountryCode = "CH"
            };

            var debtor = new Address()
            {
                Name = "Machina Futura AG",
                AddressLine1 = "Alte Fabrik 3A",
                AddressLine2 = "8400 Winterthur",
                CountryCode = "CH"
            };

            var bill = new Bill()
            {
                Creditor = creditor,
                Amount = 1756.05m,
                Currency = "CHF",
                Format = { Language = Language.DE, OutputSize = OutputSize.A4PortraitSheet },
                Account = "CH48 0900 0000 8575 7337 2",
                Debtor = debtor,
                UnstructuredMessage = "Auftrag 2830188 / Rechnung 2021007834"
            };

            bill.CreateAndSetCreditorReference("2021007834");

            return bill;
        }
#pragma warning restore CS0618 // Type or member is obsolete

        public static Bill CreateExample8()
        {
            var creditor = new Address()
            {
                Name = "Buğra Çavdarli",
                Street = "Rue du Lièvre",
                HouseNo = "13",
                PostalCode = "1219",
                Town = "Aïre",
                CountryCode = "CH"
            };

            var debtor = new Address()
            {
                Name = "L'Œil de Bœuf",
                Street = "Route d'Outre Vièze",
                HouseNo = "44",
                PostalCode = "1871",
                Town = "Choëx",
                CountryCode = "CH"
            };

            var bill = new Bill() {
                Format = { Language = Language.FR },
                Account = "CH14 8914 4587 8681 9314 7",
                Creditor = creditor,
                Amount = 179.00m,
                Currency = "CHF",
                Debtor = debtor,
                Reference = "RF35RF23452352345",
                UnstructuredMessage = "Facture 48390, €10 de réduction"
            };

            return bill;
        }

        public static string CreateQrCode1()
        {
            var qrCodeText = new StringBuilder();
            qrCodeText.AppendLine("SPC");
            qrCodeText.AppendLine("0200");
            qrCodeText.AppendLine("1");
            qrCodeText.AppendLine("CH1234567890123456789");
            qrCodeText.AppendLine("S");
            qrCodeText.AppendLine("Steuerverwaltung der Stadt Bern");
            qrCodeText.AppendLine("Bundesgasse");
            qrCodeText.AppendLine("33");
            qrCodeText.AppendLine("3011");
            qrCodeText.AppendLine("Bern");
            qrCodeText.AppendLine("CH");
            qrCodeText.AppendLine("");
            qrCodeText.AppendLine("");
            qrCodeText.AppendLine("");
            qrCodeText.AppendLine("");
            qrCodeText.AppendLine("");
            qrCodeText.AppendLine("");
            qrCodeText.AppendLine("");
            qrCodeText.AppendLine("2500.00");
            qrCodeText.AppendLine("CHF");
            qrCodeText.AppendLine("S");
            qrCodeText.AppendLine("Martina Muster");
            qrCodeText.AppendLine("Bubenbergplatz");
            qrCodeText.AppendLine("1");
            qrCodeText.AppendLine("3011");
            qrCodeText.AppendLine("Bern");
            qrCodeText.AppendLine("CH");
            qrCodeText.AppendLine("QRR");
            qrCodeText.AppendLine("123456789012345678901234567");
            qrCodeText.AppendLine("1. Steuerrate 2020");
            qrCodeText.AppendLine("EPD");
            qrCodeText.AppendLine("//S1/11/200627/30/115140892/31/200627/32/7.7/40/0:30");
            return qrCodeText.ToString();
        }

        public static string CreateInvalidQrCode1()
        {
            var qrCodeText = new StringBuilder();
            qrCodeText.AppendLine("SPC");
            qrCodeText.AppendLine("0200");
            qrCodeText.AppendLine("1");
            qrCodeText.AppendLine("CH8430000001800003797");
            qrCodeText.AppendLine("S");
            qrCodeText.AppendLine("AXA Versicherungen AG");
            qrCodeText.AppendLine("General-Guisan-Str.");
            qrCodeText.AppendLine("40");
            qrCodeText.AppendLine("8401");
            qrCodeText.AppendLine("Winterthur");
            qrCodeText.AppendLine("CH");
            qrCodeText.AppendLine("");
            qrCodeText.AppendLine("");
            qrCodeText.AppendLine("");
            qrCodeText.AppendLine("");
            qrCodeText.AppendLine("");
            qrCodeText.AppendLine("");
            qrCodeText.AppendLine("");
            qrCodeText.AppendLine("null");
            qrCodeText.AppendLine("CHF");
            qrCodeText.AppendLine("S");
            qrCodeText.AppendLine("Testfirma AG");
            qrCodeText.AppendLine("Hauptstrasse");
            qrCodeText.AppendLine("61");
            qrCodeText.AppendLine("6210");
            qrCodeText.AppendLine("Sursee");
            qrCodeText.AppendLine("CH");
            qrCodeText.AppendLine("QRR");
            qrCodeText.AppendLine("000000001000285497220812814");
            qrCodeText.AppendLine("");
            qrCodeText.AppendLine("EPD");
            return qrCodeText.ToString();
        }
    }
}
