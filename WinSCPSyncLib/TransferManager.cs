using log4net;
using System;
using System.Collections.Generic;
using WinSCPSyncLib.Model;

namespace WinSCPSyncLib
{
    public class TransferManager : ITransferManager
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(TransferManager));
        private Dictionary<int, Transfer> _transfers = new Dictionary<int, Transfer>();
        private bool _disposed = false;
        
        public void Listen(BackupJob job)
        {
            if (job == null || job.Id == 0) throw new ApplicationException("Invalid backup job");
            if (_transfers.ContainsKey(job.Id)) throw new ApplicationException("Transfer already configured");

            _log.InfoFormat("Adding job #{0} to transfers list", job.Id);

            var transfer = new Transfer(job);
            transfer.Init();

            _transfers.Add(job.Id, transfer);

            _log.InfoFormat("Job #{0} was added to transfers list", job.Id);
        }

        public void StopAll()
        {
            _log.Info("Stopping all transfers...");

            foreach (var transfer in _transfers.Values)
            {
                transfer.Stop();
            }

            _log.Info("All transfers were stopped");
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            foreach (var transfer in _transfers.Values)
            {
                transfer.Dispose();
            }

            _transfers.Clear();

            _disposed = true;

            _log.Info("Transfer Manager disposed");
        }
    }
}
