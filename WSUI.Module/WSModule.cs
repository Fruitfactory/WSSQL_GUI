using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Microsoft.Practices.ObjectBuilder2;

namespace WSUI.Module
{
    public class WSModule : IModule
    {
        private readonly IRegionManager _regionManager;
        private readonly IUnityContainer _unityContainer;

        public WSModule(IUnityContainer container, IRegionManager region)
        {
            _regionManager = region;
            _unityContainer = container;
        }

        public void Initialize()
        {
            RegistreInterfaces();
            //TODO add view to regions
        }

        private void RegistreInterfaces()
        {
            //TODO add interfaces and classes

        }

    }
}
