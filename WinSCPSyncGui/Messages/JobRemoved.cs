
namespace WinSCPSyncGui.Messages
{
    public class JobRemoved
    {
        public JobRemoved(int jobId)
        {
            JobId = jobId;
        }

        public int JobId { get; set; }
    }
}
