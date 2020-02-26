using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CopyDirectoryLib
{

    /// <summary>
    /// This class copy a directory and its content to a specified destination.
    /// </summary>
    public class CopyDirectoryUtility : ICopyDirectoryUtility
    {

        IDirCopyProgressReportFactory _progressReportFactory;
        IDirCopyProgressReportEventArgs _pReport;
        string _sourceDir, _destDir;
        long _totalSize,_totalProcessed;

        /// <summary>
        /// attach to this event if you want to monitor progress.
        /// </summary>
        public event EventHandler<IDirCopyProgressReportEventArgs> CopyProgress;

        public CopyDirectoryUtility(string sourceDir, string destDir,
            IDirCopyProgressReportFactory progressReportFactory)
        {
            _sourceDir = sourceDir;
            _destDir = destDir;

            _progressReportFactory = progressReportFactory;
          
        }

        /// <summary>
        /// Get general details about the source directory.
        /// </summary>
        /// <returns></returns>
        public DirCopyInfo GetDetails()
        {
            var info = GetDetails(_sourceDir, _destDir);
            _totalSize = info.TotalSize;
            return info;
        }

        /// <summary>
        /// Perform the copy process.
        /// </summary>
        /// <param name="token">token to allow process cancellation.</param>
        public void CopyDirectory(CancellationToken token)
        {
            _totalProcessed=0;
            _pReport = _progressReportFactory?.Create();
            if (_pReport == null)
            {
                _pReport = new DirCopyProgressReportEventArgs();
            }

            RemoveDestinationDirContent(_destDir);
            CopyDirectory(_sourceDir, _destDir, token);
        }

        private DirCopyInfo GetDetails(string sourceDir, string destDir)
        {
            var info = new DirCopyInfo();
            FindDirectoryDetails(sourceDir, ref info);

            info.DesDirHasContent = false;
            if (Directory.Exists(destDir))
            {
                info.DesDirHasContent = Directory.GetFiles(destDir).Length + Directory.GetDirectories(destDir).Length > 0;
            }
            return info;
        }

        private void CopyDirectory(string sourceDir, string destDir, CancellationToken token)
        {
            _pReport.CurrentDirectory = sourceDir;

            if (!Directory.Exists(destDir))
                Directory.CreateDirectory(destDir);

            string[] files = Directory.GetFiles(sourceDir);
            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                string dest = Path.Combine(destDir, name);

                CopyFile(file, dest, token);
                if (token != null && token.IsCancellationRequested)
                {
                    break;
                }
            }

            string[] dirs = Directory.GetDirectories(sourceDir);
            foreach (string dir in dirs)
            {
                string name = Path.GetFileName(dir);
                string dest = Path.Combine(destDir, name);

                CopyDirectory(dir, dest, token);

                if (token != null && token.IsCancellationRequested)
                {
                    break;
                }
            }
        }

        private void RemoveDestinationDirContent(string destDir)
        {
            if (Directory.Exists(destDir))
            {
                var dir = new DirectoryInfo(destDir);
                dir.Delete(true);
            }
        }

        private void CopyFile(string source, string des, CancellationToken token)
        {
            byte[] buffer = new byte[1024]; // 1kb buffer

            using (FileStream srStream = new FileStream(source, FileMode.Open, FileAccess.Read))
            {
                long fileLength = srStream.Length;

                using (FileStream destream = new FileStream(des, FileMode.CreateNew, FileAccess.Write))
                {
                    long totalBytes = 0;
                    int currentBlockSize = 0;

                    while ((currentBlockSize = srStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        totalBytes += currentBlockSize;


                        destream.Write(buffer, 0, currentBlockSize);
#if DEBUG
                        //for testing purpose
                        Thread.Sleep(5);
#endif

                        OnCopyProgress(source, fileLength, totalBytes, currentBlockSize);

                        if (token != null && token.IsCancellationRequested)
                        {
                            break;
                        }
                    }
                }
            }

        }

        private void FindDirectoryDetails(string dir, ref DirCopyInfo info)
        {
            info.TotalFileCount += Directory.GetFiles(dir).Length;

            DirectoryInfo d = new DirectoryInfo(dir);
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                info.TotalSize += fi.Length;
            }

            string[] dirs = Directory.GetDirectories(dir);
            info.DirectoryCount += dirs.Length;
            foreach (string folder in dirs)
            {
                FindDirectoryDetails(folder, ref info);
            }

        }

        protected virtual void OnCopyProgress(string currentfile, long filesize, double fileprogress, int currentBlockSize)
        {
            if (_pReport != null)
            {
                double persentage = Math.Min(100, (fileprogress * 100.0) / filesize);
                _pReport.CurrentFileSize = filesize;
                _pReport.CurrentFilePorgress = persentage;
                _pReport.CurrentFile = currentfile;
                _totalProcessed += currentBlockSize;
                _pReport.TotalSize = _totalSize;
                _pReport.OverAllProgress = Math.Min(100, (_totalProcessed * 100.0) / _totalSize);
                CopyProgress?.Invoke(this, _pReport);
            }

        }

    }
}
