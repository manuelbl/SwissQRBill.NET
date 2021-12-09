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
            Address address = new Address();
            Assert.Equal(Address.AddressType.Undetermined, address.Type);
        }

        [Fact]
        public void SetName()
        {
            Address address = new Address
            {
                Name = "ABC"
            };
            Assert.Equal("ABC", address.Name);
        }

        [Fact]
        public void SetAddressLine1()
        {
            Address address = new Address
            {
                AddressLine1 = "TYUI"
            };
            Assert.Equal("TYUI", address.AddressLine1);
            Assert.Equal(Address.AddressType.CombinedElements, address.Type);
        }

        [Fact]
        public void SetAddressLine2()
        {
            Address address = new Address
            {
                AddressLine2 = "vbnm"
            };
            Assert.Equal("vbnm", address.AddressLine2);
            Assert.Equal(Address.AddressType.CombinedElements, address.Type);
        }

        [Fact]
        public void SetStreet()
        {
            Address address = new Address
            {
                Street = "DEFGH"
            };
            Assert.Equal("DEFGH", address.Street);
            Assert.Equal(Address.AddressType.Structured, address.Type);
        }

        [Fact]
        public void SetHouseNo()
        {
            Address address = new Address
            {
                HouseNo = "fiekdd"
            };
            Assert.Equal("fiekdd", address.HouseNo);
            Assert.Equal(Address.AddressType.Structured, address.Type);
        }

        [Fact]
        public void SetPostalCode()
        {
            Address address = new Address
            {
                PostalCode = "BG19283"
            };
            Assert.Equal("BG19283", address.PostalCode);
            Assert.Equal(Address.AddressType.Structured, address.Type);
        }

        [Fact]
        public void SetTown()
        {
            Address address = new Address
            {
                Town = "IOPU-KU"
            };
            Assert.Equal("IOPU-KU", address.Town);
            Assert.Equal(Address.AddressType.Structured, address.Type);
        }

        [Fact]
        public void SetCountryCode()
        {
            Address address = new Address
            {
                CountryCode = "XY"
            };
            Assert.Equal("XY", address.CountryCode);
        }

        [Fact]
        public void ConflictTest1()
        {
            Address address = new Address
            {
                Street = "XY",
                AddressLine1 = "abc"
            };
            Assert.Equal(Address.AddressType.Conflicting, address.Type);
        }

        [Fact]
        public void ConflictTest2()
        {
            Address address = new Address
            {
                HouseNo = "XY",
                AddressLine1 = "abc"
            };
            Assert.Equal(Address.AddressType.Conflicting, address.Type);
        }

        [Fact]
        public void ConflictTest3()
        {
            Address address = new Address
            {
                PostalCode = "XY",
                AddressLine2 = "abc"
            };
            Assert.Equal(Address.AddressType.Conflicting, address.Type);
        }

        [Fact]
        public void ConflictTest4()
        {
            Address address = new Address
            {
                Town = "XY",
                AddressLine2 = "abc"
            };
            Assert.Equal(Address.AddressType.Conflicting, address.Type);
        }

        [Fact]
        public void EqualObjectsStructured()
        {
            Address address1 = CreateStructuredAddress();
            Address address2 = CreateStructuredAddress();
            Assert.Equal(address1, address2);
        }

        [Fact]
        public void EqualObjectsCombined()
        {
            Address address1 = CreateCombinedElementAddress();
            Address address2 = CreateCombinedElementAddress();
            Assert.Equal(address1, address2);
        }

        [Fact]
        public void HashObjectStructured()
        {
            Address address1 = CreateStructuredAddress();
            Address address2 = CreateStructuredAddress();
            Assert.Equal(address1.GetHashCode(), address2.GetHashCode());
        }

        [Fact]
        public void HashObjectCombined()
        {
            Address address1 = CreateCombinedElementAddress();
            Address address2 = CreateCombinedElementAddress();
            Assert.Equal(address1.GetHashCode(), address2.GetHashCode());
        }

        [Fact]
        public void ClearTestStructured()
        {
            Address address1 = CreateStructuredAddress();
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
            Address address1 = CreateCombinedElementAddress();
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
        public void TestEqualsTrivial()
        {
            Address address = CreateCombinedElementAddress();
            Assert.Equal(address, address);
            Address nullAddress = null;
            Assert.NotEqual(address, nullAddress);
            Assert.NotEqual((object)"xxx", address);
        }

        [Fact]
        public void TestEquals()
        {
            Address address1 = CreateCombinedElementAddress();
            Address address2 = CreateCombinedElementAddress();
            Assert.Equal(address1, address2);
            Assert.Equal(address2, address1);

            address2.CountryCode = "FR";
            Assert.NotEqual(address1, address2);
        }

        private Address CreateStructuredAddress()
        {
            Address address = new Address
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

        private Address CreateCombinedElementAddress()
        {
            Address address = new Address
            {
                Name = "Cornelia Singer",
                AddressLine1 = "Alte Landstrasse 75",
                AddressLine2 = "8702 Zollikon",
                CountryCode = "CH"
            };
            return address;
        }
    }
}
