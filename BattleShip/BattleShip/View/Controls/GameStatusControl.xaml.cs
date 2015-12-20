using System.Windows;

namespace BattleShip.View.Controls
{
    /// <summary>
    /// Interaction logic for GameStatusControl.xaml
    /// </summary>
    public partial class GameStatusControl
    {
        public GameStatusControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty HumanPlayerSunkShipsProperty = 
            DependencyProperty.Register("HumanPlayerSunkShips",
                typeof(int), typeof(GameStatusControl), 
                new PropertyMetadata(5)
            );

        public static readonly DependencyProperty CurrentGameStatusProperty =
            DependencyProperty.Register("CurrentGameStatus",
                typeof(string), typeof(GameStatusControl),
                new PropertyMetadata(string.Empty)
            );

        public static readonly DependencyProperty ComputerPlayerSunkShipsProperty =
            DependencyProperty.Register("ComputerPlayerSunkShips",
                typeof(int), typeof(GameStatusControl),
                new PropertyMetadata(5)
            );

        public int HumanPlayerSunkShips
        {
            get { return (int) GetValue(HumanPlayerSunkShipsProperty); }
            set { SetValue(HumanPlayerSunkShipsProperty, value); }
        }

        public string CurrentGameStatus
        {
            get { return GetValue(CurrentGameStatusProperty).ToString(); }
            set { SetValue(CurrentGameStatusProperty, value); }
        }

        public int ComputerPlayerSunkShips
        {
            get { return (int) GetValue(ComputerPlayerSunkShipsProperty); }
            set { SetValue(ComputerPlayerSunkShipsProperty, value); }
        }
    }
}
