using backend.Strategies.Ships;
using backend.Strategies;
using Shared;
using backend.Models.Entity.Bombs;

namespace backend.Models.Entity.Ships
{
    public abstract class Ship
    {
        public List<ShipCoordinate> Coordinates { get; private set; } = new List<ShipCoordinate>();
        public int Size { get; set; }
        public bool IsVertical { get; set; }
        public ShipType ShipType { get; set; }
        public virtual IAttackStrategy GetAttackStrategy() => new DefaultAttackStrategy();
        public abstract BombFactory GetShipBombFactory();

        public Ship SetVertical()
        {
            IsVertical = true;
            return this;
        }

        public virtual void AddCoordinate(int x, int y)
        {
            Coordinates.Add(new ShipCoordinate(x, y));
        }

        public virtual void AddCoordinate(ShipCoordinate coordinate)
        {
            Coordinates.Add(coordinate);
        }

        public virtual List<ShipCoordinate> GetCoordinates()
        {
            return new List<ShipCoordinate>(Coordinates);
        }

        public void RemoveAllCoordinates()
        {
            Coordinates = new();
        }

        public bool CanHitCoordinate(int x, int y)
        {
            return Coordinates.Exists(coord => coord.X == x && coord.Y == y && !coord.IsHit);
        }

        public void HitCoordinate(int x, int y)
        {
            var coordinate = Coordinates.Find(coord => coord.X == x && coord.Y == y);
            coordinate?.Hit();
            if (coordinate != null)
            {
                coordinate.Icon = ShipCoordinateIcon.Explosion;
            }
        }

        public bool IsSunk()
        {
            return Coordinates.All(coord => coord.IsHit);
        }

        public override string ToString()
        {
            if (!IsVertical)
            {
                return $"1x{Size} {ShipType}";
            }
            else
            {
                return $"1x{Size} Vertical {ShipType}";
            }
            
        }

        public abstract Ship ShallowCopy();
        public abstract Ship DeepCopy();
    }
}
