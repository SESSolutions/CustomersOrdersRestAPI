using CustomersOrdersDataAccess;
using Ninject;
using Ninject.Extensions.ChildKernel;
using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;

namespace CustomersOrdersServices.Models
{
 
        public class NinjectResolver : System.Web.Http.Dependencies.IDependencyResolver
        {
            private IKernel kernel;

            public NinjectResolver() : this(new StandardKernel())
            {
            }

            public NinjectResolver(IKernel ninjectKernel, bool scope = false)
            {
                kernel = ninjectKernel;
                if (!scope)
                {
                    AddBindings(kernel);
                }
            }


            public IDependencyScope BeginScope()
            {
                return new NinjectResolver(AddRequestBindings(new ChildKernel(kernel)), true);
            }

            public object GetService(Type serviceType)
            {
                return kernel.TryGet(serviceType);
            }

            public IEnumerable<object> GetServices(Type serviceType)
            {
                return kernel.GetAll(serviceType);
            }

            public void Dispose()
            {

            }

            private void AddBindings(IKernel kernel)
            { }

            private IKernel AddRequestBindings(IKernel kernel)
            {
                kernel.Bind<IDataSourceEntities>().To<DataSourceEntities>().InSingletonScope();
                return kernel;
            }
        }   
}