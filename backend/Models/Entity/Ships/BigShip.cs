using backend.Strategies.Ships;
using backend.Strategies;
using Shared;
using backend.Models.Entity.Bombs;
using backend.Models.Entity.Bombs.BigBomb;
using backend.Strategies.Attacks;
using backend.Visitor;

namespace backend.Models.Entity.Ships
{
    public class BigShip : Ship
    {
        public BigShip()
        {
            Stats = new Statistics(9000, 9000);
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

        public override AttackTemplate GetAttackTemplate()
        {
            return new BigShipAttack();
        }

        public override Ship ShallowCopy()
        {
            return new BigShip(this);
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
