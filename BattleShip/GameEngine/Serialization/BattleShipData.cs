using System;
using GameEngine.Attacks;

namespace GameEngine.Serialization
{
    [Serializable]
    #region BattleShipData
    public class BattleShipData
    {
        public string PlayerName { get; set; }

        public string PlayerAvatar { get; set; }

        public ShipSerializable[] Ships { get; set; }

        public AttackSerializable[,] Attacks { get; set; }

        public CoordinateSerializable MaxCoordinates { get; set; }

        public ComputerPlayerSerializable ComputerPlayer { get; set; }

        public UiState CurrentState { get; set; }
    }
    #endregion // BattleShipData

    [Serializable]
    #region ComputerPlayer
    public class ComputerPlayerSerializable
    {
        public string PlayerName { get; set; }

        public string PlayerAvatar { get; set; }

        public ShipSerializable[] Ships { get; set; }

        public AttackSerializable[,] Attacks { get; set; }

        public CoordinateSerializable MaxCoordinates { get; set; }
    }
    #endregion // ComputerPlayer

    [Serializable]
    #region Attack
    public class AttackSerializable
    {
        public AttackResult Result { get; set; }
    }
    #endregion // Atack

    [Serializable]
    #region Ship
    public class ShipSerializable
    {
        public string Name { get; set; }

        public SectionSerializable[] Sections { get; set; }
    }
    #endregion // Ship

    [Serializable]
    #region Section
    public class SectionSerializable
    {
        public bool IsDamaged { get; set; }

        public CoordinateSerializable ShipCoordinate { get; set; }
    }
    #endregion // Section

    [Serializable]
    #region Coordinate
    public class CoordinateSerializable
    {
        public int X { get; set; }

        public int Y { get; set; }
    }
    #endregion // Coordinate
}
