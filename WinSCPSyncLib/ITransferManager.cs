using WinSCPSyncLib.Model;

namespace WinSCPSyncLib
{
    public interface ITransferManager
    {
        void Listen(BackupJob job);
    }
}
