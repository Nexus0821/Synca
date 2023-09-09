using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace VRChat.Synca.API.Crypto
{
    public static class Hashing
    {
        public static string MD5(string input) => Internal_Hash(typeof(MD5), input);
        public static string SHA1(string input) => Internal_Hash(typeof(SHA1), input);
        public static string SHA256(string input) => Internal_Hash(typeof(SHA256), input);
        public static string SHA384(string input) => Internal_Hash(typeof(SHA384), input);
        public static string SHA512(string input) => Internal_Hash(typeof(SHA512), input);

        private static string Internal_Hash(Type hashType, string input)
        {
            if (!(hashType.BaseType != null && hashType.BaseType.GUID == typeof(HashAlgorithm).GUID))
            {
                Logger.Msg(ConsoleColor.Red, "Cannot Hash(): the hash algorithm used does not implement or inherit HashAlgorithm!");
                return string.Empty;
            }

            var hashDataMethodInfo = hashType.GetMethods().FirstOrDefault(x => x.Name == "HashData", null);
            if (hashDataMethodInfo == null)
            {
                Logger.Msg(ConsoleColor.Red, "Cannot Hash(): the hash algorithm used does have a HashData method!");
                return string.Empty;
            }

            byte[] hashedData = (byte[])hashDataMethodInfo.Invoke(null, new object[] { Encoding.UTF8.GetBytes(input) })!;
            string converted = BitConverter.ToString(hashedData).Replace("-", "").ToLower();

            return converted;
        }
    }
}
