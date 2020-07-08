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

        private static readonly Tuple<Action<int>, string>[] Pipeline = {
                new Tuple<Action<int>, string>(GenerateServerMetadata,
                PatcherEngineResources.PARSING_SERVER_METADATA),
                new Tuple<Action<int>, string>(DownloadMissingContent,
                PatcherEngineResources.CHECKING_MISSING_CONTENT),
                new Tuple<Action<int>, string>(DownloadOutdatedContent,
                PatcherEngineResources.CHECKING_OUTDATED_CONTENT)
            };

        public PatcherEngine(BackgroundWorker bw) {
            BW = bw;
        }

        void IPatcherEngine.Patch() {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < Pipeline.Length; i++) {
                Utils.Log(BW, string.Format(PatcherEngineResources.STEP, i + 1, Pipeline.Length) + Pipeline[i].Item2, ProgressiveWidgetsEnum.Label.InformativeLogger);
                Pipeline[i].Item1.Invoke(i);
                Utils.Progress(BW, Convert.ToInt32((i + 1f) / Pipeline.Length * 100), ProgressiveWidgetsEnum.ProgressBar.WholeProgressBar);
            }
            Finish(sw);
        }

        private static void DownloadContent(int step, List<string> content, bool isMissingContent) {
            LogDownloadingEvent(content.Count, isMissingContent);
            for (int i = 0; i < content.Count; i++) {
                Utils.Log(BW, content[i], ProgressiveWidgetsEnum.Label.DownloadLogger);
                FileSystemExplorer.FetchFile(BW, content[i], PatchDirectory + content[i], ServerMetadata[content[i]].Hash);
                Utils.Progress(BW, Convert.ToInt32(GetCurrentStepProgress(step) + (i + 1f) / content.Count * (1f / Pipeline.Length * 100)), ProgressiveWidgetsEnum.ProgressBar.WholeProgressBar);
            }
            Thread.Sleep(EngineConfigs.MS_TO_WAIT_FOR_AV_FALSE_POSITIVES);
            GenerateLocalMetadata();
        }

        private static void GenerateServerMetadata(int step) {
            string[] metadataByLine = DownloadServerMetadataFile().Trim().Split(new[] { "\n" }, StringSplitOptions.None);
            PatchDirectory = metadataByLine[0];
            ServerMetadata = new Dictionary<string, FileMetadata>((metadataByLine.Length - 1) / 2);
            for (int i = 1; i < metadataByLine.Length; i += 2)
                ServerMetadata[metadataByLine[i]] = new FileMetadata(metadataByLine[i], metadataByLine[i + 1]);
        }

        private static List<string> CalculateMissingContent() {
            List<string> missingFiles = new List<string>();
            foreach (string file in ServerMetadata.Keys)
                if (!FileSystemExplorer.FileExists(file))
                    missingFiles.Add(file);
            return missingFiles;
        }

        private static List<string> CalculateOutdatedContent() {
            List<string> outdatedFiles = new List<string>();
            foreach (KeyValuePair<string, FileMetadata> entry in ServerMetadata)
                if (!entry.Value.Hash.Equals(LocalMetadata[entry.Key].Hash))
                    outdatedFiles.Add(entry.Key);
            return outdatedFiles;
        }

        private static string DownloadServerMetadataFile() {
            return Utils.PerformPatchDirectorySanityCheck(HttpClientDownloader.DownloadData(BW, EngineConfigs.M2BOB_PATCH_METADATA, null));
        }

        private static void GenerateLocalMetadata() {
            LocalMetadata = FileSystemExplorer.GenerateLocalMetadata(ServerMetadata.Keys.ToArray(), Environment.ProcessorCount / 2);
        }

        private static void DownloadMissingContent(int step) {
            DownloadContent(step, CalculateMissingContent(), true);
        }

        private static void DownloadOutdatedContent(int step) {
            DownloadContent(step, CalculateOutdatedContent(), false);
        }

        private static int GetCurrentStepProgress(int step) {
            return Convert.ToInt32(step / (float)Pipeline.Length * 100);
        }

        private static void LogDownloadingEvent(int nResources, bool isMissingContent) {
            if (nResources == 0)
                return;
            string message = isMissingContent ? PatcherEngineResources.DOWNLOADING_MISSING_CONTENT : PatcherEngineResources.DOWNLOADING_OUTDATED_CONTENT;
            Utils.Log(BW, message, ProgressiveWidgetsEnum.Label.InformativeLogger);
        }

        private static void PerformLastSanityChecks() {
            Thread.Sleep(EngineConfigs.MS_TO_WAIT_FOR_AV_FALSE_POSITIVES);
            if (CalculateOutdatedContent().Count != 0)
                Handler.Handle(new DataTamperedException());
            if (CalculateMissingContent().Count != 0)
                Handler.Handle(new FileNotFoundException());
        }

        private static void Finish(Stopwatch sw) {
            PerformLastSanityChecks();
            sw.Stop();
            Utils.Log(BW, string.Format(PatcherEngineResources.ALL_FILES_ANALYZED, sw.Elapsed.ToString("hh\\:mm\\:ss")), ProgressiveWidgetsEnum.Label.DownloadLogger);
        }
    }
}