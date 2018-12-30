//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//


using System;

namespace Codecrete.SwissQRBill.Generator
{
    /// <summary>
    /// Alternative payment scheme instructions
    /// </summary>
    public class AlternativeScheme : IEquatable<AlternativeScheme>
    {
        /// <summary>
        /// Gets or sets the payment scheme name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the payment instruction for a given bill.
        /// </summary>
        /// <remarks>
        /// The instruction consists of a two letter abbreviation for the scheme, a separator characters
        /// and a sequence of parameters(separated by the character at index 2).
        /// </remarks>
        public string Instruction { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as AlternativeScheme);
        }

        public bool Equals(AlternativeScheme other)
        {
            return other != null &&
                   Name == other.Name &&
                   Instruction == other.Instruction;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Instruction);
        }
    }
}
