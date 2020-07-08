using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using M2BobPatcher.Downloaders;
using M2BobPatcher.ExceptionHandler;
using M2BobPatcher.FileSystem;
using M2BobPatcher.Resources.Configs;
using M2BobPatcher.Resources.TextResources;

namespace M2BobPatcher.Engine {
    class PatcherEngine : IPatcherEngine {

        private static IFileSystemExplorer Explorer;
        private static ConcurrentDictionary<string, FileMetadata> LocalMetadata;
        private static IDownloader Downloader;
        private static Dictionary<string, FileMetadata> ServerMetadata;
        private static BackgroundWorker BW;
        private static string PatchDirectory;
        private static int LogicalProcessorsCount;
        private static int CurrentStep;
        private static float PipelineLength;

        public PatcherEngine(BackgroundWorker bw) {
            BW = bw;
            Explorer = new FileSystemExplorer();
            LogicalProcessorsCount = Environment.ProcessorCount;
            Downloader = new HttpClientDownloader(BW);
            CurrentStep = -1;
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
                CurrentStep = i;
                BW.ReportProgress(-1, string.Format(PatcherEngineResources.STEP, i + 1, PipelineLength) + pipeline[i].Item2);
                pipeline[i].Item1.Invoke();
                BW.ReportProgress(Convert.ToInt32((i + 1) / PipelineLength * 100), false);
            }
            sw.Stop();
            Finish(sw.Elapsed);
        }

        private void DownloadContent(List<string> content) {
            for (int i = 0; i < content.Count; i++) {
                try {
                    BW.ReportProgress(-2, content[i]);
                    Explorer.FetchFile(content[i], PatchDirectory + content[i], Downloader, ServerMetadata[content[i]].Hash);
                    BW.ReportProgress(Convert.ToInt32(GetCurrentStepProgress() + (i + 1) / (float)content.Count * (1 / PipelineLength * 100)), false);
                } catch (Exception ex) {
                    if (ex is KeyNotFoundException)
                        Handler.Handle(new Exception("KeyNotFoundException"));
                    Handler.Handle(ex);
                }
            }
        }

        private void DownloadOutdatedContent() {
            DownloadContent(CalculateOutdatedContent());
        }

        private int GetCurrentStepProgress() {
            return Convert.ToInt32(CurrentStep / PipelineLength * 100);
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
            try {
                foreach (KeyValuePair<string, FileMetadata> entry in ServerMetadata)
                    if (!entry.Value.Hash.Equals(LocalMetadata[entry.Key].Hash))
                        outdatedFiles.Add(entry.Key);
            } catch (Exception ex) {
                if (ex is KeyNotFoundException)
                    Handler.Handle(new Exception("KeyNotFoundException"));
                Handler.Handle(ex);
            }
            return outdatedFiles;
        }

        private void DownloadMissingContent() {
            DownloadContent(CalculateMissingContent());
        }

        private string DownloadServerMetadataFile() {
            string metadata = null;
            try {
                byte[] data = Downloader.DownloadData(EngineConfigs.M2BOB_PATCH_METADATA, null);
                metadata = System.Text.Encoding.Default.GetString(data);
            } catch (Exception ex) {
                Handler.Handle(ex);
            }
            return metadata;
        }

        private void GenerateLocalMetadata() {
            try {
                LocalMetadata = Explorer.GenerateLocalMetadata(ServerMetadata.Keys.ToArray(), LogicalProcessorsCount / 2);
            } catch (Exception ex) {
                Handler.Handle(ex);
            }
        }

        private void GenerateServerMetadata() {
            string serverMetadata = DownloadServerMetadataFile();
            string[] metadataByLine = serverMetadata.Trim().Split(new[] { "\n" }, StringSplitOptions.None);
            PatchDirectory = metadataByLine[0];
            int numberOfRemoteFiles = (metadataByLine.Length - 1) / 2;
            try {
                ServerMetadata = new Dictionary<string, FileMetadata>(numberOfRemoteFiles);
                for (int i = 1; i < metadataByLine.Length; i += 2)
                    ServerMetadata[metadataByLine[i]] = new FileMetadata(metadataByLine[i], metadataByLine[i + 1]);
            } catch (Exception ex) {
                Handler.Handle(ex);
            }
        }

        private void Finish(TimeSpan elapsed) {
            BW.ReportProgress(-2, string.Format(PatcherEngineResources.ALL_FILES_ANALYZED, elapsed.ToString("hh\\:mm\\:ss")));
        }
    }
}