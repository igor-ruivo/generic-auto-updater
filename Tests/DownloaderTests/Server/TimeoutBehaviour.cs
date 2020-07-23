using GenericAutoUpdater.Resources.Configs;
using System.IO;
using System.Net;
using System.Threading;
using static Tests.Enums.HttpClientDownloaderTestsEnum;

namespace Tests.DownloaderTests.Server {
    /// <summary>
    /// This class modules the behaviour of a server with timeout behaviour.
    /// </summary>
    class TimeoutBehaviour : IBehaviour {

        /// <summary>
        /// Simulates a timeout in the server while answering the request.
        /// This is meant to trigger the CancellationToken that exists in the HttpClientDownloader class.
        /// </summary>
        public void ComputeBehaviour(HttpListenerContext context, byte[] buffer, string file) {
            long totalRead = 0;
            long totalReads = 0;
            bool moreLeftToRead = true;
            using (FileStream fileStream = File.OpenRead(file)) {
                context.Response.ContentLength64 = fileStream.Length;
                do {
                    int read = fileStream.Read(buffer, 0, buffer.Length);
                    if (read == 0)
                        moreLeftToRead = false;
                    else {
                        if (totalReads == 1)
                            Thread.Sleep(DownloaderConfigs.TIMEOUT_MS_WAITING_FOR_READ * 3);
                        context.Response.OutputStream.Write(buffer, 0, read);
                        totalRead += read;
                        totalReads++;
                    }
                }
                while (moreLeftToRead);
            }
        }

        /// <summary>
        /// Returns true if the received behaviour corresponds to the behaviour moduled by this class.
        /// Returns false otherwise.
        /// </summary>
        public static bool IsThisKind(int behaviour) {
            return behaviour == (int)ServerBehaviours.TimeoutDuringRead;
        }
    }
}
