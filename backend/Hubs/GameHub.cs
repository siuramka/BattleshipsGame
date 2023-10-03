using backend.Manager;
using backend.Models.Entity;
using backend.Models.Entity.Ships;
using backend.Service;
using backend.Strategies.Ships;
using Microsoft.AspNetCore.SignalR;
using Shared;
using System.Collections.Generic;
using System.Diagnostics;

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
        await SetupShips(game.Group.Id);
    }

    private async Task SetupShips(string gameId)
    {
        await Clients.Group(gameId).SendAsync("SetupShips", ""); //send to frontend for players to setup ships
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


        if (x > 10 || y > 10)
        {
            await Clients.Client(currentPlayer.Id).SendAsync("SetupShipResponse", false, -1, -1, 1, shipType, isVertical); //send that cant place there
        }


        await Clients.Client(currentPlayer.Id).SendAsync("SetupShipResponse", true, x, y, ship.Size, shipType, isVertical);

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

        await SendTestModeShips(currentPlayer, enemyShips);
    }

    private async Task SendTestModeShips(Player player, List<Ship> ships)
    {
        await Clients.Client(player.Id).SendAsync("ReturnEnterTestMode", ships);
    }

    public async Task MakeMove(int x, int y, ShipType shipType, bool isVertical)
    {
        Ship ship = shipFactory.GetShip(shipType);
        if (isVertical)
        {
            ship.SetVertical();
        }

        var currentPlayer = _gameManager.GetPlayer(Context.ConnectionId);
        var currentGame = _gameManager.GetPlayerGame(Context.ConnectionId);
        var enemyPlayer = currentGame.GetEnemyPlayer(currentPlayer);

        var enemyBoard = enemyPlayer.OwnBoard;

        List<ShipCoordinate> hitShipCoordinates = new();

        if (ship is Ship)
        {
            enemyBoard.SetEnemyAttackStrategy(ship.GetAttackStrategy());
            hitShipCoordinates = enemyBoard.TryHit(x, y);
        }

        // TODO: implement hit logic for bigger missiles
        bool exists = false;
        foreach(var hitCoord in hitShipCoordinates)
        {
            if (hitCoord.X == x && hitCoord.Y == y)
            {
                exists = true;
            }
            await Clients.Client(currentPlayer.Id).SendAsync("ReturnMove", hitCoord.X, hitCoord.Y, true);//return to attacker if he hit ship or not
            await Clients.Client(enemyPlayer.Id).SendAsync("OpponentResult", hitCoord.X, hitCoord.Y, true);//return to who is getting attacked whenether or not his ship got hit
        }

        if (!exists)
        {
            await Clients.Client(currentPlayer.Id).SendAsync("ReturnMove", x, y, false);
            await Clients.Client(enemyPlayer.Id).SendAsync("OpponentResult", x, y, false);
        }


        if (enemyBoard.HaveAllShipsSunk) // if all enemy ships have sunk
        {
            await Clients.Group(currentGame.Group.Id).SendAsync("GameOver", new { WinnerPlayerName = enemyPlayer.Name, WinnerPlayerConnectionId = Context.ConnectionId }); // send to players that game is over and send winner name
            return;
        }


        if (hitShipCoordinates.Any())
        {
            await Clients.Client(currentPlayer.Id).SendAsync("YourTurn", "YourTurn");
            return;
        }

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