namespace M2BobPatcher.FileSystem {
    class FileMetadata : IFileMetadata {

        private string Filename;

        public string Hash { get; }

        public FileMetadata(string filename, string hash) {
            Filename = filename;
            Hash = hash;
        }
    }
}
