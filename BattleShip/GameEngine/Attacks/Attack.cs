using System.ComponentModel;
using System.Runtime.CompilerServices;
using GameEngine.Annotations;

namespace GameEngine.Attacks
{
    public sealed class Attack : INotifyPropertyChanged
    {
        private AttackResult _attackResult;

        public AttackResult Result
        {
            get { return _attackResult; }
            set
            {
                _attackResult = value;
                OnPropertyChanged(nameof(Result));
            }
        }

        public Attack()
        {
            _attackResult = AttackResult.Unknown;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}