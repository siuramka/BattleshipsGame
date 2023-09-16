using backend.Models.Entity.Ships;

namespace backend.Models.Entity;

//Would be better to write a separate class for the grid
public class GameBoard
{
    private List<Ship> _battleships = new();
    private List<ShipCoordinate> _missedCoordinates = new(); 
    public void AddShip(Ship ship)
    {
        var shipPosition = ship.Coordinates;
        //todo: could check overlap coordinates prob
        _battleships.Add(ship);
    }
    
    public bool HitCoordinate(int x, int y)
    {
        foreach (var battleship in _battleships)
        {
            if (!battleship.IsCoordinateHit(x, y))
            {
                battleship.HitCoordinate(x, y);
                return true;
            }
        }
        _missedCoordinates.Add(new ShipCoordinate(x,y));
        return false;
    }
    
    //gameover check if all sunk
}