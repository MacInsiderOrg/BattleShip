using System;
using System.Runtime.Serialization;

namespace GameService
{
    [DataContract]
    public class Player
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public DateTime DateOfRegister { get; set; }
    }
}