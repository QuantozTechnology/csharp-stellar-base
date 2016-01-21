using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasar;
using System.Text;
using System.Collections.Generic;

namespace csharp_quasar_base.Tests
{
    [TestClass]
    public class KeyPairTests
    {
        private static string seed = "1123740522f11bfef6b3671f51e159ccf589ccf8965262dd5f97d1721d383dd4";


        [TestMethod]
        public void MasterKeyPair()
        {
            Network.CurrentNetwork = "ProjectQ";
            var keyPair = KeyPair.Master();

            CollectionAssert.AreEqual(keyPair.PrivateKey, KeyPair.FromSeed("SCEQIZTB6HSQJA4QJZKUNHDRGG3WXSWKLHINW5HJYDAYRM24M62JVXE2").PrivateKey);
            Assert.AreEqual("GD7NDO2ZZ35B22AC73KSJ7TC7C2EDRNI3HCBIPUA5NHC4GJMWGAGMP7G", keyPair.Address);
        }

        [TestMethod]
        public void TestSeed()
        {
            Network.CurrentNetwork = "ProjectQ";
            var keyPair = KeyPair.Master();

            Assert.AreEqual("SCEQIZTB6HSQJA4QJZKUNHDRGG3WXSWKLHINW5HJYDAYRM24M62JVXE2", keyPair.Seed);
        }

        [TestMethod]
        public void TestSign()
        {
            string expectedSig = "587d4b472eeef7d07aafcd0b049640b0bb3f39784118c2e2b73a04fa2f64c9c538b4b2d0f5335e968a480021fdc23e98c0ddf424cb15d8131df8cb6c4bb58309";
            KeyPair keypair = KeyPair.FromRawSeed(Chaos.NaCl.CryptoBytes.FromHexString(seed));
            string data = "hello world";
            byte[] sig = keypair.Sign(data);
            byte[] expected = Chaos.NaCl.CryptoBytes.FromHexString(expectedSig);
            CollectionAssert.AreEqual(expected, sig);
        }

        [TestMethod]
        public void TestVerifyTrue()
        {
            string sig = "587d4b472eeef7d07aafcd0b049640b0bb3f39784118c2e2b73a04fa2f64c9c538b4b2d0f5335e968a480021fdc23e98c0ddf424cb15d8131df8cb6c4bb58309";
            KeyPair keypair = KeyPair.FromRawSeed(Chaos.NaCl.CryptoBytes.FromHexString(seed));
            string data = "hello world";
            byte[] expected = Chaos.NaCl.CryptoBytes.FromHexString(sig);
            byte[] byteData = UTF8Encoding.UTF8.GetBytes(data);
            Assert.IsTrue(keypair.Verify(expected, byteData));
        }

        [TestMethod]
        public void TestVerifyFalse()
        {
            string badSig = "687d4b472eeef7d07aafcd0b049640b0bb3f39784118c2e2b73a04fa2f64c9c538b4b2d0f5335e968a480021fdc23e98c0ddf424cb15d8131df8cb6c4bb58309";
            byte[] corrupt = { 0x00 };
            string data = "hello world";
            KeyPair keypair = KeyPair.FromRawSeed(Chaos.NaCl.CryptoBytes.FromHexString(seed));
            Assert.IsFalse(keypair.Verify(Encoding.UTF8.GetBytes(data), Chaos.NaCl.CryptoBytes.FromHexString(badSig)));
            Assert.IsFalse(keypair.Verify(Encoding.UTF8.GetBytes(data), corrupt));
        }

        [TestMethod]
        public void TestFromSecretSeed()
        {
            var keypairs = new Dictionary<string, string>();
            keypairs.Add("SDJHRQF4GCMIIKAAAQ6IHY42X73FQFLHUULAPSKKD4DFDM7UXWWCRHBE", "GCZHXL5HXQX5ABDM26LHYRCQZ5OJFHLOPLZX47WEBP3V2PF5AVFK2A5D");
            keypairs.Add("SDTQN6XUC3D2Z6TIG3XLUTJIMHOSON2FMSKCTM2OHKKH2UX56RQ7R5Y4", "GDEAOZWTVHQZGGJY6KG4NAGJQ6DXATXAJO3AMW7C4IXLKMPWWB4FDNFZ");
            keypairs.Add("SDIREFASXYQVEI6RWCQW7F37E6YNXECQJ4SPIOFMMMJRU5CMDQVW32L5", "GD2EVR7DGDLNKWEG366FIKXO2KCUAIE3HBUQP4RNY7LEZR5LDKBYHMM6");
            keypairs.Add("SDAPE6RHEJ7745VQEKCI2LMYKZB3H6H366I33A42DG7XKV57673XLCC2", "GDLXVH2BTLCLZM53GF7ELZFF4BW4MHH2WXEA4Z5Z3O6DPNZNR44A56UJ");
            keypairs.Add("SDYZ5IYOML3LTWJ6WIAC2YWORKVO7GJRTPPGGNJQERH72I6ZCQHDAJZN", "GABXJTV7ELEB2TQZKJYEGXBUIG6QODJULKJDI65KZMIZZG2EACJU5EA7");

            foreach (var pair in keypairs)
            {
                string address = pair.Value;
                KeyPair keypair = KeyPair.FromSeed(pair.Key);
                Assert.AreEqual(address, keypair.Address);
                Assert.AreEqual(pair.Key, keypair.Seed);
            }
        }
    }
}
