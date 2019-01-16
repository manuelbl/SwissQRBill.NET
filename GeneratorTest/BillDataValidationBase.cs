//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using Xunit;
using static Codecrete.SwissQRBill.Generator.ValidationMessage;

namespace Codecrete.SwissQRBill.GeneratorTest
{
    /// <summary>
    /// Base class for bill data validation tests
    /// </summary>
    public class BillDataValidationBase
    {
        protected Bill SourceBill;
        protected ValidationResult Result;
        protected Bill ValidatedBill;

        public void Validate()
        {
            Result = QRBill.Validate(SourceBill);
            ValidatedBill = Result.CleanedBill;
        }

        /// <summary>
        /// Asserts that the validation succeeded with no messages.
        /// </summary>
        public void AssertNoMessages()
        {
            Assert.False(Result.HasErrors);
            Assert.False(Result.HasWarnings);
            Assert.False(Result.HasMessages);
            Assert.Empty(Result.ValidationMessages);
        }

        /// <summary>
        /// Asserts that the validation produced a single validation error message.
        /// </summary>
        /// <param name="field">the field that triggered the validation error</param>
        /// <param name="messageKey">the message key of the validation error</param>
        public void AssertSingleErrorMessage(string field, string messageKey)
        {
            Assert.True(Result.HasErrors);
            Assert.False(Result.HasWarnings);
            Assert.True(Result.HasMessages);
            Assert.Single(Result.ValidationMessages);

            ValidationMessage msg = Result.ValidationMessages[0];
            Assert.Equal(MessageType.Error, msg.Type);
            Assert.Equal(field, msg.Field);
            Assert.Equal(messageKey, msg.MessageKey);
        }

        /// <summary>
        /// Asserts thta the validation succeeded with a single warning
        /// </summary>
        /// <param name="field">the field that triggered the validation warning</param>
        /// <param name="messageKey">the message key of the validation warning</param>
        public void AssertSingleWarningMessage(string field, string messageKey)
        {
            Assert.False(Result.HasErrors);
            Assert.True(Result.HasWarnings);
            Assert.True(Result.HasMessages);
            Assert.Single(Result.ValidationMessages);

            ValidationMessage msg = Result.ValidationMessages[0];
            Assert.Equal(MessageType.Warning, msg.Type);
            Assert.Equal(field, msg.Field);
            Assert.Equal(messageKey, msg.MessageKey);
        }

        /// <summary>
        /// Creates an address with valid person data.
        /// </summary>
        /// <returns>the address</returns>
        public Address CreateValidPerson()
        {
            Address address = new Address
            {
                Name = "Zuppinger AG",
                Street = "Industriestrasse",
                HouseNo = "34a",
                PostalCode = "9548",
                Town = "Matzingen",
                CountryCode = "CH"
            };
            return address;
        }
    }
}
