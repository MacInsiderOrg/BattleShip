using System;
using System.Collections.Generic;
using System.Xml;
using GameEngine.Ships;

namespace GameEngine.Helpers
{
    public class GameInfoHelper
    {
        #region Members

        private const string GameInfoPath = @"../../Resources/BattleShipGameInfo.xml";
        private readonly Dictionary<string, int> _shipSectionNums;
        private readonly Dictionary<string, string> _shipDisplayNames;
        public readonly Dictionary<string, string> ShipImageLocation;

        #endregion // Members
        
        #region Constructor
        public GameInfoHelper()
        {
            _shipSectionNums = new Dictionary<string, int>();
            _shipDisplayNames = new Dictionary<string, string>();
            ShipImageLocation = new Dictionary<string, string>();

            // Initialize Game Info
            var xmlReader = new XmlTextReader(GameInfoPath);
            var gameInfoDocument = new XmlDocument();
            gameInfoDocument.Load(xmlReader);
            xmlReader.Close();

            // Populate ships
            PopulateShips(gameInfoDocument);
        }
        #endregion // Constructor
        
        #region Populate ships
        private void PopulateShips(XmlDocument gameInfoDocument)
        {
            foreach (XmlNode startShipList in gameInfoDocument.GetElementsByTagName("StartingShips"))
            {
                var xmlNodeList = startShipList.SelectNodes("Ship");

                if (xmlNodeList != null)
                {
                    foreach (XmlNode ship in xmlNodeList)
                    {
                        try
                        {
                            var name = ship.Attributes?["name"].Value;
                            var displayName = ship.Attributes?["displayName"].Value;
                            var shipImageLocation = ship.Attributes?["shipImage"].Value;
                            var numberOfSections = Convert.ToInt32(ship.Attributes["numSections"].Value);

                            _shipSectionNums.Add(name, numberOfSections);
                            _shipDisplayNames.Add(name, displayName);
                            ShipImageLocation.Add(name, shipImageLocation);
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                    }
                }
            }
        }
        #endregion // Populate Ships
        
        #region Get Starting Ships
        public Ship[] GetStartingShips()
        {
            var ships = new List<Ship>();
            const int startingX = -100;
            const int startingY = -100;

            foreach (var entry in _shipSectionNums)
            {
                var sections = new List<Section>();

                for (var i = entry.Value; i > 0; i--)
                    sections.Add(new Section(new Coordinate(startingX, startingY)));

                ships.Add(new Ship(entry.Key, _shipDisplayNames[entry.Key], sections.ToArray()));
            }

            return ships.ToArray();
        }
        #endregion // Get Starting Ships
    }
}