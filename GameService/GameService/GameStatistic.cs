using System;
using System.Runtime.Serialization;

namespace GameService
{
    [DataContract]
    public class GameStatistic
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public Player Player { get; set; }

        [DataMember]
        public DateTime DateOfGame { get; set; }

        [DataMember]
        public bool GameStatus { get; set; }
    }
}