//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2021 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Codecrete.SwissQRBill.Windows
{

    /// <summary>
    /// Helper class to copy metafiles to the clipboard.
    /// <para>
    /// The .NET classes <c>Clipboard</c> and <c>DataObject</c> are unable
    /// to properly deal with metafiles.
    /// </para>
    /// </summary>
    public class ClipboardMetafileHelper
    {
        private enum TernaryRasterOperations : uint
        {
            SRCCOPY = 0x00CC0020,
            SRCPAINT = 0x00EE0086,
            SRCAND = 0x008800C6,
            SRCINVERT = 0x00660046,
            SRCERASE = 0x00440328,
            NOTSRCCOPY = 0x00330008,
            NOTSRCERASE = 0x001100A6,
            MERGECOPY = 0x00C000CA,
            MERGEPAINT = 0x00BB0226,
            PATCOPY = 0x00F00021,
            PATPAINT = 0x00FB0A09,
            PATINVERT = 0x005A0049,
            DSTINVERT = 0x00550009,
            BLACKNESS = 0x00000042,
            WHITENESS = 0x00FF0062,
            CAPTUREBLT = 0x40000000
        }

        [DllImport("user32.dll")]
        private static extern bool CloseClipboard();
        [DllImport("user32.dll")]
        private static extern bool EmptyClipboard();
        [DllImport("user32.dll")]
        private static extern bool OpenClipboard(IntPtr hWndNewOwner);
        [DllImport("user32.dll")]
        private static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("user32.dll")]
        private static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);
        [DllImport("gdi32.dll", EntryPoint = "BitBlt", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool BitBlt([In] IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, [In] IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);
        [DllImport("gdi32.dll")]
        private static extern IntPtr CopyEnhMetaFile(IntPtr hemfSrc, IntPtr hNULL);
        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
        private static extern IntPtr CreateCompatibleBitmap([In] IntPtr hdc, int nWidth, int nHeight);
        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC", SetLastError = true)]
        private static extern IntPtr CreateCompatibleDC([In] IntPtr hdc);
        [DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
        private static extern bool DeleteDC([In] IntPtr hdc);
        [DllImport("gdi32.dll")]
        private static extern bool DeleteEnhMetaFile(IntPtr hemf);
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteObject([In] IntPtr hObject);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetDC(IntPtr hWnd);
        [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
        private static extern IntPtr SelectObject([In] IntPtr hdc, [In] IntPtr hgdiobj);

        /// <summary>
        /// Puts the specified metafile and optionally the specified bitmap on the clipboard.
        /// <para>
        /// The metafile and the bitmap are expected to represent the same image.
        /// </para>
        /// </summary>
        /// <param name="hWnd">A Windows handle (used to access the clipboard)</param>
        /// <param name="metafile">The metafile.</param>
        /// <param name="bitmap">The bitmap, or <c>null</c>.</param>
        /// <returns><c>true</c> if successful, <c>false</c> otherwise</returns>
        public static bool PutOnClipboard(IntPtr hWnd, Metafile metafile, Bitmap bitmap)
        {
            var hEmf = IntPtr.Zero;
            var hBitmap = IntPtr.Zero;
            var hResEmf = IntPtr.Zero;
            var hResBitmap = IntPtr.Zero;

            var hPassedEmf = metafile.GetHenhmetafile();
            if (!hPassedEmf.Equals(IntPtr.Zero))
            {
                // create a copy of the meta file
                hEmf = CopyEnhMetaFile(hPassedEmf, IntPtr.Zero);

                // create a copy of the bitmap
                if (bitmap != null)
                {
                    hBitmap = CopyBitmap(bitmap);
                }
            }

            if (!hEmf.Equals(IntPtr.Zero) && (bitmap == null || !hBitmap.Equals(IntPtr.Zero)))
            {
                // empty clipboard and put new data there
                if (OpenClipboard(hWnd))
                {
                    if (EmptyClipboard())
                    {
                        hResEmf = SetClipboardData(14 /* CF_ENHMETAFILE */, hEmf);
                        if (bitmap != null)
                            hResBitmap = SetClipboardData(2 /* CF_BITMAP */, hBitmap);
                    }

                    CloseClipboard();
                }

                if (hResEmf.Equals(hEmf) && (bitmap == null || hResBitmap.Equals(hBitmap)))
                    return true;
            }

            if (!hResEmf.Equals(hEmf))
                DeleteEnhMetaFile(hEmf);
            if (bitmap != null && !hResBitmap.Equals(hBitmap))
                DeleteObject(hBitmap);

            return false;
        }


        private static IntPtr CopyBitmap(Bitmap bitmap)
        {
            var hScreenDC = GetDC(IntPtr.Zero);

            // setup source CD
            var hSourceBitmap = bitmap.GetHbitmap();
            var hSourceDC = CreateCompatibleDC(hScreenDC);
            var hPrevSourceGdiObj = SelectObject(hSourceDC, hSourceBitmap);

            // create bitmap and associated DC
            var hDestBitmap = CreateCompatibleBitmap(hScreenDC, bitmap.Width, bitmap.Height);
            var hDestDC = CreateCompatibleDC(hScreenDC);
            var hPrevDestGdiObj = SelectObject(hDestDC, hDestBitmap);

            // copy image
            BitBlt(hDestDC, 0, 0, bitmap.Width, bitmap.Height, hSourceDC, 0, 0, TernaryRasterOperations.SRCCOPY);

            // cleanup
            SelectObject(hSourceDC, hPrevSourceGdiObj);
            DeleteDC(hSourceDC);
            SelectObject(hDestDC, hPrevDestGdiObj);
            DeleteDC(hDestDC);
            ReleaseDC(IntPtr.Zero, hScreenDC);
            DeleteObject(hSourceBitmap);

            return hDestBitmap;
        }
    }
}
