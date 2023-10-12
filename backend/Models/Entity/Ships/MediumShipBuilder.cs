using Shared;

namespace backend.Models.Entity.Ships
{
    public class MediumShipBuilder : IShipBuilder
    {
        private MediumShip mediumShip = new MediumShip();

        public Ship Build()
        {
            return mediumShip;
        }

        public IShipBuilder SetSize(int size)
        {
            mediumShip.Size = size;
            return this;
        }

        public IShipBuilder SetIsVertical(bool isVerical)
        {
            mediumShip.IsVertical = isVerical;
            return this;
        }

        public IShipBuilder SetType(ShipType type)
        {
            mediumShip.ShipType = type;
            return this;
        }
    }
}
