using System;

namespace WinSCPSyncLib.Model
{
    public class BackupJob
    {
        public int Id { get; set; }
        public string Source { get; set; }
        public string Host { get; set; }
        public string HostUsername { get; set; }
        public string HostPassword { get; set; }
        public string Destination { get; set; }
        public string Protocol { get; set; }
        public string SyncMode { get; set; }
        public bool RemoveFiles { get; set; }
        public bool Running { get; set; }
        public DateTime? RunningSince { get; set; }
        public DateTime? LastRun { get; set; }
    }
}
