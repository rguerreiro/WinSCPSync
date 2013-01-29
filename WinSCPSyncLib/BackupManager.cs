using System.Collections.Generic;
using System.Linq;
using WinSCPSyncLib.Infrastructure.Data;
using WinSCPSyncLib.Model;

namespace WinSCPSyncLib
{
    public class BackupManager : IBackupManager
    {
        private IDbContext _db;

        public BackupManager(IDbContext db)
        {
            _db = db;
        }

        public IEnumerable<BackupJob> AllJobs()
        {
            return _db.BackupJob;
        }

        public IEnumerable<BackupJob> AllRunningJobs()
        {
            return _db.BackupJob.Where(x => x.Running == true);
        }
    }
}
