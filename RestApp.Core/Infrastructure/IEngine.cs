using System;
using RestApp.Core.Configuration;
using RestApp.Core.Infrastructure.DependencyManagement;

namespace RestApp.Core.Infrastructure
{
    /// <summary>
    /// Classes implementing this interface can serve as a portal for the 
    /// various services composing the RestApp engine. Edit functionality, modules
    /// and implementations access most RestApp functionality through this 
    /// interface.
    /// </summary>
    public interface IEngine
    {
        ContainerManager ContainerManager { get; }
        
        /// <summary>
        /// Initialize components and plugins in the RestApp environment.
        /// </summary>
        /// <param name="config">Config</param>
        //void Initialize(RestAppConfig config);

        T Resolve<T>() where T : class;

        object Resolve(Type type);

        T[] ResolveAll<T>();
    }
}
