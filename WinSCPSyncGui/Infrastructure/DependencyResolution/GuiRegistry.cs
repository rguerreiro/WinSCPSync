using Caliburn.Micro;
using StructureMap.Configuration.DSL;
using WinSCPSyncGui.ViewModels;

namespace WinSCPSyncGui.Infrastructure.DependencyResolution
{
    public class GuiRegistry : Registry
    {
        public GuiRegistry()
        {
            For<IWindowManager>().Singleton().Use<WindowManager>();
            For<IEventAggregator>().Singleton().Use<EventAggregator>();

            // views
            For<ShellViewModel>().Use<ShellViewModel>();
            For<JobsViewModel>().Use<JobsViewModel>();
            For<JobViewModel>().Use<JobViewModel>();
            For<JobDialogViewModel>().Use<JobDialogViewModel>();
            For<ServiceViewModel>().Use<ServiceViewModel>();
        }
    }
}
