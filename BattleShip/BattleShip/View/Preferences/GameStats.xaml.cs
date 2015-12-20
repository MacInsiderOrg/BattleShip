using BattleShip.ViewModel;
using System.Linq;
using System.Windows;
using BattleShip.ViewModel.Preferences;

namespace BattleShip.View.Preferences
{
    /// <summary>
    /// Interaction logic for GameStats.xaml
    /// </summary>
    public partial class GameStats
    {
        private readonly AdditionalWindow _additionalWindow;

        public GameStats()
        {
            InitializeComponent();

            DataContext = new GameStatsViewModel();

            foreach (var window in Application.Current.Windows.OfType<AdditionalWindow>())
            {
                _additionalWindow = window;
            }

            _additionalWindow.Title = "Battle Ship Game - Statistics";
            Switcher.CurrentFrame = _additionalWindow?.AdditionalFrame;

        }

        private void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            Switcher.Switch("Preferences\\Settings");
        }
    }
}
