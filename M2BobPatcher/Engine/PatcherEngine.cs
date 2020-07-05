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

        /**1. ask server for metadata file with all files' full path + name + extension and their sizes and their md5
          *2. download files not present locally (check this by name)
          *3. generateMetadata for all files locally (which also exist in server info)
          *4. compare generatedMetadata with the one obtained from 1.
          *5. download files which metadata differs
          */
        void IPatcherEngine.patch() {
            //downloadServerMetadataFile();
            generateMetadata();
            
        }

        void IPatcherEngine.repair() {
            throw new NotImplementedException();
        }

        private string[] compareMetadata() {
            throw new NotImplementedException();
        }

        private void download() {
            throw new NotImplementedException();
        }

        private void downloadServerMetadataFile() {
            throw new NotImplementedException();
        }

        private void generateMetadata() {
            string[] dummyArray = {};
            explorer.generateMetadata(dummyArray, logicalProcessorsCount);
        }
    }
}
