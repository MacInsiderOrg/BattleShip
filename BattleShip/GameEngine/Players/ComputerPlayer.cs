using System;
using System.Collections.Generic;
using System.Linq;
using GameEngine.Attacks;
using GameEngine.Helpers;
using GameEngine.Ships;

namespace GameEngine.Players
{
    public class ComputerPlayer : IPlayer
    {
        public string PlayerName { get; set; }
        public string PlayerAvatar { get; set; }

        // Player ships
        public Ship[] Ships { get; set; }

        // Player attacks
        public Attack[,] Attacks { get; set; }

        // Max coordinates
        public Coordinate MaxCoordinates { get; set; }

        // Ship placement helper
        protected ShipPlacementHelper ShipPlacementHelper;

        protected ComputerPlayer(string playerName, string playerAvatar)
        {
            PlayerName = playerName;
            PlayerAvatar = playerAvatar;
        }

        public ComputerPlayer()
        {
        }

        // Initialize Player
        public virtual void Initialize(Coordinate maxCoordinates, Ship[] startingShips)
        {
            Ships = startingShips;

            if (Attacks == null)
            {
                Attacks = new Attack[maxCoordinates.X + 1, maxCoordinates.Y + 1];

                for (var x = 0; x <= maxCoordinates.X; x++)
                    for (var y = 0; y <= maxCoordinates.Y; y++) 
                        Attacks[x, y] = new Attack();
            }

            MaxCoordinates = maxCoordinates;
            ShipPlacementHelper = new ShipPlacementHelper(maxCoordinates);
        }

        // Check if user is ready to go :)
        public virtual bool IsReady()
        {
            return true;
        }

        // Place all ships
        public virtual void PlaceShips()
        {
            var placedShips = new List<Ship>();
            for (var i = 0; i < Ships.Length; i++)
            {
                Ships[i] = PlaceShip(Ships[i], placedShips);
                placedShips.Add(Ships[i]);
            }
        }

        // Place current ship
        private Ship PlaceShip(Ship ship, List<Ship> placedShips)
        {
            if (ship.Sections.Count(section => section.ShipCoordinate.X != -100 && section.ShipCoordinate.Y != -100) ==
                ship.Sections.Length)
                return ship;

            var random = new Random();
            var isSafePlacement = false;

            while (!isSafePlacement)
            {
                isSafePlacement = true;
                var x = random.Next(MaxCoordinates.X);
                var y = random.Next(MaxCoordinates.Y);

                var secMod = 0;
                var xMod = 0;
                var yMod = 0;

                // Pick a random direction
                switch (random.Next(3))
                {
                    case 0:
                        yMod = 1; // Up
                        break;

                    case 1:
                        yMod = -1; // Down
                        break;

                    case 2:
                        xMod = -1; // Left
                        break;

                    case 3:
                        xMod = 1; // Right
                        break;
                }

                // Place Ship
                foreach (var section in ship.Sections)
                {
                    section.ShipCoordinate = new Coordinate(x + secMod * xMod, y + secMod * yMod);
                    secMod++;
                }

                // Check for safe placement
                if (ShipPlacementHelper.IsInvalidPlacement(ship) ||
                    ShipPlacementHelper.PlacementCreatesConflict(ship, placedShips))
                {
                    isSafePlacement = false;
                }
            }

            return ship;
        }

        // Player attack
        public virtual Coordinate Attack()
        {
            return CalcRandomAttack();
        }

        // Update list of attacks
        public virtual void UpdateAttackResults(Coordinate lastAttack, AttackResult attackResult, bool sunkShip)
        {
            Attacks[lastAttack.X, lastAttack.Y].Result = attackResult;
        }

        // If user Win -> send notification
        public virtual void WinnerNotification(string winnerName) { }

        // Set random attack
        protected Coordinate CalcRandomAttack()
        {
            var random = new Random();
            var attack = new Coordinate(random.Next(MaxCoordinates.X + 1), random.Next(MaxCoordinates.Y + 1));

            while (AttackExists(attack))
            {
                attack = new Coordinate(random.Next(MaxCoordinates.X + 1), random.Next(MaxCoordinates.Y + 1));
            }

            return attack;
        }

        // Check if current attack is exists
        private bool AttackExists(Coordinate attack)
        {
            if (Attacks[attack.X, attack.Y].Result != AttackResult.Unknown)
                return true;

            return false;
        }

        // Calculate attack coordinates
        protected List<Coordinate> CalcAdjCoordinates(Coordinate coordinate)
        {
            var adjList = new List<Coordinate>
            {
                new Coordinate(coordinate.X + 1, coordinate.Y),
                new Coordinate(coordinate.X - 1, coordinate.Y),
                new Coordinate(coordinate.X, coordinate.Y + 1),
                new Coordinate(coordinate.X, coordinate.Y - 1)
            };

            adjList.RemoveAll(x => ShipPlacementHelper.IsOutOfBounds(x));
            return adjList;
        }

        // Update Ui State
        public void UpdateUiState(UiState currentState) { }
    }
}