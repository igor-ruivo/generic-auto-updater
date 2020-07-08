using M2BobPatcher.Downloaders;
using M2BobPatcher.Hash;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

namespace M2BobPatcher.FileSystem {
    static class FileSystemExplorer {

        public static ConcurrentDictionary<string, FileMetadata> GenerateLocalMetadata(string[] filesPaths, int concurrencyLevel) {
            ConcurrentDictionary<string, FileMetadata> metadata = new ConcurrentDictionary<string, FileMetadata>(filesPaths.Length, concurrencyLevel);
            Parallel.ForEach(filesPaths, (currentPath) => {
                using (FileStream stream = File.OpenRead(currentPath)) {
                    metadata[currentPath] = new FileMetadata(currentPath, Md5HashFactory.GeneratedMd5HashFromStream(stream));
                }
            });
            return metadata;
        }

        public static void FetchFile(BackgroundWorker bw, string path, string resource, string expectedHash) {
            new FileInfo(path).Directory.Create();
            File.WriteAllBytes(path, HttpClientDownloader.DownloadData(bw, resource, expectedHash));
        }

        public static bool FileExists(string file) {
            return File.Exists(file);
        }
    }
}