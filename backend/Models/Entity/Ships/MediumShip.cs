using backend.Strategies.Ships;
using backend.Strategies;
using Shared;

namespace backend.Models.Entity.Ships
{
    public class MediumShip : Ship
    {
        public MediumShip()
        {
            Size = 2;
            IsVertical = false;
            ShipType = ShipType.MediumShip;
        }

        public override IAttackStrategy GetAttackStrategy()
        {
            return new MediumShipAttackStrategy();
        }
    }
}
