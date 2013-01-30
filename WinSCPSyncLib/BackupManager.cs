using System;
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

        public BackupJob GetJob(int jobId)
        {
            return _db.BackupJob.FirstOrDefault(x => x.Id == jobId);
        }

        public void MarkJobAsRunning(int jobId)
        {
            var job = GetJob(jobId);

            if (job == null) throw new ArgumentException("Job unknown");
            if (job.Running) throw new ApplicationException("Job already running");

            job.Running = true;
            job.RunningSince = DateTime.Now;

            _db.Commit();
        }

        public void JobHasStopped(int jobId)
        {
            var job = GetJob(jobId);

            if (job == null) throw new ArgumentException("Job unknown");

            if (job.Running)
            {
                job.Running = false;
                job.RunningSince = null;
                job.LastRun = DateTime.Now;

                _db.Commit();
            }
        }

        public void AddJob(BackupJob job)
        {
            if (job == null) throw new ArgumentNullException("job");

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
        }

        public void RemoveJob(int jobId)
        {
            var job = GetJob(jobId);

            if (job == null) throw new ArgumentException("Job unknown");
            if (job.Running) throw new ApplicationException("Job is running");

            _db.BackupJob.Remove(job);

            _db.Commit();
        }

        public void SaveJob(BackupJob job)
        {
            if (job == null) throw new ArgumentNullException("job");
            if (job.Id == 0) throw new ArgumentException("This job is probably a new one. Please consider adding it (call AddJob()) instead of saving it");

            _db.Detach(job);

            var jobAux = GetJob(job.Id);

            if (jobAux == null) throw new ArgumentException("Job unknown");
            if (jobAux.Running) throw new ApplicationException("Job is running");

            jobAux.Source = job.Source;
            jobAux.Destination = job.Destination;
            jobAux.Protocol = job.Protocol;
            job.Running = false;
            job.RunningSince = null;

            _db.Commit();
        }
    }
}
