﻿using backend.Manager;
using backend.Models.Dtos;
using backend.Models.Entity;
using Microsoft.AspNetCore.SignalR;

namespace backend.Hubs;

public class GameHub : Hub
{
    private GameManager _gameManager = GameManager.Instance; // on refresh creates a new connection so new game each time
    //can use signalr clients.groups so dont have to foreach the players all the time probably
    //add disconnect logic later
    public async Task JoinGame()
    {
        var playerName = _gameManager.EmptyGameExist() ? "Player2" : "Player1"; // Player1 if hes the first to join the game
        var player = new Player { Id = Context.ConnectionId, Name = playerName };

        if (!_gameManager.EmptyGameExist()) // if no empty games
        {
            var game = new Game();
            game.Player1 = player;
            var groupId = Guid.NewGuid().ToString();
            game.Group.Id = groupId;
            _gameManager.Games.Add(game);
            await Clients.Client(player.Id).SendAsync("WaitingForOpponent", player.Name);
        }
        else 
        {
            var game = _gameManager.Games.First(x => x.WaitingForOpponent == true); //get the first game where player is waiting for an opponent
            game.Player2 = player;
            game.WaitingForOpponent = false;
            await Groups.AddToGroupAsync(game.Player1.Id, game.Group.Id);
            await Groups.AddToGroupAsync(game.Player2.Id, game.Group.Id);
            await Clients.Client(player.Id).SendAsync("WaitingForOpponent", player.Name);
            await SetupShips(game.Group.Id);
            // StartGame(game.Id);
        }
    }

    private async Task SetupShips(string gameId)
    {
        await Clients.Group(gameId).SendAsync("SetupShips"); //send to frontend for players to setup ships
    }

    public async Task SetShipsOnBoard(GameBoardSetupDto board)
    {
        // set ships
        int x = 0;
        //StartGame();
    }
    
    private void StartGame(string gameId)
    {
        var game = _gameManager.Games.Where(x => x.Group.Id == gameId).First();
        var gamePlayers = game.GetPlayers();
        
        foreach (var player in gamePlayers)
        {
            var gameBoard = player.OwnBoard;
            gameBoard.Initialize(); // Initialize the player's game board with 0's
            gameBoard.AddRandomShipTest(); // and some ships for testing all 1x1
            
            var enemyBoard = player.EnemyBoard;
            enemyBoard.Initialize(); // Initialize the player's opponent boards with 0's
        }
        
        foreach (var player in gamePlayers)
        {
            var opponent = gamePlayers.First(p => p.Id != player.Id);
            var opponentGameBoard = opponent.EnemyBoard; //enemy board aka board where you make hits
            
            Clients.Client(player.Id).SendAsync("OpponentName", opponent.Name);
            Clients.Client(player.Id).SendAsync("OpponentGameBoard", opponentGameBoard);
        }
    
        Clients.Client(game.Player1.Id).SendAsync("YourTurn");
    }
    
    // public async Task MakeMove(int x, int y)
    // {
    //     var currentGame = _games.Where(x => x.GetPlayers().Any(x => x.Id == Context.ConnectionId)).First();
    //     var currentPlayer = currentGame.GetPlayers().Where(x => x.Id == Context.ConnectionId).First(); // move this to global
    //     //if (currentPlayer != null && !currentPlayer.HasTurn) return;
    //
    //     var opponent = currentGame.GetPlayers().First(p => p.Id != currentPlayer.Id);
    //     var opponentGameBoard = opponent.OwnBoard;
    //
    //     var isHit = opponentGameBoard.MarkCell(x, y); // mark hit 
    //     currentPlayer.EnemyBoard.MarkCell(x, y); // show on my screen ig?
    //     
    //     await Clients.Client(currentPlayer.Id).SendAsync("MoveResult", x, y, isHit);
    //     await Clients.Client(opponent.Id).SendAsync("OpponentMove", x, y, isHit);
    //
    //     if (opponentGameBoard.IsGameOver())
    //     {
    //         foreach (var player in currentGame.GetPlayers())
    //         {
    //             await Clients.Client(player.Id).SendAsync("GameOver"); // send winner probably
    //         }
    //     }
    //     else
    //     {
    //         currentPlayer.HasTurn = false;
    //         opponent.HasTurn = true;
    //         await Clients.Client(opponent.Id).SendAsync("YourTurn");
    //     }
    // }
    
    // public override Task OnDisconnectedAsync(Exception exception)
    // {
    //     var disconnectedPlayer = _players.FirstOrDefault(p => p.Id == Context.ConnectionId);
    //     if (disconnectedPlayer != null)
    //     {
    //         _players.Remove(disconnectedPlayer);
    //         _gameBoards.Remove(disconnectedPlayer.Id);
    //         Clients.All.SendAsync("PlayerLeft", disconnectedPlayer.Name);
    //     }
    //
    //     return base.OnDisconnectedAsync(exception);
    // }
}