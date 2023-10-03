using backend.Strategies.Ships;
using backend.Strategies;
using Shared;

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
    }
}
