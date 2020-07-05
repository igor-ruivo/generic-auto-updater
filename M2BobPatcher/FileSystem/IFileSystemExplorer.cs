using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M2BobPatcher.FileSystem {
    interface IFileSystemExplorer {
        void generateMetadata(string[] filesPaths, int logicalProcessorsCount);
    }
}
