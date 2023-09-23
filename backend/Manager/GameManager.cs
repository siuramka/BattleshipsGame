using backend.Models.Entity;

namespace backend.Manager;

public class GameManager
{
    private static GameManager _instance;
    private readonly List<Game> _games = new();
    private static readonly object threadLock = new object();
    private GameManager()
    {
    }

    public static GameManager Instance
    {
        get
        {
            lock (threadLock)
            {
                if (_instance == null)
                {
                    _instance = new GameManager();
                }

                return _instance;
            }
        }
    }

    public List<Game> Games => _games;

    public bool EmptyGameExist()
    {
        return _games.Any(x => x.WaitingForOpponent == true);
    }

    public Player? GetPlayer(string playerId)
    {
        return _games.Select(game => game.GetPlayerById(playerId)).FirstOrDefault(currentPlayer => currentPlayer != null);
    }

    public Game? GetPlayerGame(string playerId)
    {
        foreach (var game in _games)
        {
            if (game.GetPlayerById(playerId) != null)
            {
                return game;
            }
        }

        return null;

    }

    // Add methods to manage games, players, and game state
}