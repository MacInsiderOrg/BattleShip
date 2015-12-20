using System.Windows.Media.Imaging;
using BattleShip.Model.Cached;

namespace BattleShip.Model.Preferences.Design
{
    public class UltimateDesign : IAppDesign
    {
        private readonly MediaFactory _mediaFactory;

        public UltimateDesign()
        {
            _mediaFactory = MediaFactory.Init;
        }

        public BitmapImage PlayerPanelImage => _mediaFactory.GetImage("ultimatePlayerPanelBackground");

        public BitmapImage NotificationPanelImage => _mediaFactory.GetImage("ultimateNotificationPanelBackground");
        
        public BitmapImage ShipboardImage => _mediaFactory.GetImage("boardUltimate");
        
        public BitmapImage BackgroundImage => _mediaFactory.GetImage("setupBackground");
    }
}
