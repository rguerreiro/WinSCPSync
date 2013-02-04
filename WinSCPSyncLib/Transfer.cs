using System;
using System.IO;
using System.Threading;
using WinSCP;
using WinSCPSyncLib.Infrastructure.DependencyResolution;
using WinSCPSyncLib.Model;

namespace WinSCPSyncLib
{
    public class Transfer : IDisposable
    {
        private bool _disposed = false;
        private bool _initialized = false;
        private Session _currSession = null;

        public Transfer(BackupJob job)
        {
            Job = job;
            Running = false;
        }

        /// <summary>
        /// Can only be called once
        /// </summary>
        public void Init()
        {
            if (_initialized) throw new ApplicationException("Transfer already initialized");

            Options = new SessionOptions
            {
                Protocol = (Protocol)Enum.Parse(typeof(Protocol), Job.Protocol),
                HostName = Job.Host,
                UserName = Job.HostUsername,
                Password = Job.HostPassword,
                SshHostKeyFingerprint = Job.HostKeyFingerprint
            };

            var path = Path.GetDirectoryName(Job.Source);
            Watcher = new FileSystemWatcher(path, "*");

            Console.WriteLine("Started listening in {0}", path);

            Watcher.IncludeSubdirectories = true;
            Watcher.NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastWrite;

            FileSystemEventHandler onChanged = (sender, args) => 
            {
                Console.WriteLine("Something {0}: {1}", args.ChangeType, args.FullPath);
                Start(); 
            };
            RenamedEventHandler onRenamed = (sender, args) =>
            {
                Console.WriteLine("Something was renamed: {0}", args.FullPath);
                Start();
            };
            ErrorEventHandler onError = (sender, args) =>
            {
                // TODO
                Console.WriteLine("Received an error: {0}", args.GetException().Message);
            };

            Watcher.Changed += onChanged;
            Watcher.Created += onChanged;
            Watcher.Deleted += onChanged;
            Watcher.Renamed += onRenamed;

            Watcher.Error += onError;

            Watcher.EnableRaisingEvents = true;

            _initialized = true;

            if (Job.Running)
            {
                Console.WriteLine("Job was previously marked as running. Probably service was abruptly closed.");
                Start();
            }
        }

        /// <summary>
        /// After calling this there's no way back. Must dispose.
        /// </summary>
        public void Stop()
        {
            Watcher.EnableRaisingEvents = false;

            if (Running && _currSession != null)
            {
                _currSession.Abort();
                _currSession.Dispose();
                _currSession = null;
            }
        }

        private void Start()
        {
            if (Running) return;

            MarkAsRunning();
            Run();
        }

        private void Run()
        {
            FileTransferredEventHandler onFileTransferred = (sender, args) =>
            {
                // TODO
                Console.WriteLine("Transferred {0} {1}...{2}", args.Destination, args.FileName, args.Error != null ? args.Error.Message : String.Empty);
            };

            ThreadPool.QueueUserWorkItem((state) =>
            {
                using (_currSession = new Session())
                {
                    try
                    {
                        _currSession.FileTransferred += onFileTransferred;
                        _currSession.Open(Options);

                        Console.WriteLine("Session was opened and going to sync directories...");

                        var result = _currSession.SynchronizeDirectories(
                            (SynchronizationMode)Enum.Parse(typeof(SynchronizationMode), Job.SyncMode),
                            Job.Source,
                            Job.Destination,
                            Job.RemoveFiles);

                        try
                        {
                            result.Check(); // it will throw on any error

                            // only marking as stopped when everything went oko. 
                            // this way it will run again whenever the service restarts or a change happens on this same directory
                            MarkAsStopped();
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine("Some errors occured in the sync: {0}", exception.GetBaseException().Message);
                        }
                    }
                    catch (SessionLocalException sle)
                    {
                        // Probably aborted
                        Console.WriteLine("Something happened locally while syncing directories: {0}", sle.GetBaseException().Message);
                    }
                    catch (SessionRemoteException sre)
                    {
                        // Connection went down?
                        Console.WriteLine("Something happened remotelly while syncing directories: {0}", sre.GetBaseException().Message);
                    }
                    catch (SessionException sessionException)
                    {
                        Console.WriteLine("Unexpected session exception while syncing directories: {0}", sessionException.GetBaseException().Message);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine("Unexpected exception while syncing directories: {0}", exception.GetBaseException().Message);
                    }

                    Console.WriteLine("Session ended.");
                }

                _currSession = null;
            });
        }

        private void MarkAsRunning()
        {
            Running = true;

            var backupManager = IoC.Resolve<IBackupManager>();
            backupManager.MarkJobAsRunning(Job.Id);
        }

        private void MarkAsStopped()
        {
            Running = false;

            var backupManager = IoC.Resolve<IBackupManager>();
            backupManager.JobHasStopped(Job.Id);
        }

        public void Dispose()
        {
            if (_disposed == true) return;

            if (Watcher != null) Watcher.Dispose();

            _disposed = true;
            Watcher = null;
        }

        public SessionOptions Options { get; private set; }
        public BackupJob Job { get; private set; }
        public FileSystemWatcher Watcher { get; private set; }
        public bool Running { get; private set; }
    }
}
