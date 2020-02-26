namespace CopyDirectoryLib
{
    public struct DirCopyInfo
    {
        /// <summary>
        /// Total number of files in the source directory and sub directories.
        /// </summary>
        public int TotalFileCount { get; set; }

        /// <summary>
        /// Total number all sub directories.
        /// </summary>
        public int DirectoryCount { get; set; }

        /// <summary>
        /// true if the destination directory has some content.
        /// </summary>
        public bool DesDirHasContent { get; set; }

        /// <summary>
        /// The source directory total size in bytes.
        /// </summary>
        public long TotalSize { get; internal set; }
    }
}
