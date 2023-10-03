using backend.Models.Entity.Ships;

namespace backend.Strategies
{
    public interface IAttackStrategy
    {
        public List<ShipCoordinate> TargetShip(int x, int y, List<IShip> battleships, List<ShipCoordinate> missedCoordinates);
    }
}
