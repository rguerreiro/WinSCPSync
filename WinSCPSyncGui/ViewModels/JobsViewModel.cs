using Caliburn.Micro;
using System.Linq;
using WinSCPSyncGui.Messages;
using WinSCPSyncLib;

namespace WinSCPSyncGui.ViewModels
{
    public class JobsViewModel : TabViewModel, IHandle<JobRemoved>
    {
        private IBackupManager _backupManager;
        private IEventAggregator _events;
        private IObservableCollection<JobViewModel> _jobs;

        public JobsViewModel(IBackupManager backupManager, IEventAggregator events)
        {
            _backupManager = backupManager;
            
            _events = events;
            _events.Subscribe(this);

            DisplayName = "BACKUP JOBS";

            Load();
        }

        private void Load()
        {
            Jobs = new BindableCollection<JobViewModel>(_backupManager.AllJobs().Select(x => new JobViewModel(x)));
        }

        private void Refresh()
        {
            Jobs.Clear();
            Jobs.AddRange(_backupManager.AllJobs().Select(x => new JobViewModel(x)));
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