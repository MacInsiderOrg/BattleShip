using System;
using System.Threading;
using BattleShip.View;
using GameEngine;
using GameEngine.Attacks;
using GameEngine.EventArgs;
using GameEngine.Players;
using GameEngine.Ships;

namespace BattleShip.ViewModel.Players
{
    public class HumanPlayer : IPlayer
    {
        private readonly GameWindow _battleShip;
        
        public string PlayerName {
            get { return _battleShip.GameViewModel.PlayerName; }
            set { _battleShip.GameViewModel.PlayerName = value; }
        }

        public string PlayerAvatar {
            get { return _battleShip.GameViewModel.PlayerAvatar; }
            set { _battleShip.GameViewModel.PlayerAvatar = value; }
        }

        // Player ships
        public Ship[] Ships
        {
            get { return _battleShip.GameViewModel.Ships; }
            set { _battleShip.GameViewModel.Ships = value; }
        }

        // Player attacks
        public Attack[,] Attacks
        {
            get { return _battleShip.GameViewModel.Attacks; }
            set { _battleShip.GameViewModel.Attacks = value; }
        }

        // Max coordinates
        public Coordinate MaxCoordinates
        {
            get { return _battleShip.GameViewModel.MaxCoordinates; }
            set { _battleShip.GameViewModel.MaxCoordinates = value; }
        }

        /* Human Player own properties */
        private const int WaitToResponseInMilliseconds = 100;
        private bool _shipsPlaced;
        private Coordinate _attackToBeMade;
        
        /* Human Player Delegates */
        private delegate bool IsReadyDelegate();
        private delegate void PlaceShipsDelegate();
        private delegate void AttackDelegate();
        private delegate void UpdateAttackResultsDelegate(Coordinate lastAttack, AttackResult attackResult, bool sunkShip);
        private delegate void WinnerNotificationDelegate(string winnerName);

        // Human Player Constructor
        public HumanPlayer(string playerName, string playerAvatar)
        {
            _battleShip = new GameWindow(playerName, playerAvatar);
        }

        // Computer Player 
        public void SetComputerPlayer(ref IPlayer computerPlayer)
        {
            _battleShip.GameViewModel.SetComputerPlayerShips(ref computerPlayer);
        }

        // Initialize Player
        public void Initialize(Coordinate maxCoordinates, Ship[] startingShips)
        {
            _battleShip.GameViewModel.Init(maxCoordinates, startingShips);
            _battleShip.GameViewModel.ShipsPlaced += HandlePiecePlaced;
            _battleShip.GameViewModel.AttackMade += HandleAttackMade;
            if (_battleShip.GameViewModel.UiState != UiState.WaitingToPlace)
                _battleShip.GameViewModel.PlaceShipsExecute();
        }

        public void UpdateUiState(UiState currentState)
        {
            _battleShip.GameViewModel.UiState = currentState;
            //_battleShip.GameViewModel.UpdateData();
        }

        public bool? ShowDialog()
        {
            return _battleShip.ShowDialog();
        }

        // Check if user is ready to go :)
        public bool IsReady()
        {
            return Convert.ToBoolean(_battleShip.Dispatcher.Invoke(new IsReadyDelegate(_battleShip.GameViewModel.IsReadyToDo)));
        }

        // Place all ships
        public void PlaceShips()
        {
            _battleShip.Dispatcher.BeginInvoke(new PlaceShipsDelegate(_battleShip.GameViewModel.PlaceAllShips));

            while (!_shipsPlaced)
                Thread.Sleep(WaitToResponseInMilliseconds);

            _shipsPlaced = false;
        }

        private void HandlePiecePlaced(object sender, ShipsPlacedEventArgs e)
        {
            _shipsPlaced = true;
        }

        // Player attack
        public Coordinate Attack()
        {
            _battleShip.Dispatcher.Invoke(new AttackDelegate(_battleShip.GameViewModel.Attack));

            while (_attackToBeMade == null)
                Thread.Sleep(WaitToResponseInMilliseconds);

            var attack = _attackToBeMade;
            _attackToBeMade = null;

            return attack;
        }

        private void HandleAttackMade(object sender, AttackMadeEventArgs e)
        {
            _attackToBeMade = e.AttackCoordinate;
        }

        // Update list of attacks
        public void UpdateAttackResults(Coordinate lastAttack, AttackResult attackResult, bool floodedShip)
        {
            _battleShip.GameViewModel.OnPropertyChanged(nameof(_battleShip.GameViewModel.HumanPlayerSunkShips));
            _battleShip.GameViewModel.OnPropertyChanged(nameof(_battleShip.GameViewModel.ComputerPlayerSunkShips));
            _battleShip.Dispatcher.Invoke(new UpdateAttackResultsDelegate(_battleShip.GameViewModel.UpdateAttackResults),
                lastAttack, attackResult, floodedShip);
        }

        // If user Win -> send notification
        public void WinnerNotification(string winnerName)
        {
            _battleShip.Dispatcher.Invoke(new WinnerNotificationDelegate(_battleShip.GameViewModel.WinnerNotification), winnerName);
        }
    }
}
