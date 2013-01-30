using System.Collections.Generic;
using WinSCPSyncLib.Model;

namespace WinSCPSyncLib
{
    public interface IBackupManager
    {
        IEnumerable<BackupJob> AllJobs();
        IEnumerable<BackupJob> AllRunningJobs();
        BackupJob GetJob(int jobId);
        void MarkJobAsRunning(int jobId);
        void JobHasStopped(int jobId);
        void AddJob(BackupJob job);
        void RemoveJob(int jobId);
        void SaveJob(BackupJob job);
    }
}
