using System.Windows;
using BattleShip.ViewModel;

namespace BattleShip.View
{
    /// <summary>
    /// Interaction logic for StartingWindow.xaml
    /// </summary>
    public partial class StartingWindow
    {
        public StartingWindow()
        {
            InitializeComponent();
            DataContext = new GameInitViewModel(this);
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            var battleShipWindow = new BattleShipWindow();
            battleShipWindow.Show();
            Close();
        }

        private void ExitButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void PreferencesButton_OnClick(object sender, RoutedEventArgs e)
        {
            var additionalWindow = new Preferences.AdditionalWindow();
            additionalWindow.Show();
            Close();
        }
    }
}
