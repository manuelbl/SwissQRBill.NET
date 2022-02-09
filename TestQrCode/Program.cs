
namespace TestQrCode
{


    static class Program
    {


        public static void ChangeNamespaces()
        {
            string basePath = @"D:\username\Documents\Visual Studio 2017\Projects\SwissQRBill.NET\libQrCodeGenerator";

            System.Collections.Generic.List<string> files = BasicFolderHelper.EnumFolders(basePath);

            foreach (string file in files)
            {
                System.Console.WriteLine(file);
                string content = System.IO.File.ReadAllText(file, System.Text.Encoding.UTF8);
                // content = content.Replace("namespace Net.Codecrete.", "namespace libQrCodeGenerator.");
                // content = content.Replace("namespace Codecrete.SwissQRBill.Generator", "namespace libQrCodeGenerator.SwissQRBill.Generator");
                // content = content.Replace("using Codecrete.SwissQRBill.Generator", "using libQrCodeGenerator.SwissQRBill.Generator");
                // content = content.Replace("using static Codecrete.SwissQRBill.Generator.", "using static libQrCodeGenerator.SwissQRBill.Generator.");
                content = content.Replace("namespace Codecrete.SwissQRBill.PixelCanvas", "namespace libQrCodeGenerator.SwissQRBill.PixelCanvas");

                System.IO.File.WriteAllText(file, content, System.Text.Encoding.UTF8);
            }

        }



        public static byte[] GetQrBill()
        {
            byte[] png = libQrCodeGenerator.Tests.GenerateQrBill();

            return png;
        }


        public static string GetQrBill2()
        {
            byte[] png = null;

            try
            {
                png = libQrCodeGenerator.Tests.GenerateQrBill();
            }
            catch (System.Exception ex)
            {
                return ex.Message + System.Environment.NewLine + ex.StackTrace;
            }
            

            return System.Convert.ToBase64String(png);
        }


        
        // Copy CorQrCode.dll, libQrCodeGenerator.dll to both: 
        // C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE\CommonExtensions\Microsoft\SSRS
        // C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE\PublicAssemblies


        // In Report, Register System.Drawing (NET40), libQrCodeGenerator.dll

        //Function GetQrBill2() As String
        //    Dim png As Byte() = Nothing

        //    Try
        //        png = libQrCodeGenerator.Tests.GenerateQrBill()
        //    Catch ex As System.Exception
        //        Return ex.Message & System.Environment.NewLine & ex.StackTrace
        //    End Try

        //    Return System.Convert.ToBase64String(png)
        //End Function


        // Function GetQrBill() As Byte()
        //      Dim png As Byte() = libQrCodeGenerator.Tests.GenerateQrBill()
        //      Return png
        // End Function



        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [System.STAThread]
        static void Main()
        {
#if false

            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            System.Windows.Forms.Application.Run(new Form1());
#endif


            byte[] png = GetQrBill();
            System.IO.File.WriteAllBytes(@"D:\QrBill.png", png);


            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        }


    }


}
