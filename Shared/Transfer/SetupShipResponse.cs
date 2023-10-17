using backend.Models.Entity.Ships;

namespace Shared.Transfer
{
    public class SetupShipResponse
    {
        public bool CanPlace { get; set; }
        public List<ShipCoordinate> ShipCoordinates { get; private set; }
        public ShipType TypeOfShip { get; set; }

        public SetupShipResponse(bool canPlace, List<ShipCoordinate> shipCoordinates, ShipType typeOfShip)
        {
            CanPlace = canPlace;
            TypeOfShip = typeOfShip;
            ShipCoordinates = new List<ShipCoordinate>();
            foreach(ShipCoordinate coord in shipCoordinates)
            {
                ShipCoordinate shipClone = new ShipCoordinate(coord.X, coord.Y);
                shipClone.Icon = coord.Icon;
                if (coord.IsHit)
                {
                    shipClone.Hit();
                }
                
                ShipCoordinates.Add(shipClone);
            }
        }
    }
}
