using backend.Models.Entity.Ships;

namespace backend.Models.Entity.Bombs.MediumBomb
{
    public class MediumMissileBomb : MissileBomb
    {
        public override int Size { get { return 2; } }
        public override Orientation OrientationOf { get { return Orientation.Vertical; } }

        public override List<ShipCoordinate> CalculateCoordinate(int x, int y)
        {
            List<ShipCoordinate> coordinates = new List<ShipCoordinate>();

            for (int i = 0; i < Size; i++)
            {
                int newY = y + i;
                coordinates.Add(new ShipCoordinate(x, newY));
            }

            return coordinates;
        }
    }
}
