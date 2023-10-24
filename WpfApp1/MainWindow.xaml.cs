using backend.Models.Entity;
using backend.Models.Entity.GameBoardExtensions;
using backend.Models.Entity.Ships;
using backend.Models.Entity.Ships.Factory;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Xaml.Behaviors;
using Shared;
using Shared.Transfer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Interaction = Microsoft.Xaml.Behaviors.Interaction;

//// if you want to update UI state, you need to call your change in this
/// as UI changes only allowed on the main thread, and this calls from the main thread. lol
//this.Dispatcher.Invoke(() =>
//{
//   myElement.Something = "lol"
//
//});
//

//todo: add game ending/winner
//todo: classes classes classes classes

//todo later: remove ship attack from dropdown if sunk

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool buttonClicked = false;
        private const int MAP_SIZE_X = 10;
        private const int MAP_SIZE_Y = 10;
        private static ShipFactory shipFactory = new ConcreteShipFactory();
        private Ship[] Ships = { 
            shipFactory.GetShip(ShipType.SmallShip), 
            shipFactory.GetShip(ShipType.MediumShip),
            shipFactory.GetShip(ShipType.BigShip),
            shipFactory.GetShip(ShipType.MediumShip).SetVertical(),
            shipFactory.GetShip(ShipType.BigShip).SetVertical(),
        };
        private Button[,] MyButtons = new Button[MAP_SIZE_X, MAP_SIZE_Y];
        private Button[,] EnemyButtons = new Button[MAP_SIZE_X, MAP_SIZE_Y];
        private Dictionary<int, Style> EnemeyBoardStyles = new Dictionary<int, Style>(); // TODO: use momento design pattern
        private string gameState = ""; // change to class ?
        private DispatcherTimer timer;
        private HubConnection _connection;

        public MainWindow()
        {
            InitializeComponent();
            InitializeUi();
            _connection = new HubConnectionBuilder()
             .WithUrl("http://localhost:5220/hub")
             .Build();

            Loaded += async (sender, e) => await ConnectToServer();
            InitializeBombAttackBox();
            SetupListeners();
        }

        private Button CreateButton(bool myBoard, int x, int y)
        {
            Button b = new Button();

            EnemeyBoardStyles.TryAdd(x * 10 + y, new Style()) ;

            this.Dispatcher.Invoke(() =>
            {
                b.IsEnabled = false;
            });

            if (myBoard)
            {
                b.ContextMenu = new ContextMenu();
                for (int i = 0; i < Ships.Length; i++)
                {
                    Ship ship = Ships[i];
                    ArrangementDto tag = new ArrangementDto(ship, x, y);
                    var item = new MenuItem { Header = ship.ToString(), Tag = tag };
                    this.Dispatcher.Invoke(() =>
                    {
                        item.Click += HandleShipArrangement;
                        if ((ship.IsVertical && y + ship.Size <= MAP_SIZE_Y) || 
                            (!ship.IsVertical && x + ship.Size <= MAP_SIZE_X))
                        {
                            b.ContextMenu.Items.Add(item);
                        }
                    });
                }
                this.Dispatcher.Invoke(() =>
                {
                    BehaviorCollection behaviors = Interaction.GetBehaviors(b);
                    behaviors.Add(new DropDownMenuBehavior());
                });
            }
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    b.Tag = new int[2] { x, y };
                    b.MouseEnter += HandleMouseEnter;
                    b.MouseLeave += HandleMouseLeave;
                    b.Click += HandleShot;
                });
            }

            return b;
        }

        private void InitializeUi()
        {
            this.Dispatcher.Invoke(() =>
            {
                ShipAttacksBox.IsEnabled = true;
            });
            this.Dispatcher.Invoke(() =>
            {
                BombAttackBox.IsEnabled = true;
            });
            for (int y = 0; y < MAP_SIZE_Y; y++)
            {
                for (int x = 0; x < MAP_SIZE_X; x++)
                {
                    var myButton = CreateButton(true, x, y);
                    var enemyButton = CreateButton(false, x, y);
                    this.Dispatcher.Invoke(() =>
                    {
                        MyButtons[y, x] = myButton;
                        EnemyButtons[y, x] = enemyButton;

                        Grid.SetRow(myButton, y + 1);
                        Grid.SetColumn(myButton, x + 1);
                        MyBoard.Children.Add(myButton);

                        Grid.SetRow(enemyButton, y + 1);
                        Grid.SetColumn(enemyButton, x + 1);
                        EnemyBoard.Children.Add(enemyButton);
                    });
                }
            }
        }

        private async Task ConnectToServer()
        {
            await _connection.StartAsync();
            await _connection.InvokeAsync("JoinGame");

            string message = "Player is connected";

            IMessage debugger = new DebuggerMessages();
            debugger.SendMessage(message);

            IMessage serverMessages = new ServerMessagesAdapter(_connection);
            serverMessages.SendMessage(message);
        }

        private void SetupListeners()
        {
            _connection.On<string>("WaitingForOpponent", HandleOnWaitingForOpponent);
            _connection.On<string>("SetupShips", HandleOnSetupShips);
            _connection.On<string>("GameStarted", HandleOnGameStarted);
            _connection.On<string>("YourTurn", HandleOnPlayerTurn);
            _connection.On<MoveResult>("UndoTurn", HandleOnPlayerUndoTurn);
            //attacks
            _connection.On<MoveResult>("ReturnMove", HandleOnReturnMove);
            _connection.On<MoveResult>("OpponentResult", HandleOnOpponentResult);

            _connection.On<MoveResult>("UndoReturnMove", HandleOnUndoReturnMove);
            _connection.On<MoveResult>("UndoOpponentResult", HandleOnUndoOpponentResult);

            _connection.On<SetupShipResponse>("SetupShipResponse", HandleOnSetShipResult); // need to add DTO or smt

            _connection.On<bool>("GameOver", HandleOnGameOver);

            // updates
            _connection.On<List<ShipCoordinate>>("AddFlags", HandleAddFlags);
            _connection.On<List<ShipCoordinate>>("AddEnemyFlags", HandleAddEnemyFlags);
            _connection.On<List<ShipCoordinate>>("RerenderCoordinates", HandleRerenderCoordinates);
            _connection.On<List<SetupShipResponse>>("RandomShipsResponse", HandleOnRandomSetShips);
            _connection.On<Shared.Color, string, Shared.Color>("SetTheme", HandleThemeMode);
        }

        private void HandleRerenderCoordinates(List<ShipCoordinate> coordinates)
        {
            foreach (ShipCoordinate coord in coordinates)
            {
                this.Dispatcher.Invoke(() =>
                {
                    UpdateButtonByCoordinate(MyButtons, coord, "");
                    SolidColorBrush? backgroundColor = ParseColorToBrush(coord.Background);
                    if (backgroundColor != null)
                    {
                        MyButtons[coord.Y, coord.X].Background = backgroundColor;
                    }
                    SolidColorBrush? borderColor = ParseColorToBrush(coord.BorderColor);
                    if (borderColor != null)
                    {
                        MyButtons[coord.Y, coord.X].BorderBrush = borderColor;
                    }
                });
            }
        }

        private SolidColorBrush? ParseColorToBrush(Shared.Color color)
        {
            switch(color)
            {
                case Shared.Color.Yellow:
                    return Brushes.Yellow;
                case Shared.Color.Blue:
                    return Brushes.Blue;
                case Shared.Color.Black:
                    return Brushes.Black;
                case Shared.Color.White:
                    return Brushes.White;
                default:
                    return null;
            }
        }

        private void HandleAddFlags(List<ShipCoordinate> coordinates)
        {
            this.Dispatcher.Invoke(() =>
            {
                foreach(ShipCoordinate coordinate in coordinates)
                {
                    UpdateButtonByCoordinate(MyButtons, coordinate, "F");
                }
            });
        }

        private void HandleAddEnemyFlags(List<ShipCoordinate> coordinates)
        {
            this.Dispatcher.Invoke(() =>
            {
                foreach (ShipCoordinate coordinate in coordinates)
                {
                    UpdateButtonByCoordinate(EnemyButtons, coordinate, "F");
                }
            });
        }

        private void HandleOnGameOver(bool hasWonGame)
        {
            this.Dispatcher.Invoke(() =>
            {
                var newWindow = new GameOverForm(hasWonGame);
                newWindow.Show();
                this.Close();
            });
        }

        private void HandleOnOpponentResult(MoveResult moveResult)
        {
            string message = moveResult.IsHit ? "Enemy hit your ship!" + moveResult.X + " " + moveResult.Y : "Enemy missed!" + moveResult.X + " " + moveResult.Y;
            SendMessageToClient(message);
            this.Dispatcher.Invoke(() => {
                Button button = MyButtons[moveResult.Y, moveResult.X];
                button.ContextMenu = null;
                Image? explosionImage = GetCoordinateImage(ShipCoordinateIcon.Explosion);
                button.Content = moveResult.IsHit ? explosionImage != null ? explosionImage : "X" : "O";
                button.Style = (Style)Resources[moveResult.IsHit ? "HitButton" : "NotHitButton"];
            });   
        }
        private void HandleOnUndoOpponentResult(MoveResult moveResult)
        {
            string message = "Enemy undone his move!";
            SendMessageToClient(message);
            this.Dispatcher.Invoke(() => {
                Button button = MyButtons[moveResult.Y, moveResult.X];
                button.Content = null;
                EnemeyBoardStyles[moveResult.X * 10 + moveResult.Y] = new Style();
            });
        }

        private void HandleOnReturnMove(MoveResult moveResult)
        {
            string message = moveResult.IsHit ? "You hit enemy ship!" + moveResult.X + " " + moveResult.Y : "You missed enemy ship!" + moveResult.X + " " + moveResult.Y;
            SendMessageToClient(message);
            this.Dispatcher.Invoke(() => {
                Button button = EnemyButtons[moveResult.Y, moveResult.X];
                Image? explosionImage = GetCoordinateImage(ShipCoordinateIcon.Explosion);
                button.Content = moveResult.IsHit ? explosionImage != null ? explosionImage : "X" : "O";
                Style newStyle = (Style)Resources[moveResult.IsHit ? "HitButton" : "NotHitButton"];
                EnemeyBoardStyles[moveResult.X * 10 + moveResult.Y] = newStyle;
                if (!moveResult.IsHit)
                {
                    button.Tag = new int[2] { moveResult.X, moveResult.Y };
                    button.MouseEnter += HandleMouseEnter;
                    button.MouseLeave += HandleMouseLeave;
                    button.Click += HandleShot;
                } else
                {
                    button.Tag = null;
                }
                button.Style = newStyle;
            });
        }
        private void HandleOnUndoReturnMove(MoveResult moveResult)
        {
            string message = "You undone your move";
            SendMessageToClient(message);
            this.Dispatcher.Invoke(() => {
                Button button = EnemyButtons[moveResult.Y, moveResult.X];
                button.Content = null;
                EnemeyBoardStyles[moveResult.X * 10 + moveResult.Y] = new Style();
            });
        }

        private Image? GetCoordinateImage(string relativeImagePath)
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = System.IO.Path.Combine(basePath, relativeImagePath);

            BitmapImage pngImage = new BitmapImage();
            try
            {
                pngImage.BeginInit();
                Uri imageUri = new Uri(filePath, UriKind.RelativeOrAbsolute);
                pngImage.UriSource = imageUri;
                pngImage.EndInit();

                Image pngImageView = new Image
                {
                    Source = pngImage,
                    Stretch = Stretch.Fill,
                    StretchDirection = StretchDirection.Both
                };

                return pngImageView;
            } catch
            {
                return null;
            }
        }

        private void SendMessageToClient(string message)
        {
            this.Dispatcher.Invoke(() =>
            {
                MessagesListbox.Items.Add(message);
            });
        }
        private void HandleOnPlayerTurn(string _)
        {
            SendMessageToClient("Your Turn!");
            EnableEnemyBoard(true);
        }

        private async void HandleOnPlayerUndoTurn(MoveResult moveResult)
        {
            SendMessageToClient("You can undo turn!");
            EnableEnemyBoard(false);
            EnableUndo(true);

            // Use a timer to set a timeout for completing the task
            //var timeoutMilliseconds = 3000; // 3 seconds timeout
            //var timer = new Timer(TimerCallback, null, timeoutMilliseconds, Timeout.Infinite);

            //// Wait for the button to be clicked
            //await Task.Delay(Timeout.Infinite); // This line effectively pauses until the timer callback sets the buttonClicked

            //// Cancel the timer
            //timer.Dispose();

            //if (buttonClicked)
            //{
            //    await _connection.SendAsync("UndoMove", moveResult);
            //    buttonClicked = false;
            //}
            //else
            //{
            //    EnableUndo(false);
            //    await _connection.SendAsync("GiveMoveToPlayer");
            //}
            await Task.Delay(3000);

            if (buttonClicked)
            {

                await _connection.SendAsync("UndoMove", moveResult);
            }
            if (!buttonClicked)
            {
                EnableUndo(false); // Attach an event handler
                await _connection.SendAsync("GiveMoveToPlayer");
            }

            buttonClicked = false;
        }

        private void EnableUndo(bool enable)
        {
            this.Dispatcher.Invoke(() =>
            {
                Undo.IsEnabled = enable;
            });
        }

        private void HandleOnGameStarted(string _)
        {
            gameState = "gameStarted";
            SendMessageToClient("Game started");
            this.Dispatcher.Invoke(() =>
            {
                TestModeButton.IsEnabled = true;
            });
        }

        private void HandleOnWaitingForOpponent(string username)
        {
            this.Dispatcher.Invoke(() =>
            {
                UserName.Text = "Welcome, " + username;
            });
        }

        private void HandleOnSetupShips(string _)
        {
            gameState = "setupingships";
            this.Dispatcher.Invoke(() =>
            {
                MessagesListbox.Items.Add("Please setup your ships!");

                ActionButton.Content = "Ready";
                ActionButton.IsEnabled = true;
                RandomShips.IsEnabled = true;

                EnableMyBoard(true);
                EnableEnemyBoard(false);
            });
        }

        private void HandleShipArrangement(object sender, RoutedEventArgs e)
        {
            var b = sender as MenuItem;
            var tag = b.Tag as ArrangementDto;

            if (tag?.Ship == null)
            {
                return;
            }

            _connection.SendAsync("AddShipToPlayer", tag.x, tag.y, tag.Ship.ShipType, tag.Ship.IsVertical);
        }

        private void HandleShipAttacks(ShipType shipType)
        {
            ShipFactory shipFactory = new ConcreteShipFactory();
            this.Dispatcher.Invoke(() =>
            {
                ShipAttacksBox.Items.Add(shipFactory.GetShip(shipType));
            });
            this.Dispatcher.Invoke(() =>
            {
                if (ShipAttacksBox.SelectedItem == null)
                {
                    ShipAttacksBox.SelectedIndex = 0;
                }
            });
        }

        private void InitializeBombAttackBox()
        {
            this.Dispatcher.Invoke(() =>
            {
                BombAttackBox.Items.Add(BombType.MissileBomb); // type index needs to be same as Items index :D
                BombAttackBox.Items.Add(BombType.AtomicBomb);
            });
            this.Dispatcher.Invoke(() =>
            {
                if (BombAttackBox.SelectedItem == null)
                {
                    BombAttackBox.SelectedIndex = 0;
                }
            });
        }
        
        private void HandleSetFlag(object sender, RoutedEventArgs e)
        {
            var b = sender as MenuItem;
            int[] tag = b.Tag as int[];
            int x = tag[0];
            int y = tag[1];

            _connection.SendAsync("FlagShip", x, y);
        }

        private void HandleSetYellowBackground(object sender, RoutedEventArgs e)
        {
            var b = sender as MenuItem;
            int[] tag = b.Tag as int[];
            int x = tag[0];
            int y = tag[1];

            _connection.SendAsync("SetShipYellowBackground", x, y);
        }

        private void HandleSetBlueBorder(object sender, RoutedEventArgs e)
        {
            var b = sender as MenuItem;
            int[] tag = b.Tag as int[];
            int x = tag[0];
            int y = tag[1];

            _connection.SendAsync("SetShipBlueBorder", x, y);
        }

        private void HandleOnSetShipResult(SetupShipResponse setupShipResponse)
        {
            if (setupShipResponse.CanPlace)
            {
                HandleShipAttacks(setupShipResponse.TypeOfShip);
                foreach (ShipCoordinate coordinate in setupShipResponse.ShipCoordinates)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        UpdateButtonByCoordinate(MyButtons, coordinate, "#");
                        MyButtons[coordinate.Y, coordinate.X].ContextMenu = new ContextMenu();

                        int[] tag = new int[] { coordinate.X, coordinate.Y };
                        var item = new MenuItem { Header = "Flag ship", Tag = tag };
                        item.Click += HandleSetFlag;
                        MyButtons[coordinate.Y, coordinate.X].ContextMenu.Items.Add(item);

                        item = new MenuItem { Header = "Set ship yellow background", Tag = tag };
                        item.Click += HandleSetYellowBackground;
                        MyButtons[coordinate.Y, coordinate.X].ContextMenu.Items.Add(item);

                        item = new MenuItem { Header = "Set ship blue border", Tag = tag };
                        item.Click += HandleSetBlueBorder;
                        MyButtons[coordinate.Y, coordinate.X].ContextMenu.Items.Add(item);
                    });
                }
            }

            EnableMyBoard(true);
        }

        private void UpdateButtonByCoordinate(Button[,] buttons, ShipCoordinate coordinate, string defaultContent)
        {
            Image? icon = GetCoordinateImage(coordinate.Icon);
            buttons[coordinate.Y, coordinate.X].Content = icon != null ? icon : defaultContent;
        }

        private void EnableMyBoard(bool enable)
        {
            foreach (Button b in MyButtons)
            {
                this.Dispatcher.Invoke(() =>
                {
                    b.IsEnabled = enable;
                });
            }
        }

        private void EnableEnemyBoard(bool enable)
        {
            foreach (Button b in EnemyButtons)
            {
                this.Dispatcher.Invoke(() =>
                {
                    b.IsEnabled = enable && b.Tag != null;
                });
            }

        }

        private void HandleAction(object sender, RoutedEventArgs e)
        {
            if (this.gameState == "setupingships")
            {
                _connection.SendAsync("DoneShipSetup");
                ClearEmptyButtons();
            }

            ActionButton.IsEnabled = false;
        }

        private void ClearEmptyButtons()
        {
            foreach (Button b in MyButtons)
            {
                var content = b.Content != null ? b.Content.ToString() : "";
                if (content?.Length > 0)
                {
                    continue;
                }
                b.ContextMenu = null;
            }
        }

        private void HandleShot(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            int[] tag = button.Tag as int[];
            if (tag == null)
            {
                return;
            }
            int x = tag[0];
            int y = tag[1];

            Ship selectedAttackShip = (Ship)ShipAttacksBox.SelectedItem;
            BombType selectedBomb = (BombType)BombAttackBox.SelectedItem;

            _connection.SendAsync("MakeMove",new MakeMove(x, y, selectedAttackShip.ShipType, selectedAttackShip.IsVertical, selectedBomb));
            EnableEnemyBoard(false);
        }

        private void HandleMouseEnter(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            int[] tag = button.Tag as int[];
            if (tag == null)
            {
                return;
            }
            int x = tag[0];
            int y = tag[1];

            // TODO: Implement bomb size logic
            //if (x + 1 < 10 && EnemyButtons[y, x + 1] != null)
            //{
            //    EnemyButtons[y, x + 1].Style = (Style)Resources["HoveredButton"];
            //}

            button.Style = (Style)Resources["HoveredButton"];
        }

        private void HandleMouseLeave(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            int[] tag = button.Tag as int[];
            if (tag == null)
            {
                return;
            }
            int x = tag[0];
            int y = tag[1];

            EnemeyBoardStyles.TryGetValue(x * 10 + y, out var style);
            button.Style = style;

            // TODO: Implement bomb size logic
            //if (x + 1 < 10 && EnemyButtons[y, x + 1] != null)
            //{
            //    EnemyButtons[y, x + 1].Style = style;
            //}
        }

        private void TestMode(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(async () =>
            {
                var newWindow = new TestModeWindow(_connection.ConnectionId);
                newWindow.Show();
                newWindow.Closed += TestClosed;
                TestModeButton.IsEnabled = false;
            });
        }

        private void TestClosed(object ?sender, EventArgs e)
        {
            TestModeButton.IsEnabled = true;
        }

        private void UndoAction(object sender, RoutedEventArgs e)
        {
            buttonClicked = true;

            EnableUndo(false);

            this.Dispatcher.Invoke(async () =>
            {
                await Task.Delay(2000);
                EnableEnemyBoard(true);
            });
            
        }

        private void PlaceRandomShipAction(object sender, RoutedEventArgs e)
        {
            _connection.InvokeAsync("GenerateRandomShips");
            RandomShips.IsEnabled = false;
        }

        private void HandleOnRandomSetShips(List<SetupShipResponse> setupShipsResponse)
        {
            foreach (SetupShipResponse setupShipResponse in setupShipsResponse)
            {
                HandleOnSetShipResult(setupShipResponse);
            }
        }

        private void ThemeMode(object sender, RoutedEventArgs e)
        {
            _connection.InvokeAsync("SetTheme");
        }

        private void HandleThemeMode(Shared.Color color, string text, Shared.Color textColor)
        {
            SolidColorBrush? backgroundColor = ParseColorToBrush(color);
            SolidColorBrush? textColorF = ParseColorToBrush(textColor);
            this.Dispatcher.Invoke(async () =>
            {
                board.Background = backgroundColor;
                Theme.Content = text;
                UserName.Foreground = textColorF;
                Histogram.Foreground = textColorF;
            });
        }
    }
}