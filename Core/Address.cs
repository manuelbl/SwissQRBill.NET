//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//


using System;
using System.Collections.Generic;
using System.Net;
using System.Xml.Linq;

namespace Codecrete.SwissQRBill.Generator
{
    /// <summary>
    /// Address of creditor or debtor.
    /// <para>
    /// You can either set street, house number, postal code and town (type <i>structured address</i>)
    /// or address line 1 and 2 (type <i>combined address elements</i>). The type is automatically set
    /// once any of these fields is set. Before setting the fields, the address type is <i>undetermined</i>.
    /// If fields of both types are set, the address type becomes <i>conflicting</i>.
    /// Name and country code must always be set unless all fields are empty.
    /// </para>
    /// <para>
    /// Banks will no longer accept payments using the combined address elements starting November 21, 2025.
    /// Therefore, it is recommended to use structured addresses immediately.
    /// </para>
    /// </summary>
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
            /// <para>
            /// This is a temporary type and not suitable for a valid payment.
            /// </para>
            /// </summary>
            Structured,
            /// <summary>
            /// Combined address elements
            /// </summary>
            CombinedElements,
            /// <summary>
            /// Conflicting
            /// <para>
            /// This a an invalid type and will prevent the generation of a QR bill.
            /// </para>
            /// </summary>
            Conflicting
        }

        /// <summary>
        /// Gets the address type.
        /// <para>
        /// The address type is automatically set by either setting street / house number
        /// or address line 1 and 2. Before setting the fields, the address type is <i>Undetermined</i>.
        /// If fields of both types are set, the address type becomes <i>Conflicting</i>.
        /// </para>
        /// </summary>
        /// <value>The address type.</value>
        public AddressType Type { get; private set; } = AddressType.Undetermined;

        private void ChangeType(AddressType desiredType)
        {
            if (Type == desiredType)
            {
                return;
            }

            Type = Type == AddressType.Undetermined ? desiredType : AddressType.Conflicting;
        }

        /// <summary>
        /// Gets or sets the name, either the first and last name of a natural person or the
        /// company name of a legal person.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        private string _addressLine1;

        /// <summary>
        /// Gets or sets the address line 1.
        /// <para>
        /// Address line 1 contains street name, house number or P.O. box.
        /// </para>
        /// <para>
        /// Setting this field sets the address type to <see cref="AddressType.CombinedElements"/> unless it's already
        /// <see cref="AddressType.Structured"/>, in which case it becomes <see cref="AddressType.Conflicting"/>.
        /// </para>
        /// <para>
        /// This field is only used for combined address elements and is optional.
        /// Starting November 25, 2025, banks will no longer accept payments using combined address elements.
        /// </para>
        /// </summary>
        /// <value>The address line 1.</value>
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
        /// <para>
        /// Address line 2 contains postal code and town.
        /// </para>
        /// <para>
        /// Setting this field sets the address type to <see cref="AddressType.CombinedElements"/> unless it's already
        /// <see cref="AddressType.Structured"/>, in which case it becomes <see cref="AddressType.Conflicting"/>.
        /// </para>
        /// <para>
        /// This field is only used for combined address elements. For this type, it is mandatory.
        /// Starting November 25, 2025, banks will no longer accept payments using combined address elements.
        /// </para>
        /// </summary>
        /// <value>The address line 2.</value>
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
        /// <para>
        /// The street must be specified without house number.
        /// </para>
        /// <para>
        /// Setting this field sets the address type to <see cref="AddressType.Structured"/> unless it's already
        /// <see cref="AddressType.CombinedElements"/>, in which case it becomes <see cref="AddressType.Conflicting"/>.
        /// </para>
        /// <para>
        /// This field is only used for structured addresses and is optional.
        /// </para>
        /// </summary>
        /// <value>The street.</value>
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
        /// <para>
        /// Setting this field sets the address type to <see cref="AddressType.Structured"/> unless it's already
        /// <see cref="AddressType.CombinedElements"/>, in which case it becomes <see cref="AddressType.Conflicting"/>.
        /// </para>
        /// <para>
        /// This field is only used for structured addresses and is optional.
        /// </para>
        /// </summary>
        /// <value>The house number.</value>
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
        /// <para>
        /// Setting this field sets the address type to <see cref="AddressType.Structured"/> unless it's already
        /// <see cref="AddressType.CombinedElements"/>, in which case it becomes <see cref="AddressType.Conflicting"/>.
        /// </para>
        /// <para>
        /// This field is only used for structured addresses. For this type, it's mandatory.
        /// </para>
        /// </summary>
        /// <value>The postal code.</value>
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
        /// <para>
        /// Setting this field sets the address type to <see cref="AddressType.Structured"/> unless it's already
        /// <see cref="AddressType.CombinedElements"/>, in which case it becomes <see cref="AddressType.Conflicting"/>.
        /// </para>
        /// <para>
        /// This field is only used for structured addresses. For this type, it's mandatory.
        /// </para>
        /// </summary>
        /// <value>The town or city.</value>
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
        ///  <para>
        /// The country code is mandatory unless the entire address contains <c>null</c> or empty values.
        /// </para>
        /// </summary>
        /// <value>The ISO country code.</value>
        public string CountryCode { get; set; }

        /// <summary>
        /// Clears all fields and sets the type to <see cref="AddressType.Undetermined"/>.
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

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Address);
        }

        /// <summary>Determines whether the specified address is equal to the current address.</summary>
        /// <param name="other">The address to compare with the current address.</param>
        /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
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

        /// <summary>Gets the hash code for this instance.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            var comparer = EqualityComparer<string>.Default;
            var hashCode = 1913794654;
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
