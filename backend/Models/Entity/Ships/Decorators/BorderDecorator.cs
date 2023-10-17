using backend.Models.Entity.Bombs;
using backend.Strategies;
using Shared;

namespace backend.Models.Entity.Ships.Decorators
{
    public class BorderDecorator : Ship
    {
        private Ship _ship;
        private Color _color;
        public BorderDecorator(Ship ship, Color color)
        {
            _ship = ship;
            _color = color;
            ShipType = ship.ShipType;
            IsVertical = ship.IsVertical;
            Size = ship.Size;
        }

        public override IAttackStrategy GetAttackStrategy()
        {
            return _ship.GetAttackStrategy();
        }

        public override BombFactory GetShipBombFactory()
        {
            return _ship.GetShipBombFactory();
        }

        public override void AddCoordinate(int x, int y)
        {
            _ship.AddCoordinate(x, y);
        }

        public override List<ShipCoordinate> GetCoordinates()
        {
            List<ShipCoordinate> coords = _ship.GetCoordinates();
            foreach (ShipCoordinate coord in coords)
            {
                if (coord.IsHit)
                {
                    continue;
                }

                coord.BorderColor = _color;
            }

            return coords;
        }
    }
}
