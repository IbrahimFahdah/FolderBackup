namespace CopyDirectoryLib
{
    public interface IDirCopyProgressReportEventArgs
    {
        /// <summary>
        /// The percentage progress of copied file.
        /// </summary>
        double CurrentFilePorgress { get; set; }

        /// <summary>
        /// The file name of the copied file.
        /// </summary>
        string CurrentFile { get; set; }

        /// <summary>
        /// The size of the copied file.
        /// </summary>
        long CurrentFileSize { get; set; }

        /// <summary>
        /// The file name of the current copied directory.
        /// </summary>
        string CurrentDirectory{ get; set; }


        /// <summary>
        /// The percentage progress of the whole process.
        /// </summary>
        double OverAllProgress { get; set; }

        /// <summary>
        /// The total size to be copied in the whole process in bytes.
        /// </summary>
        long TotalSize { get; set; }
    }
}