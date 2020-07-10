using System;
using System.IO;
using System.Security.Cryptography;

namespace M2BobPatcher.Hash {
    /// <summary>
    /// The class used to generate and manipulate md5 hashes.
    /// </summary>
    static class Md5HashFactory {
        /// <summary>
        /// Computes the md5 hash of the received <c>byte[]</c> array and returns it as a string.
        /// Invokes <c>NormalizeMd5()</c> for the conversion.
        /// </summary>
        public static string GeneratedMd5HashFromByteArray(byte[] array) {
            using (MD5 md5 = MD5.Create()) {
                return NormalizeMd5(md5.ComputeHash(array));
            }
        }

        /// <summary>
        /// Computes the md5 hash of the received <c>Stream</c> and returns it as a string.
        /// Invokes <c>NormalizeMd5()</c> for the conversion.
        /// </summary>
        public static string GeneratedMd5HashFromStream(Stream stream) {
            using (MD5 md5 = MD5.Create()) {
                return NormalizeMd5(md5.ComputeHash(stream));
            }
        }

        /// <summary>
        /// Converts the <c>byte[]</c> md5 hash received into a string.
        /// </summary>
        private static string NormalizeMd5(byte[] md5) {
            return BitConverter.ToString(md5).Replace("-", "").ToLowerInvariant();
        }
    }
}