using System;
using System.Collections.Generic;
using WinSCPSyncLib.Model;

namespace WinSCPSyncLib
{
    public class TransferManager : ITransferManager, IDisposable
    {
        private Dictionary<int, Transfer> _transfers = new Dictionary<int, Transfer>();
        private bool _disposed = false;
        
        public void Listen(BackupJob job)
        {
            if (_transfers.ContainsKey(job.Id)) throw new ApplicationException("Transfer already configured");

            var transfer = new Transfer(job);
            transfer.Init();

            _transfers.Add(job.Id, transfer);
        }

        public void StopAll()
        {
            Console.WriteLine("Stopping all transfers...");

            foreach (var transfer in _transfers.Values)
            {
                transfer.Stop();
            }

            Console.WriteLine("All transfers were stopped");
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

            Console.WriteLine("Transfer Manager disposed");
        }
    }
}
