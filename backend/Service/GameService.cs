using backend.Models.Entity;
using backend.Models.Entity.Ships;

namespace backend.Service;

public class GameService
{
    public void AddShipToPlayer(Player player, Ship ship)
    {
        var playerBoard = player.OwnBoard;
        playerBoard.AddShip(ship);

    }
}