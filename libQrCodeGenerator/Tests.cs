
using libQrCodeGenerator.SwissQRBill.Generator;
using libQrCodeGenerator.SwissQRBill.PixelCanvas;


namespace libQrCodeGenerator
{


    public class Tests
    {


        private static Bill CreateExample1()
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
                BillInformation =
                    "//S1/10/10201409/11/190512/20/1400.000-53/30/106017086/31/180508/32/7.7/40/2:10;0:30",
                AlternativeSchemes = new System.Collections.Generic.List<AlternativeScheme>
                {
                    new AlternativeScheme {Name = "Ultraviolet", Instruction = "UV;UltraPay005;12345"},
                    new AlternativeScheme {Name = "Xing Yong", Instruction = "XY;XYService;54321"}
                },
                Format = { Language = Language.EN }
            };
            return bill;
        }


        public static byte[] GenerateQrBill()
        {
            byte[] png;

            Bill bill = CreateExample1();

            // using (PNGCanvas canvas = new PNGCanvas(QRBill.QrBillWidth, QRBill.QrBillHeight, 300, "\"Liberation Sans\",Arial, Helvetica"))
            // const int dpi = 192;
            // using (PNGCanvas canvas = new PNGCanvas(QRBill.QrCodeWidth/25.4* dpi, QRBill.QrCodeHeight/25.4* dpi, dpi, "Arial"))
            // using (PNGCanvas canvas = new PNGCanvas(QRBill.QrBillWidth / 25.4 * dpi, QRBill.QrBillHeight / 25.4 * dpi, dpi, "Arial"))
            // using (PNGCanvas canvas = new PNGCanvas(QRBill.QrBillWidth, QRBill.QrBillHeight, 192, "Arial"))


            float displayResolution = 0;

            using (System.Drawing.Image img = new System.Drawing.Bitmap(1, 1))
            {
                // to set the actual width, multiply img.Resolution by width_in_mm/QRBill.QrCodeWidth
                displayResolution = img.HorizontalResolution* 2;
            }


            using (PNGCanvas canvas = new PNGCanvas(QRBill.QrCodeWidth, QRBill.QrCodeHeight, displayResolution, "Arial"))
            {
                // bill.Format.OutputSize = OutputSize.QrBillOnly;
                bill.Format.OutputSize = OutputSize.QrCodeOnly;
                QRBill.Draw(bill, canvas);
                png = canvas.ToByteArray();
            }

            return png;
        }
        

    }


}
