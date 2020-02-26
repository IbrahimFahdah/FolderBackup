using System;
using CopyDirectoryLib;

namespace CopyDirectory
{
    public class DirCopyProgressReportFactory : IDirCopyProgressReportFactory
    {
        public IDirCopyProgressReportEventArgs Create()
        {
            return new DirCopyProgressReportEventArgs();
        }
    }
}
