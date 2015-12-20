using System.Windows;
using BattleShip.Model.Preferences;
using BattleShip.Properties;

namespace BattleShip
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AppSettings.BackgroundMusic();
        }
    }
}
