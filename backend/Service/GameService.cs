using backend.Models.Entity;
using backend.Models.Entity.Ships;

namespace backend.Service;

public class GameService
{
    public void SetupPlayerShips(Player player, List<Ship> ships)
    {
        var playerBoard = player.OwnBoard;
        foreach (var ship in ships)
        {
            playerBoard.AddShip(ship);
        }

    }
}