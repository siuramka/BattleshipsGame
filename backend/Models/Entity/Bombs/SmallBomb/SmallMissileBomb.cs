using backend.Models.Entity.Ships;

namespace backend.Models.Entity.Bombs.SmallBomb
{
    public class SmallMissileBomb : MissileBomb
    {
        public override int Size { get { return 1; } }
        public override Orientation OrientationOf { get { return Orientation.Dot; } }

        public override List<ShipCoordinate> CalculateCoordinate(int x, int y)
        {

            List<ShipCoordinate> coordinates = new List<ShipCoordinate>();

            coordinates.Add(new ShipCoordinate(x, y));

            return coordinates;
        }
    }
}
