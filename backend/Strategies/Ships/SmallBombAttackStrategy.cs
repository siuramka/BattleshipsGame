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

            coordinates.Add(new ShipCoordinate(x, y));

            return coordinates;
        }

        public List<ShipCoordinate> TargetShip(int x, int y, AtomicBomb smallAtomicBomb)
        {
            List<ShipCoordinate> coordinates = new List<ShipCoordinate>();

            for (int i = 0; i < smallAtomicBomb.horizontal; i++)
            {
                for (int j = 0; j < smallAtomicBomb.vertical; j++)
                {
                    int newX = x + i;
                    int newY = y + j;
                    coordinates.Add(new ShipCoordinate(newX, newY));
                }
            }

            return coordinates;
        }
    }
}
