using System.Linq;
using System.Windows;
using BattleShip.Properties;
using BattleShip.ViewModel;
using BattleShip.ViewModel.Players;
using GameEngine.Players;

namespace BattleShip.View
{
    /// <summary>
    /// Interaction logic for OnePlayerGameSetup.xaml
    /// </summary>
    public partial class OnePlayerGameSetup
    {
        private readonly BattleShipWindow _battleShipWindow;
        public GameSetupViewModel GameSetup;
        
        public OnePlayerGameSetup()
        {
            InitializeComponent();

            GameSetup = new GameSetupViewModel(Settings.Default.PlayerName, ChoosePlayerAvatar, ChooseComputerPlayer, ChoosePlayerAvatar.SelectedItem, ChooseComputerPlayer.SelectedItem);
            DataContext = GameSetup;

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

            if (ChooseComputerPlayer.SelectedItem == null)
                errorMessage += "\nMissing Opponent Selection";

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
                GameSetup.InitializeGame();
                
                foreach (var window in Application.Current.Windows)
                {
                    (window as BattleShipWindow)?.Close();
                }

                GameSetup.StartGame();
            }
        }

        #region Resume Game

        public void ResumeGame(HumanPlayer humanPlayer, IPlayer computerPlayer)
        {
            GameSetup.ResumeGame(humanPlayer, computerPlayer);
            GameSetup.StartGame();
        }

        #endregion // Resume Current Game

        private void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            Switcher.Switch("NewGamePage");
        }
    }
}
