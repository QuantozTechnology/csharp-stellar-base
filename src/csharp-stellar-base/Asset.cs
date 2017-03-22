using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stellar
{
    public class Asset
    {
        enum AssetType
        {
            Native = 0,
            Alphanum4 = 1,
            Alphanum12 = 2
        }

        private string assetCode;
        private AssetType assetType;
        private KeyPair master;

        public static Generated.Asset Native()
        {
            return new Generated.Asset
            {
                Discriminant = Generated.AssetType.Create(Generated.AssetType.AssetTypeEnum.ASSET_TYPE_NATIVE)
            };
        }

        public Asset()
        {
            assetType = AssetType.Native;
        }

        public Asset(KeyPair issuer, string assetCode)
        {
            this.master = issuer;
            if (assetCode.Length > 0 && assetCode.Length <= 4)
            {
                this.assetCode = assetCode.PadRight(4, '\0');
                assetType = AssetType.Alphanum4;
            }
            else if (assetCode.Length > 4 && assetCode.Length <= 12)
            {
                this.assetCode = assetCode.PadRight(12, '\0');
                assetType = AssetType.Alphanum12;
            }
            else
            {
                throw new Exception("AssetCode of incorrect length.");
            }
        }

        public Generated.Asset ToXDR()
        {
            if (assetType == AssetType.Alphanum4)
            {
                return new Stellar.Generated.Asset
                {
                    Discriminant = Stellar.Generated.AssetType.Create(Stellar.Generated.AssetType.AssetTypeEnum.ASSET_TYPE_CREDIT_ALPHANUM4),
                    AlphaNum4 = new Stellar.Generated.Asset.AssetAlphaNum4
                    {
                        AssetCode = Encoding.ASCII.GetBytes(assetCode),
                        Issuer = master.AccountId
                    }
                };
            }
            else if (assetType == AssetType.Alphanum12)
            {
                return new Stellar.Generated.Asset
                {
                    Discriminant = Stellar.Generated.AssetType.Create(Stellar.Generated.AssetType.AssetTypeEnum.ASSET_TYPE_CREDIT_ALPHANUM12),
                    AlphaNum12 = new Stellar.Generated.Asset.AssetAlphaNum12
                    {
                        AssetCode = Encoding.ASCII.GetBytes(assetCode),
                        Issuer = master.AccountId
                    }
                };
            }
            else if (assetType == AssetType.Native)
            {
                return new Generated.Asset
                {
                    Discriminant = Generated.AssetType.Create(Generated.AssetType.AssetTypeEnum.ASSET_TYPE_NATIVE)
                };
            }
            else
            {
                throw new ArgumentException("invalid AssetType");
            }
        }
    }
}