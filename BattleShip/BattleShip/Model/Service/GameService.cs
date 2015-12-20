using BattleShip.BattleShipService;

namespace BattleShip.Model.Service
{
    public class BattleShipService : IGameService
    {
        private readonly IGameStatisticsService _gameStatisticsService;
        private readonly IPlayerService _playerService;

        public BattleShipService()
        {
            _gameStatisticsService = new GameStatisticsServiceClient();
            _playerService = new PlayerServiceClient();
        }

        // Add player to Players Table
        public void AddPlayer(Player player)
        {
            _playerService.AddPlayer(player);
        }

        // Get information about player from database
        public Player GetPlayerByName(string name)
        {
            return _playerService.GetPlayerByName(name);
        }

        // Add game statistic after game finished
        public void AddStatistic(GameStatistic gameStatistic)
        {
            _gameStatisticsService.AddStatistic(gameStatistic);
        }

        // Get statistics about previous succeded games
        public GameStatistic[] GetStatisticsByPlayerName(string name)
        {
            return _gameStatisticsService.GetStatisticsByPlayerName(name);
        }
    }
}
