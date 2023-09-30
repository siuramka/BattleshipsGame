using backend.Models.Entity;
using backend.Models.Entity.Ships;

namespace backend.Service;

public class GameService
{
    public void AddShipToPlayer(Player player, SmallShip ship)
    {
        var playerBoard = player.OwnBoard;
        playerBoard.AddShip(ship);
    }

    public void CalculateShipCoordinates(SmallShip ship, int x, int y)
    {
        if(ship.ShipSize == 1)
        {
            ship.AddCoordinate(x, y);
        }
    }
}