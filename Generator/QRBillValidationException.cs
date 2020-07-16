//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System;
using System.Runtime.Serialization;

namespace Codecrete.SwissQRBill.Generator
{
    /// <summary>
    /// Exception thrown if the bill data is not valid.
    /// </summary>
    [Serializable]
    public class QRBillValidationException : Exception
    {
        [NonSerialized]
        private readonly ValidationResult _result;

        /// <summary>
        /// Initializes a new instance with the specified validation result.
        /// </summary>
        /// <param name="result">The validation result.</param>
        public QRBillValidationException(ValidationResult result)
            : base("QR bill data is invalid")
        {
            _result = result;
        }

        protected QRBillValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets the validation result with the error messages.
        /// </summary>
        /// <value>The validation result.</value>
        public ValidationResult Result => _result;

        public override string Message => "QR bill data is invalid: " + _result.Description;
    }
}
