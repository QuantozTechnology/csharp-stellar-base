using System;

namespace Quasar
{
    public class NotEnoughSignaturesException : Exception
    {
        public NotEnoughSignaturesException()
            : base()
        {

        }

        public NotEnoughSignaturesException(string message)
            : base(message)
        {

        }
    }
}
