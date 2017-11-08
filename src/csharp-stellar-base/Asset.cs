using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Stellar.Preconditions;

namespace Stellar
{
    public class Asset
    {
        public string Code { get; private set; }
        public KeyPair Issuer { get; private set; }
        public enum AssetTypeEnum
        {
            ASSET_TYPE_NATIVE = 0,
            ASSET_TYPE_CREDIT_ALPHANUM4 = 1,
            ASSET_TYPE_CREDIT_ALPHANUM12 = 2,
        }
        public AssetTypeEnum Type { get; private set; }

        public Asset()
        {
            Type = AssetTypeEnum.ASSET_TYPE_NATIVE;
        }

        public Asset(string code, KeyPair issuer)
        {
            Code = CheckNotNull(code, "code cannot be null.");
            Issuer = CheckNotNull(issuer, "issuer cannot be null.");
            if (code.Length >= 1 && code.Length <= 4)
            {
                Type = AssetTypeEnum.ASSET_TYPE_CREDIT_ALPHANUM4;
            }
            else if(code.Length >= 5 && code.Length <= 12)
            {
                Type = AssetTypeEnum.ASSET_TYPE_CREDIT_ALPHANUM12;
            }
            else
            {
                throw new ArgumentException("Invalid code, should have positive length, no larger than 12.");
            }
        }

        public Generated.Asset ToXDR()
        {
            switch(Type)
            {
                case AssetTypeEnum.ASSET_TYPE_NATIVE:
                    return new Generated.Asset
                    {
                        Discriminant = Generated.AssetType.Create(Generated.AssetType.AssetTypeEnum.ASSET_TYPE_NATIVE)
                    };
                case AssetTypeEnum.ASSET_TYPE_CREDIT_ALPHANUM4:
                    return new Generated.Asset
                    {
                        Discriminant = Generated.AssetType.Create(Generated.AssetType.AssetTypeEnum.ASSET_TYPE_CREDIT_ALPHANUM4),
                        AlphaNum4 = new Stellar.Generated.Asset.AssetAlphaNum4
                        {
                            AssetCode = Encoding.ASCII.GetBytes(Code),
                            Issuer = new Generated.AccountID
                            {
                                InnerValue = new Generated.PublicKey
                                {
                                    Discriminant = Generated.PublicKeyType.Create(Generated.PublicKeyType.PublicKeyTypeEnum.PUBLIC_KEY_TYPE_ED25519),
                                    Ed25519 = new Generated.Uint256(Issuer.PublicKey)
                                }
                            }
                        }
                    };
                case AssetTypeEnum.ASSET_TYPE_CREDIT_ALPHANUM12:
                    return new Generated.Asset
                    {
                        Discriminant = Generated.AssetType.Create(Generated.AssetType.AssetTypeEnum.ASSET_TYPE_CREDIT_ALPHANUM12),
                        AlphaNum12 = new Stellar.Generated.Asset.AssetAlphaNum12
                        {
                            AssetCode = Encoding.ASCII.GetBytes(Code),
                            Issuer = new Generated.AccountID
                            {
                                InnerValue = new Generated.PublicKey
                                {
                                    Discriminant = Generated.PublicKeyType.Create(Generated.PublicKeyType.PublicKeyTypeEnum.PUBLIC_KEY_TYPE_ED25519),
                                    Ed25519 = new Generated.Uint256(Issuer.PublicKey)
                                }
                            }
                        }
                    };
                default:
                    throw new ArgumentException("Invalid Asset.");
            }  
        }

        public static Asset FromXDR(Generated.Asset asset)
        {
            switch(asset.Discriminant.InnerValue)
            {
                case Generated.AssetType.AssetTypeEnum.ASSET_TYPE_NATIVE:
                    return new Asset();
                case Generated.AssetType.AssetTypeEnum.ASSET_TYPE_CREDIT_ALPHANUM4:
                    return new Asset(Encoding.ASCII.GetString(asset.AlphaNum4.AssetCode), KeyPair.FromXdrPublicKey(asset.AlphaNum4.Issuer.InnerValue));
                case Generated.AssetType.AssetTypeEnum.ASSET_TYPE_CREDIT_ALPHANUM12:
                    return new Asset(Encoding.ASCII.GetString(asset.AlphaNum12.AssetCode), KeyPair.FromXdrPublicKey(asset.AlphaNum12.Issuer.InnerValue));
                default:
                    throw new ArgumentException("Invalid asset.");
            }
        }
    }
}