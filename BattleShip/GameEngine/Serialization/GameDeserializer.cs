using System.Collections.Generic;
using System.Linq;
using GameEngine.Attacks;
using GameEngine.Players;
using GameEngine.Ships;

namespace GameEngine.Serialization
{
    public class GameDeserializer
    {
        #region Members
        public readonly string PlayerName;
        public readonly string PlayerAvatar;
        public readonly Attack[,] Attacks;
        public readonly Ship[] Ships;
        public readonly Coordinate MaxCoordinates;
        public readonly ComputerPlayer ComputerPlayer;
        public readonly UiState CurrentState;
        #endregion // Members
        
        #region Constructor
        public GameDeserializer(BattleShipData battleShipData)
        {
            PlayerName = battleShipData.PlayerName;
            PlayerAvatar = battleShipData.PlayerAvatar;

            MaxCoordinates = GetConvertedCoordinates(battleShipData.MaxCoordinates);
            Attacks = GetConvertedAttacks(battleShipData.Attacks, battleShipData.MaxCoordinates);
            Ships = GetConvertedShips(battleShipData.Ships);

            ComputerPlayer = new ComputerPlayer
            {
                PlayerName = battleShipData.ComputerPlayer.PlayerName,
                PlayerAvatar = battleShipData.ComputerPlayer.PlayerAvatar,
                Attacks =
                    GetConvertedAttacks(battleShipData.ComputerPlayer.Attacks,
                        battleShipData.ComputerPlayer.MaxCoordinates),
                Ships = GetConvertedShips(battleShipData.ComputerPlayer.Ships),
                MaxCoordinates = GetConvertedCoordinates(battleShipData.ComputerPlayer.MaxCoordinates)
            };

            CurrentState = battleShipData.CurrentState;
        }
        #endregion // Constructor
        
        #region Methods
        private static Attack[,] GetConvertedAttacks(AttackSerializable[,] playerAttacks, CoordinateSerializable maxCoordinates)
        {
            var attacks = new Attack[maxCoordinates.X + 1, maxCoordinates.Y + 1];

            for (var i = 0; i <= maxCoordinates.X; i++)
            {
                for (var j = 0; j <= maxCoordinates.Y; j++)
                {
                    attacks[i, j] = new Attack
                    {
                        Result = playerAttacks[i, j].Result
                    };
                }
            }

            return attacks;
        }

        private static Coordinate GetConvertedCoordinates(CoordinateSerializable maxCoordinates)
        {
            return new Coordinate(maxCoordinates.X, maxCoordinates.Y);
        }

        private static Ship[] GetConvertedShips(IEnumerable<ShipSerializable> ships)
        {
            return
                (from ship in ships
                 let sections = ship.Sections.Select(section =>
                     new Section(
                         section.IsDamaged, new Coordinate(
                             section.ShipCoordinate.X,
                             section.ShipCoordinate.Y
                             )
                         )
                     ).ToArray()
                 select new Ship(
                     ship.Name,
                     ship.Name,
                     sections
                     )
                    ).ToArray();
        }
        #endregion // Methods
    }
}