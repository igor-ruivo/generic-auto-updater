using System.IO;

namespace GenericAutoUpdater.Hash {
    /// <summary>
    /// This interface declares the calls that every hasher should at least implement.
    /// </summary>
    interface IHasher {
        /// <summary>
        /// Computes the hash of the received <c>byte[]</c> array and returns it as a string.
        /// Invokes <c>NormalizeMd5()</c> for the conversion.
        /// </summary>
        string GeneratedHashFromByteArray(byte[] array);

        /// <summary>
        /// Computes the hash of the received <c>Stream</c> array and returns it as a string.
        /// Invokes <c>NormalizeMd5()</c> for the conversion.
        /// </summary>
        string GeneratedHashFromStream(Stream stream);

        /// <summary>
        /// Computes the hash of the file with the received name and returns it as a string.
        /// Invokes <c>NormalizeMd5()</c> for the conversion.
        /// </summary>
        string GeneratedHashFromFile(string filename);
    }
}