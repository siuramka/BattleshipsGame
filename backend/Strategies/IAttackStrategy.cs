using backend.Models.Entity.Bombs;
using backend.Models.Entity.Bombs.SmallBomb;
using backend.Models.Entity.Ships;
using Shared;

namespace backend.Strategies
{
    public interface IAttackStrategy
    {
        public List<ShipCoordinate> TargetShip(int x, int y, MissileBomb missileBomb);
        public List<ShipCoordinate> TargetShip(int x, int y, AtomicBomb atomicBomb);
    }
}
