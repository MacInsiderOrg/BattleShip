using System.Windows.Media.Imaging;
using BattleShip.Model.Cached;

namespace BattleShip.Model.Preferences.Design
{
    public class StandartDesign : IAppDesign
    {
        private readonly MediaFactory _mediaFactory;

        public StandartDesign()
        {
            _mediaFactory = MediaFactory.Init;
        }

        public BitmapImage PlayerPanelImage => _mediaFactory.GetImage("playerPanelBackground");

        public BitmapImage NotificationPanelImage => _mediaFactory.GetImage("notificationPanelBackground");

        public BitmapImage ShipboardImage => _mediaFactory.GetImage("board");

        public BitmapImage BackgroundImage => _mediaFactory.GetImage("setupBackground");
    }
}
