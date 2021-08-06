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
using Microsoft.WindowsAPICodePack.Dialogs;
using Prism.Ioc;
using Prism.Unity;
using Injector;

namespace EDDISpeaker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        private Config _config;

        public App() {
            _config = ConfigUtils.Load<Config>("general");

            if (_config.VoiceroidSettings.InstallationPath == null) {
                MessageBox.Show("Voiceroid2のインストール先を設定してください。");
                using var dialog = new CommonOpenFileDialog() { 
                    Title = "フォルダを選択してください",
                    IsFolderPicker = true
                };

                dialog.ShowDialog();
                _config.VoiceroidSettings.InstallationPath = dialog.FileName;
            }

            if (_config.VoiceroidSettings.AuthenticationCode == null) {
                MessageBox.Show("Voiceroid2の認証トークンが設定されていません。\n一度Voiceroid2を起動してからOKを押してください。");
                var code = Injector.Injector.GetKey("VoiceroidEditor");
                if (code != null) {
                    MessageBox.Show($"認証コード{code}を取得しました。\nVoiceroid2を終了してOKを押してください。");
                }
                _config.VoiceroidSettings.AuthenticationCode = code;
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
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
