using BattleShip.ViewModel;

namespace BattleShip.View
{
    /// <summary>
    /// Interaction logic for BattleShipWindow.xaml
    /// </summary>
    public partial class BattleShipWindow
    {
        public BattleShipWindow()
        {
            InitializeComponent();
            GameWindow.GameStarted = false;
            Switcher.CurrentFrame = GameWindowFrame;
        }
    }
}
