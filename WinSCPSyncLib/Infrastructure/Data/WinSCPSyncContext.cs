using System.Data.Entity;
using WinSCPSyncLib.Model;

namespace WinSCPSyncLib.Infrastructure.Data
{
    public class WinSCPSyncContext : DbContext, IDbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<WinSCPSyncContext>(new MigrateDatabaseToLatestVersion<WinSCPSyncContext, WinSCPSyncMigrationsConfiguration>());
        }

        public DbSet<BackupJob> BackupJob { get; set; }
    }
}