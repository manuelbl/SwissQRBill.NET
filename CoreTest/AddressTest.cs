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
    public class AddressTest
    {
        [Fact]
        public void TestUndetermined()
        {
            var address = new Address();
            Assert.Equal(Address.AddressType.Undetermined, address.Type);
        }

        [Fact]
        public void SetName()
        {
            var address = new Address
            {
                Name = "ABC"
            };
            Assert.Equal("ABC", address.Name);
        }

#pragma warning disable CS0618 // Type or member is obsolete
        [Fact]
        public void SetAddressLine1()
        {
            var address = new Address
            {
                AddressLine1 = "TYUI"
            };
            Assert.Equal("TYUI", address.AddressLine1);
            Assert.Equal(Address.AddressType.CombinedElements, address.Type);
        }

        [Fact]
        public void SetAddressLine2()
        {
            var address = new Address
            {
                AddressLine2 = "vbnm"
            };
            Assert.Equal("vbnm", address.AddressLine2);
            Assert.Equal(Address.AddressType.CombinedElements, address.Type);
        }
#pragma warning restore CS0618 // Type or member is obsolete

        [Fact]
        public void SetStreet()
        {
            var address = new Address
            {
                Street = "DEFGH"
            };
            Assert.Equal("DEFGH", address.Street);
            Assert.Equal(Address.AddressType.Structured, address.Type);
        }

        [Fact]
        public void SetHouseNo()
        {
            var address = new Address
            {
                HouseNo = "fiekdd"
            };
            Assert.Equal("fiekdd", address.HouseNo);
            Assert.Equal(Address.AddressType.Structured, address.Type);
        }

        [Fact]
        public void SetPostalCode()
        {
            var address = new Address
            {
                PostalCode = "BG19283"
            };
            Assert.Equal("BG19283", address.PostalCode);
            Assert.Equal(Address.AddressType.Structured, address.Type);
        }

        [Fact]
        public void SetTown()
        {
            var address = new Address
            {
                Town = "IOPU-KU"
            };
            Assert.Equal("IOPU-KU", address.Town);
            Assert.Equal(Address.AddressType.Structured, address.Type);
        }

        [Fact]
        public void SetCountryCode()
        {
            var address = new Address
            {
                CountryCode = "XY"
            };
            Assert.Equal("XY", address.CountryCode);
        }

#pragma warning disable CS0618 // Type or member is obsolete
        [Fact]
        public void ConflictTest1()
        {
            var address = new Address
            {
                Street = "XY",
                AddressLine1 = "abc"
            };
            Assert.Equal(Address.AddressType.Conflicting, address.Type);
        }
#pragma warning restore CS0618 // Type or member is obsolete

#pragma warning disable CS0618 // Type or member is obsolete
        [Fact]
        public void ConflictTest2()
        {
            var address = new Address
            {
                HouseNo = "XY",
                AddressLine1 = "abc"
            };
            Assert.Equal(Address.AddressType.Conflicting, address.Type);
        }

        [Fact]
        public void ConflictTest3()
        {
            var address = new Address
            {
                PostalCode = "XY",
                AddressLine2 = "abc"
            };
            Assert.Equal(Address.AddressType.Conflicting, address.Type);
        }

        [Fact]
        public void ConflictTest4()
        {
            var address = new Address
            {
                Town = "XY",
                AddressLine2 = "abc"
            };
            Assert.Equal(Address.AddressType.Conflicting, address.Type);
        }
#pragma warning restore CS0618 // Type or member is obsolete

        [Fact]
        public void EqualObjectsStructured()
        {
            var address1 = CreateStructuredAddress();
            var address2 = CreateStructuredAddress();
            Assert.Equal(address1, address2);
        }

        [Fact]
        public void EqualObjectsCombined()
        {
            var address1 = CreateCombinedElementAddress();
            var address2 = CreateCombinedElementAddress();
            Assert.Equal(address1, address2);
        }

        [Fact]
        public void HashObjectStructured()
        {
            var address1 = CreateStructuredAddress();
            var address2 = CreateStructuredAddress();
            Assert.Equal(address1.GetHashCode(), address2.GetHashCode());
        }

        [Fact]
        public void HashObjectCombined()
        {
            var address1 = CreateCombinedElementAddress();
            var address2 = CreateCombinedElementAddress();
            Assert.Equal(address1.GetHashCode(), address2.GetHashCode());
        }

#pragma warning disable CS0618 // Type or member is obsolete
        [Fact]
        public void ClearTestStructured()
        {
            var address1 = CreateStructuredAddress();
            address1.Clear();
            Assert.Equal(Address.AddressType.Undetermined, address1.Type);
            Assert.Null(address1.Name);
            Assert.Null(address1.AddressLine1);
            Assert.Null(address1.AddressLine2);
            Assert.Null(address1.Street);
            Assert.Null(address1.HouseNo);
            Assert.Null(address1.PostalCode);
            Assert.Null(address1.Town);
            Assert.Null(address1.CountryCode);
        }

        [Fact]
        public void ClearTestCombined()
        {
            var address1 = CreateCombinedElementAddress();
            address1.Clear();
            Assert.Equal(Address.AddressType.Undetermined, address1.Type);
            Assert.Null(address1.Name);
            Assert.Null(address1.AddressLine1);
            Assert.Null(address1.AddressLine2);
            Assert.Null(address1.Street);
            Assert.Null(address1.HouseNo);
            Assert.Null(address1.PostalCode);
            Assert.Null(address1.Town);
            Assert.Null(address1.CountryCode);
        }
#pragma warning restore CS0618 // Type or member is obsolete

        [Fact]
        public void TestEqualsTrivial()
        {
            var address = CreateCombinedElementAddress();
            Assert.Equal(address, address);
            Address nullAddress = null;
            Assert.NotEqual(address, nullAddress);
            Assert.NotEqual((object)"xxx", address);
        }

        [Fact]
        public void TestEquals()
        {
            var address1 = CreateCombinedElementAddress();
            var address2 = CreateCombinedElementAddress();
            Assert.Equal(address1, address2);
            Assert.Equal(address2, address1);

            address2.CountryCode = "FR";
            Assert.NotEqual(address1, address2);
        }

        private static Address CreateStructuredAddress()
        {
            var address = new Address
            {
                Name = "Cornelia Singer",
                Street = "Alte Landstrasse",
                HouseNo = "73",
                PostalCode = "3410",
                Town = "Hunzenschwil",
                CountryCode = "CH"
            };
            return address;
        }

#pragma warning disable CS0618 // Type or member is obsolete
        private static Address CreateCombinedElementAddress()
        {
            var address = new Address
            {
                Name = "Cornelia Singer",
                AddressLine1 = "Alte Landstrasse 75",
                AddressLine2 = "8702 Zollikon",
                CountryCode = "CH"
            };
            return address;
        }
#pragma warning restore CS0618 // Type or member is obsolete
    }
}
