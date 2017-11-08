using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Stellar.Preconditions;

namespace Stellar
{
    public class ChangeTrustOperation : Operation
    {
        public Asset Asset { get; private set; }
        public long Limit { get; private set; }

        private ChangeTrustOperation(Asset asset, long limit)
        {
            Asset = CheckNotNull(asset, "asset cannot be null.");
            Limit = CheckNotNull(limit, "limit cannot be null.");
        }

        public static new ChangeTrustOperation FromXDR(Generated.Operation xdr)
        {
            return (ChangeTrustOperation)Operation.FromXDR(xdr);
        }

        public override Generated.Operation.OperationBody ToOperationBody()
        {
            var op = new Generated.ChangeTrustOp
            {
                Line = Asset.ToXDR(),
                Limit = new Generated.Int64(Limit)
            };

            var body = new Generated.Operation.OperationBody
            {
                ChangeTrustOp = op,
                Discriminant = Generated.OperationType.Create(Generated.OperationType.OperationTypeEnum.CHANGE_TRUST)
            };

            return body;
        }

        public class Builder
        {
            public KeyPair SourceAccount { get; private set; }
            public Asset Asset { get; private set; }
            public long Limit { get; private set; }

            public Builder(Generated.ChangeTrustOp op)
            {
                Asset = Stellar.Asset.FromXDR(op.Line);
                Limit = op.Limit.InnerValue;
            }

            public Builder(Asset asset, long limit)
            {
                Asset = CheckNotNull(asset, "asset cannot be null.");
                if(limit < 0)
                {
                    throw new ArgumentException("limit must be non-negative.");
                }
                Limit = CheckNotNull(limit, "limit cannot be null.");
            }

            public Builder SetSourceAccount(KeyPair sourceAccount)
            {
                SourceAccount = CheckNotNull(sourceAccount, "sourceAccount cannot be null.");
                return this;
            }

            public ChangeTrustOperation Build()
            {
                ChangeTrustOperation operation =
                    new ChangeTrustOperation(Asset, Limit);
                if (SourceAccount != null)
                {
                    operation.SourceAccount = SourceAccount;
                }
                return operation;
            }
        }
    }
}
