namespace backend.Models.Entity.Ships
{
    public class ConcreteShipFactory : ShipFactory
    {
        public override Ship GetShip(string Ship)
        {
            switch (Ship)
            {
                case "SmallShip":
                    return new SmallShip();
                case "MediumShip":
                    return new MediumShip();
                case "BigShip":
                    return new BigShip();
                default:
                    throw new ApplicationException(string.Format("Ship '{0}' cannot be created", Ship));
            }
        }
    }
}
