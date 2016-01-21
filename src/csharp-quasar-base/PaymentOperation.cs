using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quasar.Generated;
using static Quasar.Preconditions;

namespace Quasar
{
    public class PaymentOperation : Operation
    {
        public KeyPair Destination { get; private set; }
        public Generated.Asset Asset { get; private set; }
        public long Amount { get; private set; }

        private PaymentOperation(KeyPair destination, Generated.Asset asset, long amount)
        {
            this.Destination = CheckNotNull(destination, "destination cannot be null.");
            this.Asset = CheckNotNull(asset, "asset cannot be null.");
            this.Amount = amount;
        }

        public override Generated.Operation.OperationBody ToOperationBody()
        {
            var op = new PaymentOp
            {
                Destination = this.Destination.AccountId,
                Amount = new Generated.Int64(Amount),
                Asset = this.Asset
            };

            var body = new Generated.Operation.OperationBody
            {
                PaymentOp = op,
                Discriminant = OperationType.Create(OperationType.OperationTypeEnum.PAYMENT)
            };

            return body;
        }

        public class Builder
        {
            public KeyPair Destination { get; private set; }
            public KeyPair SourceAccount { get; private set; }
            public Generated.Asset Asset { get; private set; }
            public long Amount { get; private set; }

            public Builder(PaymentOp op)
            {
                this.Destination = KeyPair.FromXdrPublicKey(op.Destination.InnerValue);
                this.Asset = op.Asset;
                this.Amount = op.Amount.InnerValue;
            }

            public Builder(KeyPair destination, Generated.Asset asset, long amount)
            {
                this.Destination = CheckNotNull(destination, "destination cannot be null.");
                this.Asset = CheckNotNull(asset, "asset cannot be null.");
                this.Amount = amount;
            }

            public Builder SetSourceAccount(KeyPair account)
            {
                SourceAccount = account;
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