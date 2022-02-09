﻿/* 
 * QR code generator library (.NET)
 *
 * Copyright (c) Manuel Bleichenbacher (MIT License)
 * https://github.com/manuelbl/QrCodeGenerator
 * Copyright (c) Project Nayuki (MIT License)
 * https://www.nayuki.io/page/qr-code-generator-library
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
 * IN THE SOFTWARE.
 */

using System;
using System.Diagnostics;

namespace libQrCodeGenerator.QrCodeGenerator
{
    /// <summary>
    /// Computes the Reed-Solomon error correction codewords for a sequence of data codewords at a given degree.
    /// <para>
    /// Instances are immutable, and the state only depends on the degree.
    /// This class is useful because all data blocks in a QR code share the same the divisor polynomial.
    /// </para>
    /// </summary>
    internal class ReedSolomonGenerator
    {
        #region Fields

        // Coefficients of the divisor polynomial, stored from highest to lowest power, excluding the leading term which
        // is always 1. For example the polynomial x^3 + 255x^2 + 8x + 93 is stored as the uint8 array {255, 8, 93}.
        private readonly byte[] _coefficients;

        #endregion


        #region Constructors

        /// <summary>
        /// Initializes a new Reed-Solomon ECC generator for the specified degree. 
        /// </summary>
        /// <remarks>
        /// This could be implemented as a lookup table over all possible parameter values, instead of as an algorithm.
        /// </remarks>
        /// <param name="degree">The divisor polynomial degree (between 1 and 255).</param>
        /// <exception cref="ArgumentOutOfRangeException"><c>degree</c> &lt; 1 or <c>degree</c> &gt; 255</exception>
        internal ReedSolomonGenerator(int degree)
        {
            if (degree < 1 || degree > 255)
            {
                throw new ArgumentOutOfRangeException(nameof(degree), "Degree out of range");
            }

            // Start with the monomial x^0
            _coefficients = new byte[degree];
            _coefficients[degree - 1] = 1;

            // Compute the product polynomial (x - r^0) * (x - r^1) * (x - r^2) * ... * (x - r^{degree-1}),
            // drop the highest term, and store the rest of the coefficients in order of descending powers.
            // Note that r = 0x02, which is a generator element of this field GF(2^8/0x11D).
            uint root = 1;
            for (int i = 0; i < degree; i++)
            {
                // Multiply the current product by (x - r^i)
                for (int j = 0; j < _coefficients.Length; j++)
                {
                    _coefficients[j] = Multiply(_coefficients[j], root);
                    if (j + 1 < _coefficients.Length)
                    {
                        _coefficients[j] ^= _coefficients[j + 1];
                    }
                }
                root = Multiply(root, 0x02);
            }
        }

        #endregion


        #region Methods

        /// <summary>
        /// Computes the Reed-Solomon error correction codewords for the specified
        /// sequence of data codewords.
        /// <para>
        /// This method does not alter this object's state (as it is immutable).
        /// </para>
        /// </summary>
        /// <param name="data">The sequence of data codewords.</param>
        /// <returns>The Reed-Solomon error correction codewords, as a byte array.</returns>
        /// <exception cref="ArgumentNullException">If <c>data</c> is <c>null</c>.</exception>
        internal byte[] GetRemainder(byte[] data)
        {
            Objects.RequireNonNull(data);

            // Compute the remainder by performing polynomial division
            byte[] result = new byte[_coefficients.Length];
            foreach (byte b in data)
            {
                uint factor = (uint)(b ^ result[0]);
                Array.Copy(result, 1, result, 0, result.Length - 1);
                result[result.Length - 1] = 0;
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] ^= Multiply(_coefficients[i], factor);
                }
            }
            return result;
        }

        #endregion


        #region Static functions

        // Returns the product of the two given field elements modulo GF(2^8/0x11D). The arguments and result
        // are unsigned 8-bit integers. This could be implemented as a lookup table of 256*256 entries of uint8.
        private static byte Multiply(uint x, uint y)
        {
            Debug.Assert(x >> 8 == 0 && y >> 8 == 0);
            // Russian peasant multiplication
            uint z = 0;
            for (int i = 7; i >= 0; i--)
            {
                z = (z << 1) ^ ((z >> 7) * 0x11D);
                z ^= ((y >> i) & 1) * x;
            }
            Debug.Assert(z >> 8 == 0);
            return (byte)z;
        }

        #endregion
    }
}
