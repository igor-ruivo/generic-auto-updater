using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M2BobPatcher.FileSystem {
    interface IFileSystemExplorer {
        bool fileExists(string path);
    }
}
