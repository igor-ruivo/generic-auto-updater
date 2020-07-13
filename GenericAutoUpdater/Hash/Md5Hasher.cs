using System;
using System.IO;
using System.Security.Cryptography;

namespace GenericAutoUpdater.Hash {
    /// <summary>
    /// The class used to generate and manipulate md5 hashes.
    /// </summary>
    class Md5Hasher : IHasher {
        /// <summary>
        /// Computes the md5 hash of the received <c>byte[]</c> array and returns it as a string.
        /// Invokes <c>NormalizeMd5()</c> for the conversion.
        /// </summary>
        public string GeneratedHashFromByteArray(byte[] array) {
            using (MD5 md5 = MD5.Create()) {
                return NormalizeMd5(md5.ComputeHash(array));
            }
        }

        /// <summary>
        /// Computes the md5 hash of the received <c>Stream</c> and returns it as a string.
        /// Invokes <c>NormalizeMd5()</c> for the conversion.
        /// </summary>
        public string GeneratedHashFromStream(Stream stream) {
            using (MD5 md5 = MD5.Create()) {
                return NormalizeMd5(md5.ComputeHash(stream));
            }
        }

        /// <summary>
        /// Computes the md5 hash of the file with the received name and returns it as a string.
        /// Invokes <c>NormalizeMd5()</c> for the conversion.
        /// </summary>
        public string GeneratedHashFromFile(string filename) {
            using (MD5 md5 = MD5.Create()) {
                using (FileStream stream = File.OpenRead(filename)) {
                    return NormalizeMd5(md5.ComputeHash(stream));
                }
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