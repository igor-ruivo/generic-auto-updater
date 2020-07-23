using System.Net;

namespace Tests.DownloaderTests.Server {
    /// <summary>
    /// This interface represents the behaviour of a server regarding the download mechanism.
    /// </summary>
    interface IBehaviour {
        /// <summary>
        /// Computes the behaviour of a server regarding the download mechanism.
        /// </summary>
        void ComputeBehaviour(HttpListenerContext context, byte[] buffer, string file);
    }
}
