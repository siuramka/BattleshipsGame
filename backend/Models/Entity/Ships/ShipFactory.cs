namespace backend.Models.Entity.Ships
{
    public abstract class ShipFactory
    {
        public abstract IShip GetShip(string Ship);
    }
}
