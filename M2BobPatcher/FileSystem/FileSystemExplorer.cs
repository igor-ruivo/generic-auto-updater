using M2BobPatcher.TextResources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M2BobPatcher.FileSystem {
    class FileSystemExplorer : IFileSystemExplorer {

        private string currentDirectory;

        public FileSystemExplorer() {
            currentDirectory = Environment.CurrentDirectory;
        }

        void IFileSystemExplorer.generateMetadata(string[] filesPaths, int logicalProcessorsCount) {
            foreach (string path in filesPaths) {
                if (fileShouldBeIgnored(path))
                    continue;
                //TODO
            }
        }

        private bool fileShouldBeIgnored(string path) {
            foreach (string ignoredFile in PatchIgnore.IGNORED_FILES)
                if (path.StartsWith(resolvePath(ignoredFile)))
                    return true;
            return false;
        }

        private string resolvePath(string relativePath) {
            return Path.Combine(currentDirectory, relativePath);
        }
    }
}
