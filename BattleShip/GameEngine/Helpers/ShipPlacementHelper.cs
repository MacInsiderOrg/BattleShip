using System;
using System.Collections.Generic;
using System.Linq;
using GameEngine.Ships;

namespace GameEngine.Helpers
{
    public class ShipPlacementHelper
    {
        private const int MinDistanceBetweenShips = 2;
        private readonly Coordinate _maxCoordinates;

        // Constructor for Ship Placement Helper
        public ShipPlacementHelper(Coordinate maxCoordinates)
        {
            _maxCoordinates = maxCoordinates;
        }

        // Check if placement is invalid
        public bool IsInvalidPlacement(Ship ship)
        {
            if (ship.Sections.Any(section => IsOutOfBounds(section.ShipCoordinate)))
                return true;

            // Determine a one ship orientation
            var isVertical = ship.Sections
                .All(section => section.ShipCoordinate.X == ship.Sections[0].ShipCoordinate.X);

            var isHorizontal = ship.Sections
                .All(section => section.ShipCoordinate.Y == ship.Sections[0].ShipCoordinate.Y);

            if (!(isVertical ^ isHorizontal))
                return true;

            // Get a sorted list of values along the orientation
            var consecutiveValues = ship.Sections
                .Select(section => isVertical ? section.ShipCoordinate.Y : section.ShipCoordinate.X)
                .ToList();

            consecutiveValues.Sort();

            // Ensure these values are all adjacent to one another
            for (var i = 0; i < consecutiveValues.Count - 1; i++)
            {
                if (consecutiveValues[i + 1] != (consecutiveValues[i] + 1))
                    return true;
            }

            // Good placement
            return false;
        }

        // Check if ship is out of bounds
        public bool IsOutOfBounds(Coordinate coordinate)
        {
            return (coordinate.X > _maxCoordinates.X || coordinate.X < 0) ||
                   (coordinate.Y > _maxCoordinates.Y || coordinate.Y < 0);
        }

        // Check if placement creates a conflict
        public static bool PlacementCreatesConflict(Ship ship, List<Ship> allPlacedShips)
        {
            return ship.Sections.Any(section =>
                allPlacedShips.Any(placedShips =>
                    placedShips.Sections.Any(sectionOnPlacedShip =>
                        IsPlacementConflict(sectionOnPlacedShip, section)
                    )
                )
            );
        }

        // Check if placement is conflict
        private static bool IsPlacementConflict(Section sectionA, Section sectionB)
        {
            var xIsOutsideLimit = (Math.Abs(sectionA.ShipCoordinate.X - sectionB.ShipCoordinate.X) >=
                                   MinDistanceBetweenShips);
            var yIsOutsideLimit = (Math.Abs(sectionA.ShipCoordinate.Y - sectionB.ShipCoordinate.Y) >=
                                   MinDistanceBetweenShips);

            return !xIsOutsideLimit && !yIsOutsideLimit;
        }
    }
}