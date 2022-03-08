//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator.Canvas;
using Net.Codecrete.QrCodeGenerator;
using System;

namespace Codecrete.SwissQRBill.Generator
{
    /// <summary>
    /// Generates the QR code for the Swiss QR bill.
    /// <para>
    /// Also provides functions to generate and decode the string embedded in the QR code.
    /// </para>
    /// </summary>
    internal class QRCode
    {
        internal static readonly double Size = 46; // mm

        private readonly string _embeddedText;

        /// <summary>
        /// Creates an instance of the QR code for the specified bill data.
        /// <para>
        /// The bill data must have been validated and cleaned.
        /// </para>
        /// </summary>
        /// <param name="bill">The bill data.</param>
        internal QRCode(Bill bill)
        {
            _embeddedText = QRCodeText.Create(bill);
        }

        /// <summary>
        /// Draws the QR code to the specified graphics context (canvas). The QR code will
        /// always be 46 mm by 46 mm.
        /// </summary>
        /// <param name="graphics">The graphics context.</param>
        /// <param name="offsetX">The x offset.</param>
        /// <param name="offsetY">The y offset.</param>
        internal void Draw(ICanvas graphics, double offsetX, double offsetY)
        {
            QrCode qrCode = QrCode.EncodeText(_embeddedText, QrCode.Ecc.Medium);

            bool[,] modules = CopyModules(qrCode);
            ClearSwissCrossArea(modules);

            int modulesPerSide = modules.GetLength(0);
            graphics.SetTransformation(offsetX, offsetY, 0, Size / modulesPerSide / 25.4 * 72, Size / modulesPerSide / 25.4 * 72);
            graphics.StartPath();
            DrawModulesPath(graphics, modules);
            graphics.FillPath(0, false);
            graphics.SetTransformation(offsetX, offsetY, 0, 1, 1);

            // Swiss cross
            graphics.StartPath();
            graphics.AddRectangle(20, 20, 6, 6);
            graphics.FillPath(0, false);
            const double barWidth = 7 / 6.0;
            const double barLength = 35 / 9.0;
            
            graphics.StartPath();
            //       A----B
            //       |    |
            //       |    |
            // K-----L    C-----D
            // |                |
            // |                |
            // J-----I    F-----E
            //       |    |
            //       |    |
            //       H----G
            
            // Center is (23;23)
            // Start in A
            graphics.MoveTo(23 - barWidth / 2, 23 - barLength / 2);

            // Line to B
            graphics.LineTo(23 + barWidth / 2, 23 - barLength / 2);

            // Line to C
            graphics.LineTo(23 + barWidth / 2, 23 - barWidth / 2);
            
            // Line to D
            graphics.LineTo(23 + barLength / 2, 23 - barWidth / 2);

            // Line to E
            graphics.LineTo(23 + barLength / 2, 23 + barWidth / 2);
            
            // Line to F
            graphics.LineTo(23 + barWidth / 2, 23 + barWidth / 2);
            
            // Line to G
            graphics.LineTo(23 + barWidth / 2, 23 + barLength / 2);
            
            // Line to H
            graphics.LineTo(23 - barWidth / 2, 23 + barLength / 2);
            
            // Line to I
            graphics.LineTo(23 - barWidth / 2, 23 + barWidth / 2);
            
            // Line to J
            graphics.LineTo(23 - barLength / 2, 23 + barWidth / 2);
            
            // Line to K
            graphics.LineTo(23 - barLength / 2, 23 - barWidth / 2);
            
            // Line to K
            graphics.LineTo(23 - barWidth / 2, 23 - barWidth / 2);
            
            graphics.FillPath(0xffffff, false);
        }

        private static void DrawModulesPath(ICanvas graphics, bool[,] modules)
        {
            // Simple algorithm to reduce the number of drawn rectangles
            int size = modules.GetLength(0);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if (modules[y, x])
                    {
                        DrawLargestRectangle(graphics, modules, x, y);
                    }
                }
            }
        }

        // Simple algorithms to reduce the number of rectangles for drawing the QR code
        // and reduce SVG size
        private static void DrawLargestRectangle(ICanvas graphics, bool[,] modules, int x, int y)
        {
            int size = modules.GetLength(0);

            int bestW = 1;
            int bestH = 1;
            int maxArea = 1;

            int xLimit = size;
            int iy = y;
            while (iy < size && modules[iy, x])
            {
                int w = 0;
                while (x + w < xLimit && modules[iy, x + w])
                {
                    w++;
                }

                int area = w * (iy - y + 1);
                if (area > maxArea)
                {
                    maxArea = area;
                    bestW = w;
                    bestH = iy - y + 1;
                }
                xLimit = x + w;
                iy++;
            }

            double unit = 25.4 / 72;
            graphics.AddRectangle(x * unit, (size - y - bestH) * unit, bestW * unit, bestH * unit);
            ClearRectangle(modules, x, y, bestW, bestH);
        }

        private static void ClearSwissCrossArea(bool[,] modules)
        {
            // The Swiss cross area is supposed to be 7 by 7 mm in the center of
            // the QR code, which is 46 by 46 mm.
            // We clear sufficient modules to make room for the cross.
            int size = modules.GetLength(0);
            int start = (int)Math.Floor((46 - 6.8) / 2 * size / 46);
            ClearRectangle(modules, start, start, size - 2 * start, size - 2 * start);
        }

        private static bool[,] CopyModules(QrCode qrCode)
        {
            int size = qrCode.Size;
            bool[,] modules = new bool[size, size];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    modules[y, x] = qrCode.GetModule(x, y);
                }
            }

            return modules;
        }

        private static void ClearRectangle(bool[,] modules, int x, int y, int width, int height)
        {
            for (int iy = y; iy < y + height; iy++)
            {
                for (int ix = x; ix < x + width; ix++)
                {
                    modules[iy, ix] = false;
                }
            }
        }
    }
}
