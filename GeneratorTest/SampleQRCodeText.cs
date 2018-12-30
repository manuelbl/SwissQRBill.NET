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
    public class SampleQRCodeText
    {
        private static readonly string QRCodeText1 = "SPC\n" +
            "0200\n" +
            "1\n" +
            "CH5800791123000889012\n" +
            "S\n" +
            "Robert Schneider AG\n" +
            "Rue du Lac\n" +
            "1268\n" +
            "2501\n" +
            "Biel\n" +
            "CH\n" +
            "\n" +
            "\n" +
            "\n" +
            "\n" +
            "\n" +
            "\n" +
            "\n" +
            "3949.75\n" +
            "CHF\n" +
            "S\n" +
            "Pia Rutschmann\n" +
            "Marktgasse\n" +
            "28\n" +
            "9400\n" +
            "Rorschach\n" +
            "CH\n" +
            "NON\n" +
            "\n" +
            "Bill no. 3139 for gardening work and disposal of waste material\n" +
            "EPD\n";

        public static string CreateQrCodeText1(bool withCRLF)
        {
            return HandleLinefeed(QRCodeText1, withCRLF);
        }

        public static Bill CreateBillData1()
        {
            Address creditor = new Address
            {
                Name = "Robert Schneider AG",
                Street = "Rue du Lac",
                HouseNo = "1268",
                PostalCode = "2501",
                Town = "Biel",
                CountryCode = "CH"
            };
            Address debtor = new Address
            {
                Name = "Pia Rutschmann",
                Street = "Marktgasse",
                HouseNo = "28",
                PostalCode = "9400",
                Town = " Rorschach",
                CountryCode = "CH"
            };
            Bill bill = new Bill
            {
                Account = "CH58 0079 1123 0008 8901 2",
                Creditor = creditor,
                Amount = 3949.75m,
                Currency = "CHF",
                Debtor = debtor,
                UnstructuredMessage = "Bill no. 3139 for gardening work and disposal of waste material"
            };
            bill.Format.Language = Language.EN;
            return bill;
        }

        private static readonly string QRCodeText2 = "SPC\n" +
            "0200\n" +
            "1\n" +
            "CH4431999123000889012\n" +
            "S\n" +
            "Robert Schneider AG\n" +
            "Rue du Lac\n" +
            "1268\n" +
            "2501\n" +
            "Biel\n" +
            "CH\n" +
            "\n" +
            "\n" +
            "\n" +
            "\n" +
            "\n" +
            "\n" +
            "\n" +
            "1949.75\n" +
            "CHF\n" +
            "S\n" +
            "Pia-Maria Rutschmann-Schnyder\n" +
            "Grosse Marktgasse\n" +
            "28\n" +
            "9400\n" +
            "Rorschach\n" +
            "CH\n" +
            "QRR\n" +
            "210000000003139471430009017\n" +
            "Order dated 18.06.2020\n" +
            "EPD\n" +
            "//S1/01/20170309/11/10201409/20/14000000/22/36958/30/CH106017086/40/1020/41/3010\n" +
            "UV;UltraPay005;12345\n" +
            "XY;XYService;54321";

        public static string CreateQrCodeText2(bool withCRLF)
        {
            return HandleLinefeed(QRCodeText2, withCRLF);
        }

        public static Bill CreateBillData2()
        {
            Address creditor = new Address
            {
                Name = "Robert Schneider AG",
                Street = "Rue du Lac",
                HouseNo = "1268",
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
                Account = "CH4431999123000889012",
                Creditor = creditor,
                Amount = 1949.75m,
                Currency = "CHF",
                Debtor = debtor,
                Reference = "210000000003139471430009017",
                UnstructuredMessage = "Order dated 18.06.2020",
                BillInformation = "//S1/01/20170309/11/10201409/20/14000000/22/36958/30/CH106017086/40/1020/41/3010",
                AlternativeSchemes = new List<AlternativeScheme> {
                    new AlternativeScheme{Name = "Ultraviolet", Instruction = "UV;UltraPay005;12345" },
                    new AlternativeScheme{Name = "Xing Yong", Instruction = "XY;XYService;54321" }
                }
            };
            bill.Format.Language = Language.EN;
            return bill;
        }

        private static readonly string QRCodeText3 = "SPC\n" +
            "0200\n" +
            "1\n" +
            "CH3709000000304442225\n" +
            "S\n" +
            "Salvation Army Foundation Switzerland\n" +
            "\n" +
            "\n" +
            "3000\n" +
            "Bern\n" +
            "CH\n" +
            "\n" +
            "\n" +
            "\n" +
            "\n" +
            "\n" +
            "\n" +
            "\n" +
            "\n" +
            "CHF\n" +
            "\n" +
            "\n" +
            "\n" +
            "\n" +
            "\n" +
            "\n" +
            "\n" +
            "NON\n" +
            "\n" +
            "Donnation to the Winterfest campaign\n" +
            "EPD\n";

        public static string CreateQrCodeText3(bool withCRLF)
        {
            return HandleLinefeed(QRCodeText3, withCRLF);
        }

        public static Bill CreateBillData3()
        {
            Address creditor = new Address
            {
                Name = "Salvation Army Foundation Switzerland",
                PostalCode = "3000",
                Town = "Bern",
                CountryCode = "CH"
            };
            Bill bill = new Bill
            {
                Account = "CH37 0900 0000 3044 4222 5",
                Creditor = creditor,
                Currency = "CHF",
                UnstructuredMessage = "Donnation to the Winterfest campaign"
            };
            bill.Format.Language = Language.EN;
            return bill;
        }

        private static readonly string QRCodeText4 = "SPC\n" +
            "0200\n" +
            "1\n" +
            "CH5800791123000889012\n" +
            "S\n" +
            "Robert Schneider AG\n" +
            "Rue du Lac\n" +
            "1268\n" +
            "2501\n" +
            "Biel\n" +
            "CH\n" +
            "\n" +
            "\n" +
            "\n" +
            "\n" +
            "\n" +
            "\n" +
            "\n" +
            "199.95\n" +
            "CHF\n" +
            "K\n" +
            "Pia-Maria Rutschmann-Schnyder\n" +
            "Grosse Marktgasse 28\n" +
            "9400 Rorschach\n" +
            "\n" +
            "\n" +
            "CH\n" +
            "SCOR\n" +
            "RF18539007547034\n" +
            "\n" +
            "EPD\n";

        public static string CreateQrCodeText4(bool withCRLF)
        {
            return HandleLinefeed(QRCodeText4, withCRLF);
        }

        public static Bill CreateBillData4()
        {
            Address creditor = new Address
            {
                Name = "Robert Schneider AG",
                Street = "Rue du Lac",
                HouseNo = "1268",
                PostalCode = "2501",
                Town = "Biel",
                CountryCode = "CH"
            };
            Address debtor = new Address
            {
                Name = "Pia-Maria Rutschmann-Schnyder",
                AddressLine1 = "Grosse Marktgasse 28",
                AddressLine2 = "9400 Rorschach",
                CountryCode = "CH"
            };
            Bill bill = new Bill
            {
                Account = "CH5800791123000889012",
                Creditor = creditor,
                Amount = 199.95m,
                Currency = "CHF",
                Debtor = debtor,
                Reference = "RF18539007547034"
            };
            bill.Format.Language = Language.EN;
            return bill;
        }

        private static string HandleLinefeed(string text, bool withCRLF)
        {
            if (withCRLF)
            {
                text = text.Replace("\n", "\r\n");
            }

            return text;
        }
    }
}
