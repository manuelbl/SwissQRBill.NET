//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2024 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

namespace Codecrete.SwissQRBill.Generator
{
    /// <summary>
    /// Extensions methods for <c>SpsCharacterSet</c>
    /// </summary>
    public static class SpsCharacterSetExtension
    {
        /// <summary>
        /// Returns if the Unicode code point is part of the character set. 
        /// </summary>
        /// <param name="characterSet">character set</param>
        /// <param name="codePoint">Unicode code point</param>
        /// <returns><c>true</c> if it is part of the character set, <c>false</c> if not</returns>
        public static bool Contains(this SpsCharacterSet characterSet, int codePoint)
        {
            switch (characterSet)
            {
                case SpsCharacterSet.Latin1Subset:
                    return IsInLatin1Subset(codePoint);
                case SpsCharacterSet.ExtendedLatin:
                    return IsInExtendedLatin(codePoint);
                default:
                    return true; // Unicode
            }
        }

        /// <summary>
        /// Returns if the character is part of the character set. 
        /// </summary>
        /// <param name="characterSet">character set</param>
        /// <param name="ch">character</param>
        /// <returns><c>true</c> if it is part of the character set, <c>false</c> if not</returns>
        public static bool Contains(this SpsCharacterSet characterSet, char ch)
        {
            return characterSet.Contains((int)ch);
        }

        private static bool IsInLatin1Subset(int codePoint)
        {
            if (codePoint < 0x20)
                return false;
            if (codePoint == 0x5e)
                return false;
            if (codePoint <= 0x7e)
                return true;
            if (codePoint == 0xa3 || codePoint == 0xb4)
                return true;
            if (codePoint < 0xc0 || codePoint > 0xfd)
                return false;
            if (codePoint == 0xc3 || codePoint == 0xc5 || codePoint == 0xc6)
                return false;
            if (codePoint == 0xd0 || codePoint == 0xd5 || codePoint == 0xd7 || codePoint == 0xd8)
                return false;
            if (codePoint == 0xdd || codePoint == 0xde)
                return false;
            if (codePoint == 0xe3 || codePoint == 0xe5 || codePoint == 0xe6)
                return false;
            return codePoint != 0xf0 && codePoint != 0xf5 && codePoint != 0xf8;
        }

        private static bool IsInExtendedLatin(int codePoint)
        {
            // Basic Latin
            if (codePoint >= 0x0020 && codePoint <= 0x007E)
                return true;

            // Latin-1 Supplement and Latin Extended-A
            if (codePoint >= 0x00A0 && codePoint <= 0x017F)
                return true;

            // Additional characters
            if (codePoint >= 0x0218 && codePoint <= 0x021B)
                return true;

            if (codePoint == 0x20AC)
                return true;

            return false;
        }
    }
}
