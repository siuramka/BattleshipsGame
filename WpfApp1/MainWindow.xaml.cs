using backend.Models.Entity;
using backend.Models.Entity.Ships;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.VisualBasic;
using Microsoft.Xaml.Behaviors;
using Shared;
using Shared.Transfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
        private const int MAP_SIZE_X = 10;
        private const int MAP_SIZE_Y = 10;
        private static ShipFactory shipFactory = new ConcreteShipFactory();
        private Ship[] Ships = { 
            shipFactory.GetShip(ShipType.SmallShip), 
            shipFactory.GetShip(ShipType.MediumShip),
            shipFactory.GetShip(ShipType.BigShip),
            shipFactory.GetShip(ShipType.SmallShip).SetVertical(),
            shipFactory.GetShip(ShipType.MediumShip).SetVertical(),
            shipFactory.GetShip(ShipType.BigShip).SetVertical(),
        };
        private Button[,] MyButtons = new Button[MAP_SIZE_X, MAP_SIZE_Y];
        private Button[,] EnemyButtons = new Button[MAP_SIZE_X, MAP_SIZE_Y];
        private string gameState = ""; // change to class ?
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

                b.Tag = new int[2] { x, y };
                this.Dispatcher.Invoke(() =>
                {
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
        }

        private void SetupListeners()
        {
            _connection.On<string>("WaitingForOpponent", HandleOnWaitingForOpponent);
            _connection.On<string>("SetupShips", HandleOnSetupShips);
            _connection.On<string>("GameStarted", HandleOnGameStarted);
            _connection.On<string>("YourTurn", HandleOnPlayerTurn);
            //attacks
            _connection.On<MoveResult>("ReturnMove", HandleOnReturnMove);
            _connection.On<MoveResult>("OpponentResult", HandleOnOpponentResult);

            _connection.On<SetupShipResponse>("SetupShipResponse", HandleOnSetShipResult); // need to add DTO or smt

            _connection.On<bool>("GameOver", HandleOnGameOver);
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
                button.Content = moveResult.IsHit? "X" : "O";
                button.Style = (Style)Resources[moveResult.IsHit ? "HitButton" : "NotHitButton"];
            });   
        }
        private void HandleOnReturnMove(MoveResult moveResult)
        {
            string message = moveResult.IsHit ? "You hit enemy ship!" + moveResult.X + " " + moveResult.Y : "You missed enemy ship!" + moveResult.X + " " + moveResult.Y;
            SendMessageToClient(message);
            this.Dispatcher.Invoke(() => {
                Button button = EnemyButtons[moveResult.Y, moveResult.X];
                button.Content = moveResult.IsHit ? "X" : "O";
                button.Style = (Style)Resources[moveResult.IsHit ? "HitButton" : "NotHitButton"];
            });
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

        private void HandleOnGameStarted(string _)
        {
            gameState = "gameStarted";
            SendMessageToClient("Game started");
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
            EnableMyBoard(false);
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
                BombAttackBox.Items.Add(BombType.MissileBomb);
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
        private void HandleOnSetShipResult(SetupShipResponse setupShipResponse)
        {
            if (setupShipResponse.CanPlace)
            {
                HandleShipAttacks(setupShipResponse.TypeOfShip);
                if (setupShipResponse.IsVertical)
                {
                    for (int i = setupShipResponse.Y; i < setupShipResponse.Y + setupShipResponse.ShipSize; i++)
                    {
                        this.Dispatcher.Invoke(() =>
                        {

                            MyButtons[i, setupShipResponse.X].Content = "#";
                        });
                    }
                }
                else
                {
                    for (int i = setupShipResponse.X; i < setupShipResponse.X + setupShipResponse.ShipSize; i++)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            MyButtons[setupShipResponse.Y, i].Content = "#";

                        });
                    }
                }
            }

            EnableMyBoard(true);
        }

        private void EnableMyBoard(bool enable)
        {

            foreach (Button b in MyButtons)
            {
                this.Dispatcher.Invoke(() =>
                {
                    var content = b.Content != null ? b.Content.ToString() : "";
                    b.IsEnabled = enable && content.Length == 0;
                });
            }
        }

        private void EnableEnemyBoard(bool enable)
        {

            foreach (Button b in EnemyButtons)
            {
                this.Dispatcher.Invoke(() =>
                {
                    var content = b.Content != null ? b.Content.ToString() : "";
                    b.IsEnabled = enable && content.Length == 0;
                });
            }

        }

        private void HandleAction(object sender, RoutedEventArgs e)
        {
            if (this.gameState == "setupingships")
            {
                EnableMyBoard(false);
                _connection.SendAsync("DoneShipSetup");
            }

            ActionButton.IsEnabled = false;
        }

        private void HandleShot(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            int[] tag = button.Tag as int[];
            int x = tag[0];
            int y = tag[1];

            Ship selectedAttackShip = (Ship)ShipAttacksBox.SelectedItem;
            BombType selectedBombType = (BombType)BombAttackBox.SelectedItem;

            _connection.SendAsync("MakeMove",new MakeMove(x, y, selectedAttackShip.ShipType, selectedAttackShip.IsVertical, selectedBombType));
            EnableEnemyBoard(false);
        }
    }
}