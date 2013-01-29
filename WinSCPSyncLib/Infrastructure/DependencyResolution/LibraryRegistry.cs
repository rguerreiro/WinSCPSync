using StructureMap.Configuration.DSL;
using WinSCPSyncLib.Infrastructure.Configuration;
using WinSCPSyncLib.Infrastructure.Data;

namespace WinSCPSyncLib.Infrastructure.DependencyResolution
{
    public class LibraryRegistry : Registry
    {
        public LibraryRegistry()
        {
            For<IConfigOptions>().Singleton().Use<ConfigOptions>();
            For<IDbContext>().Use<WinSCPSyncContext>();
            For<IBackupManager>().Use<BackupManager>();
        }
    }
}
