using Assets.Scripts.Abilities.Statuses;
using Assets.Scripts.Common;
using UnityEngine;

namespace Assets.Scripts.Abilities.Passives
{
    public class PossessionAbility : PassiveAbility
    {
        [SerializeField] private int possessionTurns = 1;

        public override void OnAfterAttack(Creature thisCreature, int damageDealt, Creature target, AttackType attackType)
        {
            var existingPossession = target.GetComponent<PossessionStatus>();
            if (existingPossession != null)
            {
                existingPossession.AddPossessionTurns(possessionTurns);
            }

            var possession = target.AppendAbilityOrStatus<PossessionStatus>();
            possession.Initialize(thisCreature.GetTeam(), possessionTurns);

            LogManager.instance.Log($"{name} possesses {target} for {possessionTurns} turns!");
        }
    }
}
