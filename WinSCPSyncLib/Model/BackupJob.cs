using System;

namespace WinSCPSyncLib.Model
{
    public class BackupJob
    {
        public int Id { get; set; }
        public string Source { get; set; }
        public string SourceProtocol { get; set; }
        public string Destination { get; set; }
        public string DestinationProtocol { get; set; }
        public bool Running { get; set; }
        public DateTime? RunningSince { get; set; }
        public DateTime? LastRun { get; set; }
    }
}
