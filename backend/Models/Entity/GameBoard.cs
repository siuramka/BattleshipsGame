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
    private Ship? _enemyAttackShip;

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
    public List<ShipCoordinate> GetMissedCoordinates()
    {
        return new List<ShipCoordinate>(_missedCoordinates);
    }
    public void ClearMissedCoordinates()
    {
        _missedCoordinates = new();
    }
    public List<ShipCoordinate> GetHitCoordinates(int x, int y, BombType attackBomb)
    {
        List<ShipCoordinate> hitableCordinates = GetHitableCordinates(x, y, attackBomb);

        List<ShipCoordinate> hitShipCoordinates = new();

        foreach (var ship in _battleships)
        {
            bool canTargetCoordinate = hitableCordinates.Any(cord => cord.X == x && cord.Y == y);
            if (ship.CanHitCoordinate(x, y) && canTargetCoordinate)
            {
                ship.HitCoordinate(x, y);
                hitShipCoordinates.Add(new ShipCoordinate(x, y));
            }
            else
            {
                _missedCoordinates.Add(new ShipCoordinate(x, y));
            }
        }

        return hitShipCoordinates;

    }
    private List<ShipCoordinate> GetHitableCordinates(int x, int y, BombType attackBomb)
    {
        if (_enemyAttackShip == null)
        {
            throw new InvalidOperationException("Enemy attack ship not set.");
        }

        BombFactory factory = _enemyAttackShip.GetShipBombFactory();
        IAttackStrategy attackStrategy = _enemyAttackShip.GetAttackStrategy();

        if (attackBomb == BombType.MissileBomb)
        {
            MissileBomb missileBomb = factory.CreateMissileBomb();
            return attackStrategy.TargetShip(x, y, missileBomb);
        }

        if (attackBomb == BombType.AtomicBomb)
        {
            AtomicBomb atomicBomb = factory.CreateAtomicBomb();
            return attackStrategy.TargetShip(x, y, atomicBomb);
        }

        throw new ArgumentNullException(nameof(BombType));
    }

    public List<Ship> GetShips()
    {
        return new List<Ship>(_battleships);
    }

    //gameover check if all sunk
}