//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using Xunit;

namespace Codecrete.SwissQRBill.CoreTest
{
    public class AlternativeSchemeTest
    {

        [Fact]
        public void DefaultConstructorTest()
        {
            AlternativeScheme scheme = new AlternativeScheme();
            Assert.Null(scheme.Name);
            Assert.Null(scheme.Instruction);
        }

        [Fact]
        public void ConstructorTest()
        {
            AlternativeScheme scheme = new AlternativeScheme { Name = "Paymit", Instruction = "PM,12341234,1241234" };
            Assert.Equal("Paymit", scheme.Name);
            Assert.Equal("PM,12341234,1241234", scheme.Instruction);
        }

        [Fact]
        public void TestEqualsTrivial()
        {
            AlternativeScheme scheme = new AlternativeScheme { Name = "Paymit", Instruction = "PM,12341234,1241234" };
            Assert.Equal(scheme, scheme);
            AlternativeScheme nullScheme = null;
            Assert.NotEqual(scheme, nullScheme);
            Assert.NotEqual((object)"xxx", scheme);
        }

        [Fact]
        public void TestEquals()
        {
            AlternativeScheme scheme1 = new AlternativeScheme { Name = "Paymit", Instruction = "PM,12341234,1241234" };
            AlternativeScheme scheme2 = new AlternativeScheme { Name = "Paymit", Instruction = "PM,12341234,1241234" };
            Assert.Equal(scheme1, scheme2);
            Assert.Equal(scheme1, scheme2);

            scheme2.Name = "TWINT";
            Assert.NotEqual(scheme1, scheme2);
        }

        [Fact]
        public void TestHashCode()
        {
            AlternativeScheme scheme1 = new AlternativeScheme { Name = "Paymit", Instruction = "PM,12341234,1241234" };
            AlternativeScheme scheme2 = new AlternativeScheme { Name = "Paymit", Instruction = "PM,12341234,1241234" };
            Assert.Equal(scheme1.GetHashCode(), scheme2.GetHashCode());
        }
    }
}
