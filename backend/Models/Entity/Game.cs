namespace backend.Models.Entity;

public class Game
{
    public Group Group { get; set; } = new();
    public Player Player1 { get; set; }
    public Player Player2 { get; set; }
    public bool WaitingForOpponent { get; set; } = true;

    public List<Player> GetPlayers()
    {
        return new List<Player>() {Player1, Player2};
    }
}