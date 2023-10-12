using Shared;

namespace backend.Models.Entity.Ships
{
    public class SmallShipBuilder : IShipBuilder
    {
        private SmallShip smallShip = new SmallShip();

        public Ship Build() {
            return smallShip;
        }

        public IShipBuilder SetSize(int size)
        {
            smallShip.Size = size;
            return this;
        }

        public IShipBuilder SetIsVertical(bool isVerical)
        {
            smallShip.IsVertical = isVerical;
            return this;
        }

        public IShipBuilder SetType(ShipType type)
        {
            smallShip.ShipType = type;
            return this;
        }

    }
}
