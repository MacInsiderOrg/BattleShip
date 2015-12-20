using BattleShip.BattleShipService;

namespace BattleShip.Model.Service
{
    public interface IGameService
    {
        // Add player to Players Table
        void AddPlayer(Player player);

        // Get information about player from database
        Player GetPlayerByName(string name);

        // Add game statistic after game finished
        void AddStatistic(GameStatistic gameStatistic);

        // Get statistics about previous succeded games
        GameStatistic[] GetStatisticsByPlayerName(string name);
    }
}