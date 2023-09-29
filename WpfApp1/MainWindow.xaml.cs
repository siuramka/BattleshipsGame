using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.VisualBasic;
using Microsoft.Xaml.Behaviors;
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


namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string[] ContextMenuItems = { "Cruiser horizontal1x1" };
        //private string[] ContextMenuItems = { "Cruiser horizontal1x1", "Submarine horizontal2x1",
        //    "Destroyer horizontally3x1", "Battleship horizontally4x1", "Cruiser vertically1x1",
        //    "Submarine vertically1x2", "Destroyer vertically1x3", "Battleship vertically1x4" };
        private Button[,] MyButtons = new Button[10, 10];
        private Button[,] EnemyButtons = new Button[10, 10];
        private string gameState = "";
        private HubConnection _connection;
        public MainWindow()
        {
            InitializeComponent();
            InitializeUi();
            _connection = new HubConnectionBuilder()
             .WithUrl("http://localhost:5220/hub")
             .Build();

            Loaded += async (sender, e) => await ConnectToServer();
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
                for (int i = 0; i < ContextMenuItems.Length; i++)
                {
                    var tag = new int[] { x, y, (i % 4) + 1, i / 4 };
                    var item = new MenuItem { Header = ContextMenuItems[i], Tag = tag };
                    this.Dispatcher.Invoke(() =>
                    {
                        item.Click += HandleShipArrangement;
                        b.ContextMenu.Items.Add(item);
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
            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 10; x++)
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
            _connection.On<int, int, bool>("ReturnMove", HandleOnReturnMove);
            _connection.On<int, int, bool>("OpponentResult", HandleOnOpponentResult);

            _connection.On<bool,int,int,int,bool>("SetupShipResponse", HandleOnSetShipResult); // need to add DTO or smt

        }
        private void HandleOnOpponentResult(int x, int y, bool hitMyShip)
        {
            if (hitMyShip)
            {
                SendMessageToClient("Enemy hit your ship!" + x + " " + y);
                this.Dispatcher.Invoke(() =>
                {
                    MyButtons[y, x].Content = hitMyShip ? "X" : "O";
                });
            }
            else
            {
                SendMessageToClient("Enemy missed!" + x + " " + y);
                this.Dispatcher.Invoke(() =>
                {
                    MyButtons[y, x].Content = hitMyShip ? "X" : "O";
                });
            }
        }


        private void HandleOnReturnMove(int x, int y, bool hitEnemyShip)
        {
            if (hitEnemyShip)
            {
                SendMessageToClient("You hit enemy ship!" + x + " " + y);
                this.Dispatcher.Invoke(() =>
                {
                    EnemyButtons[y, x].Content = hitEnemyShip ? "X" : "O";
                });
            }
            else
            {
                SendMessageToClient("You missed enemy ship!" + x + " " + y);
                this.Dispatcher.Invoke(() =>
                {
                    MyButtons[y, x].Content = hitEnemyShip ? "X" : "O";
                });
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
            int[] tag = b.Tag as int[];
            int x = tag[0];
            int y = tag[1];
            int size = tag[2];
            bool vertical = tag[3] == 1;

            _connection.SendAsync("AddShipToPlayer", size, x, y, vertical);
            EnableMyBoard(false);

        }
        private void HandleOnSetShipResult(bool result, int x, int y, int size, bool vertical)
        {

            if (result)
            {
                if (vertical)
                {
                    for (int i = y; i < y + size; i++)
                    {
                        this.Dispatcher.Invoke(() =>
                        {

                            MyButtons[i, x].Content = "#";
                        });
                    }
                }
                else
                {
                    for (int i = x; i < x + size; i++)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            MyButtons[y, i].Content = "#";

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

        private void SendChatMessage(object sender, KeyEventArgs e)
        {
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

            _connection.SendAsync("MakeMove",x, y);
            EnableEnemyBoard(false);
        }
        private void HandleClearBoard(object sender, RoutedEventArgs e)
        {
        }
    }
}