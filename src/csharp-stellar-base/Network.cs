using System.Security.Cryptography;
using System.Text;

namespace Stellar
{
    public static class Network
    {
        public static string CurrentNetwork { get; set; } = "";
        public static byte[] CurrentNetworkId
        {
            get
            {
                SHA256 mySHA256 = SHA256Managed.Create();
                byte[] bytes = Encoding.Default.GetBytes(CurrentNetwork);
                return mySHA256.ComputeHash(bytes);
            }
        }
    }
}