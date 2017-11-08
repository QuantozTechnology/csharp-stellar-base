using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stellar
{
    public abstract class Operation
    {
        public KeyPair SourceAccount { get; protected set; }

        public Operation()
        {

        }

        /// <summary>
        /// Generates Operation XDR object.
        /// </summary>
        /// <returns></returns>
        public Generated.Operation ToXDR()
        {
            var xdr = new Generated.Operation();
            if (SourceAccount != null)
            {
                xdr.SourceAccount = SourceAccount.AccountId;
            }

            xdr.Body = ToOperationBody();
            return xdr;
        }

        /// <summary>
        /// Returns base64-encoded Operation XDR object.
        /// </summary>
        /// <returns></returns>
        public string ToXdrBase64()
        {
            var operation = ToXDR();
            var writer = new Generated.ByteWriter();
            Generated.Operation.Encode(writer, operation);
            return Convert.ToBase64String(writer.ToArray());
        }

        public static Operation FromXDR(Generated.Operation xdr)
        {
            var body = xdr.Body;
            Operation operation = null;
            switch (body.Discriminant.InnerValue)
            {
                case Generated.OperationType.OperationTypeEnum.CREATE_ACCOUNT:
                    operation = new CreateAccountOperation.Builder(body.CreateAccountOp).Build();
                    break;
                case Generated.OperationType.OperationTypeEnum.PAYMENT:
                    operation = new PaymentOperation.Builder(body.PaymentOp).Build();
                    break;
                case Generated.OperationType.OperationTypeEnum.PATH_PAYMENT:
                    //operation = new PathPaymentOperation.Builder(body.getPathPaymentOp()).build();
                    break;
                case Generated.OperationType.OperationTypeEnum.MANAGE_OFFER:
                    //operation = new ManagerOfferOperation.Builder(body.getManageOfferOp()).build();
                    break;
                case Generated.OperationType.OperationTypeEnum.CREATE_PASSIVE_OFFER:
                    //operation = new CreatePassiveOfferOperation.Builder(body.getCreatePassiveOfferOp()).build();
                    break;
                case Generated.OperationType.OperationTypeEnum.SET_OPTIONS:
                    //operation = new SetOptionsOperation.Builder(body.getSetOptionsOp()).build();
                    break;
                case Generated.OperationType.OperationTypeEnum.CHANGE_TRUST:
                    operation = new ChangeTrustOperation.Builder(body.ChangeTrustOp).Build();
                    break;
                case Generated.OperationType.OperationTypeEnum.ALLOW_TRUST:
                    //operation = new AllowTrustOperation.Builder(body.getAllowTrustOp()).build();
                    break;
                case Generated.OperationType.OperationTypeEnum.ACCOUNT_MERGE:
                    //operation = new AccountMergeOperation.Builder(body).build();
                    break;
                default:
                    throw new Exception("Unknown operation body " + body.Discriminant.InnerValue);
            }
            if (xdr.SourceAccount != null)
            {
                operation.SourceAccount = KeyPair.FromXdrPublicKey(xdr.SourceAccount.InnerValue);
            }
            return operation;
        }

        /// <summary>
        /// Generates OperationBody XDR object
        /// </summary>
        /// <returns>OperationBody XDR object</returns>
        public abstract Generated.Operation.OperationBody ToOperationBody();
    }
}
