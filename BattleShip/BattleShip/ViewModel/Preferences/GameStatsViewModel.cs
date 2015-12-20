using System.Linq;
using System.Windows;
using BattleShip.Model.Service;
using BattleShip.Properties;

namespace BattleShip.ViewModel.Preferences
{
    public sealed class GameStatsViewModel
    {
        public object FinishedGames
        {
            get
            {
                try
                {
                    var gameService = GameService.Init;
                    var gameStatistics = gameService.GetStatisticsByPlayerName(Settings.Default.PlayerName);

                    return gameStatistics.ToList().Select(game => new
                    {
                        PlayerName = game.Player.Name,
                        Date = game.DateOfGame,
                        Result = (game.GameStatus) ? "Won" : "Lose"
                    }).ToList();
                }
                catch
                {
                    MessageBox.Show(@"Your server closed, please try again later...", @"Server Message",
                        MessageBoxButton.OK);
                    return null;
                }
            }
        }
    }
}
