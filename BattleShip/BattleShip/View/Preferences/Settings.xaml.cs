using System.Linq;
using System.Windows;
using BattleShip.ViewModel;
using BattleShip.ViewModel.Preferences;

namespace BattleShip.View.Preferences
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings
    {
        private readonly AdditionalWindow _additionalWindow;

        public Settings()
        {
            InitializeComponent();

            DataContext = new PreferencesViewModel();

            foreach (var window in Application.Current.Windows.OfType<AdditionalWindow>())
            {
                _additionalWindow = window;
            }

            _additionalWindow.Title = "Battle Ship Game - Preferences";
            Switcher.CurrentFrame = _additionalWindow?.AdditionalFrame;
        }

        private void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            var startingWindow = new StartingWindow();
            startingWindow.Show();
            _additionalWindow.Close();
        }

        private void GameStatistics_OnClick(object sender, RoutedEventArgs e)
        {
            Switcher.Switch("Preferences\\GameStats");
        }
    }
}
