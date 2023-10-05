using backend.Models.Entity.Bombs;
using backend.Models.Entity.Bombs.SmallBomb;
using backend.Models.Entity.Ships;
using System.Drawing;

namespace backend.Strategies.Ships
{
    public class MediumBombAttackStrategy : IAttackStrategy
    {
        public List<ShipCoordinate> TargetShip(int x, int y, MissileBomb mediumMissileBomb)
        {
            List<ShipCoordinate> coordinates = new List<ShipCoordinate>();

            for (int i = 0; i < mediumMissileBomb.Size; i++)
            {
                int newY = y - 1 + i;
                coordinates.Add(new ShipCoordinate(x, newY));
            }

            return coordinates;
        }

        public List<ShipCoordinate> TargetShip(int x, int y, AtomicBomb mediumAtomicBomb)
        {
            List<ShipCoordinate> coordinates = new List<ShipCoordinate>();

            for (int i = 0; i < mediumAtomicBomb.Horizontal; i++)
            {
                for (int j = 0; j < mediumAtomicBomb.Vertical; j++)
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
