using backend.Strategies.Ships;
using backend.Strategies;
using Shared;
using backend.Models.Entity.Bombs;

namespace backend.Models.Entity.Ships
{
    public class BigShip : Ship
    {
        public BigShip()
        {
            Size = 3;
            IsVertical = false;
            ShipType = ShipType.BigShip;
        }

        public override IAttackStrategy GetAttackStrategy()
        {
            return new BigShipAttackStrategy();
        }

        public override BombFactory GetShipBombFactory()
        {
            throw new NotImplementedException();
        }
    }
}
