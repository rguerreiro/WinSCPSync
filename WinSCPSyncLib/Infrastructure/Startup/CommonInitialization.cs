using System;
using WinSCPSyncLib.Infrastructure.Configuration;
using WinSCPSyncLib.Infrastructure.DependencyResolution;

namespace WinSCPSyncLib.Infrastructure.Startup
{
    public class CommonInitialization : IStartupTask
    {
        private IConfigOptions _configOptions;

        public CommonInitialization(IConfigOptions configOptions)
        {
            _configOptions = configOptions;
        }

        public void Execute()
        {
            // setting connection string variable DataDirectory
            AppDomain.CurrentDomain.SetData("DataDirectory", _configOptions.DbPath);
        }
    }
}
