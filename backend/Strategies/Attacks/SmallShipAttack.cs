using backend.Models.Entity;
using backend.Models.Entity.Bombs;
using backend.Models.Entity.Bombs.SmallBomb;
using backend.Models.Entity.Ships;
using backend.Strategies.Ships;
using Shared;

namespace backend.Strategies.Attacks
{
    public sealed class SmallShipAttack : AttackTemplate
    {

        public override void AttackTarget()
        {
            if (attackShip.Stats.HealthCount > 1000)
            {
                RemoveHealth();
            }
            else
            {
                RemoveHealth();
            }

        }

        public override void RemoveArmour()
        {
            if (bombType == BombType.MissileBomb && targetShip.Stats.ArmourCount >= ArmourDamage)
            {
                targetShip.Stats.ArmourCount -= ArmourDamage;
            }

            if (bombType == BombType.AtomicBomb && targetShip.Stats.ArmourCount >= ArmourDamage)
            {
                targetShip.Stats.ArmourCount -= ArmourDamage * 1.25;
            }
        }

        public override void RemoveHealth()
        {
            if (bombType == BombType.MissileBomb && targetShip.Stats.HealthCount >= HealthDamage)
            {
                targetShip.Stats.HealthCount -= HealthDamage;
            }

            if (bombType == BombType.AtomicBomb && targetShip.Stats.HealthCount >= HealthDamage)
            {
                targetShip.Stats.HealthCount -= HealthDamage * 1.25;
            }
        }

        public override void SetupWeapons()
        {
            bombFactory = new SmallBombFactory();
            attackStrategy = new SmallBombAttackStrategy();
            HealthDamage = 100;
            ArmourDamage = 100;
        }
    }
}
