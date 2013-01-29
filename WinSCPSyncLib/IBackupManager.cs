using System.Collections.Generic;
using WinSCPSyncLib.Model;

namespace WinSCPSyncLib
{
    public interface IBackupManager
    {
        IEnumerable<BackupJob> AllJobs();
        IEnumerable<BackupJob> AllRunningJobs();
    }
}
