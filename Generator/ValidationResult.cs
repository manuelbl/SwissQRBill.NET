//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        /// <value><c>true</c> if the result contains any error messages, <c>false</c> otherwise.</value>
        public bool HasErrors
        {
            get
            {
                return _validationMessages != null && _validationMessages.Any(message => message.Type == ValidationMessage.MessageType.Error);
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
        /// <param name="messageKey">The language-neutral message key.</param>
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

        /// <summary>
        /// Gets a human-readable description of the validation problems.
        /// <para>
        /// The description includes errors only.
        /// </para>
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get
            {
                if (!HasErrors)
                {
                    return "Valid bill data";
                }

                StringBuilder sb = new StringBuilder();

                foreach (var message in ValidationMessages)
                {
                    if (message.Type != ValidationMessage.MessageType.Error)
                        continue;

                    if (sb.Length > 0)
                        sb.Append("; ");

                    string desc = ErrorMessages.ContainsKey(message.MessageKey) ? ErrorMessages[message.MessageKey] : "Unknown error";
                    if (message.MessageKey == "field_is_mandatory")
                    {
                        desc = string.Format(desc, message.Field);
                    }
                    else if (message.MessageKey == "field_value_too_long")
                    {
                        desc = string.Format(desc, message.Field, message.MessageParameters[0]);
                    }

                    sb.Append(desc);
                    sb.Append(" (");
                    sb.Append(message.MessageKey);
                    sb.Append(")");
                }

                return sb.ToString();
            }
        }

        private static readonly Dictionary<string, string> ErrorMessages = new Dictionary<string, string>()
        {
            { "currency_is_chf_or_eur", "currency should be \"CHF\" or \"EUR\"" },
            { "amount_in_valid_range", "amount should be between 0.01 and 999 999 999.99" },
            { "account_is_ch_li_iban", "account number should start with \"CH\" or \"LI\"" },
            { "account_is_valid_iban", "IBAN is invalid (format or checksum)" },
            { "valid_iso11649_creditor_ref", "reference is invalid (reference should be empty or start with \"RF\")" },
            { "valid_qr_ref_no", "reference is invalid (numeric QR reference required)" },
            { "mandatory_for_qr_iban", "reference is needed for a payment to this account (QR-IBAN)" },
            { "field_is_mandatory", "field \"{0}\" may not be empty" },
            { "valid_country_code", "country code is invalid; it should consist of two letters" },
            { "address_type_conflict", "fields for either structured address or combined elements address may be filled but not both" },
            { "alt_scheme_max_exceed", "no more than two alternative schemes may be used" },
            { "bill_info_invalid", "structured bill information must start with \"//\"" },
            { "field_value_too_long", "the value for field \"{0}\" should not exceed a length of {1} characters" },
            { "additional_info_too_long", "the additional information and the structured bill information combined should not exceed 140 characters" }
        };

    }
}
