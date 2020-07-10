namespace GenericAutoUpdater.FileSystem {
    /// <summary>
    /// The class used to model the metadata of a file.
    /// </summary>
    class FileMetadata : IFileMetadata {
        /// <summary>
        /// The file name of the file.
        /// </summary>
        private string Filename;

        /// <summary>
        /// The md5 hash of the file.
        /// </summary>
        public string Hash { get; }

        /// <summary>
        /// Creates a new instance of the class <c>FileMetadata</c>.
        /// </summary>
        public FileMetadata(string filename, string hash) {
            Filename = filename;
            Hash = hash;
        }
    }
}