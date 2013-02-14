using System;
using System.ServiceProcess;

namespace WinSCPSyncGui.ViewModels
{
    public class ServiceViewModel : TabViewModel
    {
        private const string ServiceName = "WinSCPSyncSvc";
        private ServiceController _serviceController;

        public ServiceViewModel()
        {
            DisplayName = "SERVICE";

            _serviceController = new ServiceController(ServiceName, Environment.MachineName);
        }

        public void Start()
        {
            _serviceController.Start();
            _serviceController.WaitForStatus(ServiceControllerStatus.Running);
            NotifyOfPropertyChange(() => Status);
            NotifyOfPropertyChange(() => CanStart);
            NotifyOfPropertyChange(() => CanStop);
        }

        public bool CanStart 
        {
            get { return _serviceController.Status == ServiceControllerStatus.Stopped; }
        }

        public void Stop()
        {
            _serviceController.Stop();
            _serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
            NotifyOfPropertyChange(() => Status);
            NotifyOfPropertyChange(() => CanStart);
            NotifyOfPropertyChange(() => CanStop);
        }

        public bool CanStop
        {
            get { return _serviceController.CanStop; }
        }

        public string Status 
        {
            get { return _serviceController.Status.ToString(); }
        }
    }
}
