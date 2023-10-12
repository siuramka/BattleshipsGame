﻿using Shared;

namespace backend.Models.Entity.Ships
{
    public class BigShipBuilder : IShipBuilder
    {
        private BigShip bigShip = new BigShip();

        public Ship Build()
        {
            return bigShip;
        }

        public IShipBuilder SetSize(int size)
        {
            bigShip.Size = size;
            return this;
        }

        public IShipBuilder SetIsVertical(bool isVerical)
        {
            bigShip.IsVertical = isVerical;
            return this;
        }

        public IShipBuilder SetType(ShipType type)
        {
            bigShip.ShipType = type;
            return this;
        }
    }
}
