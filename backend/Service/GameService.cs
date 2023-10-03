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

    public void CalculateShipCoordinates(Ship ship, int x, int y)
    {
        //if small ship
        ship.AddCoordinate(x, y);
    }
}