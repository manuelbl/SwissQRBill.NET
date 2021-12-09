//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using Xunit;
using static Codecrete.SwissQRBill.Generator.ValidationMessage;

namespace Codecrete.SwissQRBill.CoreTest
{
    public class ValidationMessageTest
    {
        [Fact]
        public void DefaultConstructor()
        {
            ValidationMessage msg = new ValidationMessage();
            Assert.Null(msg.Field);
            Assert.Null(msg.MessageKey);
            Assert.Null(msg.MessageParameters);
        }

        [Fact]
        public void ConstructorWithThreeParameters()
        {
            ValidationMessage msg = new ValidationMessage(MessageType.Error, "fld", "msg3");
            Assert.Equal(MessageType.Error, msg.Type);
            Assert.Equal("fld", msg.Field);
            Assert.Equal("msg3", msg.MessageKey);
            Assert.Null(msg.MessageParameters);
        }

        [Fact]
        public void ConstructorWithFourParameters()
        {
            ValidationMessage msg = new ValidationMessage(MessageType.Warning, "addInfo", "clipped", new[] { "xxx" });
            Assert.Equal(MessageType.Warning, msg.Type);
            Assert.Equal("addInfo", msg.Field);
            Assert.Equal("clipped", msg.MessageKey);
            Assert.NotNull(msg.MessageParameters);
            Assert.Single(msg.MessageParameters);
            Assert.Equal("xxx", msg.MessageParameters[0]);
        }

        [Fact]
        public void SetType()
        {
            ValidationMessage msg = new ValidationMessage
            {
                Type = MessageType.Error
            };
            Assert.Equal(MessageType.Error, msg.Type);
        }

        [Fact]
        public void SetField()
        {
            ValidationMessage msg = new ValidationMessage
            {
                Field = "tt3"
            };
            Assert.Equal("tt3", msg.Field);
        }

        [Fact]
        public void SetMessageKey()
        {
            ValidationMessage msg = new ValidationMessage
            {
                MessageKey = "msg.err.invalid"
            };
            Assert.Equal("msg.err.invalid", msg.MessageKey);
        }

        [Fact]
        public void SetMessageParameters()
        {
            ValidationMessage msg = new ValidationMessage
            {
                MessageParameters = new[] { "abc", "def", "ghi" }
            };
            Assert.Equal(new[] { "abc", "def", "ghi" }, msg.MessageParameters);
        }
    }
}
