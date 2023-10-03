using backend.Strategies;
using Shared;

namespace backend.Models.Entity.Ships
{
    public interface IShip
    {
        IAttackStrategy GetAtackStrategy();
        void AddCoordinate(int x, int y);
        bool CanHitCoordinate(int x, int y);
        void HitCoordinate(int x, int y);
        bool IsSunk();

    }
}
