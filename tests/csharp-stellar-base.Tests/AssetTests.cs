using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stellar;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csharp_stellar_base.Tests
{
    [TestClass]
    public class AssetTests
    {
        [TestMethod]
        public void TestNativeAsset()
        {
            Asset asset = new Asset();

            Assert.AreEqual(Asset.AssetTypeEnum.ASSET_TYPE_NATIVE, asset.Type);

            Stellar.Generated.Asset genAsset = asset.ToXDR();

            Assert.AreEqual(Stellar.Generated.AssetType.AssetTypeEnum.ASSET_TYPE_NATIVE, genAsset.Discriminant.InnerValue);

            Asset resAsset = Asset.FromXDR(genAsset);

            Assert.AreEqual(Asset.AssetTypeEnum.ASSET_TYPE_NATIVE, resAsset.Type);
        }

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException), "code cannot be null.")]
        public void TestAlphaNumAssetNullCode()
        {
            Asset asset = new Asset(null, KeyPair.Master());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Invalid code, should have positive length, no larger than 12.")]
        public void TestAlphaNumAssetShortCode()
        {
            Asset asset = new Asset("", KeyPair.Master());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Invalid code, should have positive length, no larger than 12.")]
        public void TestAlphaNumAssetLongCode()
        {
            Asset asset = new Asset("ThisIsTooLongACode", KeyPair.Master());
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException), "issuer cannot be null.")]
        public void TestAlphaNumAssetNullIssuer()
        {
            Asset asset = new Asset("Test", null);
        }
    }
}
