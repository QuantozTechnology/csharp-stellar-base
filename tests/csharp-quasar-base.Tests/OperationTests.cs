using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasar;
using System;
using System.Text;

namespace csharp_quasar_base.Tests
{
    [TestClass]
    public class OperationTests
    {
        [TestMethod]
        public void PaymentOperation()
        {
            // GC5SIC4E3V56VOHJ3OZAX5SJDTWY52JYI2AFK6PUGSXFVRJQYQXXZBZF
            KeyPair source = KeyPair.FromSeed("SC4CGETADVYTCR5HEAVZRB3DZQY5Y4J7RFNJTRA6ESMHIPEZUSTE2QDK");
            // GDW6AUTBXTOC7FIKUO5BOO3OGLK4SF7ZPOBLMQHMZDI45J2Z6VXRB5NR
            KeyPair destination = KeyPair.FromSeed("SDHZGHURAYXKU2KMVHPOXI6JG2Q4BSQUQCEOY72O3QQTCLR2T455PMII");

            Quasar.Generated.Asset asset = Quasar.Asset.Native();
            long amount = 1000;

            PaymentOperation operation = new PaymentOperation.Builder(destination, asset, amount)
                .SetSourceAccount(source)
                .Build();

            Quasar.Generated.Operation xdr = operation.ToXdr();
            PaymentOperation parsedOperation = (PaymentOperation)Operation.FromXdr(xdr);

            Assert.AreEqual(source.Address, parsedOperation.SourceAccount.Address);
            Assert.AreEqual(destination.Address, parsedOperation.Destination.Address);
            //Assert.IsTrue(parsedOperation.Asset instanceof AssetTypeNative);
            Assert.AreEqual(amount, parsedOperation.Amount);

            Assert.AreEqual(
                    "AAAAAQAAAAC7JAuE3XvquOnbsgv2SRztjuk4RoBVefQ0rlrFMMQvfAAAAAEAAAAA7eBSYbzcL5UKo7oXO24y1ckX+XuCtkDsyNHOp1n1bxAAAAAAAAAAAAAAA+g=",
                    operation.ToXdrBase64());
        }
    }
}
