using System.Collections.Generic;
using Assets.Scripts.Common;
using Assets.Scripts.Grid;
using UnityEngine;

namespace Assets.Scripts.Abilities.Actives
{
    public class ArrowRainSkill : ActiveAbility
    {
        private const float damageMultiplier = 0.5f;

        public override Vector2Int[] GetArea(Vector2Int start)
        {
            var center = start.ToAxial();

            // "double flower"
            return new[]
            {
                center.FromAxial(),
                new Vector2Int(center.x + 1, center.y).FromAxial(),

                new Vector2Int(center.x, center.y + 1).FromAxial(),
                new Vector2Int(center.x - 1, center.y + 1).FromAxial(),
                new Vector2Int(center.x + 1, center.y + 1).FromAxial(),

                new Vector2Int(center.x, center.y + 2).FromAxial(),
                new Vector2Int(center.x -1, center.y + 2).FromAxial(),

                new Vector2Int(center.x, center.y - 1).FromAxial(),
                new Vector2Int(center.x + 1, center.y - 1).FromAxial(),
                new Vector2Int(center.x + 2, center.y - 1).FromAxial(),

                new Vector2Int(center.x + 1, center.y - 2).FromAxial(),
                new Vector2Int(center.x + 2, center.y - 2).FromAxial(),
            };
        }

        public override bool ApplyEffect(IEnumerable<GridCell> cells)
        {
            var thisCreature = GetComponent<Creature>();

            var damage = (int) (thisCreature.GetRangeDamage() * damageMultiplier);

            foreach (var cell in cells)
            {
                if (cell.occupier != null && cell.occupier.GetTeam() != thisCreature.GetTeam())
                {
                    cell.occupier.Damage(damage, thisCreature, AttackType.Range);
                }
            }

            LogManager.instance.Log($"{name} uses {GetName()}.");

            return true;
        }

        public override string GetName()
        {
            return "Arrow Rain";
        }
    }
}
