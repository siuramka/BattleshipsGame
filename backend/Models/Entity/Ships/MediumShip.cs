using backend.Strategies.Ships;
using backend.Strategies;
using Shared;
using backend.Models.Entity.Bombs;
using backend.Models.Entity.Bombs.SmallBomb;
using backend.Models.Entity.Bombs.MediumBomb;

namespace backend.Models.Entity.Ships
{
    public class MediumShip : Ship
    {
        public MediumShip()
        {
        }

        public MediumShip(MediumShip ship)
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
            MediumShip ship = new MediumShip(this);
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
            return new MediumBombAttackStrategy();
        }

        public override BombFactory GetShipBombFactory()
        {
            return new MediumBombFactory();
        }

        public override Ship ShallowCopy()
        {
            return new MediumShip(this);
        }
    }
}
