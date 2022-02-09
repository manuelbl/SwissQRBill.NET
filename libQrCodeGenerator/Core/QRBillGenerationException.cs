//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System;
using System.Runtime.Serialization;

namespace libQrCodeGenerator.SwissQRBill.Generator
{
    /// <summary>
    /// Exception thrown if the bill could not be generated.
    /// <para>
    /// If the bill data is not valid, a <see cref="QRBillValidationException"/> exception is thrown instead.
    /// </para>
    /// </summary>
    [Serializable]
    public class QRBillGenerationException : Exception
    {
        /// <summary>
        /// Initializes a new instance with the specified error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public QRBillGenerationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance with the specified error message and a reference to the exception that caused the error.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public QRBillGenerationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <inheritdoc />
        protected QRBillGenerationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
