//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2020 Christian Bernasconi
// Copyright (c) 2020 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System;
using System.Runtime.Serialization;

namespace Codecrete.SwissQRBill.Generator
{
    /// <summary>
    /// Exception thrown if the structured bill information is not valid Swico S1 syntax.
    /// <para>
    /// Also see see http://swiss-qr-invoice.org/downloads/qr-bill-s1-syntax-de.pdf
    /// </para>
    /// </summary>
    [Serializable]
    public class SwicoDecodingException : Exception
    {
        /// <summary>
        /// Initializes a new instance with the specified error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public SwicoDecodingException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance with the specified error message and a reference to the exception that caused the error.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public SwicoDecodingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected SwicoDecodingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
