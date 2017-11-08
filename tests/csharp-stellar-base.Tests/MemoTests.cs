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
    public class MemoTests
    {
        [TestMethod]
        public void TestMemoNone()
        {
            Memo memo = Memo.MemoNone();

            Assert.AreEqual(Memo.MemoTypeEnum.MEMO_NONE, memo.Type);

            Stellar.Generated.Memo genMemo = memo.ToXDR();

            Assert.AreEqual(Stellar.Generated.MemoType.MemoTypeEnum.MEMO_NONE, genMemo.Discriminant.InnerValue);

            Memo resMemo = Memo.FromXDR(genMemo);

            Assert.AreEqual(Memo.MemoTypeEnum.MEMO_NONE, resMemo.Type);
        }

        [TestMethod]
        public void TestMemoText()
        {
            string text = "Test";
            Memo memo = Memo.MemoText(text);

            Assert.AreEqual(text, memo.Text);
            Assert.AreEqual(Memo.MemoTypeEnum.MEMO_TEXT, memo.Type);

            Stellar.Generated.Memo genMemo = memo.ToXDR();

            Assert.AreEqual(text, genMemo.Text);
            Assert.AreEqual(Stellar.Generated.MemoType.MemoTypeEnum.MEMO_TEXT, genMemo.Discriminant.InnerValue);

            Memo resMemo = Memo.FromXDR(genMemo);

            Assert.AreEqual(text, resMemo.Text);
            Assert.AreEqual(Memo.MemoTypeEnum.MEMO_TEXT, resMemo.Type);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException), "textorhash cannot be null.")]
        public void TestMemoTextNull()
        {
            Memo memo = Memo.MemoText(null);
        }

        [TestMethod]
        public void TestMemoId()
        {
            long id = 1234567890;
            Memo memo = Memo.MemoId(id);

            Assert.AreEqual(id, memo.Id);
            Assert.AreEqual(Memo.MemoTypeEnum.MEMO_ID, memo.Type);

            Stellar.Generated.Memo genMemo = memo.ToXDR();

            Assert.AreEqual(new Stellar.Generated.Uint64((ulong)id).InnerValue, genMemo.Id.InnerValue);
            Assert.AreEqual(Stellar.Generated.MemoType.MemoTypeEnum.MEMO_ID, genMemo.Discriminant.InnerValue);

            Memo resMemo = Memo.FromXDR(genMemo);

            Assert.AreEqual(id, resMemo.Id);
            Assert.AreEqual(Memo.MemoTypeEnum.MEMO_ID, resMemo.Type);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "id must be non-negative.")]
        public void TestMemoIdNegative()
        {
            Memo memo = Memo.MemoId(-1);
        }

        [TestMethod]
        public void TestMemoHash()
        {
            string hash = "TestHashTestHashTestHashTestHash";
            Memo memo = Memo.MemoHash(hash);

            Assert.AreEqual(hash, memo.Hash);
            Assert.AreEqual(Memo.MemoTypeEnum.MEMO_HASH, memo.Type);

            Stellar.Generated.Memo genMemo = memo.ToXDR();

            Assert.AreEqual(Encoding.ASCII.GetBytes(hash).ToString(), genMemo.Hash.InnerValue.ToString());
            Assert.AreEqual(Stellar.Generated.MemoType.MemoTypeEnum.MEMO_HASH, genMemo.Discriminant.InnerValue);

            Memo resMemo = Memo.FromXDR(genMemo);

            Assert.AreEqual(hash, resMemo.Hash);
            Assert.AreEqual(Memo.MemoTypeEnum.MEMO_HASH, resMemo.Type);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException), "textorhash cannot be null.")]
        public void TestMemoHashNone()
        {
            Memo memo = Memo.MemoHash(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Invalid hash")]
        public void TestMemoHashWrong()
        {
            Memo memo = Memo.MemoHash("Wrong");
        }

        [TestMethod]
        public void TestMemoReturnHash()
        {
            string retHash = "TestHashTestHashTestHashTestHash";
            Memo memo = Memo.MemoReturnHash(retHash);

            Assert.AreEqual(retHash, memo.RetHash);
            Assert.AreEqual(Memo.MemoTypeEnum.MEMO_RETURN, memo.Type);

            Stellar.Generated.Memo genMemo = memo.ToXDR();

            Assert.AreEqual(Encoding.ASCII.GetBytes(retHash).ToString(), genMemo.RetHash.InnerValue.ToString());
            Assert.AreEqual(Stellar.Generated.MemoType.MemoTypeEnum.MEMO_RETURN, genMemo.Discriminant.InnerValue);

            Memo resMemo = Memo.FromXDR(genMemo);

            Assert.AreEqual(retHash, resMemo.RetHash);
            Assert.AreEqual(Memo.MemoTypeEnum.MEMO_RETURN, resMemo.Type);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException), "textorhash cannot be null.")]
        public void TestMemoReturnHashNone()
        {
            Memo memo = Memo.MemoReturnHash(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Invalid retHash")]
        public void TestMemoReturnHashWrong()
        {
            Memo memo = Memo.MemoReturnHash("Wrong");
        }
    }
}
