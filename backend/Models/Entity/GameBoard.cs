using backend.Models.Entity.Ships;

namespace backend.Models.Entity;

//Would be better to write a separate class for the grid
public class GameBoard
{
    private List<SmallShip> _battleships = new();
    private List<ShipCoordinate> _missedCoordinates = new();

    private int maxSizeX = 10;
    private int maxSizeY = 10;
    public void AddShip(SmallShip ship)
    {
        _battleships.Add(ship);
    }
    public bool HaveAllShipsSunk
    {
        get
        {
            return _battleships.All(x => x.IsSunk());
        }
    }
    public bool TryHit(int x, int y)
    {
        foreach (var battleship in _battleships)
        {
            if (battleship.CanHitCoordinate(x, y))
            {
                battleship.HitCoordinate(x, y);
                return true;
            }
        }
        _missedCoordinates.Add(new ShipCoordinate(x,y));
        return false;
    }

    public List<SmallShip> GetShips()
    {
        return new List<SmallShip>(_battleships);
    }
    
    //gameover check if all sunk
}