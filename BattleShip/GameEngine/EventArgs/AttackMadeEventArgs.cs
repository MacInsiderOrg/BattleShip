using GameEngine.Ships;

namespace GameEngine.EventArgs
{
    public class AttackMadeEventArgs : System.EventArgs
    {
        public readonly Coordinate AttackCoordinate;

        public AttackMadeEventArgs(Coordinate attackCoordinate)
        {
            AttackCoordinate = attackCoordinate;
        }
    }
}