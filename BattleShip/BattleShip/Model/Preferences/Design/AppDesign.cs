using System.Windows.Media.Imaging;

namespace BattleShip.Model.Preferences.Design
{
    public enum AppDesign
    {
        Standart,
        Ultimate
    }

    public interface IAppDesign
    {
        BitmapImage PlayerPanelImage { get; }
        BitmapImage NotificationPanelImage { get; }
        BitmapImage ShipboardImage { get; }
        BitmapImage BackgroundImage { get; }
    }
}
