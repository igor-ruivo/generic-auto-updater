using M2BobPatcher.Downloaders;
using M2BobPatcher.ExceptionHandler;
using M2BobPatcher.FileSystem;
using M2BobPatcher.Resources.Configs;
using M2BobPatcher.Resources.TextResources;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

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
            Console.WriteLine("patching.");
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
                //UI.Log(string.Format(PatcherEngineResources.STEP, i + 1, PipelineLength) + pipeline[i].Item2, true);
                Console.WriteLine(string.Format(PatcherEngineResources.STEP, i + 1, PipelineLength) + pipeline[i].Item2);
                pipeline[i].Item1.Invoke();
                BW.ReportProgress(Convert.ToInt32((i + 1) / PipelineLength * 100), false);
            }
            sw.Stop();
            Finish(sw.Elapsed);
        }

        private void DownloadOutdatedContent() {
            List<string> outdatedContent = CalculateOutdatedContent();
            for (int i = 0; i < outdatedContent.Count; i++) {
                try {
                    BW.ReportProgress(-2, outdatedContent[i]);
                    Explorer.FetchFile(outdatedContent[i], PatchDirectory + outdatedContent[i], Downloader, ServerMetadata[outdatedContent[i]].Hash);
                }
                catch (Exception ex) {
                    Handler.Handle(ex);
                }
                BW.ReportProgress(Convert.ToInt32(GetCurrentStepProgress() + (i + 1) / (float)outdatedContent.Count * (1 / PipelineLength * 100)), false);
            }
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
            foreach (KeyValuePair<string, FileMetadata> entry in ServerMetadata)
                if (!entry.Value.Hash.Equals(LocalMetadata[entry.Key].Hash))
                    outdatedFiles.Add(entry.Key);
            return outdatedFiles;
        }

        private void DownloadMissingContent() {
            List<string> missingContent = CalculateMissingContent();
            for (int i = 0; i < missingContent.Count; i++) {
                try {
                    BW.ReportProgress(-2, missingContent[i]);
                    Explorer.FetchFile(missingContent[i], PatchDirectory + missingContent[i], Downloader, ServerMetadata[missingContent[i]].Hash);
                }
                catch (Exception ex) {
                    Handler.Handle(ex);
                }
                BW.ReportProgress(Convert.ToInt32(GetCurrentStepProgress() + (i + 1) / (float)missingContent.Count * (1 / PipelineLength * 100)), false);
            }
        }

        private string DownloadServerMetadataFile() {
            byte[] data = null;
            try {
                data = Downloader.DownloadData(EngineConfigs.M2BOB_PATCH_METADATA, null);
            }
            catch (Exception ex) {
                Handler.Handle(ex);
            }
            return System.Text.Encoding.Default.GetString(data);
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
            ServerMetadata = new Dictionary<string, FileMetadata>(numberOfRemoteFiles);
            for (int i = 1; i < metadataByLine.Length; i += 2)
                ServerMetadata[metadataByLine[i]] = new FileMetadata(metadataByLine[i], metadataByLine[i + 1]);
        }

        private void Finish(TimeSpan elapsed) {
            BW.ReportProgress(-2, string.Format(PatcherEngineResources.ALL_FILES_ANALYZED, elapsed.ToString("hh\\:mm\\:ss")));
        }
    }
}