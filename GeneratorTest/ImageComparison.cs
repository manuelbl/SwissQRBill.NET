//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Xunit;

namespace Codecrete.SwissQRBill.GeneratorTest
{
    internal class ImageComparison
    {
        internal static void AssertGrayscaleImageContentEquals(byte[] expectedContent, byte[] actualContent)
        {
            // quick check based on file content
            if (expectedContent.SequenceEqual(actualContent))
            {
                return;
            }

            // create dummy streams
            MemoryStream expectedStream = new MemoryStream(expectedContent);
            MemoryStream actualStream = new MemoryStream(actualContent);

            // read images
            Bitmap expectedImage = new Bitmap(Image.FromStream(expectedStream));
            Bitmap actualImage = new Bitmap(Image.FromStream(actualStream));

            // compare meta data
            Assert.Equal(expectedImage.Width, actualImage.Width);
            Assert.Equal(expectedImage.Height, actualImage.Height);
            Assert.Equal(expectedImage.PixelFormat, actualImage.PixelFormat);

            // retrieve pixels
            int[] expectedPixels = RetrieveImageData(expectedImage, out int stride);
            int[] actualPixels = RetrieveImageData(actualImage, out stride);
            Assert.Equal(expectedPixels.Length, actualPixels.Length);

            // compare pixels
            int length = expectedPixels.Length;
            long diff = 0;
            for (int i = 0; i < length; i++)
            {
                if (expectedPixels[i] != actualPixels[i])
                {
                    int d = Math.Abs(expectedPixels[i] - actualPixels[i]);
                    if (d >= 70)
                    {
                        Assert.True(d < 70, $"singe pixel difference at {i % stride},{i / stride}");
                    }

                    diff += d;
                }
            }

            if (diff > 200000)
            {
                Assert.True(false, $"Pixel value difference too big: {diff}");
            }
        }

        static int[] RetrieveImageData(Bitmap bitmap, out int stride)
        {
            Rectangle size = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData desc = bitmap.LockBits(size, ImageLockMode.ReadOnly, bitmap.PixelFormat);
            stride = desc.Stride;

            int length = stride * bitmap.Height;
            int[] data = new int[length];

            Marshal.Copy(desc.Scan0, data, 0, length);

            bitmap.UnlockBits(desc);

            return data;
        }
    }
}
