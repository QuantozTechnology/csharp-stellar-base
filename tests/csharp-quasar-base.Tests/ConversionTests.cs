using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csharp_quasar_base.Tests
{
    [TestClass]
    public class ConversionTests
    {
        [TestMethod]
        public void AccountIdConversion()
        {
            Quasar.Network.CurrentNetwork = "ProjectQ";

            var masterPair = KeyPair.Master();
            var masterAccount = masterPair.AccountId;

            string sample64 = "AAAAAP7Ru1nO+h1oAv7VJP5i+LRBxajZxBQ+gOtOLhkssYBm";
            byte[] sample = Convert.FromBase64String(sample64);

            var reader = new Quasar.Generated.ByteReader(sample);
            var sampleAccount = Quasar.Generated.AccountID.Decode(reader);

            Assert.AreEqual(
                masterAccount.InnerValue.Discriminant.InnerValue,
                sampleAccount.InnerValue.Discriminant.InnerValue);

            CollectionAssert.AreEqual(
                masterAccount.InnerValue.Ed25519.InnerValue,
                sampleAccount.InnerValue.Ed25519.InnerValue);

            var writer = new Quasar.Generated.ByteWriter();
            Quasar.Generated.AccountID.Encode(writer, masterAccount);
            string master64 = Convert.ToBase64String(writer.ToArray());

            Assert.AreEqual(sample64, master64);
        }

        [TestMethod]
        public void NativeAssetConversion()
        {
            Quasar.Network.CurrentNetwork = "";

            var native = Asset.Native();

            string sample64 = "AAAAAA==";
            byte[] sample = Convert.FromBase64String(sample64);

            var reader = new Quasar.Generated.ByteReader(sample);
            var sampleAsset = Quasar.Generated.Asset.Decode(reader);

            Assert.AreEqual(
                native.Discriminant.InnerValue,
                sampleAsset.Discriminant.InnerValue);

            var writer = new Quasar.Generated.ByteWriter();
            Quasar.Generated.Asset.Encode(writer, native);
            string native64 = Convert.ToBase64String(writer.ToArray());

            Assert.AreEqual(sample64, native64);
        }

        [TestMethod]
        public void CustomAssetConversion()
        {
            Quasar.Network.CurrentNetwork = "ProjectQ";
            var master = KeyPair.Master();

            var alphaNum4 = new Quasar.Generated.Asset.AssetAlphaNum4
            {
                AssetCode = ASCIIEncoding.ASCII.GetBytes("USD\0"),
                Issuer = master.AccountId
            };

            var asset = new Quasar.Generated.Asset
            {
                AlphaNum4 = alphaNum4,
                Discriminant = Quasar.Generated.AssetType.Create(Quasar.Generated.AssetType.AssetTypeEnum.ASSET_TYPE_CREDIT_ALPHANUM4)
            };

            string sample64 = "AAAAAVVTRAAAAAAA/tG7Wc76HWgC/tUk/mL4tEHFqNnEFD6A604uGSyxgGY=";
            byte[] sample = Convert.FromBase64String(sample64);

            var reader = new Quasar.Generated.ByteReader(sample);
            var sampleAsset = Quasar.Generated.Asset.Decode(reader);

            Assert.AreEqual(
                asset.Discriminant.InnerValue,
                sampleAsset.Discriminant.InnerValue);

            CollectionAssert.AreEqual(
                asset.AlphaNum4.AssetCode,
                sampleAsset.AlphaNum4.AssetCode);

            CollectionAssert.AreEqual(
                asset.AlphaNum4.Issuer.InnerValue.Ed25519.InnerValue,
                sampleAsset.AlphaNum4.Issuer.InnerValue.Ed25519.InnerValue);

            var writer = new Quasar.Generated.ByteWriter();
            Quasar.Generated.Asset.Encode(writer, asset);
            string native64 = Convert.ToBase64String(writer.ToArray());

            Assert.AreEqual(sample64, native64);
        }
    }
}