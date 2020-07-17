using GenericAutoUpdater.Downloaders;
using GenericAutoUpdater.Hash;
using GenericAutoUpdater.Resources.Configs;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace GenericAutoUpdater.FileSystem {
    /// <summary>
    /// The class responsible for any IO related task.
    /// </summary>
    static class FileSystemExplorer {
        /// <summary>
        /// Returns a refreshed version of the local metadata.
        /// This method accomplishes this by reading from the disk all the files whose name match those in filePaths and calculating their md5 hashes concurrently.
        /// </summary>
        public static ConcurrentDictionary<string, FileMetadata> GenerateLocalMetadata(string[] filesPaths, IHasher hasher, int concurrencyLevel) {
            ConcurrentDictionary<string, FileMetadata> metadata = new ConcurrentDictionary<string, FileMetadata>(filesPaths.Length, concurrencyLevel);
            Parallel.ForEach(filesPaths, (currentPath) => {
                using (FileStream stream = File.OpenRead(currentPath)) {
                    // The bigger the files to hash the bigger the speedup!
                    metadata[currentPath] = new FileMetadata(currentPath, hasher.GeneratedHashFromStream(stream));
                }
            });
            return metadata;
        }

        /// <summary>
        /// Tries to download and write a specific file (resource) to the disk in a defined path.
        /// This method creates the respective parent directory, if needed.
        /// If the download is due to an update, this method downloads the updated file but only replaces the legacy file with it when there is evidence that the download was successful.
        /// </summary>
        public static void FetchFile(IDownloader downloader, string path, string resource, bool isMissingContent, string expectedHash) {
            new FileInfo(path).Directory.Create();
            if (isMissingContent)
                downloader.DownloadDataToFile(resource, path, expectedHash);
            else {
                try {
                    downloader.DownloadDataToFile(resource, path + FileSystemExplorerConfigs.INCOMPLETE_DOWNLOADED_FILE_EXT, expectedHash);
                }
                catch (Exception) {
                    // Something went wrong with the download. Delete the incomplete file and chain the exception to the handler.
                    if (FileExists(path + FileSystemExplorerConfigs.INCOMPLETE_DOWNLOADED_FILE_EXT))
                        File.Delete(path + FileSystemExplorerConfigs.INCOMPLETE_DOWNLOADED_FILE_EXT);
                    throw;
                }
                // The download was successful. Delete the legacy file and rename the incomplete file to its right name.
                if (FileExists(path))
                    File.Delete(path);
                File.Move(path + FileSystemExplorerConfigs.INCOMPLETE_DOWNLOADED_FILE_EXT, path);
            }
        }
        /// <summary>
        /// Checks if a file exists.
        /// </summary>
        public static bool FileExists(string file) {
            return File.Exists(file);
        }
    }
}