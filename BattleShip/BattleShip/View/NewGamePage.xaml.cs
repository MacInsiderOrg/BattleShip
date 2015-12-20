using System.Windows;
using BattleShip.ViewModel;

namespace BattleShip.View
{
    /// <summary>
    /// Interaction logic for NewGamePage.xaml
    /// </summary>
    public partial class NewGamePage
    {
        public NewGamePage()
        {
            InitializeComponent();
        }

        private void OnePlayerButton_OnClick(object sender, RoutedEventArgs e)
        {
            Switcher.Switch("OnePlayerGameSetup");
        }

        private void TwoPlayerButton_OnClick(object sender, RoutedEventArgs e)
        {
            Switcher.Switch("TwoPlayersGameSetup");
        }

        private void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            var startingWindow = new StartingWindow();
            startingWindow.Show();

            foreach (var window in Application.Current.Windows)
            {
                (window as BattleShipWindow)?.Close();
            }
        }
    }
}
