using System;
using System.Collections.Concurrent;
using M2BobPatcher.Downloaders;

namespace M2BobPatcher.FileSystem {
    interface IFileSystemExplorer {

        ConcurrentDictionary<string, FileMetadata> GenerateLocalMetadata(string[] filesPaths, int concurrencyLevel);
        void FetchFile(string path, string resource, Action<string, bool> loggerFunction, bool throughCommonLogger, IDownloader Downloader);

        bool FileExists(string file);
    }
}
