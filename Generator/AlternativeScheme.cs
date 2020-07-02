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
    /// Alternative payment scheme instructions
    /// </summary>
    public sealed class AlternativeScheme : IEquatable<AlternativeScheme>
    {
        /// <summary>
        /// Gets or sets the payment scheme name.
        /// </summary>
        /// <value>The payment scheme name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets the payment instruction for a given bill.
        /// <para>
        /// The instruction consists of a two letter abbreviation for the scheme, a separator characters
        /// and a sequence of parameters(separated by the character at index 2).
        /// </para>
        /// </summary>
        /// <value>The payment instruction.</value>
        public string Instruction { get; set; }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as AlternativeScheme);
        }

        /// <summary>Determines whether the specified alternative scheme is equal to the current alternative scheme.</summary>
        /// <param name="other">The alternative scheme to compare with the current alternative scheme.</param>
        /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
        public bool Equals(AlternativeScheme other)
        {
            return other != null &&
                   Name == other.Name &&
                   Instruction == other.Instruction;
        }

        /// <summary>Gets the hash code for this instance.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            EqualityComparer<string> stringComparer = EqualityComparer<string>.Default;
            int hashCode = -1893642763;
            hashCode = hashCode * -1521134295 + stringComparer.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + stringComparer.GetHashCode(Instruction);
            return hashCode;
        }
    }
}
