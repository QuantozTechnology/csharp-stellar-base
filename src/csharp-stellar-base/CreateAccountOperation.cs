using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Stellar.Preconditions;

namespace Stellar
{
    public class CreateAccountOperation : Operation
    {
        public KeyPair Destination { get; private set; }
        public long StartingBalance { get; private set; }

        private CreateAccountOperation(KeyPair destination, long startingBalance)
        {
            Destination = CheckNotNull(destination, "destination cannot be null.");
            if(startingBalance < 0)
            {
                throw new ArgumentException("startingBalance must be non-negative.");
            }
            StartingBalance = startingBalance;
        }

        public static new CreateAccountOperation FromXDR(Generated.Operation xdr)
        {
            return (CreateAccountOperation)Operation.FromXDR(xdr);
        }

        public override Generated.Operation.OperationBody ToOperationBody()
        {
            var op = new Generated.CreateAccountOp
            {
                Destination = Destination.AccountId,
                StartingBalance = new Generated.Int64(StartingBalance)
            };

            var body = new Generated.Operation.OperationBody
            {
                CreateAccountOp = op,
                Discriminant = Generated.OperationType.Create(Generated.OperationType.OperationTypeEnum.CREATE_ACCOUNT)
            };

            return body;
        }

        public class Builder
        {
            public KeyPair SourceAccount { get; private set; }
            public KeyPair Destination { get; private set; }
            public long StartingBalance { get; private set; }

            public Builder(Generated.CreateAccountOp op)
            {
                Destination = KeyPair.FromXdrPublicKey(op.Destination.InnerValue);
                StartingBalance = op.StartingBalance.InnerValue;
            }

            public Builder(KeyPair destination, long startingBalance)
            {
                Destination = CheckNotNull(destination, "destination cannot be null.");
                if (startingBalance < 0)
                {
                    throw new ArgumentException("startingBalance must be non-negative.");
                }
                StartingBalance = startingBalance;
            }

            public Builder SetSourceAccount(KeyPair sourceAccount)
            {
                SourceAccount = CheckNotNull(sourceAccount, "sourceAccount cannot be null.");
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
