namespace CopyDirectoryLib
{
    public class DirCopyProgressReportEventArgs : IDirCopyProgressReportEventArgs
    {
        public double CurrentFilePorgress { get; set; }
        public string CurrentFile { get; set; }
        public long CurrentFileSize { get; set; }
        public string CurrentDirectory { get; set; }
        public double OverAllProgress { get; set; }
        public long TotalSize { get; set; }
    }
}