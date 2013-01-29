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
            var jobs = backupManager.AllJobs();

            foreach (var job in jobs)
            {
                Console.WriteLine("{0} -> {1}", job.Source, job.Destination);
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
