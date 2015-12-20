using System;
using GameEngine.Serialization;
using GameEngine;
using System.Windows;
using BattleShip.View;
using BattleShip.ViewModel.Players;

namespace BattleShip.ViewModel
{
    public sealed class GameInitViewModel
    {
        #region Constructor
        public GameInitViewModel(StartingWindow window)
        {
            try
            {
                var battleShipData =
                    BattleShipSerializer.DeserializeData(@"../../Resources/BattleShipBackup.dat") as BattleShipData;

                if (battleShipData != null && (battleShipData.CurrentState == UiState.Attacking ||
                                               battleShipData.CurrentState == UiState.WaitingToAttack))
                {
                    if (!Model.Preferences.AppSettings.GameSuccessful)
                    {
                        switch (MessageBox.Show(@"Do you want to resume the game", "Notification", MessageBoxButton.YesNo))
                        {
                            case MessageBoxResult.Yes:
                                DeserializeBattleShipData(battleShipData);
                                window.Close();
                                break;
                            case MessageBoxResult.No:
                                break;
                        }

                        Model.Preferences.AppSettings.GameSuccessful = true;
                    }
                }
            }
            catch
            {
                Model.Preferences.AppSettings.GameSuccessful = true;
                Console.WriteLine(@"Error in deserialize data from backup");
            }
        }
        #endregion // Costructor

        #region Deserialize Data
        private static void DeserializeBattleShipData(BattleShipData battleShipData)
        {
            var gameDeserializer = new GameDeserializer(battleShipData);

            var onePlayerGameSetup = new OnePlayerGameSetup
            {
                GameSetup = new GameSetupViewModel
                {
                    HumanPlayerShips = gameDeserializer.ComputerPlayer.Ships,
                    HumanPlayerAttacks = gameDeserializer.ComputerPlayer.Attacks,
                    ComputerPlayerShips = gameDeserializer.Ships,
                    ComputerPlayerAttacks = gameDeserializer.Attacks,
                    CurrentState = gameDeserializer.CurrentState
                }
            };

            onePlayerGameSetup.ResumeGame(
                new HumanPlayer(gameDeserializer.PlayerName, gameDeserializer.PlayerAvatar)
                {
                    PlayerName = gameDeserializer.PlayerName,
                    PlayerAvatar = gameDeserializer.PlayerAvatar,
                    Ships = gameDeserializer.Ships,
                    Attacks = gameDeserializer.Attacks,
                    MaxCoordinates = gameDeserializer.MaxCoordinates
                },
                gameDeserializer.ComputerPlayer
            );
        }
        #endregion // Deserialize Data
    }
}

#region Comments
/*
public class GameSetupModel
{
    public Ship[] HumanPlayerShips { get; set; }
    public Ship[] ComputerPlayerShips { get; set; }

    public Attack[,] HumanPlayerAttacks { get; set; }
    public Attack[,] ComputerPlayerAttacks { get; set; }

    public UiState CurrentState { get; set; }
}

private static void DeserializateBattleShipData(BattleShipData battleShipData)
{
    var gameDeserializer = new GameDeserializer(battleShipData);

    Mapper.CreateMap<GameSetupModel, GameSetupViewModel>()
        .ForMember(
            gameView => gameView.HumanPlayerAttacks,
            gameModel => gameModel.MapFrom(obj => obj.HumanPlayerAttacks)
        )
        .ForMember(
            gameView => gameView.ComputerPlayerAttacks,
            gameModel => gameModel.MapFrom(obj => obj.ComputerPlayerAttacks)
        );
    var gameSetup = DeserializeGameSetup(gameDeserializer);

    var onePlayerGameSetup = new OnePlayerGameSetup
    {
        GameSetup = Mapper.Map<GameSetupModel, GameSetupViewModel>(gameSetup)
    };

    onePlayerGameSetup.ResumeGame(
        new HumanPlayer(gameDeserializer.PlayerName, gameDeserializer.PlayerAvatar)
        {
            PlayerName = gameDeserializer.PlayerName,
            PlayerAvatar = gameDeserializer.PlayerAvatar,
            Ships = gameDeserializer.Ships,
            Attacks = gameDeserializer.Attacks,
            MaxCoordinates = gameDeserializer.MaxCoordinates
        },
        gameDeserializer.ComputerPlayer
    );
}

private static GameSetupModel DeserializeGameSetup(GameDeserializer gameDeserializer)
{
    return new GameSetupModel
    {
        HumanPlayerShips = gameDeserializer.ComputerPlayer.Ships,
        HumanPlayerAttacks = gameDeserializer.ComputerPlayer.Attacks,
        ComputerPlayerShips = gameDeserializer.Ships,
        ComputerPlayerAttacks = gameDeserializer.Attacks,
        CurrentState = gameDeserializer.CurrentState
    };
}*/
#endregion // Comments