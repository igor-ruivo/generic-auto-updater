using M2BobPatcher.Downloaders;
using M2BobPatcher.Hash;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace M2BobPatcher.FileSystem {
    class FileSystemExplorer : IFileSystemExplorer {

        private static string CurrentDirectory;

        public FileSystemExplorer() {
            CurrentDirectory = Environment.CurrentDirectory;
        }

        ConcurrentDictionary<string, FileMetadata> IFileSystemExplorer.GenerateLocalMetadata(string[] filesPaths, int concurrencyLevel) {
            ConcurrentDictionary<string, FileMetadata> metadata;
            try {
                metadata = new ConcurrentDictionary<string, FileMetadata>(filesPaths.Length, concurrencyLevel);
                Parallel.ForEach(filesPaths, (currentPath) => {
                    using (FileStream stream = File.OpenRead(currentPath)) {
                        metadata[currentPath] = new FileMetadata(currentPath, Md5HashFactory.NormalizeMd5(Md5HashFactory.GeneratedMd5HashFromStream(stream)));
                    }
                });
            }
            catch (Exception ex) {
                if (ex is KeyNotFoundException)
                    throw new Exception("KeyNotFoundException");
                throw;
            }
            return metadata;
        }

        void IFileSystemExplorer.FetchFile(string path, string resource, IDownloader Downloader, string expectedHash) {
            try {
                FileInfo file = new FileInfo(path);
                file.Directory.Create();
                byte[] data = Downloader.DownloadData(resource, expectedHash);
                File.WriteAllBytes(path, data);
            }
            finally {

            }
        }

        bool IFileSystemExplorer.FileExists(string file) {
            return File.Exists(file);
        }
    }
}