using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Transfer
{
    public class SetupShipResponse
    {
        public bool CanPlace { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int ShipSize { get; set; }
        public ShipType TypeOfShip { get; set; }
        public bool IsVertical { get; set; }

        public SetupShipResponse(bool canPlace, int x, int y, int shipSize, ShipType typeOfShip, bool isVertical)
        {
            CanPlace = canPlace;
            X = x;
            Y = y;
            ShipSize = shipSize;
            TypeOfShip = typeOfShip;
            IsVertical = isVertical;
        }
    }
}
