using backend.Models.Entity;
using backend.Models.Entity.Bombs;
using backend.Models.Entity.Bombs.BigBomb;
using backend.Models.Entity.Bombs.SmallBomb;
using backend.Models.Entity.Ships;
using backend.Strategies.Ships;
using Shared;

namespace backend.Strategies.Attacks
{
    public sealed class BigShipAttack : AttackTemplate
    {

        public override void AttackTarget()
        {

            RemoveHealth();
            RemoveArmour();
        }
        public override List<ShipCoordinate> GetHitCoordinates(int x, int y, List<ShipCoordinate> hitableCordinates)
        {
            List<ShipCoordinate> hitableCoordinates = hitableCordinates;
            List<ShipCoordinate> hitShipCoordinates = new List<ShipCoordinate>();


            foreach (var ship in gameBoard.GetShips())
            {
                foreach (var hitableCoord in hitableCoordinates)
                {
                    if (ship.CanHitCoordinate(hitableCoord.X, hitableCoord.Y))
                    {
                        ship.HitCoordinate(hitableCoord.X, hitableCoord.Y);
                        hitShipCoordinates.Add(hitableCoord);
                        targetShip = ship;
                        AttackTarget();
                    }
                    else
                    {
                        gameBoard.AddMissed(hitableCoord);
                    }
                }
            }

            foreach (var miss in gameBoard.GetMissedCoordinates())
            {
                if (hitShipCoordinates.Contains(miss))
                {
                    gameBoard.RemoveMissed(miss);
                }
            }


            return hitShipCoordinates;
        }
        public override void RemoveArmour()
        {
            if (bombType == BombType.MissileBomb && targetShip.Stats.ArmourCount >= ArmourDamage)
            {
                targetShip.Stats.ArmourCount -= ArmourDamage * 1.5;
            }

            if (bombType == BombType.AtomicBomb && targetShip.Stats.ArmourCount >= ArmourDamage)
            {
                targetShip.Stats.ArmourCount -= ArmourDamage * 3;
            }
        }

        public override void RemoveHealth()
        {
            if (bombType == BombType.MissileBomb && targetShip.Stats.HealthCount >= HealthDamage)
            {
                targetShip.Stats.HealthCount -= HealthDamage * 1.5;
            }

            if (bombType == BombType.AtomicBomb && targetShip.Stats.HealthCount >= HealthDamage)
            {
                targetShip.Stats.HealthCount -= HealthDamage * 3;
            }
        }

        public override void SetupWeapons()
        {
            bombFactory = new BigBombFactory();
            attackStrategy = new BigBombAttackStrategy();
            HealthDamage = 500;
            ArmourDamage = 700;
        }
    }
}
