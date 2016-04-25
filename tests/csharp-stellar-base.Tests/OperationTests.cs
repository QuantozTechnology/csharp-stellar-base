using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stellar;
using System;
using System.Text;

namespace csharp_stellar_base.Tests
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

            Stellar.Generated.Asset asset = Stellar.Asset.Native();
            long amount = 1000;

            PaymentOperation operation = new PaymentOperation.Builder(destination, asset, amount)
                .SetSourceAccount(source)
                .Build();

            Stellar.Generated.Operation xdr = operation.ToXdr();
            PaymentOperation parsedOperation = (PaymentOperation)Operation.FromXdr(xdr);

            Assert.AreEqual(source.Address, parsedOperation.SourceAccount.Address);
            Assert.AreEqual(destination.Address, parsedOperation.Destination.Address);
            //Assert.IsTrue(parsedOperation.Asset instanceof AssetTypeNative);
            Assert.AreEqual(amount, parsedOperation.Amount);

            Assert.AreEqual(
                    "AAAAAQAAAAC7JAuE3XvquOnbsgv2SRztjuk4RoBVefQ0rlrFMMQvfAAAAAEAAAAA7eBSYbzcL5UKo7oXO24y1ckX+XuCtkDsyNHOp1n1bxAAAAAAAAAAAAAAA+g=",
                    operation.ToXdrBase64());
        }

        [TestMethod]
        public void ChangeTrustOperation()
        {
            // GC5SIC4E3V56VOHJ3OZAX5SJDTWY52JYI2AFK6PUGSXFVRJQYQXXZBZF
            var source = KeyPair.FromSeed("SC4CGETADVYTCR5HEAVZRB3DZQY5Y4J7RFNJTRA6ESMHIPEZUSTE2QDK");
            var assetCode = "EUR";
            var asset = new Stellar.Generated.Asset
                        {
                            Discriminant = Stellar.Generated.AssetType.Create(
                                Stellar.Generated.AssetType.AssetTypeEnum.ASSET_TYPE_CREDIT_ALPHANUM4),
                            AlphaNum4 = new Stellar.Generated.Asset.AssetAlphaNum4
                            {
                                AssetCode = Encoding.ASCII.GetBytes(assetCode),
                                Issuer = source.AccountId
                            }
                        };

            long limit = 100;

            ChangeTrustOperation operation = new ChangeTrustOperation.Builder(asset, limit)
                .SetSourceAccount(source)
                .Build();

            Stellar.Generated.Operation xdr = operation.ToXdr();
            ChangeTrustOperation parsedOperation = Stellar.ChangeTrustOperation.FromXdr(xdr);

            Assert.AreEqual(source.Address, parsedOperation.SourceAccount.Address);
            Assert.AreEqual("EUR", Encoding.ASCII.GetString(parsedOperation.Asset.AlphaNum4.AssetCode));
            Assert.AreEqual(Stellar.Generated.AssetType.AssetTypeEnum.ASSET_TYPE_CREDIT_ALPHANUM4,
                            parsedOperation.Asset.Discriminant.InnerValue);
            Assert.AreEqual(limit, parsedOperation.Limit);
            Assert.AreEqual("AAAAAQAAAAC7JAuE3XvquOnbsgv2SRztjuk4RoBVefQ0rlrFMMQvfAAAAAYAAAABRVVSAAAAAAC7JAuE3XvquOnbsgv2SRztjuk4RoBVefQ0rlrFMMQvfAAAAAAAAABk",
                    operation.ToXdrBase64());
        }
    }
}
