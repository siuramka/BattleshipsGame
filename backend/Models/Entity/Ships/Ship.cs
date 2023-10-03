using backend.Strategies.Ships;
using backend.Strategies;
using Shared;

namespace backend.Models.Entity.Ships
{
    public abstract class Ship
    {
        public List<ShipCoordinate> Coordinates { get; } = new List<ShipCoordinate>();
        public int Size { get; protected set; }
        public bool IsVertical { get; protected set; }
        public ShipType ShipType { get; protected set; }

        public abstract IAttackStrategy GetAttackStrategy();

        public void AddCoordinate(int x, int y)
        {
            if(Size == 1)
            {
                Coordinates.Add(new ShipCoordinate(x, y));
            }
            if(Size == 2)
            {
                if (IsVertical)
                {
                    Coordinates.Add(new ShipCoordinate(x, y));
                    Coordinates.Add(new ShipCoordinate(x, y + 1));
                } 
                else
                {
                    Coordinates.Add(new ShipCoordinate(x, y));
                    Coordinates.Add(new ShipCoordinate(x + 1, y));
                }

            }
            if (Size == 3)
            {
                if (IsVertical)
                {
                    Coordinates.Add(new ShipCoordinate(x, y));
                    Coordinates.Add(new ShipCoordinate(x, y + 1));
                    Coordinates.Add(new ShipCoordinate(x, y + 2));
                }
                else
                {
                    Coordinates.Add(new ShipCoordinate(x, y));
                    Coordinates.Add(new ShipCoordinate(x + 1, y));
                    Coordinates.Add(new ShipCoordinate(x + 2, y));
                }
            }

        }

        public bool CanHitCoordinate(int x, int y)
        {
            return Coordinates.Exists(coord => coord.X == x && coord.Y == y && !coord.IsHit);
        }

        public void HitCoordinate(int x, int y)
        {
            var coordinate = Coordinates.Find(coord => coord.X == x && coord.Y == y);
            coordinate?.Hit();
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
    }
}
