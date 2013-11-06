using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesignerLibrary.Services
{
    class ServiceProviderHelper
    {
        private IServiceProvider ServiceProvider { get; set; }

        public ServiceProviderHelper(IServiceProvider pServiceProvider)
        {
            ServiceProvider = pServiceProvider;
        }

        /// <summary>
        /// get service from ServiceProvider
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetService<T>()
            where T : class
        {
            return ServiceProvider.GetService( typeof( T ) ) as T;
        }
    }
}
