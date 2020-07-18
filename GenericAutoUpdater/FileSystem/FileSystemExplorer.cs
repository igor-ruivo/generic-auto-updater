using GenericAutoUpdater.Downloaders;
using GenericAutoUpdater.Hash;
using GenericAutoUpdater.Resources.Configs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        /// If the download is due to an update, this method downloads the updated file appending to it a special extension, preserving the legacy file.
        /// </summary>
        public static void FetchFile(IDownloader downloader, string path, string resource, bool isMissingContent, string expectedHash) {
            new FileInfo(path).Directory.Create();
            string filePath = path + (isMissingContent ? string.Empty : FileSystemExplorerConfigs.INCOMPLETE_DOWNLOADED_FILE_EXT);
            downloader.DownloadDataToFile(resource, filePath, expectedHash);
        }
        /// <summary>
        /// Checks if a file exists.
        /// </summary>
        public static bool FileExists(string file) {
            return File.Exists(file);
        }

        /// <summary>
        /// This method is responsible for renaming all files updated by removing their special extension regarding their incompletion.
        /// It starts by checking if the downloaded files still exist in the disk. If any of them don't, it throws a <c>FileNotFoundException</c>.
        /// However, if all of them exist, the legacy version of each of them is deleted, and the updated version gets the respective legacy file name.
        /// </summary>
        public static void ApplyUpdate(List<string> content) {
            foreach (string file in content)
                if (!FileExists(file + FileSystemExplorerConfigs.INCOMPLETE_DOWNLOADED_FILE_EXT))
                    throw new FileNotFoundException();
            foreach (string file in content) {
                if (FileExists(file))
                    File.Delete(file);
                File.Move(file + FileSystemExplorerConfigs.INCOMPLETE_DOWNLOADED_FILE_EXT, file);
            }
        }
    }
}