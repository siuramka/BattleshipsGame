using backend.Models.Entity;

namespace backend.Manager;

public class GameManager
{
    private static GameManager _instance;
    private readonly List<Game> _games = new();

    private GameManager()
    {
    }

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameManager();
            }

            return _instance;
        }
    }

    public List<Game> Games => _games;

    public bool EmptyGameExist()
    {
        return _games.Any(x => x.WaitingForOpponent == true);
    }

    // Add methods to manage games, players, and game state
}