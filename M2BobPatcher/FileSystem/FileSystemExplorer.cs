using M2BobPatcher.Downloaders;
using M2BobPatcher.Hash;
using M2BobPatcher.Resources.TextResources;
using M2BobPatcher.TextResources;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace M2BobPatcher.FileSystem {
    class FileSystemExplorer : IFileSystemExplorer {

        private static string CurrentDirectory;

        public FileSystemExplorer() {
            CurrentDirectory = Environment.CurrentDirectory;
        }

        ConcurrentDictionary<string, FileMetadata> IFileSystemExplorer.GenerateLocalMetadata(string[] filesPaths, int concurrencyLevel) {
            ConcurrentDictionary<string, FileMetadata> metadata = new ConcurrentDictionary<string, FileMetadata>(filesPaths.Length, concurrencyLevel);
            Parallel.ForEach(filesPaths, (currentPath) => {
                // This shouldn't be needed, since UserData isn't even in the server. But I think it's a cool feature.
                if (FileShouldBeIgnored(currentPath))
                    return;
                metadata[currentPath] = new FileMetadata(currentPath, Md5HashFactory.NormalizeMd5(Md5HashFactory.GeneratedMd5HashFromFile(currentPath)));
            });
            return metadata;
        }

        private bool FileShouldBeIgnored(string path) {
            foreach (string ignoredFile in PatchIgnore.IGNORED_FILES) {
                if (NormalizePath(path).StartsWith(NormalizePath(ignoredFile)))
                    return true;
            }
            return false;
        }

        private static string ResolvePath(string relativePath) {
            return Path.Combine(CurrentDirectory, relativePath);
        }

        public static string NormalizePath(string path) {
            return Path.GetFullPath(new Uri(ResolvePath(path)).LocalPath)
                       .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                       .ToUpperInvariant();
        }

        bool IFileSystemExplorer.FileExists(string file) {
            return File.Exists(file);
        }

        void IFileSystemExplorer.RequestWriteFile(string path, string resource, Action<string, bool> loggerFunction, bool throughCommonLogger, Action<int, bool> progressFunction) {
            loggerFunction(path, throughCommonLogger);
            FileInfo file = new FileInfo(path);
            file.Directory.Create();
            Task<byte[]> data = WebClientDownloader.DownloadData(resource, progressFunction);
            data.Wait();
            File.WriteAllBytes(path, data.Result);
            Console.WriteLine(FileSystemExplorerResources.FILE_WRITTEN_TO_DISK, ResolvePath(path));
        }
    }
}
