//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2020 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//
using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFsharp
{
    /// <summary>
    /// Wrapper for PDFsharp's XGraphicsPath to work around a bug
    /// </summary>
    /// <para>
    /// <c>XGraphicsPath.StartFigure()</c> is not correctly implemented. It does nothing.
    /// This prevents the use of subpaths that are not closed.
    /// </para>
    /// <para>
    /// This wrapper records the entire path. Once it's clear if the path is filled or stroked,
    /// it is replayed 1:1 (filled path) or split into separate paths (stroked path).
    /// </para>
    class GraphicsPath
    {
        enum Command
        {
            MoveTo,
            LineTo,
            CubicCurveTo,
            CloseSubpath,
            Rectangle
        }

        private List<XPoint> Points = new List<XPoint>();
        private List<Command> Commands = new List<Command>();


        public void MoveTo(XPoint pt)
        {
            Points.Add(pt);
            Commands.Add(Command.MoveTo);
        }

        public void LineTo(XPoint pt)
        {
            Points.Add(pt);
            Commands.Add(Command.LineTo);
        }

        public void CubicCurveTo(XPoint pt1, XPoint pt2, XPoint pt3)
        {
            Points.Add(pt1);
            Points.Add(pt2);
            Points.Add(pt3);
            Commands.Add(Command.CubicCurveTo);
        }

        public void AddRectangle(XPoint pt, double width, double height)
        {
            Points.Add(new XPoint(width, height));
            Points.Add(pt);
            Commands.Add(Command.Rectangle);
        }

        public void CloseSubpath()
        {
            Commands.Add(Command.CloseSubpath);
        }

        public void Fill(XGraphics graphics, XBrush brush)
        {
            XGraphicsPath path = new XGraphicsPath();
            path.FillMode = XFillMode.Winding;
            bool hasFigure = false;
            int ptIndex = 0;

            foreach (Command command in Commands)
            {

                switch (command)
                {
                    case Command.MoveTo:
                        if (hasFigure)
                        {
                            path.CloseFigure();
                            hasFigure = false;
                        }
                        ptIndex++;
                        break;

                    case Command.LineTo:
                        path.AddLine(Points[ptIndex - 1], Points[ptIndex]);
                        ptIndex += 1;
                        hasFigure = true;
                        break;

                    case Command.CubicCurveTo:
                        path.AddBezier(Points[ptIndex - 1], Points[ptIndex], Points[ptIndex + 1], Points[ptIndex + 2]);
                        ptIndex += 3;
                        hasFigure = true;
                        break;

                    case Command.CloseSubpath:
                        if (hasFigure)
                        {
                            path.CloseFigure();
                            hasFigure = false;
                        }
                        break;

                    case Command.Rectangle:
                        path.AddRectangle(Points[ptIndex + 1].X, Points[ptIndex + 1].Y, Points[ptIndex].X, Points[ptIndex].Y);
                        ptIndex += 2;
                        path.CloseFigure();
                        hasFigure = false;
                        break;
                }
            }

            if (hasFigure)
                path.CloseFigure();

            graphics.DrawPath(brush, path);
        }

        public void Stroke(XGraphics graphics, XPen pen)
        {
            XGraphicsPath path = new XGraphicsPath();
            bool hasFigure = false;
            int ptIndex = 0;

            foreach (Command command in Commands)
            {

                switch (command)
                {
                    case Command.MoveTo:
                        if (hasFigure)
                        {
                            graphics.DrawPath(pen, path);
                            path = new XGraphicsPath();
                            hasFigure = false;
                        }
                        ptIndex++;
                        break;

                    case Command.LineTo:
                        path.AddLine(Points[ptIndex - 1], Points[ptIndex]);
                        ptIndex += 1;
                        hasFigure = true;
                        break;

                    case Command.CubicCurveTo:
                        path.AddBezier(Points[ptIndex - 1], Points[ptIndex], Points[ptIndex + 1], Points[ptIndex + 2]);
                        ptIndex += 3;
                        hasFigure = true;
                        break;

                    case Command.CloseSubpath:
                        if (hasFigure)
                        {
                            path.CloseFigure();
                            hasFigure = false;
                        }
                        graphics.DrawPath(pen, path);
                        path = new XGraphicsPath();
                        break;

                    case Command.Rectangle:
                        path.AddRectangle(Points[ptIndex + 1].X, Points[ptIndex + 1].Y, Points[ptIndex].X, Points[ptIndex].Y);
                        ptIndex += 2;
                        path.CloseFigure();
                        hasFigure = false;
                        break;
                }
            }

            graphics.DrawPath(pen, path);
        }
    }
}
