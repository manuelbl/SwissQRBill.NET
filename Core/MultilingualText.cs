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

        private static readonly Dictionary<Language, ResourceSet> AllResourceSets = new Dictionary<Language, ResourceSet>();

        /// <summary>
        /// Text key for "Payment part"
        /// </summary>
        public const string KeyPaymentPart = "PaymentPart";
        /// <summary>
        /// Text key for "Account / payable to"
        /// </summary>
        internal const string KeyAccountPayableTo = "AccountPayableTo";
        /// <summary>
        /// Text key for "Reference"
        /// </summary>
        internal const string KeyReference = "Reference";
        /// <summary>
        /// Text key for "Additional information"
        /// </summary>
        internal const string KeyAdditionalInformation = "AdditionalInfo";
        /// <summary>
        /// Text key for "Currency"
        /// </summary>
        internal const string KeyCurrency = "Currency";
        /// <summary>
        /// Text key for "Amount"
        /// </summary>
        internal const string KeyAmount = "Amount";
        /// <summary>
        /// Text key for "Receipt"
        /// </summary>
        internal const string KeyReceipt = "Receipt";
        /// <summary>
        /// Text key for "Acceptance point"
        /// </summary>
        internal const string KeyAcceptancePoint = "AcceptancePoint";
        /// <summary>
        /// Text key for "Payable by"
        /// </summary>
        internal const string KeyPayableBy = "PayableBy";
        /// <summary>
        /// Text key for "Payable by (name / address)"
        /// </summary>
        internal const string KeyPayableByNameAddr = "PayableByNameAddr";
        /// <summary>
        /// Text key for "DO NOT USE FOR PAYMENT"
        /// </summary>
        internal const string KeyDoNotUseForPayment = "DoNotUseForPayment";

        /// <summary>
        /// Gets the resource set for the specified language.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <returns>The resource set for the specified language.</returns>
        internal static ResourceSet GetResourceSet(Language language)
        {
            lock (Lock)
            {
                if (AllResourceSets.ContainsKey(language))
                {
                    return AllResourceSets[language];
                }

                var languageName = language.ToString().ToLowerInvariant();
                var assembly = Assembly.GetExecutingAssembly();
                // ReSharper disable once AssignNullToNotNullAttribute
                var resourceSet = new ResourceSet(assembly.GetManifestResourceStream(typeof(MultilingualText), $"Resources.QRBillText-{languageName}.resources"));

                AllResourceSets[language] = resourceSet;
                return resourceSet;
            }
        }
    }
}
