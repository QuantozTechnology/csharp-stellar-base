using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stellar.Generated;
using static Stellar.Preconditions;

namespace Stellar
{
    public class ManageDataOperation : Operation
    {
        public string DataName { get; private set; }
        public byte[] DataValue { get; private set; }

        private ManageDataOperation(string name, byte[] value)
        {
            DataName = name;
            DataValue = value;
        }

        public override Generated.Operation.OperationBody ToOperationBody()
        {
            var op = new ManageDataOp
            {
                DataName = new String64(DataName),
                DataValue = new DataValue(DataValue)
            };

            var body = new Generated.Operation.OperationBody
            {
                ManageDataOp = op,
                Discriminant = OperationType.Create(OperationType.OperationTypeEnum.MANAGE_DATA)
            };

            return body;
        }

        public class Builder
        {
            public KeyPair SourceAccount { get; private set; }
            public string DataName { get; private set; }
            public byte[] DataValue { get; private set; }

            public Builder(ManageDataOp op)
            {
                DataName = op.DataName.InnerValue;
                DataValue = op.DataValue.InnerValue;
            }

            public Builder(string name, byte[] value)
            {
                DataName = name;
                DataValue = value;
            }

            public ManageDataOperation Build()
            {
                ManageDataOperation operation = new ManageDataOperation(DataName, DataValue);
                if (SourceAccount != null)
                {
                    operation.SourceAccount = SourceAccount;
                }
                return operation;
            }
        }
    }
}
