using NUnit.Framework;
using Stellar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csharp_stellar_base.Tests
{
    [TestFixture]
    public class AssetTests
    {
        [Test]
        public void TestNativeAsset()
        {
            Asset asset = new Asset();

            Assert.AreEqual(Asset.AssetTypeEnum.ASSET_TYPE_NATIVE, asset.Type);

            Stellar.Generated.Asset genAsset = asset.ToXDR();

            Assert.AreEqual(Stellar.Generated.AssetType.AssetTypeEnum.ASSET_TYPE_NATIVE, genAsset.Discriminant.InnerValue);

            Asset resAsset = Asset.FromXDR(genAsset);

            Assert.AreEqual(Asset.AssetTypeEnum.ASSET_TYPE_NATIVE, resAsset.Type);
        }

        [Test]
        public void TestAlphaNum4Asset()
        {
            var keyPair = KeyPair.Master();
            string code = "Test";
            Asset asset = new Asset(code, keyPair);

            Assert.AreEqual(code, asset.Code);
            Assert.AreEqual(keyPair, asset.Issuer);
            Assert.AreEqual(Asset.AssetTypeEnum.ASSET_TYPE_CREDIT_ALPHANUM4, asset.Type);

            Stellar.Generated.Asset genAsset = asset.ToXDR();

            Assert.AreEqual(Encoding.ASCII.GetBytes(code).ToString(), genAsset.AlphaNum4.AssetCode.ToString());
            Assert.AreEqual(keyPair.PublicKey.ToString(), genAsset.AlphaNum4.Issuer.InnerValue.Ed25519.InnerValue.ToString());
            Assert.AreEqual(Stellar.Generated.AssetType.AssetTypeEnum.ASSET_TYPE_CREDIT_ALPHANUM4, genAsset.Discriminant.InnerValue);

            Asset resAsset = Asset.FromXDR(genAsset);

            Assert.AreEqual(code, resAsset.Code);
            Assert.AreEqual(keyPair.Address, resAsset.Issuer.Address, keyPair.Address);
            Assert.AreEqual(keyPair.PublicKey.ToString(), resAsset.Issuer.PublicKey.ToString());
            Assert.AreEqual(Asset.AssetTypeEnum.ASSET_TYPE_CREDIT_ALPHANUM4, resAsset.Type);
        }

        [Test]
        public void TestAlphaNum12Asset()
        {
            var keyPair = KeyPair.Master();
            string code = "TestTestTest";
            Asset asset = new Asset(code, keyPair);

            Assert.AreEqual(code, asset.Code);
            Assert.AreEqual(keyPair, asset.Issuer);
            Assert.AreEqual(Asset.AssetTypeEnum.ASSET_TYPE_CREDIT_ALPHANUM12, asset.Type);

            Stellar.Generated.Asset genAsset = asset.ToXDR();

            Assert.AreEqual(Encoding.ASCII.GetBytes(code).ToString(), genAsset.AlphaNum12.AssetCode.ToString());
            Assert.AreEqual(keyPair.PublicKey.ToString(), genAsset.AlphaNum12.Issuer.InnerValue.Ed25519.InnerValue.ToString());
            Assert.AreEqual(Stellar.Generated.AssetType.AssetTypeEnum.ASSET_TYPE_CREDIT_ALPHANUM12, genAsset.Discriminant.InnerValue);

            Asset resAsset = Asset.FromXDR(genAsset);

            Assert.AreEqual(code, resAsset.Code);
            Assert.AreEqual(keyPair.Address, resAsset.Issuer.Address);
            Assert.AreEqual(keyPair.PublicKey.ToString(), resAsset.Issuer.PublicKey.ToString());
            Assert.AreEqual(Asset.AssetTypeEnum.ASSET_TYPE_CREDIT_ALPHANUM12, resAsset.Type);
        }

        [Test]
        public void TestAlphaNumAssetNullCode()
        {
            var ex = Assert.Throws<NullReferenceException>(() => new Asset(null, KeyPair.Master()));
            Assert.AreEqual(ex.Message, "code cannot be null.");
        }

        [Test]
        public void TestAlphaNumAssetShortCode()
        {
            var ex = Assert.Throws<ArgumentException>(() => new Asset("", KeyPair.Master()));
            Assert.AreEqual(ex.Message, "Invalid code, should have positive length, no larger than 12.");
        }

        [Test]
        public void TestAlphaNumAssetLongCode()
        {
            var ex = Assert.Throws<ArgumentException>(() => new Asset("ThisIsTooLongACode", KeyPair.Master()));
            Assert.AreEqual(ex.Message, "Invalid code, should have positive length, no larger than 12.");
        }

        [Test]
        public void TestAlphaNumAssetNullIssuer()
        {
            var ex = Assert.Throws<NullReferenceException>(() => new Asset("Test", null));
            Assert.AreEqual(ex.Message, "issuer cannot be null.");
        }
    }
}
