using Caliburn.Micro;
using System.Linq;
using WinSCPSyncGui.Messages;
using WinSCPSyncLib;

namespace WinSCPSyncGui.ViewModels
{
    public class JobsViewModel : TabViewModel, IHandle<JobRemoved>
    {
        private IEventAggregator _events;
        private IObservableCollection<JobViewModel> _jobs;

        public JobsViewModel(IEventAggregator events)
        {
            _events = events;
            _events.Subscribe(this);

            DisplayName = "BACKUP JOBS";

            Load();
        }

        private void Load()
        {
            var backupManager = IoC.Get<IBackupManager>();
            Jobs = new BindableCollection<JobViewModel>(backupManager.AllJobs().Select(x => new JobViewModel(x)));
        }

        public void RefreshJobs()
        {
            var backupManager = IoC.Get<IBackupManager>();
            Jobs.Clear();
            Jobs.AddRange(backupManager.AllJobs().Select(x => new JobViewModel(x)));
        }

        public void AddNew()
        {
            var job = new JobViewModel();
            Jobs.Add(job);
            job.Edit();
        }

        public IObservableCollection<JobViewModel> Jobs 
        {
            get { return _jobs; }
            set 
            {
                _jobs = value;
                NotifyOfPropertyChange(() => Jobs);
            }
        }

        public void Handle(JobRemoved message)
        {
            var job = Jobs.FirstOrDefault(x => x.JobId == message.JobId);
            Jobs.Remove(job);
        }
    }
}