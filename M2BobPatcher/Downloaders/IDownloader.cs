namespace M2BobPatcher.Downloaders {
    interface IDownloader {
        byte[] DownloadData(string address, string expectedHash);
    }
}
