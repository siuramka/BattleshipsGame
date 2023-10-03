using backend.Models.Entity.Ships;

namespace backend.Strategies.Ships
{
    public class BigShipAttackStrategy : IAttackStrategy
    {
        public List<ShipCoordinate> TargetShip(int x, int y, List<IShip> battleships, List<ShipCoordinate> missedCoordinates)
        {
            List<ShipCoordinate> hitCoordinates = new();
            foreach (var battleship in battleships)
            {
                if (battleship.CanHitCoordinate(x, y))
                {
                    battleship.HitCoordinate(x, y);
                    hitCoordinates.Add(new ShipCoordinate(x, y));
                }
                else
                {
                    missedCoordinates.Add(new ShipCoordinate(x, y));
                }
            }
            return hitCoordinates;
        }
    }
}
