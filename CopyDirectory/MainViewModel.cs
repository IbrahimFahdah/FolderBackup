using CopyDirectory.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Windows.Forms;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using CopyDirectoryLib;
using System.Threading.Tasks;

namespace CopyDirectory
{
    class MainViewModel : INotifyPropertyChanged
    {
        CancellationTokenSource cancellationTokenSource;

        public event PropertyChangedEventHandler PropertyChanged;

        bool _isBusy;
        public MainViewModel()
        {
            SourceDirSelectionCommand = new Command(SourceDirSelectionCommand_Executed, IsBusy);
            DestinationDirSelectionCommand = new Command(DestinationDirSelectionCommand_Executed, IsBusy);
            GoCommand = new Command(GoCommand_Executed, CanCopy);
            CancelCommand = new Command(CancelCommand_Executed);
        }


        #region commands
        public ICommand SourceDirSelectionCommand { get; set; }
        public ICommand DestinationDirSelectionCommand { get; set; }
        public ICommand GoCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        #endregion

        #region pro
        private string _sourceDir;//=@"C:\Users\Ibrahim\Desktop\CV";
        private string _destinationDir;//= @"C:\Users\Ibrahim\Desktop\CV2";
        private string _dirInfo;
        private string _currentFile;
        private double _currentFileProgress, _overAllProgress;
        private bool _isCancelCopyVisible;
        public string SourceDir
        {
            get => _sourceDir; set
            {
                _sourceDir = value;
                NotifyPropertyChanged();
            }
        }
        public string DestinationDir
        {
            get => _destinationDir; set
            {
                _destinationDir = value;
                NotifyPropertyChanged();
            }
        }

        public string DirInfo
        {
            get => _dirInfo; set
            {
                _dirInfo = value;
                NotifyPropertyChanged();
            }
        }

        public string CurrentFile
        {
            get => _currentFile; set
            {
                _currentFile = value;
                NotifyPropertyChanged();
            }
        }

        public double CurrentFileProgress
        {
            get => _currentFileProgress; set
            {
                _currentFileProgress = value;
                NotifyPropertyChanged();
            }
        }

        public double OverAllProgress
        {
            get => _overAllProgress; set
            {
                _overAllProgress = value;
                NotifyPropertyChanged();
            }
        }
        public bool IsCancelCopyVisible
        {
            get => _isCancelCopyVisible;
            set
            {
                _isCancelCopyVisible = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        private async void GoCommand_Executed()
        {
            SetBusyStatus(true);
            try
            {
                var result = await Task.Run(() =>
                {
                    cancellationTokenSource = new CancellationTokenSource();
                    Go(cancellationTokenSource);
                    if (cancellationTokenSource.IsCancellationRequested)
                    {
                        DirInfo = "";
                        CurrentFile = "";
                        CurrentFileProgress = 0;
                        OverAllProgress = 0;
                        return "Task canceled.";
                    }
                    return "Task completed.";
                });

                MessageBox.Show(result, "Copy directory");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Copy directory");
            }
            finally
            {
                SetBusyStatus(false);
            }

        }

        private void Go(CancellationTokenSource cancellationTokenSource)
        {
            CopyDirectoryUtility utility = new CopyDirectoryUtility(SourceDir, DestinationDir,new DirCopyProgressReportFactory());
            utility.CopyProgress += OnProgressChanged;
            var info = utility.GetDetails();
            if (info.DesDirHasContent)
            {
                var res = MessageBox.Show("Destination directory has some content.\nDo you like to remove the existing content?",
                    "Destination folder", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (res == DialogResult.Cancel)
                {
                    CancelCommand_Executed();
                    return;

                }
            }

            DirInfo = $"There are {info.TotalFileCount} files in {info.DirectoryCount} directories.";
           
            utility.CopyDirectory(cancellationTokenSource.Token);

            utility.CopyProgress -= OnProgressChanged;
        }

        private void OnProgressChanged(object sender, IDirCopyProgressReportEventArgs report)
        {
            CurrentFileProgress = report.CurrentFilePorgress;
            CurrentFile = $"Copying: {report.CurrentFile} ; file size: {report.CurrentFileSize} bytes.";

            OverAllProgress = report.OverAllProgress;
        }

        private void CancelCommand_Executed()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
            }
        }

        private void SourceDirSelectionCommand_Executed()
        {
            SourceDir = SelectDirectory();
        }

        private void DestinationDirSelectionCommand_Executed()
        {
            DestinationDir = SelectDirectory();
        }

        private string SelectDirectory()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    return dialog.SelectedPath;
                }
                return null;
            }

        }

        private void SetBusyStatus(bool isBusy)
        {
            _isBusy = isBusy;
            IsCancelCopyVisible = isBusy;
        }

        private bool IsBusy()
        {
            return !_isBusy;
        }

        private bool CanCopy()
        {
            return !_isBusy && !string.IsNullOrWhiteSpace(SourceDir) && !string.IsNullOrWhiteSpace(DestinationDir);
        }


        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
