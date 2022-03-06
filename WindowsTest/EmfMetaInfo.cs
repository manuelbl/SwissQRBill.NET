//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2021 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Codecrete.SwissQRBill.WindowsTest
{
    /// <summary>
    /// Retrieves meta infromation from EMF files.
    /// </summary>
    public class EmfMetaInfo
    {
        /// <summary>
        /// Number of EMF recrods.
        /// </summary>
        public int NumRecords;

        /// <summary>
        /// Frame of metafile (in strange unit).
        /// <para>
        /// The frame is size of the metafile set by the author.
        /// </para>
        /// </summary>
        public EmfRectangle Frame;

        /// <summary>
        /// Bounds of metafile (in strange unit).
        /// <para>
        /// The bounds are the smallest rectangle that will fit
        /// all graphics elements.
        /// </para>
        /// </summary>
        public EmfRectangle Bounds;

        /// <summary>
        /// Size of reference device, in pixel.
        /// </summary>
        public EmfSize DeviceSizePixel;

        /// <summary>
        /// Size of reference device, in millimeters.
        /// </summary>
        public EmfSize DeviceSizeMillimeter;

        /// <summary>
        /// Size of reference device, in micrometer.
        /// </summary>
        public EmfSize DeviceSizeMicrometer;

        /// <summary>
        /// DPI for pixel unit of metafile.
        /// </summary>
        public int Dpi;

        /// <summary>
        /// Creates a new meta info instance.
        /// </summary>
        /// <param name="emfFile">EMF file as byte array</param>
        public EmfMetaInfo(byte[] emfFile)
        {
            Read(emfFile);
        }

        /// <summary>
        /// Gets the metafile frame in pixels.
        /// <para>
        /// The frame is the metafile size set by the author.
        /// </para>
        /// <para>
        /// This member function is similar to <c>Metafile.GetBounds()</c>.
        /// </para>
        /// </summary>
        /// <returns>frame rectangle</returns>
        public RectangleF GetFrame()
        {
            float deviceDpiX = DeviceSizePixel.Width / (DeviceSizeMicrometer.Width / 25.4f / 1000f);
            float deviceDpiY = DeviceSizePixel.Height / (DeviceSizeMicrometer.Height / 25.4f / 1000f);
            float left = Frame.Left / (25.4f * 100) * deviceDpiX;
            float right = Frame.Right / (25.4f * 100) * deviceDpiX;
            float top = Frame.Top / (25.4f * 100) * deviceDpiY;
            float bottom = Frame.Bottom / (25.4f * 100) * deviceDpiY;

            return new RectangleF(left, top, right - left, bottom - top);
        }

        private void Read(byte[] emfFile)
        {
            int size = emfFile.Length;
            int offset = 0;

            while (offset < size)
            {
                // decode record header
                var rec = ByteArrayToStructure<EmfRecord>(emfFile, offset);
                NumRecords++;

                if (rec.Type == (int)EmfRecordType.Header)
                {
                    // decode EMF_HEADER
                    var header = ByteArrayToStructure<EmfHeader>(emfFile, offset);
                    Bounds = header.Bounds;
                    Frame = header.Frame;
                    DeviceSizePixel = header.Device;
                    DeviceSizeMillimeter = header.Millimeters;
                    DeviceSizeMicrometer = header.Micrometers;
                }
                else if (rec.Type == (int)EmfRecordType.Comment)
                {
                    // decode EMF_COMMENT
                    var comment = ByteArrayToStructure<EmfComment>(emfFile, offset);
                    if (comment.CommentIdentifier == 0x2B464D45)
                    {
                        // process EMF+ records embedded in comment
                        ProcessEmfPlusRecords(emfFile, offset + 16, offset + 16 + (int)comment.DataSize);
                    }
                }

                offset += (int)rec.Size;
            }
        }

        /// <param name="endOffset"></param>
        private void ProcessEmfPlusRecords(byte[] emfFile, int offset, int endOffset)
        {
            while (offset < endOffset)
            {
                // decode EMF+ record header
                var rec = ByteArrayToStructure<EmfPlusRecord>(emfFile, offset);

                // EMF+ header
                if (rec.Type == 0x4001)
                {
                    var header = ByteArrayToStructure<EmfPlusHeader>(emfFile, offset);
                    Dpi = (int)header.LogicalDpiX;
                }

                offset += (int)rec.Size;
            }
        }

        static T ByteArrayToStructure<T>(byte[] bytes, int offset) where T : struct
        {
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                return Marshal.PtrToStructure<T>(IntPtr.Add(handle.AddrOfPinnedObject(), offset));
            }
            finally
            {
                handle.Free();
            }
        }

        internal enum EmfRecordType
        {
            Header = 1,
            Comment = 0x46
        }

        [StructLayout(LayoutKind.Explicit, Size = 16)]
        public struct EmfRectangle
        {
            [MarshalAs(UnmanagedType.I4)]
            [FieldOffset(0)]
            public int Left;
            [MarshalAs(UnmanagedType.I4)]
            [FieldOffset(4)]
            public int Top;
            [MarshalAs(UnmanagedType.I4)]
            [FieldOffset(8)]
            public int Right;
            [MarshalAs(UnmanagedType.I4)]
            [FieldOffset(12)]
            public int Bottom;
        }

        [StructLayout(LayoutKind.Explicit, Size = 8)]
        public struct EmfSize
        {
            [MarshalAs(UnmanagedType.I4)]
            [FieldOffset(0)]
            public int Width;
            [MarshalAs(UnmanagedType.I4)]
            [FieldOffset(4)]
            public int Height;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct EmfRecord
        {
            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(0)]
            internal uint Type;

            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(4)]
            internal uint Size;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct EmfComment
        {
            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(0)]
            internal uint Type;

            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(4)]
            internal uint Size;

            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(8)]
            internal uint DataSize;

            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(12)]
            internal uint CommentIdentifier;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct EmfHeader
        {
            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(0)]
            internal uint Type;

            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(4)]
            internal uint Size;

            [FieldOffset(8)]
            internal EmfRectangle Bounds;

            [FieldOffset(24)]
            internal EmfRectangle Frame;

            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(40)]
            readonly uint Signature;

            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(44)]
            readonly uint Version;

            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(48)]
            readonly uint NumBytes;

            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(52)]
            readonly uint NumRecords;

            [MarshalAs(UnmanagedType.U2)]
            [FieldOffset(56)]
            readonly ushort NumHandles;

            [MarshalAs(UnmanagedType.U2)]
            [FieldOffset(58)]
            readonly ushort Reserved;

            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(60)]
            readonly uint NumDescription;

            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(64)]
            readonly uint OffDescription;

            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(68)]
            readonly uint PalEntries;

            [FieldOffset(72)]
            internal EmfSize Device;

            [FieldOffset(80)]
            internal EmfSize Millimeters;

            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(88)]
            readonly uint PixelFormat;

            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(92)]
            readonly uint OffPixelFormat;

            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(96)]
            readonly uint OpenGL;

            [FieldOffset(100)]
            internal EmfSize Micrometers;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct EmfPlusRecord
        {
            [MarshalAs(UnmanagedType.U2)]
            [FieldOffset(0)]
            internal ushort Type;

            [MarshalAs(UnmanagedType.U2)]
            [FieldOffset(2)]
            internal ushort Flags;

            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(4)]
            internal uint Size;

            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(8)]
            internal uint DataSize;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct EmfPlusHeader
        {
            [MarshalAs(UnmanagedType.U2)]
            [FieldOffset(0)]
            internal ushort Type;

            [MarshalAs(UnmanagedType.U2)]
            [FieldOffset(2)]
            internal ushort Flags;

            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(4)]
            internal uint Size;

            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(8)]
            internal uint DataSize;

            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(12)]
            internal uint Version;

            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(16)]
            internal uint EmfPlusFlags;

            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(20)]
            internal uint LogicalDpiX;

            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(24)]
            internal uint LogicalDpiY;
        }

    }
}
