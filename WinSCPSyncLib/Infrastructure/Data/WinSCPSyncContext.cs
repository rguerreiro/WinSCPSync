using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using WinSCPSyncLib.Model;

namespace WinSCPSyncLib.Infrastructure.Data
{
    public class WinSCPSyncContext : DbContext, IDbContext
    {
        private ObjectContext _objectContext;

        public void SaveChangesFailed(Exception exception)
        {
        }

        public void Commit()
        {
            try
            {
                this.SaveChanges();
            }
            catch (Exception exception)
            {
                SaveChangesFailed(exception);
                throw;
            }
        }

        public void Detach(object entity)
        {
            ObjectContext.Detach(entity);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<WinSCPSyncContext>(new MigrateDatabaseToLatestVersion<WinSCPSyncContext, WinSCPSyncMigrationsConfiguration>());
        }

        public ObjectContext ObjectContext
        {
            get
            {
                if (_objectContext == null) _objectContext = ((IObjectContextAdapter)this).ObjectContext;
                return _objectContext;
            }
        }

        public DbSet<BackupJob> BackupJob { get; set; }
    }
}