using System;
using System.Threading;

namespace CopyDirectoryLib
{
    public interface ICopyDirectoryUtility
    {
        void CopyDirectory( CancellationToken token);
        DirCopyInfo GetDetails();
    }
}