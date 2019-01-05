//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//


using System;
using System.Collections.Generic;

namespace Codecrete.SwissQRBill.Generator
{
    /// <summary>
    /// Address of creditor or debtor.
    /// </summary>
    /// <remarks>
    /// You can either set street, house number, postal code and town (type <i>structured address</i>)
    /// or address line 1 and 2 (type <i>combined address elements</i>). The type is automatically set
    /// once any of these fields is set. Before setting the fields, the address type is <i>undetermined</i>.
    /// If fields of both types are set, the address type becomes <i>conflicting</i>.
    /// Name and country code must always be set unless all fields are empty.
    /// </remarks>
    public sealed class Address : IEquatable<Address>
    {

        /// <summary>
        /// Address type
        /// </summary>
        public enum AddressType
        {
            /// <summary>
            /// Undetermined
            /// </summary>
            Undetermined,
            /// <summary>
            /// Structured address
            /// </summary>
            Structured,
            /// <summary>
            /// Combined address elements
            /// </summary>
            CombinedElements,
            /// <summary>
            /// Conflicting
            /// </summary>
            Conflicting
        }

        /// <summary>
        /// Gets the address type.
        /// </summary>
        /// <remarks>
        /// The address type is automatically set by either setting street / house number
        /// or address line 1 and 2. Before setting the fields, the address type is <i>Undetermined</i>.
        /// If fields of both types are set, the address type becomes <i>Conflicting</i>.
        /// </remarks>
        public AddressType Type { get; private set; } = AddressType.Undetermined;

        private void ChangeType(AddressType desiredType)
        {
            if (Type == desiredType)
            {
                return;
            }

            if (Type == AddressType.Undetermined)
            {
                Type = desiredType;
            }
            else
            {
                Type = AddressType.Conflicting;
            }
        }

        /// <summary>
        /// Gets or sets the name, either the first and last name of a natural person or the
        /// company name of a legal person.
        /// </summary>
        public string Name { get; set; }

        private string _addressLine1;

        /// <summary>
        /// Gets or sets the address line 1.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Address line 1 contains street name, house number or P.O. box.
        /// </para>
        /// <para>
        /// Setting this field sets the address type to <see cref="AddressType.CombinedElements"/> unless it's already
        /// <see cref="AddressType.Structured"/>, in which case it becomes <see cref="AddressType.Conflicting"/>.
        /// </para>
        /// <para>
        /// This field is only used for combined elements addresses and is optional.
        /// </para>
        /// </remarks>
        public string AddressLine1
        {
            get => _addressLine1;
            set
            {
                _addressLine1 = value;
                ChangeType(AddressType.CombinedElements);
            }
        }

        private string _addressLine2;

        /// <summary>
        /// Gets or sets the address line 2.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Address line 2 contains postal code and town.
        /// </para>
        /// <para>
        /// Setting this field sets the address type to <see cref="AddressType.CombinedElements"/> unless it's already
        /// <see cref="AddressType.Structured"/>, in which case it becomes <see cref="AddressType.Conflicting"/>.
        /// </para>
        /// <para>
        /// This field is only used for combined elements addresses. For this type, it's mandatory.
        /// </para>
        /// </remarks>
        public string AddressLine2
        {
            get => _addressLine2;
            set
            {
                _addressLine2 = value;
                ChangeType(AddressType.CombinedElements);
            }
        }

        private string _street;

        /// <summary>
        /// Gets or sets the street.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The street must be speicfied without house number.
        /// </para>
        /// <para>
        /// Setting this field sets the address type to <see cref="AddressType.Structured"/> unless it's already
        /// <see cref="AddressType.CombinedElements"/>, in which case it becomes <see cref="AddressType.Conflicting"/>.
        /// </para>
        /// <para>
        /// This field is only used for structured addresses and is optional.
        /// </para>
        /// </remarks>
        public string Street
        {
            get => _street;
            set
            {
                _street = value;
                ChangeType(AddressType.Structured);
            }
        }

        private string _houseNo;

        /// <summary>
        /// Gets or sets the house number.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Setting this field sets the address type to <see cref="AddressType.Structured"/> unless it's already
        /// <see cref="AddressType.CombinedElements"/>, in which case it becomes <see cref="AddressType.Conflicting"/>.
        /// </para>
        /// <para>
        /// This field is only used for structured addresses and is optional.
        /// </para>
        /// </remarks>
        public string HouseNo
        {
            get => _houseNo;
            set
            {
                _houseNo = value;
                ChangeType(AddressType.Structured);
            }
        }

        private string _postalCode;

        /// <summary>
        /// Gets or sets the postal code.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Setting this field sets the address type to <see cref="AddressType.Structured"/> unless it's already
        /// <see cref="AddressType.CombinedElements"/>, in which case it becomes <see cref="AddressType.Conflicting"/>.
        /// </para>
        /// <para>
        /// This field is only used for structured addresses. For this type, it's mandatory.
        /// </para>
        /// </remarks>
        public string PostalCode
        {
            get => _postalCode;
            set
            {
                _postalCode = value;
                ChangeType(AddressType.Structured);
            }
        }

        private string _town;

        /// <summary>
        /// Gets or sets the town or city.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Setting this field sets the address type to <see cref="AddressType.Structured"/> unless it's already
        /// <see cref="AddressType.CombinedElements"/>, in which case it becomes <see cref="AddressType.Conflicting"/>.
        /// </para>
        /// <para>
        /// This field is only used for structured addresses. For this type, it's mandatory.
        /// </para>
        /// </remarks>
        public string Town
        {
            get => _town;
            set
            {
                _town = value;
                ChangeType(AddressType.Structured);
            }
        }

        /// <summary>
        /// Gets or sets the two-letter ISO country code.
        /// </summary>
        /// <remarks>
        /// The country code is mandatory unless the entire address contains <c>null</c> or emtpy values.</remarks>
        public string CountryCode { get; set; }

        /// <summary>
        /// Clears all fields and set the type to <see cref="AddressType.Undetermined"/>.
        /// </summary>
        public void Clear()
        {
            Type = AddressType.Undetermined;
            Name = null;
            _addressLine1 = null;
            _addressLine2 = null;
            _street = null;
            _houseNo = null;
            _postalCode = null;
            _town = null;
            CountryCode = null;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Address);
        }

        public bool Equals(Address other)
        {
            return other != null &&
                   Type == other.Type &&
                   Name == other.Name &&
                   AddressLine1 == other.AddressLine1 &&
                   AddressLine2 == other.AddressLine2 &&
                   Street == other.Street &&
                   HouseNo == other.HouseNo &&
                   PostalCode == other.PostalCode &&
                   Town == other.Town &&
                   CountryCode == other.CountryCode;
        }

        public override int GetHashCode()
        {
            EqualityComparer<string> comparer = EqualityComparer<string>.Default;
            int hashCode = 1913794654;
            hashCode = hashCode * -1521134295 + Type.GetHashCode();
            hashCode = hashCode * -1521134295 + comparer.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + comparer.GetHashCode(AddressLine1);
            hashCode = hashCode * -1521134295 + comparer.GetHashCode(AddressLine2);
            hashCode = hashCode * -1521134295 + comparer.GetHashCode(Street);
            hashCode = hashCode * -1521134295 + comparer.GetHashCode(HouseNo);
            hashCode = hashCode * -1521134295 + comparer.GetHashCode(PostalCode);
            hashCode = hashCode * -1521134295 + comparer.GetHashCode(Town);
            hashCode = hashCode * -1521134295 + comparer.GetHashCode(CountryCode);
            return hashCode;
        }
    }
}
