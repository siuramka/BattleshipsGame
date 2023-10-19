using backend.Models.Entity.Bombs;
using backend.Strategies;

namespace backend.Models.Entity.Ships.Decorators
{
    public class FlagDecorator : Ship
    {
        private Ship _ship;

        public FlagDecorator(FlagDecorator decorator)
        {
            _ship = decorator._ship;
            ShipType = decorator.ShipType;
            IsVertical = decorator._ship.IsVertical;
            Size = decorator._ship.Size;
        }
        public FlagDecorator(Ship ship)
        {
            _ship = ship;
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

                coord.Icon = ShipCoordinateIcon.Flag;
            }

            return coords;
        }

        public override Ship DeepCopy()
        {
            return new FlagDecorator(this);
        }

        public override Ship ShallowCopy()
        {
            return new FlagDecorator(this);
        }
    }
}
