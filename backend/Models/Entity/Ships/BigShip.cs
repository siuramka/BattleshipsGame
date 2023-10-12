﻿using backend.Strategies.Ships;
using backend.Strategies;
using Shared;
using backend.Models.Entity.Bombs;
using backend.Models.Entity.Bombs.BigBomb;

namespace backend.Models.Entity.Ships
{
    public class BigShip : Ship
    {
        public override IAttackStrategy GetAttackStrategy()
        {
            return new BigBombAttackStrategy();
        }

        public override BombFactory GetShipBombFactory()
        {
            return new BigBombFactory();
        }
    }
}
