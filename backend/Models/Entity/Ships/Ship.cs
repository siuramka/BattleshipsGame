namespace backend.Models.Entity.Ships;

public class Ship
{
    public List<ShipCoordinate> Coordinates { get; set; } = new();

    public Ship()
    {
    }
    
    public bool IsCoordinateHit(int x, int y)
    {
        return Coordinates.Any(coord => coord.X == x && coord.Y == y && coord.IsHit);
    }

    
    public void HitCoordinate(int x, int y)
    {
        foreach (var coordinate in Coordinates)
        {
            if (coordinate.X == x && coordinate.Y == y && !coordinate.IsHit)
            {
                coordinate.Hit();
                break;
            }
        }
    }
    
    
    public bool IsSunk()
    {
        return Coordinates.All(x => x.IsHit);
    }
}