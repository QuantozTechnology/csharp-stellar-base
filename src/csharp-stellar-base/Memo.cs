using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Stellar.Preconditions;

namespace Stellar
{
    public class Memo
    {
        public string Text { get; private set; }
        public long Id { get; private set; }
        public string Hash { get; private set; }
        public string RetHash { get; private set; }
        public enum MemoTypeEnum
        {
            MEMO_NONE = 0,
            MEMO_TEXT = 1,
            MEMO_ID = 2,
            MEMO_HASH = 3,
            MEMO_RETURN = 4,
        }
        public MemoTypeEnum Type { get; private set; }

        private Memo()
        {
            Type = MemoTypeEnum.MEMO_NONE;
        }

        public static Memo MemoNone()
        {
            return new Memo();
        }

        private Memo(MemoTypeEnum type, string textorhash)
        {
            CheckNotNull(textorhash, "textorhash cannot be null.");
            switch (type)
            {
                case MemoTypeEnum.MEMO_TEXT:
                    Text = textorhash;
                    Type = type;
                    break;
                case MemoTypeEnum.MEMO_HASH:
                    if(textorhash.Length != 32)
                    {
                        throw new ArgumentException("Invalid hash.");
                    }
                    Hash = textorhash;
                    Type = type;
                    break;
                case MemoTypeEnum.MEMO_RETURN:
                    if(textorhash.Length != 32)
                    {
                        throw new ArgumentException("Invalid retHash.");
                    }
                    RetHash = textorhash;
                    Type = type;
                    break;
                default:
                    throw new ArgumentException("Invalid type.");
            }
        }

        public static Memo MemoText(string text)
        {
            return new Memo(MemoTypeEnum.MEMO_TEXT, text);
        }

        private Memo(long id)
        {
            Id = id;
            Type = MemoTypeEnum.MEMO_ID;
        }

        public static Memo MemoId(long id)
        {
            if(id < 0)
            {
                throw new ArgumentException("id must be non-negative.");
            }
            return new Memo(id);
        }

        public static Memo MemoHash(string hash)
        {
            return new Memo(MemoTypeEnum.MEMO_HASH, hash);
        }

        public static Memo MemoReturnHash(string retHash)
        {
            return new Memo(MemoTypeEnum.MEMO_RETURN, retHash);
        }

        public Generated.Memo ToXDR()
        {
            switch (Type)
            {
                case MemoTypeEnum.MEMO_NONE:
                    return new Generated.Memo
                    {
                        Discriminant = Generated.MemoType.Create(Generated.MemoType.MemoTypeEnum.MEMO_NONE)
                    };
                case MemoTypeEnum.MEMO_TEXT:
                    return new Generated.Memo
                    {
                        Discriminant = Generated.MemoType.Create(Generated.MemoType.MemoTypeEnum.MEMO_TEXT),
                        Text = Text
                    };
                case MemoTypeEnum.MEMO_ID:
                    return new Generated.Memo
                    {
                        Discriminant = Generated.MemoType.Create(Generated.MemoType.MemoTypeEnum.MEMO_ID),
                        Id = new Generated.Uint64((ulong)Id)
                    };
                case MemoTypeEnum.MEMO_HASH:
                    return new Generated.Memo
                    {
                        Discriminant = Generated.MemoType.Create(Generated.MemoType.MemoTypeEnum.MEMO_HASH),
                        Hash = new Generated.Hash(Encoding.ASCII.GetBytes(Hash))
                    };
                case MemoTypeEnum.MEMO_RETURN:
                    return new Generated.Memo
                    {
                        Discriminant = Generated.MemoType.Create(Generated.MemoType.MemoTypeEnum.MEMO_RETURN),
                        RetHash = new Generated.Hash(Encoding.ASCII.GetBytes(RetHash))
                    };
                default:
                    throw new ArgumentException("Invalid memo type.");
            }
        }

        public static Memo FromXDR(Generated.Memo memo)
        {
            switch(memo.Discriminant.InnerValue)
            {
                case Generated.MemoType.MemoTypeEnum.MEMO_NONE:
                    return MemoNone();
                case Generated.MemoType.MemoTypeEnum.MEMO_TEXT:
                    return MemoText(memo.Text);
                case Generated.MemoType.MemoTypeEnum.MEMO_ID:
                    return MemoId((long)memo.Id.InnerValue);
                case Generated.MemoType.MemoTypeEnum.MEMO_HASH:
                    return MemoHash(Encoding.ASCII.GetString(memo.Hash.InnerValue));
                case Generated.MemoType.MemoTypeEnum.MEMO_RETURN:
                    return MemoReturnHash(Encoding.ASCII.GetString(memo.RetHash.InnerValue));
                default:
                    throw new ArgumentException("Invalid memo.");
            }
        }
    }
}