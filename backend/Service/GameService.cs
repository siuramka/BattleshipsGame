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

    public void CalculateShipCoordinates(Ship ship, int x, int y)
    {
        if(ship.ShipSize == 1)
        {
            ship.AddCoordinate(x, y);
        }
    }
}