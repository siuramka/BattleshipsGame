using Shared;

namespace WpfApp1.Attacks
{
    internal abstract class ShipAttackBase
    {
        public abstract string ShipAttackName { get; }
        public abstract ShipType ShipType { get; }
    }
}