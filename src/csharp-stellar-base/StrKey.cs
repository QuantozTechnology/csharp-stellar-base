using Stellar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stellar
{
    public enum VersionByte
    {
        ed25519Publickey = 0x30,
        ed25519SecretSeed = 0x90
    }

    public class StrKey
    {
        public static string EncodeEd25519PublicKey(byte[] data)
        {
            return EncodeCheck(VersionByte.ed25519Publickey, data);
        }

        public static byte[] DecodeEd25519PublicKey(string data)
        {
            return DecodeCheck(VersionByte.ed25519Publickey, data);
        }

        public static string EncodeEd25519SecretSeed(byte[] data)
        {
            return EncodeCheck(VersionByte.ed25519SecretSeed, data);
        }

        public static byte[] DecodeEd25519SecretSeed(string data)
        {
            return DecodeCheck(VersionByte.ed25519SecretSeed, data);
        }

        public static string EncodeCheck(VersionByte versionByte, byte[] data)
        {
            var writer = new Generated.ByteWriter();
            writer.Write((byte)versionByte);
            writer.Write(data);
            byte[] checksum = StrKey.CalculateChecksum(writer.ToArray());
            writer.Write(checksum);
            return Base32Encoding.ToString(writer.ToArray());
        }

        public static byte[] DecodeCheck(VersionByte versionByte, string encoded)
        {
            byte[] decoded = Base32Encoding.ToBytes(encoded);
            byte decodedVersionByte = decoded[0];
            byte[] payload = new byte[decoded.Length - 2];
            Array.Copy(decoded, payload, decoded.Length - 2);
            byte[] data = new byte[payload.Length - 1];
            Array.Copy(payload, 1, data, 0, payload.Length - 1);
            byte[] checksum = new byte[2];
            Array.Copy(decoded, decoded.Length - 2, checksum, 0, 2);


            if (decodedVersionByte != (byte)versionByte)
            {
                throw new FormatException("Version byte is invalid");
            }

            byte[] expectedChecksum = StrKey.CalculateChecksum(payload);

            if (!expectedChecksum.IsIdentical(checksum))
            {
                throw new FormatException("Checksum invalid");
            }

            return data;
        }

        protected static byte[] CalculateChecksum(byte[] bytes)
        {
            // This code calculates CRC16-XModem checksum
            // Ported from https://github.com/alexgorbatchev/node-crc
            int crc = 0x0000;
            int count = bytes.Length;
            int i = 0;
            int code;

            while (count > 0)
            {
                code = (int)((uint)crc >> 8 & 0xFF);
                code ^= bytes[i++] & 0xFF;
                code ^= (int)((uint)code >> 4);
                crc = crc << 8 & 0xFFFF;
                crc ^= code;
                code = code << 5 & 0xFFFF;
                crc ^= code;
                code = code << 7 & 0xFFFF;
                crc ^= code;
                count--;
            }

            // little-endian
            return new byte[] {
            (byte)crc,
            (byte)((uint)crc >> 8)};
        }
    }
}
