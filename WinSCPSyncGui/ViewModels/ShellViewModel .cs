using Caliburn.Micro;

namespace WinSCPSyncGui.ViewModels
{
    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive
    {
        private JobsViewModel _jobsViewModel;
        private ServiceViewModel _serviceViewModel;

        public ShellViewModel()
        {
            _jobsViewModel = IoC.Get<JobsViewModel>();
            _serviceViewModel = IoC.Get<ServiceViewModel>();

            Items.Add(_jobsViewModel);
            Items.Add(_serviceViewModel);
        }
    }
}
