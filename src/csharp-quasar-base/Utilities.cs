using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Quasar
{
    public static class Utilities
    {
        public static byte[] Hash(byte[] data)
        {
            SHA256 mySHA256 = SHA256Managed.Create();
            byte[] hash = mySHA256.ComputeHash(data);
            return hash;
        }
    }
}
