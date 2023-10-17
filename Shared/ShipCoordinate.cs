namespace backend.Models.Entity.Ships;

public class ShipCoordinate
{
    public int X { get; set; }
    public int Y { get; set; }
    public bool IsHit { get; private set; } = false;
    public string Icon { get; set; } = ShipCoordinateIcon.Ship;

    public ShipCoordinate(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void Hit()
    {
        IsHit = true;
    }
}