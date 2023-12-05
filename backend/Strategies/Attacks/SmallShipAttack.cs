using backend.Models.Entity;
using backend.Models.Entity.Bombs;
using backend.Models.Entity.Bombs.BigBomb;
using backend.Models.Entity.Bombs.SmallBomb;
using backend.Models.Entity.Ships;
using backend.Strategies.Attacks.Damage;
using backend.Strategies.Ships;
using Shared;

namespace backend.Strategies.Attacks
{
    public sealed class SmallShipAttack : AttackTemplate
    {

        public override void AttackTarget()
        {
            if (attackShip.Stats.HealthCount > 100)
            {
                RemoveHealth();
            }
            else
            {
                //RemoveHealth();
            }

        }

        public override void SetupWeapons()
        {
            bombFactory = new BigBombFactory();
            attackStrategy = new BigBombAttackStrategy();

            var h1 = new BombDamageHandler();
            var h3 = new ShipDamageAttackHandler();

            h1.SetNext(h3);

            HealthDamage = h1.GetDamage(gameBoard,HealthDamage);
        }


    }
}
