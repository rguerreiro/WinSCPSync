using Caliburn.Micro;
using System;
using WinSCPSyncGui.Messages;
using WinSCPSyncLib;
using WinSCPSyncLib.Model;

namespace WinSCPSyncGui.ViewModels
{
    public class JobViewModel : PropertyChangedBase
    {
        private BackupJob _job;
        private bool _isNew = true;
        private IEventAggregator _events;

        public JobViewModel()
            : this(new BackupJob(), true)
        {
        }

        public JobViewModel(BackupJob job)
            : this(job, false)
        {
        }

        public JobViewModel(BackupJob job, bool isNew)
        {
            _job = job;
            _isNew = isNew;
            _events = IoC.Get<IEventAggregator>();
        }

        public void Remove()
        {
            if(!_isNew) IoC.Get<IBackupManager>().RemoveJob(_job.Id);

            _events.Publish(new JobRemoved(_job.Id));
        }

        public bool CanRemove
        {
            get { return !Running; }
        }

        public void Edit()
        {
            var windowManager = new WindowManager();
            var dialogVM = IoC.Get<JobDialogViewModel>();
            dialogVM.Init(_job, _isNew);
            var result = windowManager.ShowDialog(dialogVM);

            if (result == true)
            {
                NotifyOfPropertyChange(() => Source);
                NotifyOfPropertyChange(() => Destination);
                NotifyOfPropertyChange(() => Protocol);
                NotifyOfPropertyChange(() => Running);
                NotifyOfPropertyChange(() => RunningSince);
            }
            else if (result == false && _isNew) 
            {
                Remove();
            }
        }

        public bool CanEdit
        {
            get { return !Running; }
        }

        public int JobId
        {
            get { return _job.Id; }
        }

        public string Source
        {
            get { return _job.Source; }
        }

        public string Destination
        {
            get { return _job.Destination; }
        }

        public string Protocol
        {
            get { return _job.Protocol; }
        }

        public bool Running
        {
            get { return _job.Running; }
        }

        public DateTime? RunningSince
        {
            get { return _job.RunningSince; }
        }
    }
}
