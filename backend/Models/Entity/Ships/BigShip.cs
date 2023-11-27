using backend.Strategies.Ships;
using backend.Strategies;
using Shared;
using backend.Models.Entity.Bombs;
using backend.Models.Entity.Bombs.BigBomb;
using backend.Strategies.Attacks;

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
            foreach (ShipCoordinate coordinate in ship.GetCoordinates())
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
    }
}
