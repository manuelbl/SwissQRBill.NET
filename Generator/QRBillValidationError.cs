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
    /// Exception thrown if the bill data is not valid.
    /// </summary>
    public class QRBillValidationError : Exception
    {

        /// <summary>
        /// Initializes a new instance with the specified validation result
        /// </summary>
        /// <param name="result">validation result</param>
        public QRBillValidationError(ValidationResult result)
            : base("QR bill data is invalid")
        {
            Result = result;
        }

        /// <summary>
        /// Gets the validation result with the error messages.
        /// </summary>
        public ValidationResult Result { get; }
    }
}
