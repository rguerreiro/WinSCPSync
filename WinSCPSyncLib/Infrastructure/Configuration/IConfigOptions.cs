namespace WinSCPSyncLib.Infrastructure.Configuration
{
    public interface IConfigOptions
    {
        string DbPath { get; }
        string DbFileName { get; }
        string DbFullPath { get; }
    }
}
