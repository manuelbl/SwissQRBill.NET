using System.IO;

namespace Codecrete.SwissQRBill.GeneratorTest
{
    public static class StreamExtensions
    {
        public static byte[] ToArray(this Stream stream)
        {
            if (stream is MemoryStream memoryStream)
            {
                return memoryStream.ToArray();
            }

            using var tempStream = new MemoryStream();
            stream.CopyTo(tempStream);
            return tempStream.ToArray();
        }
    }
}