using backend.Models.Entity.Ships;
using Shared;

namespace backend.Strategies.Ships
{
    public class SmallShipAttackStrategy : IAttackStrategy
    {

        public List<ShipCoordinate> TargetShip(int x, int y, List<Ship> battleships, List<ShipCoordinate> missedCoordinates, BombType attackBomb, Ship enemyAttackShip)
        {
            List<ShipCoordinate> hitCoordinates = new();

            if(attackBomb == BombType.MissileBomb)
            {
                var bomb = enemyAttackShip.GetShipBombFactory().CreateMissileBomb();
            }
            else if (attackBomb == BombType.AtomicBomb)
            {
                var bomb = enemyAttackShip.GetShipBombFactory().CreateAtomicBomb();
                bomb.CalculateCoordinate(x, y);
            }


            return hitCoordinates;
        }
    }
}
