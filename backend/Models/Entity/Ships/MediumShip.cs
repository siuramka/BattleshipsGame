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
            Size = 2;
            IsVertical = false;
            ShipType = ShipType.MediumShip;
        }

        public override IAttackStrategy GetAttackStrategy()
        {
            return new MediumShipAttackStrategy();
        }

        public override BombFactory GetShipBombFactory()
        {
            return new MediumBombFactory();
        }
    }
}
