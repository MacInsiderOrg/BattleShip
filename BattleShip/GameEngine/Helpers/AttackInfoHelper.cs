using GameEngine.Attacks;
using GameEngine.Ships;

namespace GameEngine.Helpers
{
    public class AttackInfoHelper
    {
        public readonly Coordinate Coordinate;
        private AttackResult _attackResult;
        private bool _sunkShip;

        public AttackInfoHelper(Coordinate coordinate, AttackResult attackResult, bool sunkShip)
        {
            Coordinate = coordinate;
            _attackResult = attackResult;
            _sunkShip = sunkShip;
        }
    }
}