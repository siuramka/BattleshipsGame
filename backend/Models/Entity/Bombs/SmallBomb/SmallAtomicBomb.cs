using backend.Models.Entity.Ships;

namespace backend.Models.Entity.Bombs.SmallBomb
{
    public class SmallAtomicBomb : AtomicBomb
    {
        public override int horizontal { get { return 2; } }
        public override int vertical { get { return 2; } }

    }
}
