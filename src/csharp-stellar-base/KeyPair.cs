using System.Security.Cryptography;
using System.Text;
using Stellar.Generated;
using System.Linq;
using System;

namespace Stellar
{
    public class KeyPair
    {
        public byte[] PrivateKey { get; private set; }
        public byte[] PublicKey { get; private set; }
        public byte[] SeedBytes { get; private set; }

        public AccountID AccountId
        {
            get
            {
                return new AccountID
                {
                    InnerValue = new Generated.PublicKey
                    {
                        Discriminant = PublicKeyType.Create(PublicKeyType.PublicKeyTypeEnum.PUBLIC_KEY_TYPE_ED25519),
                        Ed25519 = new Uint256(PublicKey)
                    }
                };
            }
        }

        public string Address
        {
            get
            {
                return StrKey.EncodeCheck(VersionByte.AccountId, PublicKey);
            }
        }

        public string Seed
        {
            get
            {
                return StrKey.EncodeCheck(VersionByte.Seed, SeedBytes);
            }
        }

        public byte[] SignatureHint
        {
            get
            {
                var stream = new ByteWriter();
                AccountID.Encode(stream, AccountId);
                var bytes = stream.ToArray();
                var length = bytes.Length;
                return bytes.Skip(length - 4).Take(4).ToArray();
            }
        }

        public KeyPair(string pubKey, string secretKey, string seed)
             : this(Encoding.Default.GetBytes(pubKey),
                    Encoding.Default.GetBytes(secretKey),
                    Encoding.Default.GetBytes(seed))
        {
        }

        public KeyPair(byte[] pubKey, byte[] secretKey, byte[] seed)
        {
            if (pubKey.Length != 32)
            {
                throw new ArgumentException("pubKey must be 64 bytes");
            }

            if (secretKey.Length != 64)
            {
                throw new ArgumentException("pubKey must be 64 bytes");
            }

            if (seed.Length != 32)
            {
                throw new ArgumentException("seed must be 32 bytes");
            }

            PublicKey = pubKey;
            PrivateKey = secretKey;
            SeedBytes = seed;
        }

        public KeyPair(byte[] pubKey, byte[] seed)
        {
            if (pubKey.Length != 32)
            {
                throw new ArgumentException("pubKey must be 64 bytes");
            }

            if (seed.Length != 32)
            {
                throw new ArgumentException("seed must be 32 bytes");
            }

            PublicKey = pubKey;
            PrivateKey = Chaos.NaCl.Ed25519.ExpandedPrivateKeyFromSeed(seed);
            SeedBytes = seed;
        }

        public KeyPair(byte[] pubKey)
        {
            if (pubKey.Length != 32)
            {
                throw new ArgumentException("pubKey must be 32 bytes");
            }

            PublicKey = pubKey;
        }

        public byte[] Sign(byte[] message)
        {
            return Chaos.NaCl.Ed25519.Sign(message, PrivateKey);
        }

        public byte[] Sign(string message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            return Chaos.NaCl.Ed25519.Sign(bytes, PrivateKey);
        }

        public DecoratedSignature SignDecorated(byte[] message)
        {
            var rawSig = Sign(message);
            return new DecoratedSignature
            {
                Hint = new SignatureHint(SignatureHint),
                Signature = new Signature(rawSig)
            };
        }

        public bool Verify(byte[] signature, byte[] message)
        {
            try
            {
                return Chaos.NaCl.Ed25519.Verify(signature, message, PublicKey);
            }
            catch
            {
                return false;
            }
        }

        public static KeyPair FromXdrPublicKey(PublicKey key)
        {
            return FromPublicKey(key.Ed25519.InnerValue);
        }

        public static KeyPair FromSeed(string seed)
        {
            var bytes = StrKey.DecodeCheck(VersionByte.Seed, seed);
            return FromRawSeed(bytes);
        }

        public static KeyPair FromRawSeed(byte[] seed)
        {
            byte[] pubKey, privKey;
            Chaos.NaCl.Ed25519.KeyPairFromSeed(out pubKey, out privKey, seed);
            return new KeyPair(pubKey, privKey, seed);
        }

        public static KeyPair FromPublicKey(byte[] bytes)
        {
            return new KeyPair(bytes);
        }

        public static KeyPair FromAccountId(string accountId)
        {
            var bytes = StrKey.DecodeCheck(VersionByte.AccountId, accountId);
            return FromPublicKey(bytes);
        }

        public static KeyPair FromAddress(string accountId)
        {
            return FromAccountId(accountId);
        }

        public static KeyPair Random()
        {
            var b = new byte[32];
            using (var rngCrypto = new RNGCryptoServiceProvider())
            {
                rngCrypto.GetBytes(b);
            }
            return KeyPair.FromRawSeed(b);
        }

        public static KeyPair FromNetworkPassphrase(string passPhrase)
        {
            SHA256 mySHA256 = SHA256Managed.Create();
            byte[] bytes = Encoding.Default.GetBytes(passPhrase);
            byte[] networkId = mySHA256.ComputeHash(bytes);
            return FromRawSeed(networkId);
        }

        public static KeyPair Master()
        {
            return FromRawSeed(Network.CurrentNetworkId);
        }
    }
}