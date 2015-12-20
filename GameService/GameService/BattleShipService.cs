using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace GameService
{
    public class BattleShipService : IPlayerService, IGameStatisticsService
    {
        private readonly SqlConnection _connection;
        private SqlDataAdapter _dataAdapter;

        public BattleShipService()
        {
            const string connectionString =
                "Data Source=PC; Initial Catalog=BattleShipDB;Integrated Security=True;Pooling=True";
            _connection = new SqlConnection(connectionString);
        }

        private void OpenConnection()
        {
            try
            {
                _connection.Open();
            }
            catch (SqlException)
            {
                Console.WriteLine(@"Cannot connection to the database");
            }
        }

        private void CloseConnection()
        {
            _connection.Close();
        }

        public void AddPlayer(Player player)
        {
            OpenConnection();

            try
            {
                var query = $"INSERT INTO Players VALUES ('{player.Name}', '{player.DateOfRegister.ToString(CultureInfo.CurrentCulture)}');";
                var insertCommand = new SqlCommand(query, _connection);
                insertCommand.ExecuteNonQuery();
            }
            catch
            {
                CloseConnection();
            }

            if (_connection != null)
                CloseConnection();
        }

        public Player GetPlayerByName(string name)
        {
            OpenConnection();
            var dataSet = new DataSet("Player");

            try
            {
                var command = _connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = $"SELECT * FROM Players WHERE player_name = '{name}';";

                _dataAdapter = new SqlDataAdapter(command);
                _dataAdapter.Fill(dataSet);
                command.ExecuteNonQuery();
            }
            catch
            {
                CloseConnection();
            }

            if (_connection != null)
                CloseConnection();

            try
            {
                var currentRow = dataSet.Tables[0].Rows[0];

                return new Player
                {
                    Id = Convert.ToInt32(currentRow.ItemArray[0].ToString()),
                    Name = currentRow.ItemArray[1].ToString(),
                    DateOfRegister = Convert.ToDateTime(currentRow.ItemArray[2].ToString())
                };
            }
            catch
            {
                return null;
            }
        }

        public void AddStatistic(GameStatistic gameStatistic)
        {
            OpenConnection();

            try
            {
                var gameStatus = (gameStatistic.GameStatus) ? "True" : "False";
                var query = "INSERT INTO GameStatistics VALUES " +
                            $"({gameStatistic.Player.Id}, " +
                            $"'{gameStatistic.DateOfGame.ToString(CultureInfo.CurrentCulture)}', " +
                            $"'{gameStatus}');";
                var insertCommand = new SqlCommand(query, _connection);
                insertCommand.ExecuteNonQuery();
            }
            catch
            {
                CloseConnection();
            }

            if (_connection != null)
                CloseConnection();
        }

        public ObservableCollection<GameStatistic> GetStatisticsByPlayerName(string name)
        {
            OpenConnection();
            var dataSet = new DataSet("GameStatistics");

            try
            {
                var command = _connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT stat_ID, GameStatistics.player_ID, Players.player_name, " +
                                      "Players.dateof_register, dateof_game, game_status " +
                                      "FROM GameStatistics JOIN Players " +
                                      "ON GameStatistics.player_ID = Players.player_ID " +
                                      $"WHERE Players.player_name = '{name}';";

                _dataAdapter = new SqlDataAdapter(command);
                _dataAdapter.Fill(dataSet);
                command.ExecuteNonQuery();
            }
            catch
            {
                CloseConnection();
            }

            if (_connection != null)
                CloseConnection();

            var gameStatistics = new ObservableCollection<GameStatistic>();

            for (var i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                var currentRow = dataSet.Tables[0].Rows[i];

                gameStatistics.Add(
                    new GameStatistic
                    {
                        Id = Convert.ToInt32(currentRow.ItemArray[0]),
                        Player = new Player
                        {
                            Id = Convert.ToInt32(currentRow.ItemArray[1]),
                            Name = currentRow.ItemArray[2].ToString(),
                            DateOfRegister = Convert.ToDateTime(currentRow.ItemArray[3])
                        },
                        DateOfGame = Convert.ToDateTime(currentRow.ItemArray[4]),
                        GameStatus = Convert.ToBoolean(currentRow.ItemArray[5])
                    }
                );
            }

            return gameStatistics;
        }
    }
}