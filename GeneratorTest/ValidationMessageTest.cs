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
    public class ValidationMessageTest
    {
        [Fact]
        void DefaultConstructor()
        {
            ValidationMessage msg = new ValidationMessage();
            Assert.Null(msg.Field);
            Assert.Null(msg.MessageKey);
            Assert.Null(msg.MessageParameters);
        }

        [Fact]
        void ConstructorWithThreeParameters()
        {
            ValidationMessage msg = new ValidationMessage(MessageType.Error, "fld", "msg3");
            Assert.Equal(MessageType.Error, msg.Type);
            Assert.Equal("fld", msg.Field);
            Assert.Equal("msg3", msg.MessageKey);
            Assert.Null(msg.MessageParameters);
        }

        [Fact]
        void ConstructorWithFourParameters()
        {
            ValidationMessage msg = new ValidationMessage(MessageType.Warning, "addInfo", "clipped", new string[] { "xxx" });
            Assert.Equal(MessageType.Warning, msg.Type);
            Assert.Equal("addInfo", msg.Field);
            Assert.Equal("clipped", msg.MessageKey);
            Assert.NotNull(msg.MessageParameters);
            Assert.Single(msg.MessageParameters);
            Assert.Equal("xxx", msg.MessageParameters[0]);
        }

        [Fact]
        void SetType()
        {
            ValidationMessage msg = new ValidationMessage
            {
                Type = MessageType.Error
            };
            Assert.Equal(MessageType.Error, msg.Type);
        }

        [Fact]
        void SetField()
        {
            ValidationMessage msg = new ValidationMessage
            {
                Field = "tt3"
            };
            Assert.Equal("tt3", msg.Field);
        }

        [Fact]
        void SetMessageKey()
        {
            ValidationMessage msg = new ValidationMessage
            {
                MessageKey = "msg.err.invalid"
            };
            Assert.Equal("msg.err.invalid", msg.MessageKey);
        }

        [Fact]
        void SetMessageParameters()
        {
            ValidationMessage msg = new ValidationMessage
            {
                MessageParameters = new string[] { "abc", "def", "ghi" }
            };
            Assert.Equal(new string[] { "abc", "def", "ghi" }, msg.MessageParameters);
        }
    }
}
