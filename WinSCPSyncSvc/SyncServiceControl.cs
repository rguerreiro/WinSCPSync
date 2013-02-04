using Topshelf;
using WinSCPSyncLib;
using WinSCPSyncLib.Infrastructure.DependencyResolution;

namespace WinSCPSyncSvc
{
    public class SyncServiceControl : ServiceControl
    {
        private ITransferManager _transferManager;
        
        public SyncServiceControl()
        {
            _transferManager = IoC.Resolve<ITransferManager>();
        }

        public bool Start(HostControl hostControl)
        {
            var jobs = IoC.Resolve<IBackupManager>().AllJobs();

            foreach (var job in jobs)
            {
                _transferManager.Listen(job);
            }

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            _transferManager.StopAll();

            return true;
        }
    }
}
