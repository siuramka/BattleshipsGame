using backend.Models.Entity.Ships;

namespace backend.Models.Entity.Bombs
{
    /// <summary>
    /// AtomicBomb that has same width and lenth of diferent size
    /// </summary>
    public abstract class AtomicBomb
    {
        public abstract int horizontal { get; }
        public abstract int vertical { get; }
    }
}
