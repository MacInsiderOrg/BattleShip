using System.Linq;
using System.Windows;
using BattleShip.ViewModel;

namespace BattleShip.View
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow
    {
        private readonly BattleShipWindow _battleShipWindow;
        public readonly GameViewModel GameViewModel;

        public static bool HumanAttackMade = false;
        public static bool GameStarted = false;

        public GameWindow(string playerName, string playerAvatar)
        {
            InitializeComponent();

            GameViewModel = new GameViewModel(playerName, playerAvatar,
                ShipBoard, HitBoard, HumanPlayerShips, ComputerPlayerShips,
                NotificationPanel);

            DataContext = GameViewModel;

            foreach (var window in Application.Current.Windows.OfType<BattleShipWindow>())
            {
                _battleShipWindow = window;
            }

            Closing += GameViewModel.OnWindowClosing;

            Switcher.CurrentFrame = _battleShipWindow?.GameWindowFrame;
        }

        private void GameWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            GameViewModel.IsReady = true;
            GameViewModel.GenerateDesign();
        }

        private void NewGame_OnClick(object sender, RoutedEventArgs e)
        {
            var battleShipWindow = new BattleShipWindow();
            battleShipWindow.Show();
            Switcher.Switch("NewGamePage");
            Close();
        }

        private void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            var battleShipWindow = new BattleShipWindow();
            battleShipWindow.Show();
            Switcher.Switch("OnePlayerGameSetup");
            Close();
        }
    }
}
