using backend.Models.Entity.Bombs;
using backend.Models.Entity.Bombs.SmallBomb;
using backend.Strategies;
using backend.Strategies.Ships;
using Shared;

namespace backend.Models.Entity.Ships;

public class SmallShip : Ship
{
    public override IAttackStrategy GetAttackStrategy()
    {
        return new SmallBombAttackStrategy();
    }

    public override BombFactory GetShipBombFactory()
    {
        return new SmallBombFactory();
    }
}