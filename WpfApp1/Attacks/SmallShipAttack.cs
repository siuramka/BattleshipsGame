using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Attacks
{
    internal class SmallShipAttack : ShipAttackBase
    {
        public override ShipType ShipType { get; }
        public override string ShipAttackName { get; }
        public SmallShipAttack() {
            ShipType = ShipType.SmallShip;
            ShipAttackName = "1x1 Small Ship Attack";
        }

        public override string ToString()
        {
            return ShipAttackName;
        }
    }
}
