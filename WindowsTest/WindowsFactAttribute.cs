using System.Runtime.InteropServices;
using Xunit;

namespace Codecrete.SwissQRBill.WindowsTest
{
    public sealed class WindowsFactAttribute : FactAttribute
    {
        private static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public WindowsFactAttribute()
        {
            if (!IsWindows)
            {
                Skip = "Only supported on Windows";
            }
        }
    }
}
