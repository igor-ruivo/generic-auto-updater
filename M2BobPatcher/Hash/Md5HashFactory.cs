using System;
using System.IO;
using System.Security.Cryptography;

namespace M2BobPatcher.Hash {

    static class Md5HashFactory {

        public static byte[] GeneratedMd5HashFromByteArray(byte[] array) {
            using (MD5 md5 = MD5.Create()) {
                return md5.ComputeHash(array);
            }
        }
        public static byte[] GeneratedMd5HashFromStream(Stream stream) {
            using (MD5 md5 = MD5.Create()) {
                return md5.ComputeHash(stream);
            }
        }

        public static string NormalizeMd5(byte[] md5) {
            return BitConverter.ToString(md5).Replace("-", "").ToLowerInvariant();
        }
    }
}