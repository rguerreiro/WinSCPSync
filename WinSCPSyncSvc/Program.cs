using Topshelf;
using WinSCPSyncLib.Infrastructure.DependencyResolution;

namespace WinSCPSyncSvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Dependency injection initialization
            IoC.Init();

            HostFactory.Run(x =>
            {
                x.UseLog4Net("log4net.config");

                x.Service<SyncServiceControl>();

                x.RunAsLocalSystem();

                x.StartAutomatically();

                x.SetDescription("Service to sync directories between local computer and a remote FTP/SFTP host");
                x.SetDisplayName("WinSCP Sync Service");
                x.SetServiceName("WinSCPSyncSvc");
            });
        }
    }
}
