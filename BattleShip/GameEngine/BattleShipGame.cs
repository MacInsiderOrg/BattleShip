using System.Collections.Generic;
using System.Linq;
using GameEngine.Attacks;
using GameEngine.Helpers;
using GameEngine.Players;
using GameEngine.Ships;

namespace GameEngine
{
    public class BattleShipGame
    {
        #region Battle Ship Game Fields

        // Current Players
        private IPlayer FirstPlayer { get; }
        private IPlayer SecondPlayer { get; }

        public Coordinate MaxCoordinate { get; set; }

        // Ship placement helper
        private readonly ShipPlacementHelper _shipPlacementHelper;

        /**
         * Player Actions
         * Contain
         * - Turns
         * - Misses
         * - Hits
         */
        private readonly PlayerActions _firstPlayerActions;
        private readonly PlayerActions _secondPlayerActions;

        #endregion // Battle Ship Game Fields

        #region Battle Ship Game Constructors

        public BattleShipGame(IPlayer computerPlayer, IPlayer humanPlayer, Coordinate boardDimension)
        {
            FirstPlayer = computerPlayer;
            SecondPlayer = humanPlayer;

            _shipPlacementHelper = new ShipPlacementHelper(boardDimension);
            var gameInfoHelper = new GameInfoHelper();

            FirstPlayer.Initialize(boardDimension, gameInfoHelper.GetStartingShips());
            SecondPlayer.Initialize(boardDimension, gameInfoHelper.GetStartingShips());

            _firstPlayerActions = new PlayerActions();
            _secondPlayerActions = new PlayerActions();
        }

        public BattleShipGame(IPlayer computerPlayer, IPlayer humanPlayer,
            Coordinate boardDimension, Ship[] humanPlayerShips, Ship[] computerPlayerShips,
            Attack[,] humanPlayerAttacks, Attack[,] computerPlayerAttacks, UiState currentState)
        {
            FirstPlayer = computerPlayer;
            SecondPlayer = humanPlayer;

            _shipPlacementHelper = new ShipPlacementHelper(boardDimension);
            //var gameInfoHelper = new GameInfoHelper();

            SecondPlayer.UpdateUiState(currentState);

            FirstPlayer.Initialize(boardDimension, humanPlayerShips);
            FirstPlayer.Attacks = humanPlayerAttacks;

            SecondPlayer.Initialize(boardDimension, computerPlayerShips);
            SecondPlayer.Attacks = computerPlayerAttacks;

            _firstPlayerActions = new PlayerActions();
            _secondPlayerActions = new PlayerActions();
        }
        #endregion // Battle Ship Game Constructors

        #region Run BattleShip Game
        public void RunGame()
        {
            // Set Play order
            var currentPlayer = FirstPlayer;
            var computerPlayer = SecondPlayer;

            // Wait for both players to be ready
            while (!IsReady(FirstPlayer) || !IsReady(SecondPlayer)) { }

            // Place Ships
            PlaceShips(FirstPlayer);
            PlaceShips(SecondPlayer);

            // Begin game
            while (!IsWinner())
            {
                if (currentPlayer == FirstPlayer)
                    _firstPlayerActions.Turns++;
                else
                    _secondPlayerActions.Turns++;

                // Get an attack than hasn't been made previously
                var attack = currentPlayer.Attack();
                while (currentPlayer.Attacks[attack.X, attack.Y].Result != AttackResult.Unknown)
                    attack = currentPlayer.Attack();

                // Calculate attack result
                CalculateAttackResult(currentPlayer, computerPlayer, attack);

                // Switch players
                SwitchPlayers(ref currentPlayer, out computerPlayer);
            }

            ResolveWinner();
        }
        #endregion // Run Game

        #region Is player ready
        private static bool IsReady(IPlayer player)
        {
            return player.IsReady();
        }
        #endregion // Is player ready

        #region Place player ships
        private void PlaceShips(IPlayer player)
        {
            var placementAccepted = false;

            while (!placementAccepted)
            {
                player.PlaceShips();
                placementAccepted = true;
                var checkedShips = new List<Ship>();

                foreach (var ship in player.Ships)
                {
                    if (_shipPlacementHelper.IsInvalidPlacement(ship) ||
                        ShipPlacementHelper.PlacementCreatesConflict(ship, checkedShips))
                    {
                        placementAccepted = false;
                        break;
                    }

                    checkedShips.Add(ship);
                }
            }
        }
        #endregion // Place player ships

        #region Check who's winner
        private bool IsWinner()
        {
            return (FirstPlayer.Ships.All(ship => ship.IsDestroyed()) ||
                    SecondPlayer.Ships.All(ship => ship.IsDestroyed())
            );
        }
        #endregion // Check who's winner

        #region Calculate Attack Result
        private void CalculateAttackResult(IPlayer currentPlayer, IPlayer computerPlayer, Coordinate attack)
        {
            var attackResult = AttackResult.Miss;
            Ship hitShip = null;
            var sunkShip = false;

            if (GetHitShip(computerPlayer.Ships, attack) != null)
                hitShip = GetHitShip(computerPlayer.Ships, attack);
            else
            {
                if (currentPlayer == FirstPlayer)
                    _firstPlayerActions.Misses++;
                else
                    _secondPlayerActions.Misses++;
            }

            if (hitShip != null)
            {
                hitShip.Sections.First(section =>
                    section.ShipCoordinate.X == attack.X && section.ShipCoordinate.Y == attack.Y).IsDamaged = true;

                attackResult = AttackResult.Hit;

                if (currentPlayer == FirstPlayer)
                    _firstPlayerActions.Hits++;
                else
                    _secondPlayerActions.Hits++;

                sunkShip = hitShip.IsDestroyed();
            }

            // Let current Player know how the attack went
            currentPlayer.UpdateAttackResults(attack, attackResult, sunkShip);
        }
        #endregion // Calculate Attack Result

        #region Get Hit Ship
        private static Ship GetHitShip(IEnumerable<Ship> ships, Coordinate attack)
        {
            return ships.FirstOrDefault(
                ship => ship.Sections.Any(
                    section => section.ShipCoordinate.X == attack.X && section.ShipCoordinate.Y == attack.Y
                )
            );
        }
        #endregion // Get Hit Ship

        #region Switch Players
        private void SwitchPlayers(ref IPlayer currentPlayer, out IPlayer computerPlayer)
        {
            if (currentPlayer.Equals(FirstPlayer))
            {
                currentPlayer = SecondPlayer;
                computerPlayer = FirstPlayer;
            }
            else
            {
                currentPlayer = FirstPlayer;
                computerPlayer = SecondPlayer;
            }
        }
        #endregion // Switch Players

        #region Resolve winner of current game
        private void ResolveWinner()
        {
            var winner = "Not found";

            if (FirstPlayer.Ships.All(ship => ship.IsDestroyed()))
                winner = SecondPlayer.PlayerName;
            else if (SecondPlayer.Ships.All(ship => ship.IsDestroyed()))
                winner = FirstPlayer.PlayerName;

            FirstPlayer.WinnerNotification(winner);
            SecondPlayer.WinnerNotification(winner);
        }
        #endregion // Resolve Winner
    }
}