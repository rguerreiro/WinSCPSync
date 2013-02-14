using Caliburn.Micro;
using StructureMap;
using WinSCPSyncGui.ViewModels;

namespace WinSCPSyncGui
{
    public class Bootstrapper : Bootstrapper<ShellViewModel>
    {
        private IContainer _container;

        protected override void Configure()
        {
            _container = WinSCPSyncLib.Infrastructure.DependencyResolution.IoC.Init();
        }

        protected override object GetInstance(System.Type serviceType, string key)
        {
            return string.IsNullOrEmpty(key) ? _container.GetInstance(serviceType) : _container.GetInstance(serviceType ?? typeof(object), key);
        }

        protected override System.Collections.Generic.IEnumerable<object> GetAllInstances(System.Type serviceType)
        {
            foreach (var obj in _container.GetAllInstances(serviceType))
            {
                yield return obj;
            }
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }
    }
}
