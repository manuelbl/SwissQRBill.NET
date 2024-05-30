//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2024 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System;

namespace Codecrete.SwissQRBill.Generator
{
    internal static class MathUtil
    {
        /// <summary>
        /// Tests if two floating-point numbers are equal or almost equal.
        /// </summary>
        /// <param name="value1">first number</param>
        /// <param name="value2">second number</param>
        /// <returns><c>true</c> if they are equal or almost equal, <c>false</c> otherwise</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Bug", "S1244:Floating point numbers should not be tested for equality", Justification = "Quick check; full check follows thereafter")]
        public static bool AreClose(double value1, double value2)
        {
            if (value1 == value2)
                return true;
            const double epsilon = 2.22044604925031e-16;
            double num1 = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * epsilon;
            double num2 = value1 - value2;
            if (-num1 < num2)
                return num1 > num2;
            return false;
        }

        /// <summary>
        /// Tests if two floating-point numbers are equal or almost equal.
        /// </summary>
        /// <param name="value1">first number</param>
        /// <param name="value2">second number</param>
        /// <returns><c>true</c> if they are equal or almost equal, <c>false</c> otherwise</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Bug", "S1244:Floating point numbers should not be tested for equality", Justification = "Quick check; full check follows thereafter")]
        public static bool AreClose(float value1, float value2)
        {
            if (value1 == value2)
                return true;
            const float epsilon = 1.192093e-7f;
            float num1 = (Math.Abs(value1) + Math.Abs(value2) + 10.0f) * epsilon;
            float num2 = value1 - value2;
            if (-num1 < num2)
                return num1 > num2;
            return false;
        }
    }
}
