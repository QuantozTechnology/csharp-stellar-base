using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Stellar.Preconditions;

namespace Stellar
{
    public class Transaction
    {
        private int BASE_FEE = 100;

        private int mFee;
        private Operation[] mOperations;

        public long SequenceNumber { get; private set; }
        public KeyPair SourceAccount { get; private set; }
        public Memo Memo { get; private set; }

        private IList<Generated.DecoratedSignature> mSignatures;

        public Transaction(KeyPair sourceAccount, long sequenceNumber, Operation[] operations, Memo memo)
        {
            SourceAccount = CheckNotNull(sourceAccount, "sourceAccount cannot be null");
            SequenceNumber = CheckNotNull(sequenceNumber, "sequenceNumber cannot be null");
            mOperations = CheckNotNull(operations, "operations cannot be null");

            if (operations.Length <= 0)
            {
                throw new ArgumentException("At least one operation required");
            }

            mFee = operations.Length * BASE_FEE;
            mSignatures = new List<Generated.DecoratedSignature>();
            Memo = memo != null ? memo : Stellar.Memo.MemoNone();
        }

        /// <summary>
        /// Adds a new signature to this transaction.
        /// </summary>
        /// <param name="signer">object representing a signer</param>
        public void Sign(KeyPair signer)
        {
            byte[] txHash = this.Hash();
            mSignatures.Add(signer.SignDecorated(txHash));
        }

        /// <summary>
        /// Returns transaction hash.
        /// </summary>
        /// <returns></returns>
        public byte[] Hash()
        {
            return Utilities.Hash(this.SignatureBase());
        }

        /// <summary>
        /// Returns signature base.
        /// </summary>
        /// <returns></returns>
        public byte[] SignatureBase()
        {
            var writer = new Generated.ByteWriter();

            // Hashed NetworkID
            writer.Write(Network.CurrentNetworkId);

            // Envelope Type - 4 bytes
            Generated.EnvelopeType.Encode(writer, Generated.EnvelopeType.Create(Generated.EnvelopeType.EnvelopeTypeEnum.ENVELOPE_TYPE_TX));

            // Transaction XDR bytes
            var txWriter = new Generated.ByteWriter();
            Generated.Transaction.Encode(txWriter, this.ToXDR());

            writer.Write(txWriter.ToArray());

            return writer.ToArray();
        }

        /// <summary>
        /// Generates Transaction XDR object.
        /// </summary>
        /// <returns></returns>
        public Generated.Transaction ToXDR()
        {
            // fee
            Generated.Uint32 fee = new Generated.Uint32((uint)mFee);
            // sequenceNumber
            Generated.Uint64 sequenceNumberUint = new Generated.Uint64((ulong)SequenceNumber);
            Generated.SequenceNumber sequenceNumber = new Generated.SequenceNumber(sequenceNumberUint);

            // sourceAccount
            Generated.AccountID sourceAccount = new Generated.AccountID(SourceAccount.AccountId.InnerValue);
            // operations
            Generated.Operation[] operations = mOperations.Select(tx => tx.ToXDR()).ToArray();
            // ext
            Generated.Transaction.TransactionExt ext = new Generated.Transaction.TransactionExt()
            {
                Discriminant = 0
            };

            Generated.Transaction transaction = new Generated.Transaction()
            {
                Fee = fee,
                SeqNum = sequenceNumber,
                SourceAccount = sourceAccount,
                Operations = operations,
                Memo = Memo.ToXDR(),
                Ext = ext,
                //TimeBounds = null,
            };

            return transaction;
        }

        /// <summary>
        /// Generates TransactionEnvelope XDR object. Transaction need to have at least one signature.
        /// </summary>
        /// <returns></returns>
        public Generated.TransactionEnvelope ToEnvelopeXDR()
        {
            if (mSignatures.Count() == 0)
            {
                throw new NotEnoughSignaturesException("Transaction must be signed by at least one signer. Use transaction.Sign().");
            }

            Generated.TransactionEnvelope xdr = new Generated.TransactionEnvelope()
            {
                Tx = ToXDR(),
                Signatures = mSignatures.ToArray()
            };

            return xdr;
        }

        /// <summary>
        /// Returns base64-encoded TransactionEnvelope XDR object. Transaction need to have at least one signature.
        /// </summary>
        /// <returns></returns>
        public string ToEnvelopeXdrBase64()
        {
            var envelope = ToEnvelopeXDR();
            var writer = new Generated.ByteWriter();
            Generated.TransactionEnvelope.Encode(writer, envelope);
            return Convert.ToBase64String(writer.ToArray());
        }

        public class Builder
        {
            public ITransactionBuilderAccount SourceAccount { get; private set; }
            public Memo Memo { get; private set; }
            public IList<Operation> Operations { get; private set; }

            /// <summary>
            /// Construct a new transaction builder.
            /// </summary>
            /// <param name="sourceAccount">
            /// The source account for this transaction. This account is the account
            /// who will use a sequence number. When Build() is called, the account object's sequence number
            /// will be incremented.
            /// </param>
            public Builder(ITransactionBuilderAccount sourceAccount)
            {
                SourceAccount = CheckNotNull(sourceAccount, "sourceAccount cannot be null");
                Operations = new List<Operation>();
            }

            public Builder AddOperation(Operation operation)
            {
                CheckNotNull(operation, "operation cannot be null.");
                Operations.Add(operation);
                return this;
            }

            public Builder AddMemo(Memo memo)
            {
                if (Memo != null)
                {
                    throw new Exception("Memo has already been added.");
                }

                Memo = CheckNotNull(memo, "memo cannot be null.");
                return this;
            }

            public Transaction Build()
            {
                return new Transaction(SourceAccount.KeyPair, SourceAccount.SequenceNumber, Operations.ToArray(), Memo);
            }
        }
    }
}
