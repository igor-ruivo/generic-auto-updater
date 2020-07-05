using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M2BobPatcher.FileSystem {
    interface IFileSystemExplorer {
        Dictionary<string, FileMetadata> GenerateLocalMetadata(string[] filesPaths);
        void RequestWriteFile(string path, string resource);
    }
}
