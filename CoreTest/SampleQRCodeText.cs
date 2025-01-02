//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using System.Collections.Generic;

namespace Codecrete.SwissQRBill.CoreTest
{
    public static class SampleQRCodeText
    {
        private static readonly string[] QRCodeText1 = {
            "SPC",
            "0200",
            "1",
            "CH5800791123000889012",
            "S",
            "Robert Schneider AG",
            "Rue du Lac",
            "1268",
            "2501",
            "Biel",
            "CH",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "3949.75",
            "CHF",
            "S",
            "Pia Rutschmann",
            "Marktgasse",
            "28",
            "9400",
            "Rorschach",
            "CH",
            "NON",
            "",
            "Bill no. 3139 for gardening work and disposal of waste material",
            "EPD"
        };

        public static string CreateQrCodeText1(string newLine = "\n")
        {
            return string.Join(newLine, QRCodeText1);
        }

        public static Bill CreateBillData1(Bill.QrDataSeparator separator)
        {
            var creditor = new Address
            {
                Name = "Robert Schneider AG",
                Street = "Rue du Lac",
                HouseNo = "1268",
                PostalCode = "2501",
                Town = "Biel",
                CountryCode = "CH"
            };
            var debtor = new Address
            {
                Name = "Pia Rutschmann",
                Street = "Marktgasse",
                HouseNo = "28",
                PostalCode = "9400",
                Town = " Rorschach",
                CountryCode = "CH"
            };
            var bill = new Bill
            {
                Account = "CH58 0079 1123 0008 8901 2",
                Creditor = creditor,
                Amount = 3949.75m,
                Currency = "CHF",
                Debtor = debtor,
                UnstructuredMessage = "Bill no. 3139 for gardening work and disposal of waste material",
                Format = { Language = Language.EN },
                Separator = separator
            };
            return bill;
        }

        private static readonly string[] QRCodeText2 = {
            "SPC",
            "0200",
            "1",
            "CH4431999123000889012",
            "S",
            "Robert Schneider AG",
            "Rue du Lac",
            "1268",
            "2501",
            "Biel",
            "CH",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "1949.75",
            "CHF",
            "S",
            "Pia-Maria Rutschmann-Schnyder",
            "Grosse Marktgasse",
            "28",
            "9400",
            "Rorschach",
            "CH",
            "QRR",
            "210000000003139471430009017",
            "Order dated 18.06.2020",
            "EPD",
            "//S1/01/20170309/11/10201409/20/14000000/22/36958/30/CH106017086/40/1020/41/3010",
            "UV;UltraPay005;12345",
            "XY;XYService;54321"
        };

        public static string CreateQrCodeText2(string newLine = "\n")
        {
            return string.Join(newLine, QRCodeText2);
        }

        public static Bill CreateBillData2(Bill.QrDataSeparator separator)
        {
            var creditor = new Address
            {
                Name = "Robert Schneider AG",
                Street = "Rue du Lac",
                HouseNo = "1268",
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
                Account = "CH4431999123000889012",
                Creditor = creditor,
                Amount = 1949.75m,
                Currency = "CHF",
                Debtor = debtor,
                Reference = "210000000003139471430009017",
                UnstructuredMessage = "Order dated 18.06.2020",
                BillInformation =
                    "//S1/01/20170309/11/10201409/20/14000000/22/36958/30/CH106017086/40/1020/41/3010",
                AlternativeSchemes = new List<AlternativeScheme>
                {
                    new AlternativeScheme {Name = "Ultraviolet", Instruction = "UV;UltraPay005;12345"},
                    new AlternativeScheme {Name = "Xing Yong", Instruction = "XY;XYService;54321"}
                },
                Format = { Language = Language.EN },
                Separator = separator
            };
            return bill;
        }

        private static readonly string[] QRCodeText3 = {
            "SPC",
            "0200",
            "1",
            "CH3709000000304442225",
            "S",
            "Salvation Army Foundation Switzerland",
            "",
            "",
            "3000",
            "Bern",
            "CH",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "CHF",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "NON",
            "",
            "Donnation to the Winterfest campaign",
            "EPD"
        };

        public static string CreateQrCodeText3(string newLine = "\n")
        {
            return string.Join(newLine, QRCodeText3);
        }

        public static Bill CreateBillData3(Bill.QrDataSeparator separator)
        {
            var creditor = new Address
            {
                Name = "Salvation Army Foundation Switzerland",
                PostalCode = "3000",
                Town = "Bern",
                CountryCode = "CH"
            };
            var bill = new Bill
            {
                Account = "CH37 0900 0000 3044 4222 5",
                Creditor = creditor,
                Currency = "CHF",
                UnstructuredMessage = "Donnation to the Winterfest campaign",
                Format = { Language = Language.EN },
                Separator = separator
            };
            return bill;
        }

        private static readonly string[] QRCodeText4 =
        {
            "SPC",
            "0200",
            "1",
            "CH5800791123000889012",
            "S",
            "Robert Schneider AG",
            "Rue du Lac",
            "1268",
            "2501",
            "Biel",
            "CH",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "199.95",
            "CHF",
            "K",
            "Pia-Maria Rutschmann-Schnyder",
            "Grosse Marktgasse 28",
            "9400 Rorschach",
            "",
            "",
            "CH",
            "SCOR",
            "RF18539007547034",
            "",
            "EPD"
        };

        public static string CreateQrCodeText4(string newLine = "\n")
        {
            return string.Join(newLine, QRCodeText4);
        }

#pragma warning disable CS0618 // Type or member is obsolete
        public static Bill CreateBillData4(Bill.QrDataSeparator separator)
        {
            var creditor = new Address
            {
                Name = "Robert Schneider AG",
                Street = "Rue du Lac",
                HouseNo = "1268",
                PostalCode = "2501",
                Town = "Biel",
                CountryCode = "CH"
            };
            var debtor = new Address
            {
                Name = "Pia-Maria Rutschmann-Schnyder",
                AddressLine1 = "Grosse Marktgasse 28",
                AddressLine2 = "9400 Rorschach",
                CountryCode = "CH"
            };
            var bill = new Bill
            {
                Account = "CH5800791123000889012",
                Creditor = creditor,
                Amount = 199.95m,
                Currency = "CHF",
                Debtor = debtor,
                Reference = "RF18539007547034",
                Format = { Language = Language.EN },
                Separator = separator
            };
            return bill;
        }
#pragma warning restore CS0618 // Type or member is obsolete

        private static readonly string[] QRCodeText5 =
        {
            "SPC",
            "0200",
            "1",
            "CH5800791123000889012",
            "S",
            "Robert Schneider AG",
            "Rue du Lac",
            "1268",
            "2501",
            "Biel",
            "CH",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "0.50",
            "CHF",
            "K",
            "Pia-Maria Rutschmann-Schnyder",
            "Grosse Marktgasse 28",
            "9400 Rorschach",
            "",
            "",
            "CH",
            "SCOR",
            "RF18539007547034",
            "",
            "EPD",
        };

        public static string CreateQrCodeText5(string newLine = "\n")
        {
            return string.Join(newLine, QRCodeText5);
        }

#pragma warning disable CS0618 // Type or member is obsolete
        public static Bill CreateBillData5(Bill.QrDataSeparator separator)
        {
            var creditor = new Address
            {
                Name = "Robert Schneider AG",
                Street = "Rue du Lac",
                HouseNo = "1268",
                PostalCode = "2501",
                Town = "Biel",
                CountryCode = "CH"
            };
            var debtor = new Address
            {
                Name = "Pia-Maria Rutschmann-Schnyder",
                AddressLine1 = "Grosse Marktgasse 28",
                AddressLine2 = "9400 Rorschach",
                CountryCode = "CH"
            };
            var bill = new Bill
            {
                Account = "CH5800791123000889012",
                Creditor = creditor,
                Amount = 0.50m,
                Currency = "CHF",
                Debtor = debtor,
                Reference = "RF18539007547034",
                Format = { Language = Language.EN },
                Separator = separator
            };
            return bill;
        }
#pragma warning restore CS0618 // Type or member is obsolete

        private static readonly string[][] QrCodeTexts =
        {
            QRCodeText1,
            QRCodeText2,
            QRCodeText3,
            QRCodeText4,
            QRCodeText5
        };

        public static string CreateQrCodeText(int sample)
        {
            return CreateQrCodeText(sample, "\n");
        }

        public static string CreateQrCodeText(int sample, string newLine)
        {
            return string.Join(newLine, QrCodeTexts[sample - 1]);
        }

        public static Bill CreateBillData(int sample, Bill.QrDataSeparator separator)
        {
            Bill bill = null;
            switch (sample)
            {
                case 1:
                    bill = CreateBillData1(separator);
                    break;
                case 2:
                    bill = CreateBillData2(separator);
                    break;
                case 3:
                    bill = CreateBillData3(separator);
                    break;
                case 4:
                    bill = CreateBillData4(separator);
                    break;
                case 5:
                    bill = CreateBillData5(separator);
                    break;
            }
            return bill;
        }
    }
}
