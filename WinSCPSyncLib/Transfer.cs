using System;
using System.IO;
using System.Threading;
using WinSCP;
using WinSCPSyncLib.Model;

namespace WinSCPSyncLib
{
    public class Transfer : IDisposable
    {
        private bool _disposed = false;

        public Transfer(BackupJob job)
        {
            Job = job;
            Running = false;
        }

        public void Init()
        {
            var path = Path.GetDirectoryName(Job.Source);
            Watcher = new FileSystemWatcher(path, "*");

            Watcher.IncludeSubdirectories = true;
            Watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size;

            FileSystemEventHandler onChanged = (sender, args) => Start();
            RenamedEventHandler onRenamed = (sender, args) => Start();
            ErrorEventHandler onError = (sender, args) =>
            {
                // TODO
            };

            Watcher.Changed += onChanged;
            Watcher.Created += onChanged;
            Watcher.Deleted += onChanged;
            Watcher.Renamed += onRenamed;

            Watcher.Error += onError;

            Watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            Watcher.EnableRaisingEvents = false;

            if (Running)
            {
                // TODO: stop the sync
            }
        }

        private void Start()
        {
            var options = new SessionOptions
            {
                Protocol = (Protocol)Enum.Parse(typeof(Protocol), Job.Protocol),
                HostName = Job.Host,
                UserName = Job.HostUsername,
                Password = Job.HostPassword
            };

            FileTransferredEventHandler onFileTransferred = (sender, args) =>
            {
                // TODO
            };

            WinSCPSession = new Session();
            WinSCPSession.FileTransferred += onFileTransferred;
            WinSCPSession.Open(options);

            Run();
        }

        private void Run()
        {
            ThreadPool.QueueUserWorkItem((state) =>
            {
                Running = true;
                
                var result = WinSCPSession.SynchronizeDirectories(
                    (SynchronizationMode)Enum.Parse(typeof(SynchronizationMode), Job.SyncMode),
                    Job.Source,
                    Job.Destination,
                    Job.RemoveFiles);

                Running = false;

                try
                {
                    result.Check(); // it will throw on any error
                }
                catch (Exception exception)
                {
                    // TODO
                }
            });
        }

        public void Dispose()
        {
            if (_disposed == true) return;

            if (WinSCPSession != null) WinSCPSession.Dispose();
            if (Watcher != null) Watcher.Dispose();

            _disposed = true;
            WinSCPSession = null;
            Watcher = null;
        }

        public Session WinSCPSession { get; private set; }
        public BackupJob Job { get; private set; }
        public FileSystemWatcher Watcher { get; private set; }
        public bool Running { get; private set; }
    }
}
