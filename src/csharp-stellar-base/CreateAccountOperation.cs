using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stellar.Generated;
using static Stellar.Preconditions;

namespace Stellar
{
    public class CreateAccountOperation : Operation
    {
        public KeyPair Destination { get; private set; }
        public long StartingBalance { get; private set; }

        private CreateAccountOperation(KeyPair destination, long startingBalance)
        {
            this.Destination = CheckNotNull(destination, "destination cannot be null.");
            this.StartingBalance = startingBalance;
        }

        public override Generated.Operation.OperationBody ToOperationBody()
        {
            var op = new CreateAccountOp
            {
                Destination = this.Destination.AccountId,
                StartingBalance = new Generated.Int64(StartingBalance)
            };

            var body = new Generated.Operation.OperationBody
            {
                CreateAccountOp = op,
                Discriminant = OperationType.Create(OperationType.OperationTypeEnum.CREATE_ACCOUNT)
            };

            return body;
        }

        public class Builder
        {
            public KeyPair SourceAccount { get; private set; }
            public KeyPair Destination { get; private set; }
            public long StartingBalance { get; private set; }

            public Builder(CreateAccountOp op)
            {
                this.Destination = KeyPair.FromXdrPublicKey(op.Destination.InnerValue);
                this.StartingBalance = op.StartingBalance.InnerValue;
            }

            public Builder(KeyPair destination, long startingBalance)
            {
                this.Destination = CheckNotNull(destination, "destination cannot be null.");
                this.StartingBalance = startingBalance;
            }

            public Builder SetSourceAccount(KeyPair account)
            {
                SourceAccount = account;
                return this;
            }

            public CreateAccountOperation Build()
            {
                CreateAccountOperation operation =
                    new CreateAccountOperation(Destination, StartingBalance);
                if (SourceAccount != null)
                {
                    operation.SourceAccount = SourceAccount;
                }
                return operation;
            }
        }
    }
}
