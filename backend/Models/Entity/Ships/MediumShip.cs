using backend.Strategies.Ships;
using backend.Strategies;
using Shared;
using backend.Models.Entity.Bombs;
using backend.Models.Entity.Bombs.SmallBomb;
using backend.Models.Entity.Bombs.MediumBomb;
using backend.Strategies.Attacks;
using backend.Visitor;

namespace backend.Models.Entity.Ships
{
    public class MediumShip : Ship
    {
        public MediumShip()
        {
            Stats = new Statistics(5000, 5000);

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

        public override AttackTemplate GetAttackTemplate()
        {
            return new MediumShipAttack();
        }

        public override Ship ShallowCopy()
        {
            return new MediumShip(this);
        }

        public override Ship GetShip(ShipCoordinate c)
        {
            if (Coordinates.Contains(c))
            {
                return this;
            }
            return null;
        }

        public override int Accept(ShipInspector shipInspector)
        {
            return shipInspector.visit(this);
        }
    }
}
