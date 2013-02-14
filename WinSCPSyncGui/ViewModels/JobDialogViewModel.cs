using Caliburn.Micro;
using System.Collections.Generic;
using System.Windows.Controls;
using WinSCPSyncGui.Views;
using WinSCPSyncLib;
using WinSCPSyncLib.Infrastructure.Security;
using WinSCPSyncLib.Model;

namespace WinSCPSyncGui.ViewModels
{
    public class JobDialogViewModel : Screen
    {
        private BackupJob _job;
        private bool _isNew;

        public void Init(BackupJob job)
        {
            Init(job, false);
        }

        public void Init(BackupJob job, bool isNew)
        {
            _job = job;
            _isNew = isNew;
        }

        protected override void OnViewLoaded(object view)
        {
            var myView = view as JobDialogView;
            myView.HostPassword.Password = HostPassword;
        }

        public void PasswordChanged(PasswordBox passwordControl)
        {
            HostPassword = passwordControl.Password;
        }

        public void Browse()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                Source = dialog.SelectedPath;
            }
        }

        public void Cancel()
        {
            TryClose(false);
        }

        public void Save()
        {
            var backupManager = IoC.Get<IBackupManager>();

            if (_isNew)
                backupManager.AddJob(_job);
            else
                backupManager.SaveJob(_job);

            TryClose(true);
        }

        public List<string> AvailableProtocols
        {
            get { return new List<string> { "Sftp", "Ftp", "Scp" }; }
        }

        public List<string> AvailableSyncModes
        {
            get { return new List<string> { "Local", "Remote", "Both" }; }
        }

        public string Source
        {
            get { return _job.Source; }
            set
            {
                _job.Source = value;
                NotifyOfPropertyChange(() => Source);
            }
        }

        public string Host
        {
            get { return _job.Host; }
            set
            {
                _job.Host = value;
                NotifyOfPropertyChange(() => Host);
            }
        }

        public string HostUsername
        {
            get { return _job.HostUsername; }
            set
            {
                _job.HostUsername = value;
                NotifyOfPropertyChange(() => HostUsername);
            }
        }

        public string HostPassword
        {
            get { 
                return Cryptor.Decrypt(_job.HostPassword); }
            set
            {
                _job.HostPassword = Cryptor.Encrypt(value);
                NotifyOfPropertyChange(() => HostPassword);
            }
        }

        public string HostKeyFingerprint
        {
            get { return _job.HostKeyFingerprint; }
            set
            {
                _job.HostKeyFingerprint = value;
                NotifyOfPropertyChange(() => HostKeyFingerprint);
            }
        }

        public string SyncMode
        {
            get { return _job.SyncMode; }
            set
            {
                _job.SyncMode = value;
                NotifyOfPropertyChange(() => SyncMode);
            }
        }

        public bool RemoveFiles
        {
            get { return _job.RemoveFiles; }
            set
            {
                _job.RemoveFiles = value;
                NotifyOfPropertyChange(() => RemoveFiles);
            }
        }

        public string Destination
        {
            get { return _job.Destination; }
            set
            {
                _job.Destination = value;
                NotifyOfPropertyChange(() => Destination);
            }
        }

        public string Protocol
        {
            get { return _job.Protocol; }
            set
            {
                _job.Protocol = value;
                NotifyOfPropertyChange(() => Protocol);
            }
        }

        public bool Running
        {
            get { return _job.Running; }
        }
    }
}
