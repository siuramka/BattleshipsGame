using backend.Strategies.Ships;
using backend.Strategies;
using Shared;
using backend.Models.Entity.Bombs;
using backend.Strategies.Attacks;
using backend.Models.Entity.Iterator;
using backend.Visitor;

namespace backend.Models.Entity.Ships
{
    public abstract class Ship
    {
        private ShipCoordinatesCollection Coordinates = new ShipCoordinatesCollection();
        public int Size { get; set; }
        public bool IsVertical { get; set; }
        public ShipType ShipType { get; set; }
        public abstract AttackTemplate GetAttackTemplate();
        public Statistics? Stats { get; set;  }

        public int PlacedX { get; set; }
        public int PlacedY { get; set; }

        public Ship SetVertical()
        {
            IsVertical = true;
            return this;
        }

        public virtual void AddCoordinate(int x, int y)
        {
            Coordinates.AddItem(new ShipCoordinate(x, y));
        }

        public virtual void AddCoordinate(ShipCoordinate coordinate)
        {
            Coordinates.AddItem(coordinate);
        }

        public virtual List<ShipCoordinate> GetCoordinates()
        {
            return new List<ShipCoordinate>(Coordinates.getItems());
        }

        public void RemoveAllCoordinates()
        {
            Coordinates = new();
        }

        public bool CanHitCoordinate(int x, int y)
        {
            return Coordinates.getItems().Exists(coord => coord.X == x && coord.Y == y && !coord.IsHit);
        }

        public void HitCoordinate(int x, int y)
        {
            var coordinate = Coordinates.getItems().Find(coord => coord.X == x && coord.Y == y);
            coordinate?.Hit();
            if (coordinate != null)
            {
                coordinate.Icon = ShipCoordinateIcon.Explosion;
            }
        }

        public bool IsSunk()
        {
            return Coordinates.getItems().All(coord => coord.IsHit);
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

        public abstract Ship GetShip(ShipCoordinate coordinate);

        public abstract int Accept(ShipInspector shipInspector);
    }
}
