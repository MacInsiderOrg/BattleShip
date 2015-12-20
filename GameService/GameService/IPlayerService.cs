using System.ServiceModel;

namespace GameService
{
    [ServiceContract]
    public interface IPlayerService
    {
        [OperationContract]
        void AddPlayer(Player player);

        [OperationContract]
        Player GetPlayerByName(string name);
    }
}