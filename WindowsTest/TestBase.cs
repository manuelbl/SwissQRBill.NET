using System;
using System.Runtime.InteropServices;

namespace Codecrete.SwissQRBill.WindowsTest
{
    public class TestBase
    {
        protected TestBase()
        {
            SetProcessDPIAware();
        }


        [DllImport("User32.dll")]
        static extern bool SetProcessDPIAware();
    }
}
