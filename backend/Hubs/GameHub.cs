using backend.Manager;
using backend.Models.Entity;
using backend.Models.Entity.Ships;
using backend.Service;
using Microsoft.AspNetCore.SignalR;

namespace backend.Hubs;

//todo: add disconnect logic

public class GameHub : Hub
{
    private GameManager
        _gameManager = GameManager.Instance; //singleton because each new connection creates new hub instance

    private GameService _gameService;
    private Player _currentPlayer = null;
    private Game _currentGame = null;
    public GameHub() : base()
    {
        _gameService = new();
        _currentPlayer = _gameManager.GetPlayer(Context.ConnectionId);
        _currentGame = _gameManager.GetPlayerGame(Context.ConnectionId);
    }

    private class HitObject
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public bool IsHit { get; set; }
    }

    public async Task JoinGame()
    {
        var playerName =_gameManager.EmptyGameExist() ? "Player2" : "Player1"; // Player1 if hes the first to join the game
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
        await Clients.Group(gameId).SendAsync("SetupShips"); //send to frontend for players to setup ships
    }

    public async Task SetShipsOnBoard(List<Ship> ships)
    {
        if (_currentPlayer == null || _currentGame == null) return;

        _gameService.SetupPlayerShips(_currentPlayer, ships);
        _currentPlayer.HasSetupShips = true;

        if (_currentGame.HavePlayersSetupShips())
        {
            await StartGame(_currentGame);
        }
    }
    
    private async Task StartGame(Game _currentGame)
    {
        await Clients.Group(_currentGame.Group.Id).SendAsync("GameStarted");
        await Task.Delay(TimeSpan.FromSeconds(1)); // wait a second
        await Clients.Client(_currentGame.Player1.Id).SendAsync("YourTurn"); // Player1 starts the game
    }

    public async Task EnterTestMode()
    {
        var enemyPlayer = _currentGame.GetEnemyPlayer(_currentPlayer);
        var enemyShips = enemyPlayer.OwnBoard.GetShips();

        await SendTestModeShips(_currentPlayer, enemyShips);
    }

    private async Task SendTestModeShips(Player player, List<Ship> ships)
    {
        await Clients.Client(player.Id).SendAsync("ReturnEnterTestMode", ships);
    }
    
    public async Task NewMakeMove(int x, int y)
    {
        var enemyPlayer = _currentGame.GetEnemyPlayer(_currentPlayer);
        
        var enemyBoard = enemyPlayer.OwnBoard;

        bool hitEnemyShip = enemyBoard.HitCoordinate(x, y);
        
        await Clients.Client(_currentPlayer.Id).SendAsync("ReturnMove", x,y ,hitEnemyShip);//return to attacker if he hit ship or not
        await Clients.Client(enemyPlayer.Id).SendAsync("OpponentResult", x,y,hitEnemyShip);//return to who is getting attacked whenether or not his ship got hit

        if (enemyBoard.HaveAllShipsSunk) // if all enemy ships have sunk
        {
            await Clients.Group(_currentGame.Group.Id).SendAsync("GameOver", new { WinnerPlayerName = _currentPlayer.Name, WinnerPlayerConnectionId = Context.ConnectionId}); // send to players that game is over and send winner name
            return;
        }
        
        //return to enemy that its his turn
        await Clients.Client(enemyPlayer.Id).SendAsync("YourTurn");
    }
    
    public async Task MakeMove(int x, int y)
    {
        var _currentPlayer = _gameManager.GetPlayer(Context.ConnectionId);
        var _currentGame = _gameManager.GetPlayerGame(Context.ConnectionId);


        var hitObject = new HitObject();
        hitObject.Row = y;
        hitObject.Column = x;
        hitObject.IsHit = true;
        await Clients.Client(_currentPlayer.Id).SendAsync("ReturnMove", hitObject);
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