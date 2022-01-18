using Assets.Scripts.Common;
using UnityEngine;

namespace Assets.Scripts.Abilities.Passives
{
    public class ArmorAbility : PassiveAbility
    {
        private int triggerCount = 0;

        public override void OnBeforeDamage(Creature thisCreature, ref int damage, ref Creature attacker, ref AttackType attackType)
        {
            var reducedDamageMultiplier = triggerCount switch
            {
                0 => 0.01f,
                1 => 0.34f,
                2 => 0.67f,
                _ => 1f,
            };

            triggerCount++;

            var reducedDamage = (int)(reducedDamageMultiplier * damage);

            LogManager.instance.Log($"{name} reduces incoming damage from {damage} to {reducedDamage}. Times this ability triggered: {triggerCount}.");

            damage = reducedDamage;
        }
    }
}
