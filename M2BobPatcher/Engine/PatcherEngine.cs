using M2BobPatcher.Downloaders;
using M2BobPatcher.FileSystem;
using M2BobPatcher.Hash;
using M2BobPatcher.Resources.Configs;
using M2BobPatcher.Resources.TextResources;
using M2BobPatcher.TextResources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M2BobPatcher.Engine {
    class PatcherEngine : IPatcherEngine {

        private IFileSystemExplorer Explorer;
        private Dictionary<string, FileMetadata> LocalMetadata;
        private Dictionary<string, FileMetadata> ServerMetadata;
        private string PatchDirectory;
        private int LogicalProcessorsCount;

        public PatcherEngine() {
            Explorer = new FileSystemExplorer();
            LogicalProcessorsCount = Environment.ProcessorCount;
        }

        /**1. ask server for metadata file with all files' full path + name + extension and their sizes and their md5
          *2. download files not present locally (check this by name)
          *3. generateMetadata for all files locally (which also exist in server info)
          *4. compare generatedMetadata with the one obtained from 1.
          *5. download files which metadata differs
          */
        void IPatcherEngine.Patch() {
            GenerateServerMetadata(DownloadServerMetadataFile());
            DownloadMissingContent();
            GenerateLocalMetadata();
            //DownloadOutdatedContent();
        }

        void IPatcherEngine.Repair() {
            throw new NotImplementedException();
        }

        private string[] CompareMetadata() {
            throw new NotImplementedException();
        }

        private void DownloadOutdatedContent() {
            throw new NotImplementedException();
        }

        private void DownloadMissingContent() {
            foreach (string file in ServerMetadata.Keys)
                Explorer.RequestWriteFile(file, PatchDirectory + file);
        }

        private string DownloadServerMetadataFile() {
            return WebClientDownloader.DownloadString(EngineConfigs.M2BOB_PATCH_METADATA);
        }

        private void GenerateLocalMetadata() {
            LocalMetadata = Explorer.GenerateLocalMetadata(ServerMetadata.Keys.ToArray());
        }

        private void GenerateServerMetadata(string serverMetadata) {
            string[] metadataByLine = serverMetadata.Trim().Split(new[] { "\n" }, StringSplitOptions.None);
            PatchDirectory = metadataByLine[0];
            int numberOfRemoteFiles = (metadataByLine.Length - 1) / 2;
            ServerMetadata = new Dictionary<string, FileMetadata>(numberOfRemoteFiles);
            for (int i = 1; i < metadataByLine.Length; i += 2) {
                string filename = metadataByLine[i];
                string fileMd5 = metadataByLine[i + 1];
                ServerMetadata[filename] = new FileMetadata(filename, fileMd5);
            }
        }

        private void DebugPrintMetadata() {
            Console.WriteLine("Debug server:");
            foreach (KeyValuePair<string, FileMetadata> kvp in ServerMetadata) {
                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value.Hash);
            }
            Console.WriteLine("Debug local:");
            foreach (KeyValuePair<string, FileMetadata> kvp in LocalMetadata) {
                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value.Hash);
            }
        }
    }
}
