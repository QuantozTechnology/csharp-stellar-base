using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stellar;
using Stellar.Generated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csharp_stellar_base.Tests
{
    [TestClass]
    public class TransactionTests
    {
        public TransactionTests()
        {
            Stellar.Network.CurrentNetwork = "";
        }

        public Stellar.Transaction SampleTransaction()
        {
            Stellar.Network.CurrentNetwork = "";

            var master = KeyPair.Master();
            var random = KeyPair.Random();

            var sourceAccount = new Account(master, 1);

            CreateAccountOperation operation =
                new CreateAccountOperation.Builder(random, 1000)
                //.SetSourceAccount(master)
                .Build();

            Stellar.Transaction transaction =
                new Stellar.Transaction.Builder(sourceAccount)
                .AddOperation(operation)
                .Build();

            return transaction;
        }

        [TestMethod]
        public void SignatureBaseTest()
        {
            var transaction = SampleTransaction();
            var txXdr = transaction.ToXdr();

            var writer = new Stellar.Generated.ByteWriter();
            Stellar.Generated.Transaction.Encode(writer, txXdr);
            string sig64 = Convert.ToBase64String(writer.ToArray());

            string sigSample64 = "AAAAAP7Ru1nO+h1oAv7VJP5i+LRBxajZxBQ+gOtOLhkssYBmAAAAZAAAAAAAAAAAAAAAAAAAAAEAAAAAAAAAAAAAAAEAAAAAAAAACQAAAAFVU0QAAAAAAP7Ru1nO+h1oAv7VJP5i+LRBxajZxBQ+gOtOLhkssYBmAAAAAQAAAAEAAAAA";
            byte[] sigSample = Convert.FromBase64String(sigSample64);

            var reader = new Stellar.Generated.ByteReader(sigSample);
            var sampleTx = Stellar.Generated.Transaction.Decode(reader);

            CollectionAssert.AreEqual(writer.ToArray(), sigSample);

            Assert.AreEqual(sigSample64, sig64);
        }

        [TestMethod]
        public void HashTest()
        {
            var transaction = SampleTransaction();

            byte[] hash = transaction.Hash();
            string hash64 = Convert.ToBase64String(hash);

            string hashSample64 = "8eKVr1wYJFImQO7p4Ol0qC4TI2yVYjbRnV+d+a3uGHc=";

            Assert.AreEqual(hashSample64, hash64);
        }
    }
}