using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar
{
    public static class Memo
    {
        /// <summary>
        /// Creates MEMO_NONE type memo.
        /// </summary>
        /// <returns></returns>
        public static Generated.Memo None()
        {
            return new Generated.Memo
            {
                Discriminant = Generated.MemoType.Create(Generated.MemoType.MemoTypeEnum.MEMO_NONE)
            };
        }
    }
}