using Assets.Scripts.Abilities.Passives;
using Assets.Scripts.Common;

namespace Assets.Scripts.Abilities.Statuses
{
    public class PossessionStatus : PassiveAbility
    {
        private Team initialTeam;
        private int possessionTurnsLeft;
        private Creature possessedCreature;

        public void Initialize(Team possessTeam, int possessionTurns)
        {
            possessionTurnsLeft = possessionTurns;
            possessedCreature.SetTeam(possessTeam);
        }

        public void AddPossessionTurns(int possessionTurnsToAdd)
        {
            possessionTurnsLeft += possessionTurnsToAdd;
        }

        public override void OnAbilityApplied(Creature thisCreature)
        {
            initialTeam = thisCreature.GetTeam();
            possessedCreature = thisCreature;
        }

        public override void OnTurnEnd(Creature thisCreature)
        {
            possessionTurnsLeft--;

            if (possessionTurnsLeft <= 0)
            {
                thisCreature.SetTeam(initialTeam);
                toRemove = true;
                LogManager.instance.Log($"{name} possession ended!");
            }
        }
    }
}
