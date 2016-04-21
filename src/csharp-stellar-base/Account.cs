using static Stellar.Preconditions;

namespace Stellar
{
    public class Account : ITransactionBuilderAccount
    {
        public KeyPair KeyPair
        {
            get;
            private set;
        }

        public long SequenceNumber
        {
            get;
            private set;
        }

        public Account(KeyPair keyPair, long sequenceNumber)
        {
            this.KeyPair = CheckNotNull(keyPair, "keyPair cannot be null.");
            this.SequenceNumber = sequenceNumber;
        }

        public void IncrementSequenceNumber()
        {
            SequenceNumber++;
        }
    }
}
