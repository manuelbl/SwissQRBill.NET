﻿//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2021 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System;
using System.IO;
using System.Text;

namespace libQrCodeGenerator.SwissQRBill.PixelCanvas
{
    /// <summary>
    /// Processes PNG imagate data.
    /// </summary>
    public class PngProcessor
    {
        /// <summary>
        /// Modifies the PNG image data by inserting a "pHYs" chunk indicating the resolution.
        /// </summary>
        /// <param name="source">image data, in PNG format</param>
        /// <param name="target">stream to write modified image data</param>
        /// <param name="dpi">resolution, in pixels per inch</param>
        public static void InsertDpi(Stream source, Stream target, int dpi)
        {
            byte[] header = new byte[8];
            int bytesRead;
            byte[] buf = new byte[1024];
            bool physChunkWritten = false;

            // read and write header
            bytesRead = source.Read(header, 0, 8);
            if (bytesRead != 8)
            {
                goto InvalidPngData;
            }

            target.Write(header, 0, 8);

            // process chunks
            while (true)
            {
                // read chunk header
                bytesRead = source.Read(header, 0, 8);

                // check for end-of-file
                if (bytesRead == 0)
                {
                    if (!physChunkWritten)
                    {
                        goto InvalidPngData;
                    }

                    return;
                }
                if (bytesRead != 8)
                {
                    goto InvalidPngData;
                }

                // decode chunk header
                uint chunkLen = LoadBigEndianUInt(header, 0);
                string chunkType = Encoding.ASCII.GetString(header, 4, 4);

                // "pHYs" chunk must be inserted before first "IDAT" chunk
                if (!physChunkWritten && chunkType == "IDAT")
                {
                    WritePhysChunk(target, dpi);
                    physChunkWritten = true;
                }

                // if there is already a "pHYs" chunk, it will be discarded
                bool discardChunk = chunkType == "pHYs";
                if (!discardChunk)
                {
                    target.Write(header, 0, 8);
                }

                // read chunk data and CRC
                int len = (int)chunkLen + 4;
                while (len > 0)
                {
                    bytesRead = source.Read(buf, 0, Math.Min(buf.Length, len));
                    if (bytesRead <= 0)
                    {
                        goto InvalidPngData;
                    }

                    if (!discardChunk)
                    {
                        // copy to target
                        target.Write(buf, 0, bytesRead);
                    }

                    len -= bytesRead;
                }
            }

        InvalidPngData:
            throw new ArgumentException("Invalid PNG image data");
        }

        private static void WritePhysChunk(Stream target, int dpi)
        {
            byte[] chunk = new byte[21];

            // chunk length
            StoreBigEndianUInt(9, chunk, 0);

            // chunk type
            chunk[4] = (byte)'p';
            chunk[5] = (byte)'H';
            chunk[6] = (byte)'Y';
            chunk[7] = (byte)'s';

            // pixels per unit, X axis
            int pixelsPerMeter = (int)Math.Round(dpi / 25.4 * 1000);
            chunk[8] = (byte)(pixelsPerMeter >> 24);
            chunk[9] = (byte)(pixelsPerMeter >> 16);
            chunk[10] = (byte)(pixelsPerMeter >> 8);
            chunk[11] = (byte)pixelsPerMeter;

            // pixels per unit, Y axis
            chunk[12] = chunk[8];
            chunk[13] = chunk[9];
            chunk[14] = chunk[10];
            chunk[15] = chunk[11];

            // unit: 1 = meter
            chunk[16] = 1;

            // CRC
            StoreBigEndianUInt(CalucalteCRC(chunk, 4, 13), chunk, 17);

            target.Write(chunk, 0, 21);
        }

        /// <summary>
        /// Table of CRCs of all 8-bit messages
        /// </summary>
        private static uint[] crcTable;

        /// <summary>
        /// Make the table for a fast CRC.
        /// </summary>
        private static void CreateCrcTable()
        {
            crcTable = new uint[256];

            for (int i = 0; i < 256; i++)
            {
                uint c = (uint)i;
                for (int k = 0; k < 8; k++)
                {
                    if ((c & 1) != 0)
                    {
                        c = 0xEDB88320 ^ (c >> 1);
                    }
                    else
                    {
                        c >>= 1;
                    }
                }
                crcTable[i] = c;
            }
        }

        /// <summary>
        /// Update a running CRC.
        /// </summary>
        /// <param name="crc">current CRC value</param>
        /// <param name="buf">buffer containing the bytes</param>
        /// <param name="offset">offset into the buffer</param>
        /// <param name="count">number of bytes to process</param>
        /// <returns>updated CRC value</returns>
        private static uint UpdateCrc(uint crc, byte[] buf, int offset, int count)
        {
            if (crcTable == null)
            {
                CreateCrcTable();
            }

            for (int i = offset; i < offset + count; i++)
            {
                crc = crcTable[(crc ^ buf[i]) & 0xff] ^ (crc >> 8);
            }

            return crc;
        }

        /// <summary>
        /// Calculates the CRC value for the specified buffer
        /// </summary>
        /// <param name="buf">buffer containing the bytes</param>
        /// <param name="offset">offset into the buffer</param>
        /// <param name="count">number of bytes to process</param>
        /// <returns>calculated CRC value</returns>
        private static uint CalucalteCRC(byte[] buf, int offset, int count)
        {
            return UpdateCrc(0xffffffff, buf, offset, count) ^ 0xffffffff;
        }

        private static uint LoadBigEndianUInt(byte[] buf, int offset)
        {
            uint result = buf[offset++];
            result = (result << 8) | buf[offset++];
            result = (result << 8) | buf[offset++];
            result = (result << 8) | buf[offset++];
            return result;
        }

        private static void StoreBigEndianUInt(uint value, byte[] buf, int offset)
        {
            buf[offset + 0] = (byte)(value >> 24);
            buf[offset + 1] = (byte)(value >> 16);
            buf[offset + 2] = (byte)(value >> 8);
            buf[offset + 3] = (byte)value;
        }
    }
}
