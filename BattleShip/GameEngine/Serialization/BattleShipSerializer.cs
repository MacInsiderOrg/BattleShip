using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GameEngine.Serialization
{
    public static class BattleShipSerializer
    {
        public static void SerializeData(string fileName, object battleShipData)
        {
            var formatter = new BinaryFormatter();
            var fileStream = new FileStream(fileName, FileMode.Create);
            formatter.Serialize(fileStream, battleShipData);
            fileStream.Close();
        }

        public static object DeserializeData(string fileName)
        {
            var fileStream = new FileStream(fileName, FileMode.Open);
            var formatter = new BinaryFormatter();
            return formatter.Deserialize(fileStream);
        }
    }
}