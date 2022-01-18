using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common;
using Assets.Scripts.Grid;
using UnityEngine;

namespace Assets.Scripts.Abilities.Actives
{
    public class SwapSkill : ActiveAbility
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

            if (cell == null || cell.occupier == null) return false;

            var thisCreature = GetComponent<Creature>();

            var prevPosition = cell.occupier.gridPosition;
            cell.occupier.SetGridPosition(thisCreature.gridPosition, true);
            thisCreature.SetGridPosition(prevPosition, true);

            LogManager.instance.Log($"{name} uses {GetName()} to swap with {cell.occupier.name}.");

            return true;
        }

        public override string GetName()
        {
            return "Swap";
        }
    }
}
