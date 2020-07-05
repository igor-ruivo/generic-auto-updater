using System;
using System.Collections.Concurrent;

namespace M2BobPatcher.FileSystem {
    interface IFileSystemExplorer {
        ConcurrentDictionary<string, FileMetadata> GenerateLocalMetadata(string[] filesPaths, int concurrencyLevel);
        void RequestWriteFile(string path, string resource, Action<string, bool> loggerFunction, bool throughCommonLogger, Action<int, bool> progressFunction);

        bool FileExists(string file);
    }
}
