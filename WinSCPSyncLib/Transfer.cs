using log4net;
using System;
using System.IO;
using System.Threading;
using WinSCP;
using WinSCPSyncLib.Infrastructure.DependencyResolution;
using WinSCPSyncLib.Infrastructure.Security;
using WinSCPSyncLib.Model;

namespace WinSCPSyncLib
{
    public class Transfer : IDisposable
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(Transfer));
        private static readonly object _syncObject = new object();
        private bool _disposed = false;
        private bool _initialized = false;
        private Session _currSession = null;

        public Transfer(BackupJob job)
        {
            Job = job;
            Running = false;

            TransferSource = Path.GetDirectoryName(job.Source);
        }

        /// <summary>
        /// Can only be called once
        /// </summary>
        public void Init()
        {
            if (_initialized) throw new ApplicationException("Transfer already initialized");

            _log.InfoFormat("Initializing transfer for job #{0} in path {1}", Job.Id, TransferSource);

            Options = new SessionOptions
            {
                Protocol = (Protocol)Enum.Parse(typeof(Protocol), Job.Protocol),
                HostName = Job.Host,
                UserName = Job.HostUsername,
                Password = Cryptor.Decrypt(Job.HostPassword),
                SshHostKeyFingerprint = Job.HostKeyFingerprint
            };

            Watcher = new FileSystemWatcher(TransferSource, "*");
            Watcher.IncludeSubdirectories = true;
            Watcher.NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastWrite;

            FileSystemEventHandler onChanged = (sender, args) =>
            {
                _log.DebugFormat("Something {0} at {1}", args.ChangeType, args.FullPath);
                Start();
            };
            RenamedEventHandler onRenamed = (sender, args) =>
            {
                _log.DebugFormat("Something was renamed at {0}", args.FullPath);
                Start();
            };
            ErrorEventHandler onError = (sender, args) =>
            {
                _log.Error("An error occured while transfering", args.GetException());
            };

            Watcher.Changed += onChanged;
            Watcher.Created += onChanged;
            Watcher.Deleted += onChanged;
            Watcher.Renamed += onRenamed;

            Watcher.Error += onError;

            Watcher.EnableRaisingEvents = true;

            _log.InfoFormat("Started listening in {0}", TransferSource);

            _initialized = true;

            if (Job.Running)
            {
                _log.InfoFormat("Job #{0} was previously marked as running. Probably service was abruptly closed. Starting the sync...", Job.Id);
                Start();
            }
        }

        /// <summary>
        /// After calling this there's no way back. Must dispose.
        /// </summary>
        public void Stop()
        {
            if (!_initialized) throw new ApplicationException("Transfer wasn't initialized. Please call Init()");

            Watcher.EnableRaisingEvents = false;

            _log.InfoFormat("Stopped listening at {0}", TransferSource);

            if (IsTransferRunning() && _currSession != null)
            {
                _log.InfoFormat("Stopping current transfer at {0}...", TransferSource);

                _currSession.Abort();
                _currSession.Dispose();
                _currSession = null;

                _log.InfoFormat("Current transfer was stopped at {0}", TransferSource);
            }
        }

        private void Start()
        {
            if (IsTransferRunning()) return;

            _log.InfoFormat("Starting transfer at {0}...", TransferSource);

            MarkAsRunning();
            Run();
        }

        private void Run()
        {
            FileTransferredEventHandler onFileTransferred = (sender, args) =>
            {
                _log.DebugFormat("Transferred {0} to {1}", args.FileName, args.Destination);

                if (args.Error != null)
                    _log.Error("Error while transferring file " + args.FileName, args.Error);
            };

            ThreadPool.QueueUserWorkItem((state) =>
            {
                using (_currSession = new Session())
                {
                    try
                    {
                        _currSession.FileTransferred += onFileTransferred;
                        _currSession.Open(Options);

                        _log.InfoFormat("Session was opened and going to sync directories from {0}...", TransferSource);

                        var result = _currSession.SynchronizeDirectories(
                            (SynchronizationMode)Enum.Parse(typeof(SynchronizationMode), Job.SyncMode),
                            Job.Source,
                            Job.Destination,
                            Job.RemoveFiles);

                        try
                        {
                            result.Check(); // it will throw on any error

                            // only marking as stopped when everything went ok. 
                            // this way it will run again whenever the service restarts or a change happens on this same directory
                            MarkAsStopped();

                            _log.InfoFormat("{0} was synced with success", TransferSource);
                        }
                        catch (Exception exception)
                        {
                            _log.Error("Some errors occured while syncing " + TransferSource, exception);
                        }
                    }
                    catch (SessionLocalException sle)
                    {
                        // Probably aborted
                        _log.Error("Something happened locally while syncing " + TransferSource, sle);
                        ErrorHappened(sle.GetBaseException().Message);
                    }
                    catch (SessionRemoteException sre)
                    {
                        // Connection went down or refused
                        _log.Error("Something happened remotely while syncing " + TransferSource, sre);
                        ErrorHappened(sre.GetBaseException().Message);
                    }
                    catch (SessionException sessionException)
                    {
                        _log.Error("Unexpected session exception while syncing " + TransferSource, sessionException);
                        ErrorHappened(sessionException.GetBaseException().Message);
                    }
                    catch (Exception exception)
                    {
                        _log.Error("Unexpected exception while syncing " + TransferSource, exception);
                        ErrorHappened(exception.GetBaseException().Message);
                    }

                    _log.InfoFormat("Session for {0} ended", TransferSource);
                }

                _currSession = null;
            });
        }

        private void MarkAsRunning()
        {
            _log.DebugFormat("Marking transfer at {0} as running", TransferSource);

            lock (_syncObject)
            {
                Running = true;

                var backupManager = IoC.Resolve<IBackupManager>();
                backupManager.MarkJobAsRunning(Job.Id);
            }
        }

        private void MarkAsStopped()
        {
            _log.DebugFormat("Marking transfer at {0} as stopped", TransferSource);

            lock (_syncObject)
            {
                Running = false;

                var backupManager = IoC.Resolve<IBackupManager>();
                backupManager.JobHasStopped(Job.Id);
            }
        }

        private void ErrorHappened(string message)
        {
            var backupManager = IoC.Resolve<IBackupManager>();
            backupManager.MarkJobWithError(Job.Id, message);
        }

        /// <summary>
        /// Thread-safe check to see if transfer is already running
        /// </summary>
        /// <returns></returns>
        public bool IsTransferRunning()
        {
            lock (_syncObject)
            {
                return Running;
            }
        }

        public void Dispose()
        {
            if (_disposed == true) return;

            if (Watcher != null) Watcher.Dispose();

            _disposed = true;
            Watcher = null;

            _log.DebugFormat("Transfer for job #{0} was disposed", Job.Id);
        }

        public SessionOptions Options { get; private set; }
        public string TransferSource { get; private set; }
        public BackupJob Job { get; private set; }
        public FileSystemWatcher Watcher { get; private set; }
        public bool Running { get; private set; }
    }
}