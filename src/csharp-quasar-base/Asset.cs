using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stellar
{
    public class Asset
    {
        public static Generated.Asset Native()
        {
            return new Generated.Asset
            {
                Discriminant = Generated.AssetType.Create(Generated.AssetType.AssetTypeEnum.ASSET_TYPE_NATIVE)
            };
        }
    }
}