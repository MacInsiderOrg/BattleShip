using System.ComponentModel;
using System.Runtime.CompilerServices;
using BattleShip.Annotations;
using BattleShip.Model.Preferences;
using BattleShip.Model.Preferences.Design;
using BattleShip.Properties;

namespace BattleShip.ViewModel.Preferences
{
    public sealed class PreferencesViewModel : INotifyPropertyChanged
    {
        public string PlayerName
        {
            get { return Settings.Default.PlayerName; }
            set
            {
                Settings.Default.PlayerName = value;
                Settings.Default.Save();
                OnPropertyChanged(nameof(PlayerName));
            }
        }

        public bool IsBackgroundMusic
        {
            get { return Settings.Default.BackgroundMusic; }
            set
            {
                Settings.Default.BackgroundMusic = value;
                AppSettings.BackgroundMusic();
                Settings.Default.Save();

                OnPropertyChanged(nameof(IsBackgroundMusic));
            }
        }

        public bool IsAttacksSounds
        {
            get { return Settings.Default.AttacksSounds; }
            set
            {
                Settings.Default.AttacksSounds = value;
                Settings.Default.Save();
                OnPropertyChanged(nameof(IsAttacksSounds));
            }
        }

        public AppDesign AppDesign
        {
            get
            {
                return Settings.Default.CurrentDesign == "Standart"
                    ? AppDesign.Standart
                    : AppDesign.Ultimate;
            }
            set
            {
                Settings.Default.CurrentDesign = value == AppDesign.Standart
                    ? "Standart"
                    : "Ultimate";
                Settings.Default.Save();
                OnPropertyChanged(nameof(AppDesign));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
