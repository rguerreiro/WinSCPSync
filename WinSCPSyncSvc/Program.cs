using System;
using WinSCPSyncLib;
using WinSCPSyncLib.Infrastructure.DependencyResolution;

namespace WinSCPSyncSvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Dependency injection
            IoC.Init();

            var backupManager = IoC.Resolve<IBackupManager>();
            var transferManager = IoC.Resolve<ITransferManager>();
            var jobs = backupManager.AllJobs();

            foreach (var job in jobs)
            {
                transferManager.Listen(job);
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            transferManager.StopAll();

            Console.WriteLine("Exited!");
        }
    }
}
