//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System.Collections.Generic;
using System.Reflection;
using System.Resources;

namespace Codecrete.SwissQRBill.Generator
{
    /// <summary>
    /// Helper class providing multilingual texts printed on QR bills.
    /// </summary>
    internal static class MultilingualText
    {
        private static readonly object Lock = new object();

        private static Dictionary<Language, ResourceSet> allResourceSets = new Dictionary<Language, ResourceSet>();

        /// <summary>
        /// Text key for "Payment part"
        /// </summary>
        public static readonly string KeyPaymentPart = "PaymentPart";
        /// <summary>
        /// Text key for "Account / payable to"
        /// </summary>
        internal static readonly string KeyAccountPayableTo = "AccountPayableTo";
        /// <summary>
        /// Text key for "Reference"
        /// </summary>
        internal static readonly string KeyReference = "Reference";
        /// <summary>
        /// Text key for "Additional information"
        /// </summary>
        internal static readonly string KeyAdditionalInformation = "AdditionalInfo";
        /// <summary>
        /// Text key for "Currency"
        /// </summary>
        internal static readonly string KeyCurrency = "Currency";
        /// <summary>
        /// Text key for "Amount"
        /// </summary>
        internal static readonly string KeyAmount = "Amount";
        /// <summary>
        /// Text key for "Receipt"
        /// </summary>
        internal static readonly string KeyReceipt = "Receipt";
        /// <summary>
        /// Text key for "Acceptance point"
        /// </summary>
        internal static readonly string KeyAcceptancePoint = "AcceptancePoint";
        /// <summary>
        /// Text key for "Payable by"
        /// </summary>
        internal static readonly string KeyPayableBy = "PayableBy";
        /// <summary>
        /// Text key for "Payable by (name / address)"
        /// </summary>
        internal static readonly string KeyPayableByNameAddr = "PayableByNameAddr";

        /// <summary>
        /// Gets the resource set for the specified language.
        /// </summary>
        /// <param name="language"language></param>
        /// <returns>resource set</returns>
        internal static ResourceSet GetResourceSet(Language language)
        {
            lock (Lock)
            {
                if (allResourceSets.ContainsKey(language))
                {
                    return allResourceSets[language];
                }

                string languageName = language.ToString().ToLowerInvariant();
                Assembly assembly = Assembly.GetExecutingAssembly();
                ResourceSet resourceSet = new ResourceSet(assembly.GetManifestResourceStream(typeof(MultilingualText), $"Resources.QRBillText-{languageName}.resources"));

                allResourceSets[language] = resourceSet;
                return resourceSet;
            }
        }
    }
}
