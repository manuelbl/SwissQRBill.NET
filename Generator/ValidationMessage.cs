//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//


namespace Codecrete.SwissQRBill.Generator
{
    /// <summary>
    /// QR bill validation message.
    /// </summary>
    public sealed class ValidationMessage
    {
        /// <summary>
        /// The type of validatin message.
        /// </summary>
        public enum MessageType
        {
            /// <summary>
            /// Warning.
            /// <para>
            /// A warning does not prevent the QR bill from being generated. Warnings usually
            /// indicate that data was truncated or otherwise modified.
            /// </para>
            /// </summary>
            Warning,
            /// <summary>
            /// Error.
            /// <para>
            /// Errors prevent the QR bill from being generated.
            /// </para>
            /// </summary>
            Error
        }

        /// <summary>
        /// Gets or sets the type of message.
        /// </summary>
        /// <value>The message type.</value>
        public MessageType Type { get; set; }

        /// <summary>
        /// Gets or sets the name of the affected field.
        /// <para>
        /// All field names are available as constants of the <see cref="ValidationConstants"/> class.
        /// Fields nested in data structures are given like a path using the dot as a separator,
        /// e.g. <c>creditor.name</c> for the creditor name.
        /// </para>
        /// </summary>
        /// <value>The name of the affected field.</value>
        /// <seealso cref="ValidationConstants"/>
        public string Field { get; set; }

        /// <summary>
        /// Gets or sets the language neutral key of the message.
        /// <para>
        /// All message keys are available as constants of the <see cref="ValidationConstants"/> class.
        /// </para>
        /// </summary>
        /// <value>The message key.</value>
        /// <seealso cref="ValidationConstants"/>
        public string MessageKey { get; set; }

        /// <summary>
        /// Gets additional message parameters (text) that are inserted into the localized message.
        /// </summary>
        /// <value>The message parameters (if any) or <c>null</c>.</value>
        public string[] MessageParameters { get; set; }

        /// <summary>
        /// Initializes a new instance with <c>null</c> values.
        /// </summary>
        public ValidationMessage() { }

        /// <summary>
        /// Initializes a new instance with the given values.
        /// <para>
        /// For valid field names and message keys, see the constants in the <see cref="ValidationConstants"/> class.
        /// </para>
        /// </summary>
        /// <param name="type">The message type.</param>
        /// <param name="field">The name of the affected field.</param>
        /// <param name="messageKey">The language-neutral message key.</param>
        /// <param name="messageParameters">The optional variable text parts to be inserted into localized message.</param>
        /// <seealso cref="ValidationConstants"/>
        public ValidationMessage(MessageType type, string field, string messageKey, string[] messageParameters = null)
        {
            Type = type;
            Field = field;
            MessageKey = messageKey;
            MessageParameters = messageParameters;
        }
    }
}
