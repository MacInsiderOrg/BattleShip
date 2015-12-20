using System.Collections.Generic;
using System.Linq;
using GameEngine.Attacks;
using GameEngine.Players;
using GameEngine.Ships;

namespace GameEngine.Serialization
{
    public class GameSerializer
    {
        public readonly BattleShipData BattleShipData;

        #region Constructor
        public GameSerializer(string playerName, string playerAvatar, Attack[,] attacks,
            IEnumerable<Ship> ships, Coordinate maxCoordinate, IPlayer computerPlayer, UiState currentState)
        {
            BattleShipData = new BattleShipData
            {
                PlayerName = playerName,
                PlayerAvatar = playerAvatar,
                Attacks = GetConvertedAttacks(attacks, maxCoordinate),
                Ships = GetConvertedShips(ships),
                MaxCoordinates = GetConvertedCoordinates(maxCoordinate),
                ComputerPlayer = new ComputerPlayerSerializable
                {
                    PlayerName = computerPlayer.PlayerName,
                    PlayerAvatar = computerPlayer.PlayerAvatar,
                    Attacks = GetConvertedAttacks(computerPlayer.Attacks, computerPlayer.MaxCoordinates),
                    Ships = GetConvertedShips(computerPlayer.Ships),
                    MaxCoordinates = GetConvertedCoordinates(computerPlayer.MaxCoordinates)
                },
                CurrentState = currentState
            };
        }
        #endregion // Constructor
        
        #region Methods
        private static AttackSerializable[,] GetConvertedAttacks(Attack[,] playerAttacks, Coordinate maxCoordinates)
        {
            var attacks = new AttackSerializable[maxCoordinates.X + 1, maxCoordinates.Y + 1];
            for (var i = 0; i <= maxCoordinates.X; i++)
            {
                for (var j = 0; j <= maxCoordinates.Y; j++)
                {
                    attacks[i, j] = new AttackSerializable
                    {
                        Result = playerAttacks[i, j].Result
                    };
                }
            }

            return attacks;
        }

        private static CoordinateSerializable GetConvertedCoordinates(Coordinate maxCoordinates)
        {
            return new CoordinateSerializable
            {
                X = maxCoordinates.X,
                Y = maxCoordinates.Y
            };
        }

        private static ShipSerializable[] GetConvertedShips(IEnumerable<Ship> ships)
        {
            return
                (from ship in ships
                 let sections = ship.Sections.Select(section =>
                     new SectionSerializable
                     {
                         IsDamaged = section.IsDamaged,
                         ShipCoordinate = new CoordinateSerializable
                         {
                             X = section.ShipCoordinate.X,
                             Y = section.ShipCoordinate.Y
                         }
                     }).ToArray()
                 select new ShipSerializable
                 {
                     Name = ship.Name,
                     Sections = sections
                 }).ToArray();
        }
        #endregion // Methods
    }
}