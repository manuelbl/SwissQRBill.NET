using System;
using System.Runtime.Serialization;

namespace Codecrete.SwissQRBill.Generator
{
    public class QRBillInformationValidationException : Exception
    {
        public QRBillInformationValidationException()
        {
        }

        public QRBillInformationValidationException(string message)
            : base(message)
        {
        }

        public QRBillInformationValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected QRBillInformationValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
