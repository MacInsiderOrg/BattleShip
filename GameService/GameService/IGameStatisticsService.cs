using System.Collections.ObjectModel;
using System.ServiceModel;

namespace GameService
{
    [ServiceContract]
    public interface IGameStatisticsService
    {
        [OperationContract]
        void AddStatistic(GameStatistic gameStatistic);

        [OperationContract]
        ObservableCollection<GameStatistic> GetStatisticsByPlayerName(string name);
    }
}