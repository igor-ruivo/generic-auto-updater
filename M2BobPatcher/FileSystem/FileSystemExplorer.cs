using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M2BobPatcher.FileSystem {
    class FileSystemExplorer : IFileSystemExplorer {
        bool IFileSystemExplorer.fileExists(string path) {
            return File.Exists(path);
        }
    }
}
