using Assets.Scripts.Common;
using UnityEngine;

namespace Assets.Scripts.Abilities.Passives
{
    public abstract class PassiveAbility : MonoBehaviour
    {
        public int priority;
        public bool toRemove;

        public virtual void OnBeforeDamage(Creature thisCreature, ref int damage, ref Creature attacker, ref AttackType attackType)
        {
        }

        public virtual void OnAfterDamage(Creature thisCreature, int damageTaken, int unitsLost, Creature attacker, AttackType attackType)
        {
        }

        public virtual void OnAfterAttack(Creature thisCreature, int damageDealt, Creature target, AttackType attackType)
        {
        }

        public virtual void OnTurnEnd(Creature thisCreature)
        {
        }

        public virtual void OnAbilityApplied(Creature thisCreature)
        {
        }

        public virtual void OnAbilityRemoved(Creature thisCreature)
        {

        }
    }
}
