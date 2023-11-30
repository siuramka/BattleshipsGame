namespace backend.Models.Entity.Ships
{
    public class ShipMemento
    {
        //private setter
        public List<ShipCoordinate> Coordinates { get; }

        public ShipMemento(List<ShipCoordinate> coordinates)
        {
            Coordinates = new List<ShipCoordinate>(coordinates);
        }
    }
}
