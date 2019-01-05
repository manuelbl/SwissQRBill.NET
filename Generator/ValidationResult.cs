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
    /// Container for validation results
    /// </summary>
    public sealed class ValidationResult
    {
        private static readonly List<ValidationMessage> EmptyList = new List<ValidationMessage>();

        private List<ValidationMessage> validationMessages;

        /// <summary>
        /// Gets the list of validation messages.
        /// </summary>
        public List<ValidationMessage> ValidationMessages
        {
            get
            {
                if (validationMessages == null)
                {
                    return EmptyList;
                }

                return validationMessages;
            }
        }

        /// <summary>
        /// Gets if this validation result contains any messages
        /// </summary>
        public bool HasMessages => validationMessages != null;

        /// <summary>
        /// Gets if this validation result contains any warning messages.
        /// </summary>
        public bool HasWarnings
        {
            get
            {
                if (validationMessages == null)
                {
                    return false;
                }

                foreach (ValidationMessage message in validationMessages)
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
        public bool HasErrors
        {
            get
            {
                if (validationMessages == null)
                {
                    return false;
                }

                foreach (ValidationMessage message in validationMessages)
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
        /// </summary>
        public bool IsValid => !HasErrors;

        /// <summary>
        /// Adds a new validation message to this validation result.
        /// </summary>
        /// <param name="type">message type</param>
        /// <param name="field">name of the affected field</param>
        /// <param name="messageKey">language-netural message key</param>
        /// <param name="messageParameters">optional message parameters (text) to be inserted into the localized message</param>
        public void AddMessage(ValidationMessage.MessageType type, string field, string messageKey, string[] messageParameters = null)
        {
            ValidationMessage message = new ValidationMessage(type, field, messageKey, messageParameters);
            if (validationMessages == null)
            {
                validationMessages = new List<ValidationMessage>();
            }

            validationMessages.Add(message);
        }

        /// <summary>
        /// Gets or sets the cleaned bill data.
        /// </summary>
        public Bill CleanedBill { get; set; }
    }
}
