namespace backend.Models.Entity;

public class Game
{
    public Group Group { get; } = new();
    public Player Player1 { get; set; }
    public Player Player2 { get; set; }
    public bool WaitingForOpponent { get; set; } = true;

    public Player? GetPlayerById(string id)
    {
        if (Player1.Id == id) return Player1;
        if (Player2.Id == id) return Player2;
        return null;
    }

    public bool HavePlayersSetupShips()
    {
        return Player1.HasSetupShips && Player2.HasSetupShips;
    }
    public void RemovePlayerFromGame(Player player)
    {
        if (Player1 == player)
        {
            Player1 = null;
            return;
        }
        Player2 = null;
    }

    public Player GetEnemyPlayer(Player player)
    {
        return Player1 == player ? Player2 : Player1;
    }
}