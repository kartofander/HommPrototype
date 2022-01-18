using Assets.Scripts.Common;
using Assets.Scripts.Grid;
using UnityEngine;

namespace Assets.Scripts.Abilities.Passives
{
    public class ReplicationAbility : PassiveAbility
    {
        private int triggerCount = 0;

        public override void OnAfterDamage(
            Creature thisCreature, 
            int damageTaken, 
            int unitsLost, 
            Creature attacker,
            AttackType attackType)
        {
            // do nothing if no units were lost
            if (unitsLost < 1) return;

            // do nothing if there is no space to create new unit
            var cell = GridManager.instance.GetRandomFreeNeighbor(thisCreature.gridPosition);
            if (cell == null) return;

            triggerCount++;

            var newReplica = Instantiate(gameObject);
            var replicationAbility = newReplica.GetComponent<ReplicationAbility>();
            DestroyImmediate(replicationAbility);

            var controller = newReplica.GetComponent<Creature>();
            controller.Initialize(cell.position, unitsLost);

            var statsMultiplier = triggerCount switch
            {
                1 => 0.70f,
                2 => 0.40f,
                _ => 0.10f,
            };

            var initStats = thisCreature.stats;
            controller.stats = new Stats()
            {
                health = CalculateStat(initStats.health, statsMultiplier),
                speed = CalculateStat(initStats.speed, statsMultiplier),
                initiative = CalculateStat(initStats.initiative, statsMultiplier),
                minDamageRange = CalculateStat(initStats.minDamageRange, statsMultiplier),
                maxDamageRange = CalculateStat(initStats.maxDamageRange, statsMultiplier),
                minDamageMelee = CalculateStat(initStats.minDamageMelee, statsMultiplier),
                maxDamageMelee = CalculateStat(initStats.maxDamageMelee, statsMultiplier),
                turnRepeatChance = Mathf.Clamp01(initStats.turnRepeatChance * statsMultiplier),
            };

            CombatControlManager.instance.AddCreatureToQueue(controller);

            newReplica.GetComponentInChildren<SpriteRenderer>().color = new Color(255, 255, 255, statsMultiplier);
            newReplica.name = $"{name}'s Remnant";

            LogManager.instance.Log($"{name} replicates itself! Times this ability triggered: {triggerCount}.");
        }

        private int CalculateStat(int init, float multiplier)
        {
            var value = (int) (init * multiplier);
            // preventing stats from falling too low, but its ok if original was 0
            return value < 1 && init > 0 
                ? 1 
                : value;
        }
    }
}
