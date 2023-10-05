﻿using Shared;

namespace backend.Models.Entity.Ships.Factory
{
    public class ConcreteShipFactory : ShipFactory
    {
        public override Ship GetShip(ShipType Ship)
        {
            switch (Ship)
            {
                case ShipType.SmallShip:
                    return new SmallShip();
                case ShipType.MediumShip:
                    return new MediumShip();
                case ShipType.BigShip:
                    return new BigShip();
                default:
                    throw new ApplicationException(string.Format("Ship '{0}' cannot be created", Ship));
            }
        }
    }
}
