using Assets.Scripts.Common;

namespace Assets.Scripts.Abilities.Passives
{
    public class RegenerationAbility : PassiveAbility
    {
        public override void OnTurnEnd(Creature thisCreature)
        {
            LogManager.instance.Log($"{name} regenerates.");
            thisCreature.Heal(thisCreature.stats.health);
        }
    }
}
