using System.Linq;

namespace GameEngine.Ships
{
    public class Ship
    {
        public string Name { get; private set; }
        private string DisplayName { get; set; }

        // Ship section
        public Section[] Sections { get; }

        // Ship constructor
        public Ship(string name, string displayName, Section[] sections)
        {
            Name = name;
            DisplayName = displayName;
            Sections = sections;
        }

        // Check if all parts of ship are destroyed
        // Check if ship is destroyed
        public bool IsDestroyed()
        {
            return Sections.All(parts => parts.IsDamaged);
        }
    }
}