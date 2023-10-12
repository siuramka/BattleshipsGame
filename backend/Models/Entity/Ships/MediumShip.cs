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
        public override IAttackStrategy GetAttackStrategy()
        {
            return new MediumBombAttackStrategy();
        }

        public override BombFactory GetShipBombFactory()
        {
            return new MediumBombFactory();
        }
    }
}
