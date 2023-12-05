using backend.Models.Entity.Bombs;
using backend.Models.Entity.GameBoardExtensions;
using backend.Models.Entity.Ships;
using backend.Strategies;
using Shared;

namespace backend.Models.Entity;

//Would be better to write a separate class for the grid
public class GameBoard
{
    private List<Ship> _battleships = new();
    private HashSet<ShipCoordinate> _missedCoordinates = new();
    private Ship? _enemyAttackShip;
    public ThemeAbstraction theme { get; set; }

    public int maxSizeX { get { return 10; } }
    public int maxSizeY { get { return 10; } }
    public void SetEnemyAttackShip(Ship ship)
    {
        _enemyAttackShip = ship;
    }
    public Ship GetEnemyAttackShip()
    {
        return _enemyAttackShip ?? throw new NullReferenceException("Enemy ship doesnt exist");
    }
    public void AddMissed(ShipCoordinate coordinate)
    {
        _missedCoordinates.Add(coordinate);
    }
    public void RemoveMissed(ShipCoordinate coordinate)
    {
        _missedCoordinates.Remove(coordinate);
    }
    public void AddShip(Ship ship)
    {
        _battleships.Add(ship);
    }
    public Ship GetHitShip(ShipCoordinate coordinate)
    {
        foreach(var hitShip in _battleships)
        {
            if (hitShip.Coordinates.Contains(coordinate))
                return hitShip;
        }
        return null;
    }
    public void ReplaceShipAt(int index, Ship ship)
    {
        if (index >= _battleships.Count)
        {
            return;
        }
        _battleships[index] = ship;
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

    public List<Ship> GetShips()
    {
        return new List<Ship>(_battleships);
    }

    //gameover check if all sunk
}