using backend.Models.Entity.Bombs;
using backend.Models.Entity.Bombs.SmallBomb;
using backend.Models.Entity.Ships;
using System.Drawing;

namespace backend.Strategies.Ships
{
    public class BigBombAttackStrategy : IAttackStrategy
    {
        public List<ShipCoordinate> TargetShip(int x, int y, MissileBomb bigMissileBomb)
        {
            List<ShipCoordinate> coordinates = new List<ShipCoordinate>();

            for (int i = 0; i < bigMissileBomb.Size; i++)
            {
                int newX = x + i;
                coordinates.Add(new ShipCoordinate(newX, y));
            }

            return coordinates;
        }

        public List<ShipCoordinate> TargetShip(int x, int y,  AtomicBomb bigAtomicBomb)
        {
            List<ShipCoordinate> coordinates = new List<ShipCoordinate>();

            for (int i = 0; i < bigAtomicBomb.horizontal; i++)
            {
                for (int j = 0; j < bigAtomicBomb.vertical; j++)
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
