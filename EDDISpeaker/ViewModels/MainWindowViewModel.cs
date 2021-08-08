using System;
using System.Collections.Generic;
using System.Text;
using Prism.Mvvm;

namespace EDDISpeaker.ViewModels
{
    class MainWindowViewModel: BindableBase
    {
        private readonly Config _config;

        public MainWindowViewModel(Config config)
        {
            _config = config;
        }

        public string AppName => AppInfo.AppName;
        public string Version => AppInfo.Version;
    }
}
