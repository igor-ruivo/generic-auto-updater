using M2BobPatcher.Downloaders;
using M2BobPatcher.FileSystem;
using M2BobPatcher.Resources.Configs;
using M2BobPatcher.Resources.TextResources;
using M2BobPatcher.UI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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
        private double PipelineLength;

        public PatcherEngine(UIComponents ui) {
            Explorer = new FileSystemExplorer();
            LogicalProcessorsCount = Environment.ProcessorCount;
            UI = ui;
        }

        void IPatcherEngine.Patch() {
            Tuple<Action, string>[] pipeline = {
                new Tuple<Action, string>(GenerateServerMetadata,
                PatcherEngineResources.PARSING_SERVER_METADATA),
                new Tuple<Action, string>(DownloadMissingContent,
                PatcherEngineResources.DOWNLOADING_MISSING_CONTENT),
                new Tuple<Action, string>(GenerateLocalMetadata,
                PatcherEngineResources.GENERATING_LOCAL_METADATA),
                new Tuple<Action, string>(DownloadOutdatedContent,
                PatcherEngineResources.DOWNLOADING_OUTDATED_CONTENT)
            };
            PipelineLength = pipeline.Length;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < PipelineLength; i++) {
                UI.Log(string.Format(PatcherEngineResources.STEP, i + 1, PipelineLength) + pipeline[i].Item2, true);
                pipeline[i].Item1.Invoke();
                UI.RegisterProgress(Convert.ToInt32((i + 1) / PipelineLength * 100), false);
            }
            sw.Stop();
            Finish(sw.Elapsed);
        }

        private void DownloadOutdatedContent() {
            List<string> outdatedContent = CalculateOutdatedContent();
            int currentProgress = UI.GetProgressBarProgress();
            for (int i = 0; i < outdatedContent.Count; i++) {
                Explorer.RequestWriteFile(outdatedContent[i], PatchDirectory + outdatedContent[i], UI.Log, false, UI.RegisterProgress);
                UI.RegisterProgress(Convert.ToInt32(currentProgress + (i + 1) / (float)outdatedContent.Count * (1 / PipelineLength * 100)), false);
            }
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
            List<string> missingContent = CalculateMissingContent();
            int currentProgress = UI.GetProgressBarProgress();
            for (int i = 0; i < missingContent.Count; i++) {
                Explorer.RequestWriteFile(missingContent[i], PatchDirectory + missingContent[i], UI.Log, false, UI.RegisterProgress);
                UI.RegisterProgress(Convert.ToInt32(currentProgress + (i + 1) / (float)missingContent.Count * (1 / PipelineLength * 100)), false);
            }
        }

        private string DownloadServerMetadataFile() {
            Task<byte[]> data = WebClientDownloader.DownloadData(EngineConfigs.M2BOB_PATCH_METADATA, UI.RegisterProgress);
            data.Wait();
            return System.Text.Encoding.Default.GetString(data.Result);
        }

        private void GenerateLocalMetadata() {
            LocalMetadata = Explorer.GenerateLocalMetadata(ServerMetadata.Keys.ToArray(), LogicalProcessorsCount / 2);
        }

        private void GenerateServerMetadata() {
            string serverMetadata = DownloadServerMetadataFile();
            string[] metadataByLine = serverMetadata.Trim().Split(new[] { "\n" }, StringSplitOptions.None);
            PatchDirectory = metadataByLine[0];
            int numberOfRemoteFiles = (metadataByLine.Length - 1) / 2;
            ServerMetadata = new Dictionary<string, FileMetadata>(numberOfRemoteFiles);
            for (int i = 1; i < metadataByLine.Length; i += 2)
                ServerMetadata[metadataByLine[i]] = new FileMetadata(metadataByLine[i], metadataByLine[i + 1]);
        }

        private void Finish(TimeSpan elapsed) {
            UI.Log(PatcherEngineResources.FINISHED, true);
            UI.Log(string.Format(PatcherEngineResources.ALL_FILES_ANALYZED, elapsed.ToString("hh\\:mm\\:ss")), false);
            UI.Toggle(true);
        }
    }
}