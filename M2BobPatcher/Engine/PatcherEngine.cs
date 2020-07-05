using M2BobPatcher.FileSystem;
using M2BobPatcher.TextResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M2BobPatcher.Engine {
    class PatcherEngine : IPatcherEngine {

        private IFileSystemExplorer explorer;
        private int logicalProcessorsCount;

        public PatcherEngine() {
            explorer = new FileSystemExplorer();
            logicalProcessorsCount = Environment.ProcessorCount;
        }

        string[] IPatcherEngine.compareMetadata() {
            throw new NotImplementedException();
        }

        void IPatcherEngine.download() {
            throw new NotImplementedException();
        }

        void IPatcherEngine.downloadServerMetadataFile() {
            throw new NotImplementedException();
        }

        void IPatcherEngine.generateMetadataFile() {
            throw new NotImplementedException();
        }

        bool IPatcherEngine.hasMetadataFile() {
            return explorer.fileExists(FilePaths.METADATA_FILE_PATH);
        }
    }
}
