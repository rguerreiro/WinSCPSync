using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using WinSCPSyncLib.Infrastructure.Data;
using WinSCPSyncLib.Model;

namespace WinSCPSyncLib
{
    public class BackupManager : IBackupManager
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(BackupManager));
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

        public BackupJob GetJob(int jobId)
        {
            return _db.BackupJob.FirstOrDefault(x => x.Id == jobId);
        }

        public void MarkJobAsRunning(int jobId)
        {
            _log.DebugFormat("Marking job #{0} as running in the db", jobId);

            var job = GetJob(jobId);

            if (job == null)
                throw new ArgumentException("Job unknown");

            if (job.Running)
            {
                _log.DebugFormat("Job #{0} was already running in the db", jobId);
                return; // not throwing an exception because some point in time the job is needed to be running and we already got that
            }

            job.Running = true;
            job.RunningSince = DateTime.Now;

            _db.Commit();

            _log.DebugFormat("Job #{0} was marked as running in the db", jobId);
        }

        public void JobHasStopped(int jobId)
        {
            _log.DebugFormat("Marking job #{0} as stopped in the db", jobId);

            var job = GetJob(jobId);

            if (job == null) throw new ArgumentException("Job unknown");

            if (!job.Running)
            {
                _log.DebugFormat("Job #{0} was already stopped in the db", jobId);
                return;
            }

            job.Running = false;
            job.RunningSince = null;
            job.LastRun = DateTime.Now;

            _db.Commit();

            _log.DebugFormat("Job #{0} was marked as stopped in the db", jobId);
        }

        public void AddJob(BackupJob job)
        {
            if (job == null) throw new ArgumentNullException("job");

            _log.InfoFormat("Adding new job to db for folder {0}", job.Source);

            var countSimilar = _db.BackupJob.Where(x =>
                x.Source == job.Source
                && x.Destination == job.Destination
                && x.Protocol == job.Protocol
            ).Count();

            if (countSimilar > 0) throw new ApplicationException("Already exists a similar job");

            job.Running = false;
            job.RunningSince = null;
            job.LastRun = null;

            _db.BackupJob.Add(job);

            _db.Commit();

            _log.DebugFormat("Job was added to db for folder {0}", job.Source);
        }

        public void RemoveJob(int jobId)
        {
            _log.InfoFormat("Removing job #{0} from db", jobId);

            var job = GetJob(jobId);

            if (job == null) throw new ArgumentException("Job unknown");
            if (job.Running) throw new ApplicationException("Job is running");

            _db.BackupJob.Remove(job);

            _db.Commit();

            _log.DebugFormat("Job #{0} was removed from db", jobId);
        }

        public void SaveJob(BackupJob job)
        {
            if (job == null) throw new ArgumentNullException("job");
            if (job.Id == 0) throw new ArgumentException("This job is probably a new one. Please consider adding it (call AddJob()) instead of saving it");

            _log.DebugFormat("Saving job #{0} to db", job.Id);

            var jobAux = GetJob(job.Id);

            if (jobAux == null) throw new ArgumentException("Job unknown");
            if (jobAux.Running) throw new ApplicationException("Job is running");

            jobAux.Source = job.Source;
            jobAux.Destination = job.Destination;
            jobAux.Protocol = job.Protocol;
            jobAux.Host = job.Host;
            jobAux.HostKeyFingerprint = job.HostKeyFingerprint;
            jobAux.HostPassword = job.HostPassword;
            jobAux.HostUsername = job.HostUsername;
            jobAux.RemoveFiles = job.RemoveFiles;
            jobAux.SyncMode = job.SyncMode;

            _db.Commit();

            _log.DebugFormat("Job #{0} was saved to db", job.Id);
        }
    }
}
