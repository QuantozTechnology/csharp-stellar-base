using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stellar.Generated;
using static Stellar.Preconditions;

namespace Stellar
{
    public class ChangeTrustOperation : Operation
    {
        public Generated.Asset Asset { get; private set; }
        public long Limit { get; private set; }

        private ChangeTrustOperation(Generated.Asset asset, long limit)
        {
            Asset = CheckNotNull(asset, "asset cannot be null.");
            Limit = CheckNotNull(limit, "limit cannot be null.");
        }

        public static new ChangeTrustOperation FromXdr(Generated.Operation xdr)
        {
            return (ChangeTrustOperation)Operation.FromXdr(xdr);
        }

        public override Generated.Operation.OperationBody ToOperationBody()
        {
            var op = new ChangeTrustOp
            {
                Line = Asset,
                Limit = new Generated.Int64(Limit)
            };

            var body = new Generated.Operation.OperationBody
            {
                ChangeTrustOp = op,
                Discriminant = OperationType.Create(OperationType.OperationTypeEnum.CHANGE_TRUST)
            };

            return body;
        }

        public class Builder
        {
            public KeyPair SourceAccount { get; private set; }
            public Generated.Asset Asset { get; private set; }
            public long Limit { get; private set; }

            public Builder(ChangeTrustOp op)
            {
                Asset = op.Line;
                Limit = op.Limit.InnerValue;
            }

            public Builder(Generated.Asset asset, long limit)
            {
                Asset = CheckNotNull(asset, "asset cannot be null.");
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
