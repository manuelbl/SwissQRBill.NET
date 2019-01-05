//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//


namespace Codecrete.SwissQRBill.Generator
{
    /// <summary>
    /// QR bill validation message
    /// </summary>
    public sealed class ValidationMessage
    {
        /// <summary>
        /// Type of validatin message
        /// </summary>
        public enum MessageType
        {
            /// <summary>
            /// Warning.
            /// </summary>
            /// <remarks>
            /// A warning does not prevent the QR bill from being generated. Warnings usually
            /// indicate that data was truncated or otherwise modified.
            /// </remarks>
            Warning,
            /// <summary>
            /// Error
            /// </summary>
            /// <remarks>
            /// Errors prevent the QR bill from being generated.
            /// </remarks>
            Error
        }

        /// <summary>
        /// Gets or sets the type of message.
        /// </summary>
        public MessageType Type { get; set; }

        /// <summary>
        /// Gets or sets the name of the affected field.
        /// </summary>
        /// <remarks>
        /// All field names are available as constants of the <see cref="Bill"/> class.
        /// </remarks>
        public string Field { get; set; }

        /// <summary>
        /// Gets or sets the language neutral key of the message.
        /// </summary>
        public string MessageKey { get; set; }

        /// <summary>
        /// Gets additional message parameters (text) that are inserted into the localized message.
        /// </summary>
        public string[] MessageParameters { get; set; }

        /// <summary>
        /// Initializes a new instance with <c>null</c> values.
        /// </summary>
        public ValidationMessage() { }

        /// <summary>
        /// Initializes a new instance with the given values.
        /// </summary>
        /// <param name="type">Message type</param>
        /// <param name="field">Affected field</param>
        /// <param name="messageKey">Language-neutral message key</param>
        public ValidationMessage(MessageType type, string field, string messageKey)
        {
            Type = type;
            Field = field;
            MessageKey = messageKey;
        }

        /// <summary>
        /// Initializes a new instance with the given values.
        /// </summary>
        /// <param name="type">Message type</param>
        /// <param name="field">Affected field</param>
        /// <param name="messageKey">Language-neutral message key</param>
        /// <param name="messageParameters">variable text parts to be inserted into localized message</param>
        public ValidationMessage(MessageType type, string field, string messageKey, string[] messageParameters)
        {
            Type = type;
            Field = field;
            MessageKey = messageKey;
            MessageParameters = messageParameters;
        }
    }
}
