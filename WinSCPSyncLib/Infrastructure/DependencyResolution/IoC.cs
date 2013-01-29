using StructureMap;
using System;
using System.Threading.Tasks;
using WinSCPSyncLib.Infrastructure.Startup;

namespace WinSCPSyncLib.Infrastructure.DependencyResolution
{
    public class IoC
    {
        public static IContainer Init()
        {
            ObjectFactory.Initialize(x => 
            {
                x.Scan(scan =>
                {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();
                    scan.LookForRegistries();
                    scan.AddAllTypesOf<IStartupTask>();
                });
            });

            var startupTasks = ObjectFactory.GetAllInstances<IStartupTask>();
            Parallel.ForEach(startupTasks, currTask => currTask.Execute());

            return ObjectFactory.Container;
        }

        public static object Resolve(Type type)
        {
            return ObjectFactory.TryGetInstance(type);
        }

        public static T Resolve<T>()
            where T : class
        {
            return ObjectFactory.TryGetInstance<T>();
        }
    }
}
