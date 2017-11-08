using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Stellar.Preconditions;

namespace Stellar
{
    public class PaymentOperation : Operation
    {
        public KeyPair Destination { get; private set; }
        public Asset Asset { get; private set; }
        public long Amount { get; private set; }

        private PaymentOperation(KeyPair destination, Asset asset, long amount)
        {
            Destination = CheckNotNull(destination, "destination cannot be null.");
            Asset = CheckNotNull(asset, "asset cannot be null.");
            if(amount < 0)
            {
                throw new ArgumentException("amount must be non-negative.");
            }
            Amount = amount;
        }

        public static new PaymentOperation FromXDR(Generated.Operation xdr)
        {
            return (PaymentOperation)Operation.FromXDR(xdr);
        }

        public override Generated.Operation.OperationBody ToOperationBody()
        {
            var op = new Generated.PaymentOp
            {
                Destination = Destination.AccountId,
                Amount = new Generated.Int64(Amount),
                Asset = Asset.ToXDR()
            };

            var body = new Generated.Operation.OperationBody
            {
                PaymentOp = op,
                Discriminant = Generated.OperationType.Create(Generated.OperationType.OperationTypeEnum.PAYMENT)
            };

            return body;
        }

        public class Builder
        {
            public KeyPair Destination { get; private set; }
            public KeyPair SourceAccount { get; private set; }
            public Asset Asset { get; private set; }
            public long Amount { get; private set; }

            public Builder(Generated.PaymentOp op)
            {
                Destination = KeyPair.FromXdrPublicKey(op.Destination.InnerValue);
                Asset = Asset.FromXDR(op.Asset);
                Amount = op.Amount.InnerValue;
            }

            public Builder(KeyPair destination, Asset asset, long amount)
            {
                Destination = CheckNotNull(destination, "destination cannot be null.");
                Asset = CheckNotNull(asset, "asset cannot be null.");
                if (amount < 0)
                {
                    throw new ArgumentException("amount must be non-negative.");
                }
                Amount = amount;
            }

            public Builder SetSourceAccount(KeyPair sourceAccount)
            {
                SourceAccount = CheckNotNull(sourceAccount, "sourceAccount cannot be null.");
                return this;
            }

            public PaymentOperation Build()
            {
                PaymentOperation operation = new PaymentOperation(Destination, Asset, Amount);
                if (SourceAccount != null)
                {
                    operation.SourceAccount = SourceAccount;
                }
                return operation;
            }
        }
    }
}