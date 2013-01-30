using System;
using System.Data.Entity;
using WinSCPSyncLib.Model;

namespace WinSCPSyncLib.Infrastructure.Data
{
    public interface IDbContext : IDisposable
    {
        void Commit();
        void Detach(object entity);

        DbSet<BackupJob> BackupJob { get; set; } 
    }
}
