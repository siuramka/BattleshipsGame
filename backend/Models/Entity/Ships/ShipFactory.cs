using Shared;

namespace backend.Models.Entity.Ships
{
    public abstract class ShipFactory
    {
        public abstract Ship GetShip(ShipType Ship);
    }
}
