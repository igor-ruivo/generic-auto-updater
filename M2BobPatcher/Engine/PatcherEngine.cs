using M2BobPatcher.Downloaders;
using M2BobPatcher.FileSystem;
using M2BobPatcher.Resources.Configs;
using M2BobPatcher.Resources.TextResources;
using M2BobPatcher.UI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace M2BobPatcher.Engine {
    class PatcherEngine : IPatcherEngine {

        private IFileSystemExplorer Explorer;
        private ConcurrentDictionary<string, FileMetadata> LocalMetadata;
        private Dictionary<string, FileMetadata> ServerMetadata;
        private UIComponents UI;
        private string PatchDirectory;
        private int LogicalProcessorsCount;

        public PatcherEngine(UIComponents ui) {
            Explorer = new FileSystemExplorer();
            LogicalProcessorsCount = Environment.ProcessorCount;
            UI = ui;
        }

        void IPatcherEngine.Patch() {
            GenerateServerMetadata(DownloadServerMetadataFile());
            DownloadMissingContent();
            GenerateLocalMetadata();
            DownloadOutdatedContent();
            Finish();
        }

        private void DownloadOutdatedContent() {
            UI.Log(PatcherEngineResources.DOWNLOADING_OUTDATED_CONTENT, true);
            List<string> outdatedContent = CalculateOutdatedContent();
            for (int i = 0; i < outdatedContent.Count; i++) {
                Explorer.RequestWriteFile(outdatedContent[i], PatchDirectory + outdatedContent[i], UI.Log, false, UI.RegisterProgress);
                UI.RegisterProgress(Convert.ToInt32(80 + (i + 1) / (float)outdatedContent.Count * 20), false);
            }
            UI.RegisterProgress(100, false);
        }

        private List<string> CalculateMissingContent() {
            List<string> missingFiles = new List<string>();
            foreach (string file in ServerMetadata.Keys)
                if (!Explorer.FileExists(file))
                    missingFiles.Add(file);
            return missingFiles;
        }

        private List<string> CalculateOutdatedContent() {
            List<string> outdatedFiles = new List<string>();
            foreach (KeyValuePair<string, FileMetadata> entry in ServerMetadata)
                if (!entry.Value.Hash.Equals(LocalMetadata[entry.Key].Hash))
                    outdatedFiles.Add(entry.Key);
            return outdatedFiles;
        }

        private void DownloadMissingContent() {
            UI.Log(PatcherEngineResources.DOWNLOADING_MISSING_CONTENT, true);
            List<string> missingContent = CalculateMissingContent();
            for (int i = 0; i < missingContent.Count; i++) {
                Explorer.RequestWriteFile(missingContent[i], PatchDirectory + missingContent[i], UI.Log, false, UI.RegisterProgress);
                UI.RegisterProgress(Convert.ToInt32(40 + (i + 1) / (float)missingContent.Count * 20), false);
            }
            UI.RegisterProgress(60, false);
        }

        private string DownloadServerMetadataFile() {
            UI.Log(PatcherEngineResources.DOWNLOADING_SERVER_METADATA, true);
            Task<byte[]> data = WebClientDownloader.DownloadData(EngineConfigs.M2BOB_PATCH_METADATA, UI.RegisterProgress);
            data.Wait();
            UI.RegisterProgress(20, false);
            return System.Text.Encoding.Default.GetString(data.Result);
        }

        private void GenerateLocalMetadata() {
            UI.Log(PatcherEngineResources.GENERATING_LOCAL_METADATA, true);
            LocalMetadata = Explorer.GenerateLocalMetadata(ServerMetadata.Keys.ToArray(), LogicalProcessorsCount / 2);
            UI.RegisterProgress(80, false);
        }

        private void GenerateServerMetadata(string serverMetadata) {
            UI.Log(PatcherEngineResources.PARSING_SERVER_METADATA, true);
            string[] metadataByLine = serverMetadata.Trim().Split(new[] { "\n" }, StringSplitOptions.None);
            PatchDirectory = metadataByLine[0];
            int numberOfRemoteFiles = (metadataByLine.Length - 1) / 2;
            ServerMetadata = new Dictionary<string, FileMetadata>(numberOfRemoteFiles);
            for (int i = 1; i < metadataByLine.Length; i += 2) {
                string filename = metadataByLine[i];
                string fileMd5 = metadataByLine[i + 1];
                ServerMetadata[filename] = new FileMetadata(filename, fileMd5);
            }
            UI.RegisterProgress(40, false);
        }

        private void Finish() {
            UI.Log(PatcherEngineResources.FINISHED, true);
            UI.Log(PatcherEngineResources.ALL_FILES_ANALYZED, false);
            UI.Toggle(true);
        }
    }
}