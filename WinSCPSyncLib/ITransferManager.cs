using System;
using WinSCPSyncLib.Model;

namespace WinSCPSyncLib
{
    public interface ITransferManager : IDisposable
    {
        void Listen(BackupJob job);
        void StopAll();
    }
}
