//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using System.Collections.Generic;
using Xunit;
using static Codecrete.SwissQRBill.Generator.ValidationMessage;

namespace Codecrete.SwissQRBill.GeneratorTest
{
    public class ValidationResultTest
    {
        [Fact]
        private void DefaultConstructor()
        {
            ValidationResult result = new ValidationResult();
            Assert.True(result.IsValid);
            Assert.False(result.HasMessages);
            Assert.False(result.HasWarnings);
            Assert.False(result.HasErrors);
            Assert.Equal(new List<ValidationMessage>(), result.ValidationMessages);
            Assert.Null(result.CleanedBill);
        }

        [Fact]
        private void SingleWarning()
        {
            ValidationResult result = new ValidationResult();
            result.AddMessage(MessageType.Warning, "tfd", "dkw");
            Assert.True(result.IsValid);
            Assert.True(result.HasMessages);
            Assert.True(result.HasWarnings);
            Assert.False(result.HasErrors);

            List<ValidationMessage> messages = result.ValidationMessages;
            Assert.NotNull(messages);
            Assert.Single(messages);
            Assert.Equal(MessageType.Warning, messages[0].Type);
            Assert.Equal("tfd", messages[0].Field);
            Assert.Equal("dkw", messages[0].MessageKey);
            Assert.Null(messages[0].MessageParameters);
        }

        [Fact]
        private void SingleError()
        {
            ValidationResult result = new ValidationResult();
            result.AddMessage(MessageType.Error, "kdef.def", "qrdv.dwek-eke");
            Assert.False(result.IsValid);
            Assert.True(result.HasMessages);
            Assert.False(result.HasWarnings);
            Assert.True(result.HasErrors);

            List<ValidationMessage> messages = result.ValidationMessages;
            Assert.NotNull(messages);
            Assert.Single(messages);
            Assert.Equal(MessageType.Error, messages[0].Type);
            Assert.Equal("kdef.def", messages[0].Field);
            Assert.Equal("qrdv.dwek-eke", messages[0].MessageKey);
            Assert.Null(messages[0].MessageParameters);
        }

        [Fact]
        private void MultipleMessages()
        {
            ValidationResult result = new ValidationResult();
            result.AddMessage(MessageType.Error, "abd-fds", "asdf.asdfe.werk");
            result.AddMessage(MessageType.Warning, "ieow.se3", "iwer.asdfwerk.asdf");
            Assert.False(result.IsValid);
            Assert.True(result.HasMessages);
            Assert.True(result.HasWarnings);
            Assert.True(result.HasErrors);

            List<ValidationMessage> messages = result.ValidationMessages;
            Assert.NotNull(messages);
            Assert.Equal(2, messages.Count);

            Assert.Equal(MessageType.Error, messages[0].Type);
            Assert.Equal("abd-fds", messages[0].Field);
            Assert.Equal("asdf.asdfe.werk", messages[0].MessageKey);
            Assert.Null(messages[0].MessageParameters);

            Assert.Equal(MessageType.Warning, messages[1].Type);
            Assert.Equal("ieow.se3", messages[1].Field);
            Assert.Equal("iwer.asdfwerk.asdf", messages[1].MessageKey);
            Assert.Null(messages[1].MessageParameters);
        }

        [Fact]
        private void MessageWithMessageParameters()
        {
            ValidationResult result = new ValidationResult();
            result.AddMessage(MessageType.Warning, "jkr", "wcw.dw", new[] { ")(*$" });
            Assert.True(result.IsValid);
            Assert.True(result.HasMessages);
            Assert.True(result.HasWarnings);
            Assert.False(result.HasErrors);

            List<ValidationMessage> messages = result.ValidationMessages;
            Assert.NotNull(messages);
            Assert.Single(messages);
            Assert.Equal(MessageType.Warning, messages[0].Type);
            Assert.Equal("jkr", messages[0].Field);
            Assert.Equal("wcw.dw", messages[0].MessageKey);
            Assert.Equal(new[] { ")(*$" }, messages[0].MessageParameters);
        }

        [Fact]
        private void SetCleanedBill()
        {
            ValidationResult result = new ValidationResult
            {
                CleanedBill = SampleData.CreateExample2()
            };

            Assert.Equal(SampleData.CreateExample2(), result.CleanedBill);
        }
    }
}
