using backend.Models.Entity.Bombs;
using backend.Models.Entity.Bombs.SmallBomb;
using backend.Models.Entity.Ships;
using Shared;

namespace backend.Strategies.Ships
{
    public class SmallBombAttackStrategy : IAttackStrategy
    {

        public List<ShipCoordinate> TargetShip(int x, int y, MissileBomb smallMissileBomb)
        {
            List<ShipCoordinate> coordinates = new List<ShipCoordinate>();

            coordinates.Add(new ShipCoordinate(x, y)); // 1x1

            return coordinates;
        }

        public List<ShipCoordinate> TargetShip(int x, int y, AtomicBomb smallAtomicBomb)
        {
            List<ShipCoordinate> coordinates = new List<ShipCoordinate>();
            for (int i = 0; i < smallAtomicBomb.Horizontal; i++)
            {
                for (int j = 0; j < smallAtomicBomb.Vertical; j++)
                {
                    int newX = x - 1 + i;
                    int newY = y - 1 + j;
                    coordinates.Add(new ShipCoordinate(newX, newY));
                }
            }

            return coordinates;
        }
    }
}
