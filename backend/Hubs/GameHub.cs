using backend.Command;
using backend.Manager;
using backend.Models.Entity;
using backend.Models.Entity.Bombs;
using backend.Models.Entity.Ships;
using backend.Models.Entity.Ships.Factory;
using backend.Observer;
using backend.Service;
using backend.Strategies.Ships;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.SignalR;
using Shared;
using Shared.Transfer;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

namespace backend.Hubs;

//todo: add disconnect logic

public class GameHub : Hub
{
    private GameManager
        _gameManager = GameManager.Instance; //singleton because each new connection creates new hub instance

    private GameService _gameService;
    private ShipFactory shipFactory;
    public GameHub() : base()
    {
        _gameService = new();
        shipFactory = new ConcreteShipFactory();
    }

    private class HitObject
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public bool IsHit { get; set; }
    }

    public async Task JoinGame()
    {
        var playerName = _gameManager.EmptyGameExist() ? "Player2" : "Player1"; // Player1 if hes the first to join the game
        var player = new Player { Id = Context.ConnectionId, Name = playerName };
        Game game = new Game();

        if (!_gameManager.EmptyGameExist()) // if no empty games
        {
            game.Player1 = player;
            var groupId = Guid.NewGuid().ToString();
            game.Group.Id = groupId;
            _gameManager.Games.Add(game);
            await Clients.Client(player.Id).SendAsync("WaitingForOpponent", player.Name);
            return;
        }

        game = _gameManager.Games.First(x =>
            x.WaitingForOpponent == true); //get the first game where player is waiting for an opponent
        game.Player2 = player;
        game.WaitingForOpponent = false;

        await Groups.AddToGroupAsync(game.Player1.Id, game.Group.Id);
        await Groups.AddToGroupAsync(game.Player2.Id, game.Group.Id);
        await Clients.Client(player.Id).SendAsync("WaitingForOpponent", player.Name);
        await SetupShips(game);
    }

    private async Task SetupShips(Game game)
    {

        Subject s = new Subject(Clients);
        s.AddObserver(new Observer.Observer(game.Player1.Id));
        s.AddObserver(new Observer.Observer(game.Player2.Id));
        s.SubjectState = "SetupShips";
        s.SubjectMessage = "";

        await s.NotifyObservers();

        //send to frontend for players to setup ships
        //await Clients.Group(gameId).SendAsync("SetupShips", "");
    }
    // _connection.SendAsync("SetShip", size, x, y, vertical);
    public async Task AddShipToPlayer(int x, int y, ShipType shipType, bool isVertical)
    {
        var currentPlayer = _gameManager.GetPlayer(Context.ConnectionId);
        var currentGame = _gameManager.GetPlayerGame(Context.ConnectionId);
        if (currentPlayer == null || currentGame == null)
        {
            return;
        }

        //
        // add handling for ship placement on different size ships etc...
        //
        Ship ship = shipFactory.GetShip(shipType);
        if (isVertical)
        {
            ship.SetVertical();
        }
        _gameService.CalculateShipCoordinates(ship, x, y);
        currentPlayer.OwnBoard.AddShip(ship);


        if (checkIfShipDoesNotFit(ship))
        {
            await Clients.Client(currentPlayer.Id).SendAsync("SetupShipResponse", new SetupShipResponse(false, ship.Coordinates, shipType)); //send that cant place there
        }


        await Clients.Client(currentPlayer.Id).SendAsync("SetupShipResponse", new SetupShipResponse(true, ship.Coordinates, shipType));

    }

    private bool checkIfShipDoesNotFit(Ship ship)
    {
        foreach (ShipCoordinate coord in ship.Coordinates)
        {
            if(coord.X > 10 || coord.Y > 10)
            {
                return true;
            }
        }
        return false;
    }
    public async Task DoneShipSetup()
    {
        var currentPlayer = _gameManager.GetPlayer(Context.ConnectionId);

        currentPlayer.HasSetupShips = true;

        var currentGame = _gameManager.GetPlayerGame(Context.ConnectionId);

        if (currentGame.HavePlayersSetupShips())
        {
            await StartGame(currentGame);
        }
    }

    private async Task StartGame(Game _currentGame)
    {
        await Clients.Group(_currentGame.Group.Id).SendAsync("GameStarted", "GameStarted");

        await Task.Delay(TimeSpan.FromSeconds(1)); // wait a second

        await Clients.Client(_currentGame.Player1.Id).SendAsync("YourTurn", "YourTurn"); // Player1 starts the game
    }

    public async Task EnterTestMode()
    {
        var currentPlayer = _gameManager.GetPlayer(Context.ConnectionId);
        var currentGame = _gameManager.GetPlayerGame(Context.ConnectionId);

        var enemyPlayer = currentGame.GetEnemyPlayer(currentPlayer);
        var enemyShips = enemyPlayer.OwnBoard.GetShips();

        List<ShipCoordinate> shipCoordinates = new List<ShipCoordinate>();
        foreach (var ship in enemyShips)
        {
            foreach(var shipCoordinate in ship.Coordinates)
            {
                shipCoordinates.Add(shipCoordinate);
            }
        }

        await SendTestModeShips(currentPlayer, new List<ShipCoordinate>(shipCoordinates));
    }

    private async Task SendTestModeShips(Player player, List<ShipCoordinate> ships)
    {
        await Clients.Client(player.Id).SendAsync("ReturnEnterTestMode", ships);
    }

    //public async Task MakeMove(int x, int y, ShipType shipType, bool isVertical)
    public async Task MakeMove(MakeMove move)

    {
        var currentPlayer = _gameManager.GetPlayer(Context.ConnectionId);
        var currentGame = _gameManager.GetPlayerGame(Context.ConnectionId);
        var enemyPlayer = currentGame.GetEnemyPlayer(currentPlayer);

        // Create a new instance of ShotCommand and execute it
        var shotCommand = new ShotCommand(currentPlayer, enemyPlayer, move.X, move.Y, move.TypeOfShip,
            move.IsVertical, move.AttackBomb, _gameManager, Clients.Client(currentPlayer.Id),
            Clients.Client(enemyPlayer.Id), enemyPlayer.OwnBoard);

        await shotCommand.Execute();

    }

    public async Task UndoMove(MakeMove move)

    {
        var currentPlayer = _gameManager.GetPlayer(Context.ConnectionId);
        var currentGame = _gameManager.GetPlayerGame(Context.ConnectionId);
        var enemyPlayer = currentGame.GetEnemyPlayer(currentPlayer);

        // Create a new instance of ShotCommand and execute it
        var shotCommand = new ShotCommand(currentPlayer, enemyPlayer, move.X, move.Y, move.TypeOfShip,
            move.IsVertical, move.AttackBomb, _gameManager, Clients.Client(currentPlayer.Id),
            Clients.Client(enemyPlayer.Id), enemyPlayer.OwnBoard);

        await shotCommand.Undo();

    }

    public async Task GiveMoveToPlayer()
    {
        var currentPlayer = _gameManager.GetPlayer(Context.ConnectionId);
        var currentGame = _gameManager.GetPlayerGame(Context.ConnectionId);
        var enemyPlayer = currentGame.GetEnemyPlayer(currentPlayer);
        await Clients.Client(enemyPlayer.Id).SendAsync("YourTurn", "YourTurn");
    }
        


    // public override Task OnDisconnectedAsync(Exception exception)
    // {
    //     var _currentPlayer = _gameManager.GetPlayer(Context.ConnectionId);
    //     var _currentGame = _gameManager.GetPlayerGame(Context.ConnectionId);
    //     
    //     _currentGame.RemovePlayerFromGame(_currentPlayer);
    //     Clients.Group(_currentGame.Group.Id).SendAsync("GameOverPlayerLeft");
    //
    //     return base.OnDisconnectedAsync(exception);
    // }
}