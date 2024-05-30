//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using System;
using Xunit;

namespace Codecrete.SwissQRBill.CoreTest
{
    public class QRBillGenerationExceptionTest
    {
        [Fact]
        public void MessageOnly()
        {
            QRBillGenerationException e = Assert.Throws<QRBillGenerationException>(
                () => ThrowQRBillException()
            );

            Assert.Equal("ABC", e.Message);
            Assert.Null(e.InnerException);
        }

        private static void ThrowQRBillException()
        {
            throw new QRBillGenerationException("ABC");
        }

        [Fact]
        public void MessageAndCause()
        {
            QRBillGenerationException e = Assert.Throws<QRBillGenerationException>(
                () => ThrowNestedNullRefException()
            );

            Assert.Equal("QRS", e.Message);
            Assert.Same(typeof(NullReferenceException), e.InnerException.GetType());
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S1481:Unused local variables should be removed", Justification = "Required to throw NullReference exception")]
        private static void ThrowNestedNullRefException()
        {
            try
            {
                int len = ((string)null).Length;
            }
            catch (Exception npe)
            {
                throw new QRBillGenerationException("QRS", npe);
            }

        }
    }
}
