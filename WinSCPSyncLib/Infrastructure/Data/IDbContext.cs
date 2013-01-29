using System;
using System.Data.Entity;
using WinSCPSyncLib.Model;

namespace WinSCPSyncLib.Infrastructure.Data
{
    public interface IDbContext : IDisposable
    {
        DbSet<BackupJob> BackupJob { get; set; } 
    }
}
