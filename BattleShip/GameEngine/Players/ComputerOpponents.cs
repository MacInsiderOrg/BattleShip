using System;
using System.Collections.Generic;
using System.Xml;
using GameEngine.Levels;

namespace GameEngine.Players
{
    public class ComputerOpponents
    {
        #region Members
        public string Name { get; private set; }
        public string Avatar { get; private set; }

        // Opponents Diffuculty
        public Difficulty Difficulty { get; private set; }
        #endregion // Members

        #region Get Computer Opponents
        public static IEnumerable<ComputerOpponents> GetComputerOpponents()
        {
            var computerOpponents = new List<ComputerOpponents>();

            // Read all data about players from XML File
            var xmlReader = new XmlTextReader(@"../../Resources/ComputerOpponentsList.xml");
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlReader);
            xmlReader.Close();

            // Make Computer Opponents List
            foreach (XmlNode xmlTag in xmlDocument.GetElementsByTagName("ComputerOpponent"))
            {
                try
                {
                    if (xmlTag.Attributes != null)
                    {
                        var computerOpponent = new ComputerOpponents
                        {
                            Name = xmlTag.Attributes["name"].Value,
                            Avatar = $"players/{xmlTag.Attributes["avatar"].Value}"
                        };

                        switch (xmlTag.Attributes["difficulty"].Value.ToUpper())
                        {
                            case "NORMAL":
                                computerOpponent.Difficulty = Difficulty.Normal;
                                break;

                            case "HARD":
                                computerOpponent.Difficulty = Difficulty.Hard;
                                break;

                            //case "EASY":
                            default:
                                computerOpponent.Difficulty = Difficulty.Easy;
                                break;
                        }

                        computerOpponents.Add(computerOpponent);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(@"Error -> Make computer opponents list with message {0}", e.Message);
                }
            }

            return computerOpponents;
        }
        #endregion // Get Computer Opponents
    }
}