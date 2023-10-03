using backend.Models.Entity.Ships;
using backend.Strategies;

namespace backend.Models.Entity;

//Would be better to write a separate class for the grid
public class GameBoard
{
    private List<IShip> _battleships = new();
    private List<ShipCoordinate> _missedCoordinates = new();
    private IAttackStrategy _attackStrategy;

    private int maxSizeX = 10;
    private int maxSizeY = 10;
    public void SetEnemyAttackStrategy(IAttackStrategy strategy)
    {
        _attackStrategy = strategy;
    }
    public void AddShip(IShip ship)
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
    public List<ShipCoordinate> TryHit(int x, int y)
    {
        return _attackStrategy.TargetShip(x, y, _battleships, _missedCoordinates);
    }

    public List<IShip> GetShips()
    {
        return new List<IShip>(_battleships);
    }

    //gameover check if all sunk
}