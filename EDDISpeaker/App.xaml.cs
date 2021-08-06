using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Windows;
using EDDISpeaker.Utils;
using EDDISpeaker.Views;
using Prism.Ioc;
using Prism.Unity;

namespace EDDISpeaker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        private Config _config;

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            _config = ConfigUtils.Load<Config>("general");
            containerRegistry.RegisterInstance(_config);
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ConfigUtils.Save(_config, "general");
            base.OnExit(e);
        }
    }
}
