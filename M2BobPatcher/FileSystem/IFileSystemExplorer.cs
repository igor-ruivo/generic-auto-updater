using M2BobPatcher.Downloaders;
using System.Collections.Concurrent;

namespace M2BobPatcher.FileSystem {
    interface IFileSystemExplorer {

        ConcurrentDictionary<string, FileMetadata> GenerateLocalMetadata(string[] filesPaths, int concurrencyLevel);

        void FetchFile(string path, string resource, IDownloader Downloader, string expectedHash);

        bool FileExists(string file);
    }
}
