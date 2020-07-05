using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace M2BobPatcher.Hash {
    static class Md5HashFactory {
        public static byte[] GeneratedMd5HashFromFile(string filename) {
            using (MD5 md5 = MD5.Create()) {
                using (FileStream stream = File.OpenRead(filename)) {
                    return md5.ComputeHash(stream);
                }
            }
        }

        public static string NormalizeMd5(byte[] md5) {
            return BitConverter.ToString(md5).Replace("-", "").ToLowerInvariant();
        }
    }
}