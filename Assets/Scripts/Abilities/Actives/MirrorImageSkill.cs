using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Abilities.Passives;
using Assets.Scripts.Abilities.Statuses;
using Assets.Scripts.Common;
using Assets.Scripts.Grid;
using UnityEngine;

namespace Assets.Scripts.Abilities.Actives
{
    public class MirrorImageSkill : ActiveAbility
    {
        public override Vector2Int[] GetArea(Vector2Int center)
        {
            return new[]
            {
                center,
            };
        }

        public override bool ApplyEffect(IEnumerable<GridCell> cells)
        {
            var cell = cells.FirstOrDefault();

            // cancel skill if there is no appropriate target
            if (cell == null || cell.occupier == null) return false;

            // cancel skill if there is no space to create new unit
            var freeCell = GridManager.instance.GetRandomFreeNeighbor(cell.occupier.gridPosition);
            if (freeCell == null) return false;

            var copy = Instantiate(cell.occupier.gameObject);

            // removing all abilities from copy
            foreach (var passive in copy.GetComponents<PassiveAbility>())
            {
                Destroy(passive);
            }
            foreach (var active in copy.GetComponents<ActiveAbility>())
            {
                Destroy(active);
            }

            var thisCreature = GetComponent<Creature>();

            // configuring copy
            var controller = copy.GetComponent<Creature>();
            controller.Initialize(freeCell.position, cell.occupier.GetStack());
            controller.SetTeam(thisCreature.GetTeam());
            controller.AppendAbilityOrStatus<CopyStatus>();

            CombatControlManager.instance.AddCreatureToQueue(controller);

            copy.GetComponentInChildren<SpriteRenderer>().color = new Color(195, 89, 250, 0.5f);
            copy.name = $"{cell.occupier.name}'s Copy";

            LogManager.instance.Log($"{name} uses a {GetName()} to create a copy of {cell.occupier.name}.");

            return true;
        }

        public override string GetName()
        {
            return "Mirror Image";
        }
    }
}
