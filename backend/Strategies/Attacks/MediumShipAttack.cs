using backend.Models.Entity;
using backend.Models.Entity.Bombs;
using backend.Models.Entity.Bombs.MediumBomb;
using backend.Models.Entity.Bombs.SmallBomb;
using backend.Models.Entity.Ships;
using backend.Strategies.Ships;
using Shared;

namespace backend.Strategies.Attacks
{
    public sealed class MediumShipAttack : AttackTemplate
    {

        public override void AttackTarget()
        {
            if (attackShip.Stats.HealthCount > 2000)
            {
                RemoveHealth();
            }
            else
            {
                RemoveHealth();
                RemoveArmour();
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
                targetShip.Stats.ArmourCount -= ArmourDamage * 2;
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
                targetShip.Stats.HealthCount -= HealthDamage * 2;
            }
        }

        public override void SetupWeapons()
        {
            bombFactory = new MediumBombFactory();
            attackStrategy = new MediumBombAttackStrategy();
            HealthDamage = 200;
            ArmourDamage = 400;
        }
    }
}
