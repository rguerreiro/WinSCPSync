using System;
using System.IO;

namespace WinSCPSyncLib.Infrastructure.Configuration
{
    public class ConfigOptions : IConfigOptions
    {
        public string CommonApplicationData 
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData, Environment.SpecialFolderOption.Create); }
        }

        public string DbPath
        {
            get { return Path.Combine(CommonApplicationData, "WinSCPSync"); }
        }

        public string DbFileName 
        {
            get { return "jobs.db"; }
        }

        public string DbFullPath
        {
            get { return Path.Combine(DbPath, DbFileName); }
        }
    }
}
