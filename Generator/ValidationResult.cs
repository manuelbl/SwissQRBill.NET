//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System.Collections.Generic;

namespace Codecrete.SwissQRBill.Generator
{
    /// <summary>
    /// The validation result, consisting of a list of validation messages
    /// and flags indicating the validation state.
    /// </summary>
    public sealed class ValidationResult
    {
        private static readonly List<ValidationMessage> EmptyList = new List<ValidationMessage>();

        private List<ValidationMessage> _validationMessages;

        /// <summary>
        /// Gets the list of validation messages.
        /// <para>
        /// The result is never <c>null</c>.
        /// </para>
        /// </summary>
        /// <value>The validation message list.</value>
        public List<ValidationMessage> ValidationMessages => _validationMessages ?? EmptyList;

        /// <summary>
        /// Gets if this validation result contains any messages.
        /// </summary>
        /// <value><c>true</c> if the result contains any messages, <c>false</c> otherwise.</value>
        public bool HasMessages => _validationMessages != null;

        /// <summary>
        /// Gets if this validation result contains any warning messages.
        /// </summary>
        /// <value><c>true</c> if the result contains any warning messages, <c>false</c> otherwise.</value>
        public bool HasWarnings
        {
            get
            {
                if (_validationMessages == null)
                {
                    return false;
                }

                foreach (ValidationMessage message in _validationMessages)
                {
                    if (message.Type == ValidationMessage.MessageType.Warning)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Gets if this validation result contains any error messages.
        /// </summary>
        /// <value><c>true</c> if the result contains any messages, <c>false</c> otherwise.</value>
        public bool HasErrors
        {
            get
            {
                if (_validationMessages == null)
                {
                    return false;
                }

                foreach (ValidationMessage message in _validationMessages)
                {
                    if (message.Type == ValidationMessage.MessageType.Error)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Gets if the bill data is valid and the validation therefore has succeeded.
        /// <para>
        /// A successful validation may still produce warning messages.
        /// </para>
        /// </summary>
        /// <value><c>true</c> if the validation has succeeded, <c>false</c> otherwise.</value>
        public bool IsValid => !HasErrors;

        /// <summary>
        /// Adds a new validation message to this validation result.
        /// </summary>
        /// <param name="type">The message type.</param>
        /// <param name="field">The name of the affected field.</param>
        /// <param name="messageKey">The language-netural message key.</param>
        /// <param name="messageParameters">The optional message parameters (text) to be inserted into the localized message.</param>
        /// <seealso cref="ValidationMessage"/>
        public void AddMessage(ValidationMessage.MessageType type, string field, string messageKey, string[] messageParameters = null)
        {
            ValidationMessage message = new ValidationMessage(type, field, messageKey, messageParameters);
            if (_validationMessages == null)
            {
                _validationMessages = new List<ValidationMessage>();
            }

            _validationMessages.Add(message);
        }

        /// <summary>
        /// Gets or sets the cleaned bill data.
        /// <para>
        /// As part of the validation, the bill data is cleaned, i.e. leading and trailing whitespace is trimmed,
        /// empty values are replaced with <c>null</c>, invalid characters are replaced and too long data is truncated.
        /// The result is the cleaned bill data. 
        /// </para>
        /// </summary>
        /// <value>The cleaned bill data.</value>
        public Bill CleanedBill { get; set; }
    }
}
