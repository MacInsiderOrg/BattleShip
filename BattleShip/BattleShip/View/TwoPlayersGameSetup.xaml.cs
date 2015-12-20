using System.Linq;
using System.Windows;
using BattleShip.Properties;
using BattleShip.ViewModel;

namespace BattleShip.View
{
    /// <summary>
    /// Interaction logic for TwoPlayersGameSetup.xaml
    /// </summary>
    public partial class TwoPlayersGameSetup
    {
        private readonly BattleShipWindow _battleShipWindow;
        private readonly TwoPlayersGameSetupViewModel _gameSetup;

        public TwoPlayersGameSetup()
        {
            InitializeComponent();

            _gameSetup = new TwoPlayersGameSetupViewModel(Settings.Default.PlayerName, ChoosePlayerAvatar, ChoosePlayerAvatar.SelectedItem);
            DataContext = _gameSetup;

            foreach (var window in Application.Current.Windows.OfType<BattleShipWindow>())
            {
                _battleShipWindow = window;
            }

            Switcher.CurrentFrame = _battleShipWindow?.GameWindowFrame;
        }

        private bool ValidatingGameSetup()
        {
            var errorMessage = "";

            if (string.IsNullOrEmpty(PlayerNameTextBox.Text))
                errorMessage += "\nMissing Player Name";

            if (ChoosePlayerAvatar.SelectedItem == null)
                errorMessage += "\nMissing Avatar Selection";

            if (string.IsNullOrEmpty(errorMessage))
                return true;

            MessageBox.Show(_battleShipWindow, "To continue, the following errors must by addressed:" + errorMessage,
                "Insufficient Data");

            return false;
        }

        private void StartGameButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (ValidatingGameSetup())
            {
                /*_gameSetup.InitializeGame();

                foreach (var window in Application.Current.Windows)
                {
                    (window as BattleShipWindow)?.Close();
                }

                _gameSetup.StartGame();*/

                MessageBox.Show(@"Multiplayer game will be available in the next version", @"Coming soon update", MessageBoxButton.OK);
            }
        }

        private void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            Switcher.Switch("NewGamePage");
        }
    }
}
