using System;
using System.Collections.Generic;
using WinSCPSyncLib.Model;

namespace WinSCPSyncLib
{
    public class TransferManager : ITransferManager
    {
        private Dictionary<int, Transfer> _transfers = new Dictionary<int, Transfer>();

        public void Listen(BackupJob job)
        {
            if (_transfers.ContainsKey(job.Id)) throw new ApplicationException("Transfer already configured");

            var transfer = new Transfer(job);
            transfer.Init();

            _transfers.Add(job.Id, transfer);
        }
    }
}
