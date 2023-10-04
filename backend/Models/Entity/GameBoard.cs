using backend.Models.Entity.Bombs;
using backend.Models.Entity.Ships;
using backend.Strategies;
using Shared;

namespace backend.Models.Entity;

//Would be better to write a separate class for the grid
public class GameBoard
{
    private List<Ship> _battleships = new();
    private List<ShipCoordinate> _missedCoordinates = new();
    private Ship _enemyAttackShip;

    private int maxSizeX = 10;
    private int maxSizeY = 10;
    public void SetEnemyAttackShip(Ship ship)
    {
        _enemyAttackShip = ship;
    }
    public void AddShip(Ship ship)
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
    public List<ShipCoordinate> TryHit(int x, int y, BombType attackBomb)
    {
        var aaa = _enemyAttackShip.GetShipBombFactory();

        return _enemyAttackShip.GetAttackStrategy().TargetShip(x, y, _battleships, _missedCoordinates, attackBomb);
    }

    public List<Ship> GetShips()
    {
        return new List<Ship>(_battleships);
    }

    //gameover check if all sunk
}