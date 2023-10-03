namespace backend.Models.Entity.Ships
{
    public abstract class ShipFactory
    {
        public abstract Ship GetShip(string Ship);
    }
}
