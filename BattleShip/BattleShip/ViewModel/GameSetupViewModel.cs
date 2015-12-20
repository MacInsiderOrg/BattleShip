using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BattleShip.Annotations;
using BattleShip.Model.Cached;
using BattleShip.Model.Service;
using BattleShip.ViewModel.Players;
using GameEngine;
using GameEngine.Attacks;
using GameEngine.Levels;
using GameEngine.Players;
using GameEngine.Ships;
using Image = System.Windows.Controls.Image;
using BattleShip.BattleShipService;
using BattleShip.Properties;

namespace BattleShip.ViewModel
{
    public sealed class GameSetupViewModel : INotifyPropertyChanged
    {
        #region Game Setup Fields

        private readonly string _playerAvatarsPath;
        private readonly Dictionary<ListBoxItem, ComputerOpponents> _computerOpponentses;
        private BattleShipGame _battleShipGame;
        private HumanPlayer _humanPlayer;
        private readonly MediaFactory _mediaFactory;

        public Ship[] HumanPlayerShips;
        public Ship[] ComputerPlayerShips;

        public Attack[,] HumanPlayerAttacks;
        public Attack[,] ComputerPlayerAttacks;

        public UiState CurrentState;

        #endregion

        #region Game Setup Constructors
        public GameSetupViewModel() {}
        
        public GameSetupViewModel(string playerName, ListBox humanPlayerAvatar, ListBox computerPlayer, object humanPlayerAvatarItem, object computerPlayerItem)
        {
            _playerAvatarsPath = @"../../Resources/images/players";
            _computerOpponentses = new Dictionary<ListBoxItem, ComputerOpponents>();
            _mediaFactory = MediaFactory.Init;

            PlayerName = playerName;
            HumanPlayerAvatar = humanPlayerAvatar;
            ComputerPlayer = computerPlayer;
            HumanPlayerAvatarItem = (ListBoxItem) humanPlayerAvatarItem;
            ComputerPlayerItem = (ListBoxItem) computerPlayerItem;

            PopulatePlayersImages();
            PopulateComputerOpponents();
        }
        #endregion // Game Setup Constructors
        
        #region Resume Game
        public void ResumeGame(HumanPlayer humanPlayer, IPlayer computerPlayer)
        {
            _humanPlayer = humanPlayer;
            _humanPlayer.SetComputerPlayer(ref computerPlayer);

            var boardDimensions = new Coordinate(9, 9);
            
            _battleShipGame = new BattleShipGame(
                computerPlayer,
                _humanPlayer,
                boardDimensions,
                HumanPlayerShips,
                ComputerPlayerShips,
                HumanPlayerAttacks,
                ComputerPlayerAttacks,
                CurrentState
            );
        }
        #endregion // Resume Game

        #region Initialize Game
        public void InitializeGame()
        {
            IPlayer computerPlayer = GetOpponentPlayer();

            _humanPlayer = GetHumanPlayer();
            _humanPlayer.SetComputerPlayer(ref computerPlayer);

            var boardDimensions = new Coordinate(9, 9);

            _battleShipGame = new BattleShipGame(
                computerPlayer,
                _humanPlayer,
                boardDimensions
            );
        }
        #endregion // Initialize Game

        #region Start Game
        public void StartGame()
        {
            try
            {
                var gameService = GameService.Init;

                Settings.Default.PlayerName = PlayerName;
                if (gameService.GetPlayerByName(PlayerName) == null)
                {
                    gameService.AddPlayer(
                        new Player
                        {
                            Id = 0,
                            Name = PlayerName,
                            DateOfRegister = DateTime.Now
                        }
                    );
                }
            }
            catch
            {
                MessageBox.Show(@"Your server closed, all information about your games cannot writing to the database...", @"Server Message",
                        MessageBoxButton.OK);
            }

            var thread = new Thread(_battleShipGame.RunGame);
            thread.Start();
            _humanPlayer.ShowDialog();
            thread.Abort();
        }
        #endregion // Start Game

        #region Binding Properties

        #region Human Player Properties

        private string _playerName;
        public string PlayerName
        {
            get { return _playerName; }
            set
            {
                _playerName = value;
                OnPropertyChanged(nameof(PlayerName));
            }
        }

        private ListBox _humanPlayerAvatar;
        public ListBox HumanPlayerAvatar
        {
            get { return _humanPlayerAvatar; }
            set
            {
                _humanPlayerAvatar = value;
                OnPropertyChanged(nameof(HumanPlayerAvatar));
            }
        } 

        private ListBoxItem _humanPlayerAvatarItem;
        public ListBoxItem HumanPlayerAvatarItem
        {
            get { return _humanPlayerAvatarItem; }
            set
            {
                _humanPlayerAvatarItem = value;
                OnPropertyChanged(nameof(HumanPlayerAvatarItem));
            }
        }
        #endregion // Human Player Properties

        #region Computer Player Properties

        private ListBox _computerPlayer;
        public ListBox ComputerPlayer
        {
            get { return _computerPlayer; }
            set
            {
                _computerPlayer = value;
                OnPropertyChanged(nameof(ComputerPlayer));
            }
        }

        private ListBoxItem _computerPlayerItem;
        public ListBoxItem ComputerPlayerItem
        {
            get { return _computerPlayerItem; }
            set
            {
                _computerPlayerItem = value;
                OnPropertyChanged(nameof(ComputerPlayerItem));
            }
        }
        #endregion //Computer Player Properties

        #endregion // Game Setup Properties

        #region Populate images for Human Player
        private void PopulatePlayersImages()
        {
            var imagesCount = Array.FindAll(Directory.GetFiles(_playerAvatarsPath), IsPlayerImage).Length;

            for (var i = 1; i <= imagesCount; i++)
            {
                HumanPlayerAvatar.Items.Add(GetImageItem(_mediaFactory.GetImage($"players/player_{i}")));
            }
        }

        private bool IsPlayerImage(string imagePath)
        {
            var imageName = imagePath.Substring(imagePath.LastIndexOf("\\", StringComparison.Ordinal) + 1);
            return imageName.StartsWith("player_") && imageName.EndsWith(".png");
        }
        #endregion // Populate images for Human Player

        #region Get Image by image path
        private ListBoxItem GetImageItem(BitmapImage imagePath)
        {
            var image = new Image
            {
                Source = imagePath,
                Stretch = Stretch.Uniform
            };

            var imageItem = new ListBoxItem
            {
                Height = 120,
                Content = image
            };

            return imageItem;
        }
        #endregion // Get Image by image path

        #region Get Players Info
        private HumanPlayer GetHumanPlayer()
        {
            var bitmapImage = ((Image)(HumanPlayerAvatarItem).Content).Source as BitmapImage;
            var bitmapSource = bitmapImage?.UriSource.ToString();

            var playerImage = bitmapSource?.Remove(0, bitmapSource.IndexOf("players", StringComparison.CurrentCulture));
            playerImage = playerImage?.Substring(0, playerImage.IndexOf('.'));
            
            return new HumanPlayer(PlayerName, playerImage);
        }

        private ComputerPlayer GetOpponentPlayer()
        {
            var computerOpponent = _computerOpponentses[ComputerPlayerItem];
            var avatarLocation = computerOpponent.Avatar;

            switch (_computerOpponentses[ComputerPlayerItem].Difficulty)
            {
                case Difficulty.Normal:
                    return new NormalLevel(computerOpponent.Name, avatarLocation);

                case Difficulty.Hard:
                    return new HardLevel(computerOpponent.Name, avatarLocation);

                default:
                    return new EasyLevel(computerOpponent.Name, avatarLocation);
            }
        }
        #endregion // Get Players Info
        
        #region Populate Computer Opponents
        private void PopulateComputerOpponents()
        {
            var computerOpponents = ComputerOpponents.GetComputerOpponents();

            foreach (var computerOpponent in computerOpponents)
            {
                var item = GetOpponentItem(computerOpponent);
                _computerOpponentses.Add(item, computerOpponent);
                ComputerPlayer.Items.Add(item);
            }
        }
        #endregion // Populate Computer Opponents

        #region Get Opponent by selected item
        private ListBoxItem GetOpponentItem(ComputerOpponents computerOpponent)
        {
            var image = new Image
            {
                Source =  _mediaFactory.GetImage(computerOpponent.Avatar),
                Stretch = Stretch.Uniform
            };

            var label = new Label
            {
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Content = computerOpponent.Name
            };

            var dockPanel = new DockPanel
            {
                Height = 50,
                Margin = new Thickness(0, 2, 0, 2),
                Children = {image, label}
            };

            var imageItem = new ListBoxItem
            {
                Background = new SolidColorBrush(GetOpponentColor(computerOpponent.Difficulty)),
                Content = dockPanel
            };

            return imageItem;
        }
        #endregion // Get Opponent by selected item

        #region Get Opponent Color by Difficulty
        private Color GetOpponentColor(Difficulty difficulty)
        {
            var easyColor = Color.FromRgb(38, 204, 135);
            var normalColor = Color.FromRgb(235, 176, 56);
            var hardColor = Color.FromRgb(229, 53, 53);

            switch (difficulty)
            {
                case Difficulty.Normal:
                    return Color.FromScRgb(0.7f, normalColor.ScR, normalColor.ScG, normalColor.ScB);

                case Difficulty.Hard:
                    return Color.FromScRgb(0.7f, hardColor.ScR, hardColor.ScG, hardColor.ScB);

                //case Difficulty.Easy:
                default:
                    return Color.FromScRgb(0.7f, easyColor.ScR, easyColor.ScG, easyColor.ScB);
            }
        }
        #endregion // Get Opponent Color by Difficulty

        #region INotify Property Changed Members

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // INotify Property Changed Members
    }
}
