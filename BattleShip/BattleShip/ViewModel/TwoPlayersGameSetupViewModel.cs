using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BattleShip.Annotations;
using BattleShip.BattleShipService;
using BattleShip.Model.Cached;
using BattleShip.Model.Service;
using BattleShip.Properties;
using BattleShip.ViewModel.Players;
using GameEngine;
using GameEngine.Levels;
using GameEngine.Players;
using GameEngine.Ships;
using Image = System.Windows.Controls.Image;


namespace BattleShip.ViewModel
{
    public sealed class TwoPlayersGameSetupViewModel : INotifyPropertyChanged
    {
        #region Game Setup Fields

        private readonly string _playerAvatarsPath;
        private BattleShipGame _battleShipGame;
        private HumanPlayer _humanPlayer;
        private readonly MediaFactory _mediaFactory;

        #endregion
        
        #region Game Setup Constructor

        public TwoPlayersGameSetupViewModel(string playerName, ListBox humanPlayerAvatar, object humanPlayerAvatarItem)
        {
            _playerAvatarsPath = @"../../Resources/images/players";
            _mediaFactory = MediaFactory.Init;

            PlayerName = playerName;
            HumanPlayerAvatar = humanPlayerAvatar;
            HumanPlayerAvatarItem = (ListBoxItem) humanPlayerAvatarItem;

            PopulatePlayersImages();
        }

        #endregion // Game Setup Constructor

        #region Binding Properties

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

        #endregion // Binding Properties

        #region Initialize Game

        public void InitializeGame()
        {
            
            IPlayer computerPlayer = new NormalLevel(PlayerName, GetHumanPlayer().PlayerAvatar);

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

        #endregion // Get Players Info

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}