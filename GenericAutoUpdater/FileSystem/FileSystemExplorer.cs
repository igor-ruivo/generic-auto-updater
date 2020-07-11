using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using GenericAutoUpdater.Downloaders;
using GenericAutoUpdater.Hash;

namespace GenericAutoUpdater.FileSystem {
    /// <summary>
    /// The class responsible for any IO related task.
    /// </summary>
    static class FileSystemExplorer {
        /// <summary>
        /// Returns a refreshed version of the local metadata.
        /// This method accomplishes this by reading from the disk all the files whose name match those in filePaths and calculating their md5 hashes concurrently.
        /// </summary>
        public static ConcurrentDictionary<string, FileMetadata> GenerateLocalMetadata(string[] filesPaths, int concurrencyLevel) {
            ConcurrentDictionary<string, FileMetadata> metadata = new ConcurrentDictionary<string, FileMetadata>(filesPaths.Length, concurrencyLevel);
            Parallel.ForEach(filesPaths, (currentPath) => {
                using (FileStream stream = File.OpenRead(currentPath)) {
                    // The bigger the files to hash the bigger the speedup!
                    metadata[currentPath] = new FileMetadata(currentPath, Md5HashFactory.GeneratedMd5HashFromStream(stream));
                }
            });
            return metadata;
        }

        /// <summary>
        /// Tries to download and write a specific file (resource) to the disk in a defined path.
        /// This method creates the respective parent directory, if needed.
        /// </summary>
        public static void FetchFile(BackgroundWorker bw, string path, string resource, string expectedHash) {
            new FileInfo(path).Directory.Create();
            File.WriteAllBytes(path, HttpClientDownloader.DownloadData(bw, resource, expectedHash));
        }
        /// <summary>
        /// Checks if a file exists.
        /// </summary>
        public static bool FileExists(string file) {
            return File.Exists(file);
        }
    }
}