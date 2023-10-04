using backend.Models.Entity.Ships;

namespace backend.Models.Entity.Bombs.BigBomb
{
    public class BigMissileBomb : MissileBomb
    {
        public override int Size { get { return 3; } }
        public override Orientation OrientationOf { get { return Orientation.Horizontal; } }

        public override List<ShipCoordinate> CalculateCoordinate(int x, int y)
        {
            List<ShipCoordinate> coordinates = new List<ShipCoordinate>();

            for (int i = 0; i < Size; i++)
            {
                int newX = x + i;
                coordinates.Add(new ShipCoordinate(newX, y));
            }

            return coordinates;
        }
    }
}
