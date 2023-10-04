using backend.Models.Entity.Ships;

namespace backend.Models.Entity.Bombs.MediumBomb
{
    public class MediumAtomicBomb : AtomicBomb
    {
        public override int horizontal { get { return 3; } }
        public override int vertical { get { return 2; } }

    }
}
