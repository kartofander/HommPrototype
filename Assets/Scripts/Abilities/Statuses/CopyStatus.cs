using Assets.Scripts.Abilities.Passives;
using Assets.Scripts.Common;

namespace Assets.Scripts.Abilities.Statuses
{
    public class CopyStatus : PassiveAbility
    {
        public override void OnAfterDamage(Creature thisCreature, int damageTaken, int unitsLost, Creature attacker, AttackType attackType)
        {
            thisCreature.Kill();
        }
    }
}
