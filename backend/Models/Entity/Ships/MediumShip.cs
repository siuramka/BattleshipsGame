using backend.Strategies.Ships;
using backend.Strategies;
using Shared;

namespace backend.Models.Entity.Ships
{
    public class MediumShip : IShip
    {
        public List<ShipCoordinate> Coordinates { get; } = new();
        public int Size = 2;
        public bool IsVertical = false;
        public ShipType ShipType { get; }
        private IAttackStrategy _attackStrategy;

        public MediumShip()
        {
            _attackStrategy = new MediumShipAttackStrategy();
            ShipType = ShipType.MediumShip;
        }
        public IAttackStrategy GetAtackStrategy()
        {
            return _attackStrategy;
        }
        public void AddCoordinate(int x, int y)
        {
            Coordinates.Add(new ShipCoordinate(x, y));
        }
        public bool CanHitCoordinate(int x, int y)
        {
            return Coordinates.Any(coord => (coord.X == x && coord.Y == y && !coord.IsHit));
        }
        public void HitCoordinate(int x, int y)
        {
            foreach (var coordinate in Coordinates)
            {
                if (coordinate.X == x && coordinate.Y == y)
                {
                    coordinate.Hit();
                    return;
                }
            }
        }
        public bool IsSunk()
        {
            return Coordinates.All(x => x.IsHit);
        }
        public override string ToString()
        {
            return "Small Ship 1x2";
        }
    }
}
