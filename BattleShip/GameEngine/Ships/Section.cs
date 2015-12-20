using System.ComponentModel;
using System.Runtime.CompilerServices;
using GameEngine.Annotations;

namespace GameEngine.Ships
{
    public sealed class Section : INotifyPropertyChanged
    {
        private bool _isDamaged;

        public bool IsDamaged
        {
            get { return _isDamaged;}
            set
            {
                _isDamaged = value;
                OnPropertyChanged(nameof(IsDamaged));
            }
        }

        public Coordinate ShipCoordinate { get; set; }

        public Section(Coordinate shipCoordinate)
        {
            _isDamaged = false;
            ShipCoordinate = shipCoordinate;
        }

        public Section(bool isDamaged, Coordinate shipCoordinate)
        {
            IsDamaged = isDamaged;
            ShipCoordinate = shipCoordinate;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}