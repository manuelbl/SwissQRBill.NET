//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2021 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System;

namespace Codecrete.SwissQRBill.Generator.Canvas
{
    /// <summary>
    /// 3-by-3 matrix for affine geometric transformation.
    /// </summary>
    public class TransformationMatrix
    {
        // matrix elements in row-major order
        private readonly double[] _elements;

        /// <summary>
        /// Creates a new identity matrix instance.
        /// </summary>
        public TransformationMatrix()
        {
            // identity matrix
            _elements = new double[] { 1, 0, 0, 1, 0, 0 };
        }

        /// <summary>
        /// Matrix elements in row-major order.
        /// <para>
        /// As the elements of the third column are always [ 0, 0, 1 ],
        /// only the first two columns are returned, i.e. 6 elements.
        /// </para>
        /// </summary>
        public double[] Elements
        {
            get { return _elements; }
        }

        /// <summary>
        /// Gets the horizontal translation.
        /// </summary>
        public double TranslationX
        {
            get { return _elements[4]; }
        }

        /// <summary>
        /// Gets the vertical translation.
        /// </summary>
        public double TranslationY
        {
            get { return _elements[5]; }
        }

        /// <summary>
        /// Applies a translation to the matrix (prepend).
        /// </summary>
        /// <param name="dx">horizontal translation</param>
        /// <param name="dy">vertical translation</param>
        public void Translate(double dx, double dy)
        {
            // check for simpler case without scaling or rotation
            if (_elements[0] == 1 && _elements[1] == 0 && _elements[2] == 0 && _elements[3] == 1)
            {
                _elements[4] += dx;
                _elements[5] += dy;
            }
            else
            {
                _elements[4] += _elements[0] * dx + _elements[2] * dy;
                _elements[5] += _elements[1] * dx + _elements[3] * dy;
            }
        }

        /// <summary>
        /// Applies a scaling relative to the origin (prepend).
        /// </summary>
        /// <param name="sx">horizontal scaling</param>
        /// <param name="sy">vertical sclaing</param>
        public void Scale(double sx, double sy)
        {
            if (sx == 1 && sy == 1)
            {
                return;
            }

            _elements[0] *= sx;
            _elements[1] *= sx;
            _elements[2] *= sy;
            _elements[3] *= sy;
        }

        /// <summary>
        /// Applies a rotation about the origin (prepend).
        /// </summary>
        /// <param name="angle">Rotation angle (in radians)</param>
        public void Rotate(double angle)
        {
            if (angle == 0)
            {
                return;
            }

            double c = Math.Cos(angle);
            double s = Math.Sin(angle);
            double e0 = _elements[0];
            double e1 = _elements[1];
            double e2 = _elements[2];
            double e3 = _elements[3];
            _elements[0] = e0 * c + e2 * s;
            _elements[1] = e1 * c + e3 * s;
            _elements[2] = -e0 * s + e2 * c;
            _elements[3] = -e1 * s + e3 * c;
        }
    }
}
