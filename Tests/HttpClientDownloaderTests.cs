using GenericAutoUpdater.Downloaders;
using GenericAutoUpdater.Hash;
using GenericAutoUpdater.Resources.Configs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;

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
            byte[] smallFile = Downloader.DownloadDataToMemory(EndpointUrl + filename);
            Assert.AreEqual(expectedHash, Hasher.GeneratedHashFromByteArray(smallFile));
        }

        /// <summary>
        /// Tests the DownloadDataToFile method by downloading a small file to the disk and asserting the equality between the original file hash and the hash of the downloaded file.
        /// </summary>
        [TestMethod]
        public void DownloadDataToDiskTest() {
            string filename = "SmallFile.txt";
            string expectedHash = Hasher.GeneratedHashFromFile(ServerFilesDirectory + filename);
            Downloader.DownloadDataToFile(EndpointUrl + filename, DownloadedFilesDirectory + filename, expectedHash);
            Assert.AreEqual(expectedHash, Hasher.GeneratedHashFromFile(DownloadedFilesDirectory + filename));
        }

        /// <summary>
        /// This method is run on the server thread.
        /// Acts as a server, waiting for any request through the HttpListener.
        /// Expects a request on a file present in the local file's directory. Whenever it receives one, it reads the file and writes its content in the request's OutputStream.
        /// </summary>
        private static void ServerThread() {
            while (true) {
                HttpListenerContext context = httpListener.GetContext();
                string file = ServerFilesDirectory + context.Request.Url.LocalPath;
                if (!File.Exists(file)) {
                    context.Response.StatusCode = 404;
                    context.Response.Close();
                    continue;
                }
                long totalRead = 0;
                long totalReads = 0;
                byte[] buffer = new byte[DownloaderConfigs.BUFFER_SIZE];
                bool moreLeftToRead = true;
                using (FileStream fileStream = File.OpenRead(file)) {
                    context.Response.ContentLength64 = fileStream.Length;
                    do {
                        int read = fileStream.Read(buffer, 0, buffer.Length);
                        if (read == 0)
                            moreLeftToRead = false;
                        else {
                            context.Response.OutputStream.Write(buffer, 0, read);
                            totalRead += read;
                            totalReads++;
                        }
                    }
                    while (moreLeftToRead);
                }
                if (totalRead != new FileInfo(file).Length)
                    context.Response.StatusCode = 500;
                context.Response.Close();
            }
        }
    }
}