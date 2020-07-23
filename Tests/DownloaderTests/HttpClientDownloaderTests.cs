using GenericAutoUpdater.Downloaders;
using GenericAutoUpdater.Hash;
using GenericAutoUpdater.Resources;
using GenericAutoUpdater.Resources.Configs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;
using Tests.DownloaderTests.Server;
using static Tests.Enums.HttpClientDownloaderTestsEnum;

namespace Tests {
    /// <summary>
    /// The class containing all tests regarding the HttpClientDownloader.
    /// </summary>
    [TestClass]
    public class HttpClientDownloaderTests {
        /// <summary>
        /// The HttpListener used as the local server.
        /// </summary>
        private static readonly HttpListener httpListener = new HttpListener();

        /// <summary>
        /// The Endpoint's Url to access the server.
        /// </summary>
        private static string EndpointUrl;

        /// <summary>
        /// The local directory where the server files are kept.
        /// </summary>
        private static string ServerFilesDirectory;

        /// <summary>
        /// The local directory to where the downloaded files are written.
        /// </summary>
        private static string DownloadedFilesDirectory;

        /// <summary>
        /// The <c>IDownloader</c>, used to download resources.
        /// </summary>
        private static IDownloader Downloader;

        /// <summary>
        /// The <c>IHasher</c>, used to compute any hash operation whenever appropriate.
        /// </summary>
        private static IHasher Hasher;

        /// <summary>
        /// The thread running the Server.
        /// </summary>
        private static Thread Server;

        /// <summary>
        /// This method performs any setup required before running the tests.
        /// It acts as a constructor for this test class.
        /// </summary>
        [ClassInitialize]
        public static void ClassInit(TestContext context) {
            httpListener.Prefixes.Add("http://localhost:5000/");
            httpListener.Prefixes.Add("http://127.0.0.1:5000/");
            httpListener.Start();
            EndpointUrl = "http://localhost:5000/";
            ServerFilesDirectory = Directory.GetCurrentDirectory() + "/../../TestServerFiles/";
            DownloadedFilesDirectory = Directory.GetCurrentDirectory() + "/../../TestDownloadedFiles/";
            BackgroundWorker BW = new BackgroundWorker {
                WorkerReportsProgress = true
            };
            Hasher = new Md5Hasher();
            Downloader = new HttpClientDownloader(BW, Hasher);
            Server = new Thread(ServerThread);
            Server.Start();
            new FileInfo(DownloadedFilesDirectory).Directory.Create();
        }

        /// <summary>
        /// This method performs any cleanup required after running the tests.
        /// It is responsible for stopping the Server and thus killing the thread it lived on.
        /// </summary>
        [ClassCleanup()]
        public static void ClassCleanup() {
            httpListener.Stop();
            if (Server.IsAlive) {
                Thread.Sleep(1000);
                if (Server.IsAlive)
                    Server.Abort();
            }
        }

        /// <summary>
        /// Tests the DownloadDataToMemory method by downloading a small file to memory and asserting the equality between the original file hash and the hash of the byte[] obtained from the download.
        /// </summary>
        [TestMethod]
        public void DownloadDataToMemoryTest() {
            string filename = "SmallFile.txt";
            string expectedHash = Hasher.GeneratedHashFromFile(ServerFilesDirectory + filename);
            byte[] smallFile = Downloader.DownloadDataToMemory(EndpointUrl + filename + "?behaviour=" + (int)ServerBehaviours.Normal);
            Assert.AreEqual(expectedHash, Hasher.GeneratedHashFromByteArray(smallFile));
        }

        /// <summary>
        /// Tests the DownloadDataToFile method by downloading a small file to the disk and asserting the equality between the original file hash and the hash of the downloaded file.
        /// </summary>
        [TestMethod]
        public void DownloadDataToDiskTest() {
            string filename = "SmallFile.txt";
            string expectedHash = Hasher.GeneratedHashFromFile(ServerFilesDirectory + filename);
            Downloader.DownloadDataToFile(EndpointUrl + filename + "?behaviour=" + (int)ServerBehaviours.Normal, DownloadedFilesDirectory + filename, expectedHash);
            Assert.AreEqual(expectedHash, Hasher.GeneratedHashFromFile(DownloadedFilesDirectory + filename));
        }

        /// <summary>
        /// Tests if a download is successful even if the server has considerable latency.
        /// </summary>
        [TestMethod]
        public void DownloadWithLatencyTest() {
            string filename = "20kb.dat";
            string expectedHash = Hasher.GeneratedHashFromFile(ServerFilesDirectory + filename);
            Downloader.DownloadDataToFile(EndpointUrl + filename + "?behaviour=" + (int)ServerBehaviours.Latency, DownloadedFilesDirectory + filename, expectedHash);
            Assert.AreEqual(expectedHash, Hasher.GeneratedHashFromFile(DownloadedFilesDirectory + filename));
        }

        /// <summary>
        /// Tests if an IOException is thrown whenever there is a read timeout on the downloader.
        /// Assures the downloader never gets stuck and that the CancellationToken works.
        /// </summary>
        [TestMethod]
        public void DownloadWithTimeoutTest() {
            string filename = "20kb.dat";
            string expectedHash = Hasher.GeneratedHashFromFile(ServerFilesDirectory + filename);
            try {
                Downloader.DownloadDataToFile(EndpointUrl + filename + "?behaviour=" + (int)ServerBehaviours.TimeoutDuringRead, DownloadedFilesDirectory + filename, expectedHash);
            }
            catch (Exception ex) {
                if (ex is IOException || ex is AggregateException exception && Utils.AggregateContainsSpecificException(exception, new IOException()))
                    return;
                Assert.Fail();
            }
        }

        /// <summary>
        /// Tests if an InvalidDataException is thrown whenever the downloader detects inconsistency between the expected hash and the downloaded file's hash.
        /// </summary>
        [TestMethod]
        public void DownloadInconsistentTest() {
            string filename = "SmallFile.txt";
            string expectedHash = Hasher.GeneratedHashFromFile(ServerFilesDirectory + filename);
            try {
                Downloader.DownloadDataToFile(EndpointUrl + filename + "?behaviour=" + (int)ServerBehaviours.Inconsistent, DownloadedFilesDirectory + filename, expectedHash);
            }
            catch (Exception ex) {
                if (ex is InvalidDataException || ex is AggregateException exception && Utils.AggregateContainsSpecificException(exception, new InvalidDataException()))
                    return;
                Assert.Fail();
            }
        }

        /// <summary>
        /// This method is run on the server thread.
        /// Acts as a server, waiting for any request through the HttpListener.
        /// Expects a request on a file present in the local file's directory. Whenever it receives one, it reads the file and writes its content in the request's OutputStream.
        /// The request must contain a query parameter named behaviour, with the desired behaviour to be simulated by this server while answering the respective request.
        /// </summary>
        private static void ServerThread() {
            while (true) {
                HttpListenerContext context = httpListener.GetContext();
                int behaviour = int.Parse(context.Request.QueryString["behaviour"]);
                string file = ServerFilesDirectory + context.Request.Url.LocalPath;
                if (!File.Exists(file)) {
                    context.Response.StatusCode = 404;
                    context.Response.Close();
                    continue;
                }
                byte[] buffer = new byte[DownloaderConfigs.BUFFER_SIZE];
                if (NormalBehaviour.IsThisKind(behaviour))
                    new NormalBehaviour().ComputeBehaviour(context, buffer, file);
                if (LatencyBehaviour.IsThisKind(behaviour))
                    new LatencyBehaviour().ComputeBehaviour(context, buffer, file);
                if (TimeoutBehaviour.IsThisKind(behaviour))
                    new TimeoutBehaviour().ComputeBehaviour(context, buffer, file);
                if (InconsistentBehaviour.IsThisKind(behaviour))
                    new InconsistentBehaviour().ComputeBehaviour(context, buffer, file);
                context.Response.Close();
            }
        }
    }
}