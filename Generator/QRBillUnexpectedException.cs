//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//
using System;

namespace Codecrete.SwissQRBill.Generator
{
    /// <summary>
    /// Exception thrown if the bill could not be generated.
    /// </summary>
    public class QRBillUnexpectedException : Exception
    {
        /// <summary>
        /// Initializes a new instance with the specified error message
        /// </summary>
        /// <param name="message">error message</param>
        public QRBillUnexpectedException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance with the specified error message and a reference to the exception that caused the error
        /// </summary>
        /// <param name="message">error message</param>
        /// <param name="innerException">inner exception</param>
        public QRBillUnexpectedException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
