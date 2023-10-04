using backend.Models.Entity.Ships;

namespace backend.Models.Entity.Bombs.MediumBomb
{
    public class MediumAtomicBomb : AtomicBomb
    {
        public override int horizontal { get { return 3; } }
        public override int vertical { get { return 2; } }

        public override List<ShipCoordinate> CalculateCoordinate(int x, int y)
        {
            List<ShipCoordinate> coordinates = new List<ShipCoordinate>();

            for (int i = 0; i < horizontal; i++)
            {
                for (int j = 0; j < vertical; j++)
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
