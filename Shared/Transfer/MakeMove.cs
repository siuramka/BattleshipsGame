using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Transfer
{
    public class MakeMove
    {
        public int X { get; set; }
        public int Y { get; set; }
        public ShipType TypeOfShip { get; set; }
        public bool IsVertical { get; set; }

        public MakeMove(int x, int y, ShipType typeOfShip, bool isVertical)
        {
            X = x;
            Y = y;
            TypeOfShip = typeOfShip;
            IsVertical = isVertical;
        }
    }
}
