using Codecrete.SwissQRBill.Generator;
using System;
using System.Collections.Generic;

namespace Codecrete.SwissQRBill.CoreTest
{
    internal static class SwicoExamples
    {
        internal static SwicoBillInformation CreateExample1()
        {
            return new SwicoBillInformation
            {
                InvoiceNumber = "10201409",
                InvoiceDate = new DateTime(2019, 5, 12),
                CustomerReference = "1400.000-53",
                VatNumber = "106017086",
                VatDate = new DateTime(2018, 5, 8),
                VatRate = 7.7m,
                PaymentConditions = new List<(decimal, int)> { (2m, 10), (0m, 30) }
            };
        }

        internal const string Example1Text =
                "//S1/10/10201409/11/190512/20/1400.000-53/30/106017086/31/180508/32/7.7/40/2:10;0:30";

        internal static SwicoBillInformation CreateExample2()
        {
            return new SwicoBillInformation
            {
                InvoiceNumber = "10104",
                InvoiceDate = new DateTime(2018, 2, 28),
                VatNumber = "395856455",
                VatStartDate = new DateTime(2018, 2, 26),
                VatEndDate = new DateTime(2018, 2, 27),
                VatRateDetails = new List<(decimal, decimal)> { (3.7m, 400.19m), (7.7m, 553.39m), (0m, 14m) },
                PaymentConditions = new List<(decimal, int)> { (0m, 30) }
            };
        }

        internal const string Example2Text =
                "//S1/10/10104/11/180228/30/395856455/31/180226180227/32/3.7:400.19;7.7:553.39;0:14/40/0:30";

        internal static SwicoBillInformation CreateExample3()
        {
            return new SwicoBillInformation
            {
                InvoiceNumber = "4031202511",
                InvoiceDate = new DateTime(2018, 1, 7),
                CustomerReference = "61257233.4",
                VatNumber = "105493567",
                VatRateDetails = new List<(decimal, decimal)> { (8m, 49.82m) },
                VatImportTaxes = new List<(decimal, decimal)> { (2.5m, 14.85m) },
                PaymentConditions = new List<(decimal, int)> { (0m, 30) }
            };
        }

        internal const string Example3Text =
                "//S1/10/4031202511/11/180107/20/61257233.4/30/105493567/32/8:49.82/33/2.5:14.85/40/0:30";

        internal static SwicoBillInformation CreateExample4()
        {
            return new SwicoBillInformation
            {
                InvoiceNumber = "X.66711/8824",
                InvoiceDate = new DateTime(2020, 7, 12),
                CustomerReference = "MW-2020-04",
                VatNumber = "107978798",
                VatRateDetails = new List<(decimal, decimal)> { (2.5m, 117.22m) },
                PaymentConditions = new List<(decimal, int)> { (3m, 5), (1.5m, 20), (1m, 40), (0m, 60) }
            };
        }

        internal const string Example4Text =
                @"//S1/10/X.66711\/8824/11/200712/20/MW-2020-04/30/107978798/32/2.5:117.22/40/3:5;1.5:20;1:40;0:60";

        internal static SwicoBillInformation CreateExample5()
        {
            return new SwicoBillInformation
            {
                InvoiceNumber = "79269",
                InvoiceDate = new DateTime(2020, 7, 14),
                CustomerReference = "66359",
                VatNumber = "109532551",
                VatRate = 7.7m,
                PaymentConditions = new List<(decimal, int)> { (0m, 30) }
            };
        }

        internal const string Example5Text =
            "//S1/10/79269/11/200714210713/20/66359/30/109532551/32/7.7/40/0:30";

        internal static SwicoBillInformation CreateExample6()
        {
            return new SwicoBillInformation
            {
                InvoiceNumber = "802277",
                InvoiceDate = new DateTime(2020, 7, 1),
                CustomerReference = "55878",
                VatNumber = "109532551",
                VatRate = 7.7m,
                PaymentConditions = new List<(decimal, int)> { (0m, 30) }
            };
        }

        internal const string Example6Text =
            "//S1/10/802277/11/2007012107/20/55878/30/109532551/32/7.7/40/0:30";
    }
}
