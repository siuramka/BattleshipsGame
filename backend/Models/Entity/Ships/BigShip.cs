using backend.Strategies.Ships;
using backend.Strategies;
using Shared;
using backend.Models.Entity.Bombs;
using backend.Models.Entity.Bombs.BigBomb;

namespace backend.Models.Entity.Ships
{
    public class BigShip : Ship
    {
        public BigShip()
        {
        }

        public BigShip(BigShip ship)
        {
            Size = ship.Size;
            IsVertical = ship.IsVertical;
            ShipType = ship.ShipType;
            foreach (ShipCoordinate coordinate in ship.Coordinates)
            {
                AddCoordinate(coordinate);
            }
        }

        public override Ship DeepCopy()
        {
            BigShip ship = new BigShip(this);
            List<ShipCoordinate> coords = GetCoordinates();
            RemoveAllCoordinates();
            foreach (ShipCoordinate coordinate in coords)
            {
                AddCoordinate(coordinate);
            }
            return ship;
        }

        public override IAttackStrategy GetAttackStrategy()
        {
            return new BigBombAttackStrategy();
        }

        public override BombFactory GetShipBombFactory()
        {
            return new BigBombFactory();
        }

        public override Ship ShallowCopy()
        {
            return new BigShip(this);
        }
    }
}
