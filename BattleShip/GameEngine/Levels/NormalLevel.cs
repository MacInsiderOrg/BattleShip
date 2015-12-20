using System.Collections.Generic;
using GameEngine.Players;
using System;
using GameEngine.Attacks;
using GameEngine.Helpers;
using GameEngine.Ships;

namespace GameEngine.Levels
{
    public class NormalLevel : ComputerPlayer
    {
        #region Private Members
        private enum AttackState
        {
            StabInTheDark = 0,
            FindFollowUp,
            FinishEmOff
        }

        private AttackState _currentAttackState;
        private List<Coordinate> _targets;
        private AttackInfoHelper _firstHit;
        private AttackInfoHelper _secondHit;

        #endregion // Private Members
        
        #region Constructor
        public NormalLevel(string playerName, string playerAvatar) :
            base(playerName, playerAvatar)
        {
            _currentAttackState = AttackState.StabInTheDark;
            _targets = new List<Coordinate>();
        }
        #endregion // Constructor
        
        #region Attack
        public override Coordinate Attack()
        {
            switch (_currentAttackState)
            {
                case AttackState.StabInTheDark:
                    return CalcRandomAttack();

                case AttackState.FindFollowUp:
                    if (_targets.Count == 0)
                    {
                        _currentAttackState = AttackState.StabInTheDark;
                        return CalcRandomAttack();
                    }

                    var random = new Random();
                    var i = random.Next(_targets.Count);
                    var target = _targets[i];
                    _targets.RemoveAt(i);
                    return target;

                case AttackState.FinishEmOff:
                    var coordinate = FinishEmOff();

                    if (coordinate == null)
                    {
                        _currentAttackState = AttackState.StabInTheDark;
                        return CalcRandomAttack();
                    }

                    return coordinate;

                default:
                    _currentAttackState = AttackState.StabInTheDark;
                    return CalcRandomAttack();
            }
        }
        #endregion // Attack

        #region Update Attack Results
        public override void UpdateAttackResults(Coordinate lastAttack, AttackResult attackResult, bool sunkShip)
        {
            switch (_currentAttackState)
            {
                case AttackState.StabInTheDark:
                    if (attackResult == AttackResult.Hit)
                    {
                        _currentAttackState = AttackState.FindFollowUp;
                        _firstHit = new AttackInfoHelper(lastAttack, attackResult, sunkShip);
                        _targets = CalcAdjCoordinates(lastAttack)
                            .FindAll(coordinate => Attacks[coordinate.X, coordinate.Y].Result == AttackResult.Unknown);
                    }
                    break;

                case AttackState.FindFollowUp:
                    if (sunkShip)
                    {
                        _currentAttackState = AttackState.StabInTheDark;
                        break;
                    }

                    if (attackResult == AttackResult.Hit)
                    {
                        _currentAttackState = AttackState.FinishEmOff;
                        _secondHit = new AttackInfoHelper(lastAttack, attackResult, sunkShip);
                        _targets.Clear();
                    }

                    break;

                case AttackState.FinishEmOff:
                    if (attackResult == AttackResult.Hit && sunkShip)
                    {
                        _currentAttackState = AttackState.StabInTheDark;
                        _firstHit = null;
                        _secondHit = null;
                    }

                    break;

                default:
                    _currentAttackState = AttackState.StabInTheDark;
                    break;
            }

            base.UpdateAttackResults(lastAttack, attackResult, sunkShip);
        }
        #endregion // Update Attack Results

        #region FinishEmOff
        private Coordinate FinishEmOff()
        {
            var isHorizontalChain = (_firstHit.Coordinate.Y == _secondHit.Coordinate.Y);
            var coordinateOptions = new List<Coordinate>();

            if (isHorizontalChain)
            {
                var nextRight = new Coordinate(_firstHit.Coordinate.X + 1, _firstHit.Coordinate.Y);
                var nextLeft = new Coordinate(_firstHit.Coordinate.X - 1, _firstHit.Coordinate.Y);

                if (!ShipPlacementHelper.IsOutOfBounds(nextRight))
                {
                    while (Attacks[nextRight.X, nextRight.Y].Result == AttackResult.Hit)
                    {
                        nextRight.X++;
                        if (ShipPlacementHelper.IsOutOfBounds(nextRight))
                            break;
                    }
                }

                if (!ShipPlacementHelper.IsOutOfBounds(nextLeft))
                {
                    while (Attacks[nextLeft.X, nextLeft.Y].Result == AttackResult.Hit)
                    {
                        nextLeft.X--;
                        if (ShipPlacementHelper.IsOutOfBounds(nextLeft))
                            break;
                    }
                }

                coordinateOptions.Add(nextRight);
                coordinateOptions.Add(nextLeft);
            }
            else // Vertical Chain
            {
                var nextUp = new Coordinate(_firstHit.Coordinate.X, _firstHit.Coordinate.Y + 1);
                var nextDown = new Coordinate(_firstHit.Coordinate.X, _firstHit.Coordinate.Y - 1);

                if (!ShipPlacementHelper.IsOutOfBounds(nextUp))
                {
                    while (Attacks[nextUp.X, nextUp.Y].Result == AttackResult.Hit)
                    {
                        nextUp.Y++;
                        if (ShipPlacementHelper.IsOutOfBounds(nextUp))
                            break;
                    }
                }

                if (!ShipPlacementHelper.IsOutOfBounds(nextDown))
                {
                    while (Attacks[nextDown.X, nextDown.Y].Result == AttackResult.Hit)
                    {
                        nextDown.Y--;
                        if (ShipPlacementHelper.IsOutOfBounds(nextDown))
                            break;
                    }
                }

                coordinateOptions.Add(nextUp);
                coordinateOptions.Add(nextDown);
            }

            coordinateOptions.RemoveAll(x => ShipPlacementHelper.IsOutOfBounds(x));
            coordinateOptions.RemoveAll(x => Attacks[x.X, x.Y].Result == AttackResult.Miss);

            // Pick a random coord
            var random = new Random();
            return coordinateOptions.Count <= 0
                ? null
                : coordinateOptions[random.Next(coordinateOptions.Count - 1)];
        }
        #endregion // Finish Em Off
    }
}