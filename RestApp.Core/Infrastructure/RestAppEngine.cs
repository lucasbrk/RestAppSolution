using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Autofac;
using RestApp.Core.Configuration;
using RestApp.Core.Data;
using RestApp.Core.Infrastructure.DependencyManagement;

namespace RestApp.Core.Infrastructure
{
    public class RestAppEngine : IEngine
    {
        #region Fields

        private ContainerManager _containerManager;

        #endregion

        #region Ctor

        /// <summary>
		/// Creates an instance of the content engine using default settings and configuration.
		/// </summary>
		public RestAppEngine() 
            : this(EventBroker.Instance, new ContainerConfigurer())
		{
		}

		public RestAppEngine(EventBroker broker, ContainerConfigurer configurer)
		{
            var config = ConfigurationManager.GetSection("RestAppConfig") as RestAppConfig;
            InitializeContainer(configurer, broker, config);
		}
        
        #endregion

        #region Utilities
       
        private void InitializeContainer(ContainerConfigurer configurer, EventBroker broker, RestAppConfig config)
        {
            var builder = new ContainerBuilder();

            _containerManager = new ContainerManager(builder.Build());
            configurer.Configure(this, _containerManager, broker, config);
        }

        #endregion

        #region Methods              

        public T Resolve<T>() where T : class
		{
            return ContainerManager.Resolve<T>();
		}

        public object Resolve(Type type)
        {
            return ContainerManager.Resolve(type);
        }
        
        public T[] ResolveAll<T>()
        {
            return ContainerManager.ResolveAll<T>();
        }

		#endregion

        #region Properties

        public IContainer Container
        {
            get { return _containerManager.Container; }
        }

        public ContainerManager ContainerManager
        {
            get { return _containerManager; }
        }

        #endregion
    }
}
