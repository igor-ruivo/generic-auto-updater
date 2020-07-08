using M2BobPatcher.Downloaders;
using M2BobPatcher.ExceptionHandler;
using M2BobPatcher.ExceptionHandler.Exceptions;
using M2BobPatcher.FileSystem;
using M2BobPatcher.Resources;
using M2BobPatcher.Resources.Configs;
using M2BobPatcher.Resources.TextResources;
using M2BobPatcher.UI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace M2BobPatcher.Engine {
    class PatcherEngine : IPatcherEngine {

        private static ConcurrentDictionary<string, FileMetadata> LocalMetadata;
        private static Dictionary<string, FileMetadata> ServerMetadata;
        private static BackgroundWorker BW;
        private static string PatchDirectory;
        private static int CurrentStep;
        private static float PipelineLength;

        private static readonly int LogicalProcessorsCount = Environment.ProcessorCount;

        public PatcherEngine(BackgroundWorker bw) {
            BW = bw;
            CurrentStep = -1;
        }

        void IPatcherEngine.Patch() {
            Tuple<Action, string>[] pipeline = {
                new Tuple<Action, string>(GenerateServerMetadata,
                PatcherEngineResources.PARSING_SERVER_METADATA),
                new Tuple<Action, string>(DownloadMissingContent,
                PatcherEngineResources.CHECKING_MISSING_CONTENT),
                new Tuple<Action, string>(DownloadOutdatedContent,
                PatcherEngineResources.CHECKING_OUTDATED_CONTENT)
            };
            PipelineLength = pipeline.Length;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < PipelineLength; i++) {
                CurrentStep = i;
                Utils.Log(BW, string.Format(PatcherEngineResources.STEP, i + 1, PipelineLength) + pipeline[i].Item2, ProgressiveWidgetsEnum.Label.InformativeLogger);
                pipeline[i].Item1.Invoke();
                Utils.Progress(BW, Convert.ToInt32((i + 1) / PipelineLength * 100), ProgressiveWidgetsEnum.ProgressBar.WholeProgressBar);
            }
            Finish(sw);
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

        private void DownloadContent(List<string> content, bool isMissingContent) {
            LogDownloadingEvent(content.Count, isMissingContent);
            for (int i = 0; i < content.Count; i++) {
                Utils.Log(BW, content[i], ProgressiveWidgetsEnum.Label.DownloadLogger);
                FileSystemExplorer.FetchFile(BW, content[i], PatchDirectory + content[i], ServerMetadata[content[i]].Hash);
                Utils.Progress(BW, Convert.ToInt32(GetCurrentStepProgress() + (i + 1) / (float)content.Count * (1 / PipelineLength * 100)), ProgressiveWidgetsEnum.ProgressBar.WholeProgressBar);
            }
            Thread.Sleep(EngineConfigs.MS_TO_WAIT_FOR_AV_FALSE_POSITIVES);
            GenerateLocalMetadata();
        }

        private List<string> CalculateMissingContent() {
            List<string> missingFiles = new List<string>();
            foreach (string file in ServerMetadata.Keys)
                if (!FileSystemExplorer.FileExists(file))
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

        private string DownloadServerMetadataFile() {
            byte[] data = HttpClientDownloader.DownloadData(BW, EngineConfigs.M2BOB_PATCH_METADATA, null);
            return Utils.PerformPatchDirectorySanityCheck(data);
        }

        private void GenerateLocalMetadata() {
            LocalMetadata = FileSystemExplorer.GenerateLocalMetadata(ServerMetadata.Keys.ToArray(), LogicalProcessorsCount / 2);
        }

        private void DownloadMissingContent() {
            DownloadContent(CalculateMissingContent(), true);
        }

        private void DownloadOutdatedContent() {
            DownloadContent(CalculateOutdatedContent(), false);
        }

        private int GetCurrentStepProgress() {
            return Convert.ToInt32(CurrentStep / PipelineLength * 100);
        }

        private void LogDownloadingEvent(int nResources, bool isMissingContent) {
            if (nResources > 0) {
                string message = isMissingContent ? PatcherEngineResources.DOWNLOADING_MISSING_CONTENT : PatcherEngineResources.DOWNLOADING_OUTDATED_CONTENT;
                Utils.Log(BW, message, ProgressiveWidgetsEnum.Label.InformativeLogger);
            }
        }

        private void PerformLastSanityChecks() {
            Thread.Sleep(EngineConfigs.MS_TO_WAIT_FOR_AV_FALSE_POSITIVES);
            if (CalculateOutdatedContent().Count != 0)
                Handler.Handle(new DataTamperedException());
            if (CalculateMissingContent().Count != 0)
                Handler.Handle(new FileNotFoundException());
        }

        private void Finish(Stopwatch sw) {
            PerformLastSanityChecks();
            sw.Stop();
            Utils.Log(BW, string.Format(PatcherEngineResources.ALL_FILES_ANALYZED, sw.Elapsed.ToString("hh\\:mm\\:ss")), ProgressiveWidgetsEnum.Label.DownloadLogger);
        }
    }
}