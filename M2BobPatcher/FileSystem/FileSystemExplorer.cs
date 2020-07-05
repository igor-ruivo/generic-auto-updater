using M2BobPatcher.Hash;
using M2BobPatcher.TextResources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace M2BobPatcher.FileSystem {
    class FileSystemExplorer : IFileSystemExplorer {

        private string CurrentDirectory;

        public FileSystemExplorer() {
            CurrentDirectory = Environment.CurrentDirectory;
        }

        Dictionary<string, FileMetadata> IFileSystemExplorer.GenerateLocalMetadata(string[] filesPaths) {
            Dictionary<string, FileMetadata> metadata = new Dictionary<string, FileMetadata>(filesPaths.Length);
            Parallel.ForEach(filesPaths, (currentPath) => {
                if (FileShouldBeIgnored(currentPath))
                    return;
                metadata[currentPath] = new FileMetadata(currentPath,
                    Md5HashFactory.NormalizeMd5(Md5HashFactory.GeneratedMd5HashFromFile(currentPath)));
                //Console.WriteLine($"Processing {currentPath} on thread {Thread.CurrentThread.ManagedThreadId}");
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

        private string ResolvePath(string relativePath) {
            return Path.Combine(CurrentDirectory, relativePath);
        }

        public string NormalizePath(string path) {
            return Path.GetFullPath(new Uri(ResolvePath(path)).LocalPath)
                       .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                       .ToUpperInvariant();
        }
    }
}
