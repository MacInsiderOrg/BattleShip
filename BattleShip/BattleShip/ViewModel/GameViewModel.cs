using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BattleShip.Annotations;
using BattleShip.BattleShipService;
using BattleShip.Model.Cached;
using BattleShip.Model.Converters;
using BattleShip.Model.Preferences.Design;
using BattleShip.Model.Service;
using BattleShip.Properties;
using BattleShip.View;
using BattleShip.ViewModel.Commands;
using GameEngine.Attacks;
using GameEngine.EventArgs;
using GameEngine.Helpers;
using GameEngine.Players;
using GameEngine.Ships;
using Image = System.Windows.Controls.Image;
using GameEngine;
using GameEngine.Serialization;
using BattleShip.Model.Preferences;

namespace BattleShip.ViewModel
{
    public sealed class GameViewModel : INotifyPropertyChanged
    {
        #region Game View Model Constructor

        public GameViewModel(string playerName, string playerAvatar, Grid shipBoard, Grid hitBoard, StackPanel humanPlayerShips, StackPanel computerPlayerShips, StackPanel notificationPanel)
        {
            PlayerName = playerName;
            PlayerAvatar = playerAvatar;

            ShipBoard = shipBoard;
            HitBoard = hitBoard;

            HumanPlayerShips = humanPlayerShips;
            ComputerPlayerShips = computerPlayerShips;

            NotificationPanel = notificationPanel;

            _shipOrientationIsVertical = true;
            _placedShips = new List<Image>();
            UiState = UiState.WaitingToPlace;
            IsReady = false;

            _mediaFactory = MediaFactory.Init;
            _sunkEnemyShips = 0;

            SetupGameDesign();
        }

        public bool IsReadyToDo()
        {
            return IsReady;
        }

        #endregion // GameViewModel Constructor

        #region Current Player Information

        public string PlayerName { get; set; }
        public string PlayerAvatar { get; set; }
        public Ship[] Ships { get; set; }
        public Attack[,] Attacks { get; set; }
        public Coordinate MaxCoordinates { get; set; }

        #endregion // Current Player Information

        #region Game General Members

        private IPlayer _computerPlayer;
        private GameInfoHelper _gameInfoHelper;
        public bool IsReady;
        public UiState UiState;
        private readonly List<Image> _placedShips;
        private Border _selectedBorder;
        private Rectangle _highlightGridImage;
        private Image _highlightCenterImageGrid;
        private bool _shipOrientationIsVertical;
        private IAppDesign _appDesign;
        private readonly MediaFactory _mediaFactory;

        private int _sunkEnemyShips;

        #endregion

        #region Events (ShipsPlaced, AttackMade)

        /* Event declarations */
        public delegate void ShipsPlacedEventHandler(object sender, ShipsPlacedEventArgs e);
        public delegate void AttackMadeEventHandler(object sender, AttackMadeEventArgs e);

        public event ShipsPlacedEventHandler ShipsPlaced;
        public event AttackMadeEventHandler AttackMade;

        // Invoke the Piece Placed event
        public void OnShipsPlaced(ShipsPlacedEventArgs e)
        {
            ShipsPlaced?.Invoke(this, e);
        }

        // Invoke the Attack Made event
        private void OnAttackMade(AttackMadeEventArgs e)
        {
            AttackMade?.Invoke(this, e);
        }

        private void SendShipsPlaced(ShipsPlacedEventArgs shipsPlacedEventArgs)
        {
            ShipsPlaced?.Invoke(this, shipsPlacedEventArgs);
        }

        #endregion EndEvents

        #region Initialize current game

        public void Init(Coordinate maxCoordinates, Ship[] startingShips)
        {
            // Initialize Properties
            Ships = startingShips;

            MaxCoordinates = maxCoordinates;
            var nothingAttacks = false;

            if (Attacks == null)
            {
                Attacks = new Attack[maxCoordinates.X + 1, maxCoordinates.Y + 1];
                nothingAttacks = true;
            }

            // Initialize Helpers
            _gameInfoHelper = new GameInfoHelper();

            // Generate Attacks
            if (nothingAttacks)
                for (var x = 0; x <= maxCoordinates.X; x++)
                    for (var y = 0; y <= maxCoordinates.Y; y++)
                        Attacks[x, y] = new Attack();

            InitializeGrids();
            InitializeShips();

            if (!nothingAttacks)
                Ships = startingShips;

            ResetShipsEnabled = false;
            ResetShipsVisibility = false;
        }

        #endregion

        #region Initialize Grids

        private void InitializeGrids()
        {
            Grid[] grids =
            {
                ShipBoard,
                HitBoard
            };

            var attackTables = new List<Attack[,]>
            {
                _computerPlayer.Attacks,
                Attacks
            };

            for (var i = 0; i < grids.Length; i++)
            {
                /* Set Rows and Columns */
                for (var j = 0; j <= MaxCoordinates.Y; j++)
                {
                    grids[i].RowDefinitions.Add(
                        new RowDefinition
                        {
                            Height = new GridLength(1, GridUnitType.Star)
                        }
                    );
                }

                for (var j = 0; j <= MaxCoordinates.X; j++)
                {
                    grids[i].ColumnDefinitions.Add(
                        new ColumnDefinition
                        {
                            Width = new GridLength(1, GridUnitType.Star)
                        }
                    );
                }

                /* Fill the grid with images */
                for (var x = 0; x <= MaxCoordinates.X; x++)
                {
                    for (var y = 0; y <= MaxCoordinates.Y; y++)
                    {
                        var bindToAttackResult = new Binding("Result")
                        {
                            Source = attackTables[i][x, y],
                            Mode = BindingMode.OneWay,
                            Converter = new AttackResultConverter()
                        };

                        var image = new Image
                        {
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            Stretch = Stretch.UniformToFill
                        };

                        image.SetBinding(Image.SourceProperty, bindToAttackResult);

                        Grid.SetRow(image, y);
                        Grid.SetColumn(image, x);

                        // Set GridImage Events
                        image.MouseEnter += GridImage_MouseEnter;

                        grids[i].Children.Add(image);
                    }
                }

                // Set Grid Events
                grids[i].MouseLeave += Grid_MouseLeave;
            }
        }

        #endregion

        #region Initialize Ships

        private void InitializeShips()
        {
            StackPanel[] shipPanels =
            {
                HumanPlayerShips,
                ComputerPlayerShips
            };

            Ship[][] shipsList =
            {
                Ships,
                _computerPlayer.Ships
            };

            for (var i = 0; i < shipPanels.Length; i++)
            {
                foreach (var ship in shipsList[i])
                {
                    // Create border for Grid
                    var border = new Border
                    {
                        Name = ship.Name,
                        BorderThickness = new Thickness(3),
                        CornerRadius = new CornerRadius(3)
                    };

                    // Set Events
                    if (Equals(shipPanels[i], HumanPlayerShips))
                    {
                        border.MouseLeftButtonUp += ShipBorder_MouseLeftButtonUp;
                        border.MouseEnter += Border_MouseEnter;
                        border.MouseLeave += Border_MouseLeave;
                    }

                    // Create Grid as a Ship
                    var gridShip = new Grid()
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Width = 35 * ship.Sections.Length
                    };

                    // Set Columns
                    foreach (var columnDefinition in ship.Sections.Select(
                        section => new ColumnDefinition
                        {
                            Width = new GridLength(1, GridUnitType.Star)
                        }
                    ))
                    {
                        gridShip.ColumnDefinitions.Add(columnDefinition);
                    }

                    // Add Ship Image
                    var shipImage = new Image
                    {
                        Source = _mediaFactory.GetImage(GetImageForShip(ship)),
                        Stretch = Stretch.UniformToFill,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    Grid.SetRow(shipImage, 0);
                    Grid.SetColumn(shipImage, 0);
                    Grid.SetColumnSpan(shipImage, ship.Sections.Length);
                    gridShip.Children.Add(shipImage);

                    // Add AttackResult Images
                    for (var j = 0; j < ship.Sections.Length; j++)
                    {
                        var bindToIsDamaged = new Binding("IsDamaged")
                        {
                            Source = ship.Sections[j],
                            Converter = new SectionIsDamagedConverter(),
                            Mode = BindingMode.OneWay
                        };

                        var image = new Image();
                        image.SetBinding(Image.SourceProperty, bindToIsDamaged);

                        image.Stretch = Stretch.UniformToFill;
                        image.HorizontalAlignment = HorizontalAlignment.Center;
                        image.VerticalAlignment = VerticalAlignment.Center;

                        Grid.SetRow(image, 0);
                        Grid.SetColumn(image, j);
                        gridShip.Children.Add(image);
                    }

                    border.Child = gridShip;
                    shipPanels[i].Children.Add(border);
                }
            }
        }

        #endregion // Initialize Ships

        #region Players Name and Avatars Properties

        public string HumanPlayerName => PlayerName;

        public ImageSource HumanPlayerImageSource => _mediaFactory.GetImage(PlayerAvatar);

        public string ComputerPlayerName => _computerPlayer.PlayerName;

        public ImageSource ComputerPlayerImageSource => _mediaFactory.GetImage(_computerPlayer.PlayerAvatar);

        #endregion // Players Name and Avatars Properties
        
        #region Design Properties

        public ImageSource BackgroundImageSource => _appDesign.BackgroundImage;

        public ImageSource PlayerPanelImageSource => _appDesign.PlayerPanelImage;
        
        #endregion // Design Properties

        #region Game Status Control Properties

        public string CurrentGameStatus => GetEnumDescription(UiState);

        public int HumanPlayerSunkShips => Ships.Count(ship => ship.IsDestroyed());

        public int ComputerPlayerSunkShips
        {
            get
            {
                var shipsDestroyed = _computerPlayer.Ships.Count(ship => ship.IsDestroyed());

                if (shipsDestroyed > _sunkEnemyShips)
                {
                    _sunkEnemyShips = shipsDestroyed;
                    _mediaFactory.PlayMedia("sunk");
                }

                return shipsDestroyed;
            }
        }

        #endregion // Game Status Control Properties

        #region Preferences Panel Members

        public bool IsBackgroundMusic
        {
            get { return Settings.Default.BackgroundMusic; }
            set
            {
                Settings.Default.BackgroundMusic = value;
                AppSettings.BackgroundMusic();
                Settings.Default.Save();

                OnPropertyChanged(nameof(IsBackgroundMusic));
            }
        }

        public bool IsAttacksSounds
        {
            get { return Settings.Default.AttacksSounds; }
            set
            {
                Settings.Default.AttacksSounds = value;
                Settings.Default.Save();

                OnPropertyChanged(nameof(IsAttacksSounds));
            }
        }

        #endregion // Preferences Panel Members

        #region Reset Ships Command

        private RelayCommand _resetShipsCommand;
        public ICommand ResetShipsCommand
        {
            get
            {
                return _resetShipsCommand ?? (
                    _resetShipsCommand = new RelayCommand(
                        param => ResetShipsPlacement()
                    )
                );
            }
        }

        private void ResetShipsPlacement()
        {
            ClearShipsBoard();
            PlaceShipsEnabled = false;
            PlaceShipsVisibility = false;
        }

        #endregion

        #region Place Ships Command

        private RelayCommand _placeShipsCommand;
        public ICommand PlaceShipsCommand
        {
            get
            {
                return _placeShipsCommand ?? (
                    _placeShipsCommand = new RelayCommand(
                        param => PlaceShipsExecute()
                    )
                );
            }
        }

        public void PlaceShipsExecute()
        {
            ResetShipsEnabled = false;
            ResetShipsVisibility = false;
            SendShipsPlaced(new ShipsPlacedEventArgs());
            GameWindow.GameStarted = true;
        }

        #endregion // Place Ships Command

        #region Visibility Buttons

        /* Visibility button which activate action - place ships */
        private bool _placeShipsVisibility;
        public bool PlaceShipsVisibility
        {
            get { return _placeShipsVisibility; }
            set
            {
                _placeShipsVisibility = value;
                OnPropertyChanged(nameof(PlaceShipsVisibility));
            }
        }

        /* Visibility button which activate action - reset ships */
        private bool _resetShipsVisibility;
        public bool ResetShipsVisibility
        {
            get
            {
                return _resetShipsVisibility;
            }
            set
            {
                _resetShipsVisibility = value;
                OnPropertyChanged(nameof(ResetShipsVisibility));
            }
        }

        #endregion // Visibility Buttons

        #region Enabled Buttons

        /* Enabled button which activate action - place ships */
        private bool _placeShipsEnabled;
        public bool PlaceShipsEnabled
        {
            get { return _placeShipsEnabled; }
            set
            {
                _placeShipsEnabled = value;
                OnPropertyChanged(nameof(PlaceShipsEnabled));
            }
        }

        /* Enabled button which activate action - reset ships */
        private bool _resetShipsEnabled;
        public bool ResetShipsEnabled
        {
            get
            {
                return _resetShipsEnabled;
            }
            set
            {
                _resetShipsEnabled = value;
                OnPropertyChanged(nameof(ResetShipsEnabled));
            }
        }

        #endregion // Enabled Buttons

        #region Notification Panel

        private StackPanel _notificationPanel;

        public StackPanel NotificationPanel
        {
            get { return _notificationPanel; }
            set
            {
                _notificationPanel = value;
                OnPropertyChanged(nameof(NotificationPanel));
            }
        }

        #endregion // Notification Panel

        #region Ship Board

        private Grid _shipBoard;
        public Grid ShipBoard
        {
            get { return _shipBoard; }
            set
            {
                _shipBoard = value;
                OnPropertyChanged(nameof(ShipBoard));
            }
        }

        #endregion // Ship Board

        #region Hit Board

        private Grid _hitBoard;
        public Grid HitBoard
        {
            get { return _hitBoard; }
            set
            {
                _hitBoard = value;
                OnPropertyChanged(nameof(HitBoard));
            }
        }

        #endregion // Hit Board

        #region Human Player Ships

        private StackPanel _humanPlayerShips;

        public StackPanel HumanPlayerShips
        {
            get { return _humanPlayerShips; }
            set
            {
                _humanPlayerShips = value;
                OnPropertyChanged(nameof(HumanPlayerShips));
            }
        }

        #endregion // Human Player Ships

        #region Computer Player Ships

        private StackPanel _computerPlayerShips;

        public StackPanel ComputerPlayerShips
        {
            get { return _computerPlayerShips; }
            set
            {
                _computerPlayerShips = value;
                OnPropertyChanged(nameof(ComputerPlayerShips));
            }
        }

        #endregion // Computer Player Ships

        #region Setup Player opponent ships

        public void SetComputerPlayerShips(ref IPlayer computerPlayer)
        {
            _computerPlayer = computerPlayer;
        }

        #endregion

        #region Place All Ships Method

        public void PlaceAllShips()
        {
            switch (UiState)
            {
                case UiState.WaitingToPlace:
                    UiState = UiState.Placing;
                    ShowNotification("Place your ships on the board:\n" +
                        "Select a ship and place it on your board. Right click to rotate.\n" +
                        "Ships must be placed at least one square apart.");
                    break;

                case UiState.Placing:
                    ShowNotification("Bad Placements! Try again...\n" +
                       "Remember: ships must be placed at least one square apart.");
                    break;

                default:
                    HandleBadUIState();
                    break;
            }

            if (UiState == UiState.Attacking || UiState == UiState.GameFinished)
            {
                ResetShipsEnabled = false;
                ResetShipsVisibility = false;
            } else {
                ResetShipsEnabled = true;
                ResetShipsVisibility = true;
                ResetShipsPlacement();
            }

            OnPropertyChanged(nameof(CurrentGameStatus));
        }

        #endregion // Place All Ships

        #region Attack Method

        public void Attack()
        {
            switch (UiState)
            {
                case UiState.Placing:
                    PlaceShipsEnabled = false;
                    PlaceShipsVisibility = false;
                    UiState = UiState.Attacking;
                    break;

                case UiState.WaitingToAttack:
                    UiState = UiState.Attacking;
                    break;

                case UiState.Attacking:
                    return;

                default:
                    HandleBadUIState();
                    break;
            }

            OnPropertyChanged(nameof(CurrentGameStatus));
            ShowNotification($"Make an attack by clicking on {_computerPlayer.PlayerName}'s board...");
        }

        #endregion // Attack
        
        #region Update Attack Results Method

        public void UpdateAttackResults(Coordinate lastAttack, AttackResult result, bool sunkShip)
        {
            if (UiState == UiState.Attacking)
            {
                UiState = UiState.WaitingToAttack;
                ShowNotification($"Waiting for {_computerPlayer.PlayerName} to attack...");
            }
            else
            {
                HandleBadUIState();
            }

            // Update attacks and ui elements
            Attacks[lastAttack.X, lastAttack.Y].Result = result;
            OnPropertyChanged(nameof(CurrentGameStatus));
        }

        #endregion // Update Attack Results
        
        #region Send Winner Notification

        public void WinnerNotification(string winnerName)
        {
            if (UiState == UiState.WaitingToAttack)
            {
                UiState = UiState.GameFinished;
            }

            if (UiState != UiState.GameFinished)
            {
                HandleBadUIState();
            }

            ShowNotification(
                winnerName.Equals(PlayerName, StringComparison.Ordinal)
                ? $"Congratulations! {winnerName} won the game!!"
                : $"Sorry, {winnerName} won the game. Try Again!");

            try
            {
                var gameService = GameService.Init;

                gameService.AddStatistic(
                    new GameStatistic
                    {
                        Id = 0,
                        Player = gameService.GetPlayerByName(PlayerName),
                        DateOfGame = DateTime.Now,
                        GameStatus = winnerName.Equals(PlayerName)
                    }
                );
            }
            catch
            {
                MessageBox.Show(@"Your server closed, this result cannot write to the database...", @"Server Message",
                        MessageBoxButton.OK);
            }

            OnPropertyChanged(nameof(CurrentGameStatus));
        }

        #endregion // Send Winner Notification

        #region Grid Image UI Events

        #region Grid Image - Mouse Left Button Up

        private void HighlightGridImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var rectangle = (sender as Rectangle);
            var grid = (Grid) rectangle?.Parent;

            if (Equals(grid, ShipBoard))
            {
                if (UiState == UiState.Placing && _selectedBorder != null)
                {
                    var selectedShip =
                        Ships.First(section => section.Name.Equals(_selectedBorder.Name, StringComparison.Ordinal));

                    var col = Grid.GetColumn(_highlightGridImage);
                    var row = Grid.GetRow(_highlightGridImage);
                    var colSpan = Grid.GetColumnSpan(_highlightGridImage);
                    
                    var shipImage = new Image
                    {
                        Name = _selectedBorder.Name
                    };

                    shipImage.Source = _shipOrientationIsVertical
                        ? (ImageSource) RotateImage(_mediaFactory.GetImage(GetImageForShip(selectedShip))) 
                        : _mediaFactory.GetImage(GetImageForShip(selectedShip));

                    shipImage.Stretch = Stretch.UniformToFill;
                    shipImage.HorizontalAlignment = HorizontalAlignment.Center;
                    shipImage.VerticalAlignment = VerticalAlignment.Center;

                    Grid.SetRow(shipImage, row);
                    Grid.SetColumn(shipImage, col);

                    if (colSpan > 1)
                    {
                        Grid.SetColumnSpan(shipImage, selectedShip.Sections.Length);
                    }
                    else
                    {
                        Grid.SetRowSpan(shipImage, selectedShip.Sections.Length);
                    }

                    grid?.Children.Insert(0, shipImage);
                    _placedShips.Add(shipImage);

                    for (var i = 0; i < selectedShip.Sections.Length; i++)
                    {
                        if (colSpan > 1)
                        {
                            selectedShip.Sections[i].ShipCoordinate.X = col + i;
                            selectedShip.Sections[i].ShipCoordinate.Y = row;
                        }
                        else
                        {
                            selectedShip.Sections[i].ShipCoordinate.X = col;
                            selectedShip.Sections[i].ShipCoordinate.Y = row + i;
                        }
                    }

                    UnhighlightBorder(_selectedBorder);
                    _selectedBorder = null;

                    foreach (var ship in Ships)
                    {
                        if (
                            ship.Sections.Count(
                                section => section.ShipCoordinate.X == -100 || section.ShipCoordinate.Y == -100) <=
                            0)
                        {
                            var image =
                                _placedShips.Single(
                                    placeShip => placeShip.Name.Equals(ship.Name, StringComparison.Ordinal));

                            if (image == null)
                                return;
                        }
                        else
                        {
                            return;
                        }
                    }

                    PlaceShipsEnabled = true;
                    PlaceShipsVisibility = true;
                    ShowNotification("Click \"Place Ships\" to continue...");
                }
            }

            if (Equals(grid, HitBoard) && UiState == UiState.Attacking)
            {
                if (rectangle != null)
                    OnAttackMade(
                        new AttackMadeEventArgs(
                            new Coordinate(
                                Grid.GetColumn(rectangle),
                                Grid.GetRow(rectangle)
                            )
                        )
                    );
            }
        }

        #endregion // Grid Image - Mouse Left Button Up

        #region Grid Image - Mouse Right Button Up

        private void HighlightGridImage_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var grid = (Grid) (sender as Rectangle)?.Parent;
            var isPlacing = Equals(UiState, UiState.Placing);
            var shipSelected = (_selectedBorder != null);
            var isShipBoard = Equals(grid, ShipBoard);

            if (isPlacing && shipSelected && isShipBoard)
            {
                UnhighlightImageGrid(grid);
                _shipOrientationIsVertical = !_shipOrientationIsVertical;
                HighlightImageGrid(_highlightCenterImageGrid);
            }
        }

        #endregion // Grid Image - Mouse Right Button Up

        #region Grid Image - Mouse Enter

        private void GridImage_MouseEnter(object sender, MouseEventArgs e)
        {
            var image = sender as Image;
            var grid = (Grid) image?.Parent;

            var isShipBoard = Equals(grid, ShipBoard);
            var isPlacing = Equals(UiState, UiState.Placing);
            var isAttacking = Equals(UiState, UiState.Attacking);
            var shipSelected = (_selectedBorder != null);

            if (isShipBoard && isPlacing && shipSelected)
            {
                UnhighlightImageGrid(grid);
                HighlightImageGrid(image);
            }
            else if (!isShipBoard && isAttacking)
            {
                HighlightImageGrid(image);
            }
        }

        #endregion // Grid Image - Mouse Enter

        #region Grid Image - Mouse Leave

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            UnhighlightImageGrid((Grid)sender);
        }

        #endregion // Grid Image - Mouse Leave

        #region Ship Border - Mouse Left Button Up

        private void ShipBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var border = (Border) sender;
            var isPlacing = Equals(UiState, UiState.Placing);

            var isPlaced = _placedShips.Any(ship =>
                ship.Name.Equals(border.Name, StringComparison.Ordinal)
                );

            var isSelected = (_selectedBorder != null) &&
                                (_selectedBorder.Name.Equals(border.Name, StringComparison.Ordinal));

            if (isPlacing && !isPlaced && !isSelected)
            {
                if (_selectedBorder != null)
                    UnhighlightBorder(_selectedBorder);

                _selectedBorder = border;
            }
        }

        #endregion // Ship Border - Mouse Left Button Up

        #region Ship Border - Mouse Enter

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            var border = (Border) sender;
            var isPlacing = Equals(UiState, UiState.Placing);

            var isPlaced = _placedShips.Any(ship =>
                ship.Name.Equals(border.Name, StringComparison.Ordinal)
                );

            var isSelected = (_selectedBorder != null) &&
                                (_selectedBorder.Name.Equals(border.Name, StringComparison.Ordinal));

            if (isPlacing && !isPlaced && !isSelected)
                HighlightBorder(border);
        }

        #endregion // Ship Border - Mouse Enter

        #region Ship Border - Mouse Leave

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            var border = (Border)sender;
            var isPlacing = Equals(UiState, UiState.Placing);

            var isPlaced = _placedShips.Any(ship =>
                ship.Name.Equals(border.Name, StringComparison.Ordinal)
            );

            var isSelected = (_selectedBorder != null) &&
                (_selectedBorder.Name.Equals(border.Name, StringComparison.Ordinal));

            if (isPlacing && !isPlaced && !isSelected)
                UnhighlightBorder(border);
        }

        #endregion // Ship Border - Mouse Leave

        #region Show Notification

        private void ShowNotification(string notify)
        {
            NotificationPanel.Children.Clear();
            var notifications = notify.Split('\n');

            // Make new textbox for each token and 
            // add it to the notification panel
            var fontSize = Convert.ToInt32(
                Math.Round(
                    (NotificationPanel.Height - 5)*.75/notifications.Length)
            );

            if (fontSize > 20)
                fontSize = 20;

            for (var i = 0; i < notifications.Length; i++)
            {
                var textBlock = new TextBlock
                {
                    Text = notifications[i],
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = fontSize,
                    FontFamily = new FontFamily("Arial")
                };

                if (i == 0)
                    textBlock.Margin = new Thickness(0, 5, 0, 0);

                NotificationPanel.Children.Add(textBlock);
            }
        }

        #endregion // Show Notification

        #region Highlight Border

        private static void HighlightBorder(Border border)
        {
            var borderBrush = new LinearGradientBrush();
            borderBrush.GradientStops.Add(new GradientStop(Colors.CadetBlue, 0.0));
            borderBrush.GradientStops.Add(new GradientStop(Colors.Beige, 0.5));
            borderBrush.GradientStops.Add(new GradientStop(Colors.CadetBlue, 1.0));

            border.BorderBrush = borderBrush;
            border.Background = new SolidColorBrush(Colors.WhiteSmoke) {Opacity = 0.3};
        }

        #endregion // Highlight Border

        #region Unhighlight Border

        private static void UnhighlightBorder(Border selectedBorder)
        {
            selectedBorder.BorderBrush = null;
            selectedBorder.Background = null;
        }

        #endregion Unhighlight Border

        #region Highlight Image Grid

        private void HighlightImageGrid(Image image)
        {
            try
            {
                if (image == null)
                    return;

                var grid = (Grid)image.Parent;
                int startCol, startRow;
                var colSpan = 1;
                var rowSpan = 1;

                /* Ensure highLightGridImage is set or reset */
                if (_highlightGridImage == null || _highlightCenterImageGrid == null)
                {
                    _highlightGridImage = new Rectangle
                    {
                        Fill = new SolidColorBrush(Colors.WhiteSmoke),
                        Opacity = 0.6,
                        Stretch = Stretch.Fill
                    };

                    _highlightGridImage.MouseRightButtonUp += HighlightGridImage_MouseRightButtonUp;
                    _highlightGridImage.MouseLeftButtonUp += HighlightGridImage_MouseLeftButtonUp;

                    _highlightCenterImageGrid = image;
                }

                /* Determine placement */
                if (Equals(grid, ShipBoard))
                {
                    /* Highlight ship's area with GridImage */
                    var coordinate = new Coordinate(Grid.GetColumn(image), Grid.GetRow(image));
                    var selectedShip =
                        Ships.First(ship => ship.Name.Equals(_selectedBorder.Name, StringComparison.Ordinal));
                    var halfLength = selectedShip.Sections.Length / 2;

                    if (_shipOrientationIsVertical)
                    {
                        startCol = coordinate.X;
                        startRow = Convert.ToInt32(Math.Round((double)(coordinate.Y - halfLength)));
                        rowSpan = selectedShip.Sections.Length;
                    }
                    else
                    {
                        startCol = Convert.ToInt32(Math.Round((double)(coordinate.X - halfLength)));
                        startRow = coordinate.Y;
                        colSpan = selectedShip.Sections.Length;
                    }
                }
                else
                {
                    // Only highlight the one gridImage
                    var coordinate = new Coordinate(Grid.GetColumn(image), Grid.GetRow(image));
                    startCol = coordinate.X;
                    startRow = coordinate.Y;
                }

                if (startCol >= 0 && startRow >= 0 &&
                    ((startRow + rowSpan <= MaxCoordinates.X + 1) && (startCol + colSpan <= MaxCoordinates.Y + 1)))
                {
                    Grid.SetColumn(_highlightGridImage, startCol);
                    Grid.SetRow(_highlightGridImage, startRow);
                    Grid.SetColumnSpan(_highlightGridImage, colSpan);
                    Grid.SetRowSpan(_highlightGridImage, rowSpan);

                    if (grid.Children.IndexOf(_highlightGridImage) == -1)
                        grid.Children.Add(_highlightGridImage);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception in HighlightImageGrid with message {e.Message}");
            }
        }

        #endregion // Highlight Image Grid

        #region Unhighlight Image Grid

        private void UnhighlightImageGrid(Panel grid)
        {
            grid.Children.Remove(_highlightGridImage);
            _highlightGridImage = null;
            _highlightCenterImageGrid = null;
        }

        #endregion // Unhighlight Image Grid

        #region Clear Ships Board

        private void ClearShipsBoard()
        {
            try
            {
                foreach (var ship in Ships)
                {
                    foreach (var section in ship.Sections)
                    {
                        section.ShipCoordinate.X = -100;
                        section.ShipCoordinate.Y = -100;
                    }
                }

                foreach (var placedShip in _placedShips)
                {
                    ((Grid) placedShip.Parent).Children.Remove(placedShip);
                }

                _placedShips.Clear();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        #endregion // Clear Ships Board

        #region Get Image For Ship

        private string GetImageForShip(Ship selectedShip)
        {
            return _gameInfoHelper.ShipImageLocation[selectedShip.Name];
        }

        #endregion // Get Image For Ship

        #region Rotate Image

        private TransformedBitmap RotateImage(BitmapImage bitmapImage)
        {
            // Properties must be set between BeginInit and EndInit calls.
            var transformedBitmap = new TransformedBitmap();
            transformedBitmap.BeginInit();
            transformedBitmap.Source = bitmapImage;

            // Set image rotation.
            var transform = new RotateTransform(270);
            transformedBitmap.Transform = transform;
            transformedBitmap.EndInit();
            return transformedBitmap;
        }

        #endregion // Rotate Image

        private void HandleBadUIState()
        {
        }

        #endregion

        #region Setup Game Design

        private void SetupGameDesign()
        {
            switch (Settings.Default.CurrentDesign)
            {
                case "Standart":
                    _appDesign = new StandartDesign();
                    break;

                case "Ultimate":
                    _appDesign = new UltimateDesign();
                    break;
            }
        }

        public void GenerateDesign()
        {
            var notificationImageBrush = new ImageBrush
            {
                Stretch = Stretch.UniformToFill,
                ImageSource = _appDesign.NotificationPanelImage
            };

            NotificationPanel.Background = notificationImageBrush;

            var shipBoardImageBrush = new ImageBrush
            {
                Stretch = Stretch.UniformToFill,
                ImageSource = _appDesign.ShipboardImage
            };

            ShipBoard.Background = shipBoardImageBrush;
            HitBoard.Background = shipBoardImageBrush;
        }

        private static string GetEnumDescription(Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            var attributes =
                (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        #endregion // Setup Game Design

        #region Game Serializer

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            SerializeGameData();
        }
        
        private void SerializeGameData()
        {
            var gameSerializer = new GameSerializer(PlayerName, PlayerAvatar, Attacks, Ships,
                MaxCoordinates, _computerPlayer, UiState);

            var battleShipData = gameSerializer.BattleShipData;

            BattleShipSerializer.SerializeData(@"../../Resources/BattleShipBackup.dat", battleShipData);
        }
        
        #endregion // Game Serializer

        #region INotify Property Changed Members

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // INotify Property Changed Members
    }
}