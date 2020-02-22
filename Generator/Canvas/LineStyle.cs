//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2020 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System;

namespace Codecrete.SwissQRBill.Generator.Canvas
{
    /// <summary>
    /// Line style
    /// </summary>
    public enum LineStyle
    {
        /// <summary>
        /// Solid line
        /// </summary>
        Solid,
        /// <summary>
        /// Dashed line (dashes are about 4 times the line width long and apart)
        /// </summary>
        Dashed,
        /// <summary>
        /// Dotted line (dots are spaced 3 times the line width apart)
        /// </summary>
        Dotted
    }
}
