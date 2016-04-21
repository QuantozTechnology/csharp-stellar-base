using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stellar;
using System;

namespace csharp_stellar_base.Tests
{
    [TestClass]
    public class StrKeyTests
    {
        [TestMethod]
        public void TestDecodeEncode()
        {
            String seed = "SDJHRQF4GCMIIKAAAQ6IHY42X73FQFLHUULAPSKKD4DFDM7UXWWCRHBE";
            byte[] secret = StrKey.DecodeCheck(VersionByte.Seed, seed);
            String encoded = StrKey.EncodeCheck(VersionByte.Seed, secret);
            Assert.AreEqual(seed, encoded);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void TestDecodeInvalidVersionByte()
        {
            String accountId = "GCZHXL5HXQX5ABDM26LHYRCQZ5OJFHLOPLZX47WEBP3V2PF5AVFK2A5D";
            StrKey.DecodeCheck(VersionByte.Seed, accountId);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void TestDecodeInvalidSeed()
        {
            String seed = "SAA6NXOBOXP3RXGAXBW6PGFI5BPK4ODVAWITS4VDOMN5C2M4B66ZML";
            StrKey.DecodeCheck(VersionByte.Seed, seed);
        }
    }
}
