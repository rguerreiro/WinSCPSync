using System.Data.Entity.Migrations;

namespace WinSCPSyncLib.Infrastructure.Data
{
    public class WinSCPSyncMigrationsConfiguration : DbMigrationsConfiguration<WinSCPSyncContext>
    {
        public WinSCPSyncMigrationsConfiguration()
        {
            AutomaticMigrationDataLossAllowed = true;
            AutomaticMigrationsEnabled = true;
        }
    }
}
