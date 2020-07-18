namespace GenericAutoUpdater.Downloaders {
    /// <summary>
    /// This interface declares the calls that every downloader should at least implement.
    /// </summary>
    public interface IDownloader {
        /// <summary>
        /// The Downloader API call to download a specific file (address) directly into memory.
        /// If an expectedHash is specfied, the downloaded file's hash is compared with this hash, throwing an InvalidDataException if they don't match.
        /// It returns the downloaded content as a <c>byte[]</c>.
        /// </summary>
        byte[] DownloadDataToMemory(string address, string expectedHash = null);

        /// <summary>
        /// The Downloader API call to download a specific file (address) to the disk in the path specified by filePath.
        /// If an expectedHash is specfied, the downloaded file's hash is compared with this hash, throwing an InvalidDataException if they don't match.
        /// </summary>
        void DownloadDataToFile(string address, string filePath, string expectedHash = null);
    }
}
