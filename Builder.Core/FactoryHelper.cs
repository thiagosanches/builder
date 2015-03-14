using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity.Configuration;

namespace Builder.Core
{
    internal static class FactoryHelper
    {
        public static T GetInstance<T>(string name)
        {
            var unityContainer = new UnityContainer();
            unityContainer.LoadConfiguration();
            return unityContainer.Resolve<T>(name);
        }
    }
}
